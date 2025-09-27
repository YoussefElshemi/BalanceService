import uuid
import threading
import random
import requests
import psycopg2
from time import sleep

STARTING_BALANCE = 100000
MAX_TRANSACTION_AMOUNT = 100
MAX_HOLD_AMOUNT = 50

BASE_HEADERS = {
    "Content-Type": "application/json",
    "X-Username": "locust"
}

DB_CONN = {
    "dbname": "balance_service",
    "user": "postgres",
    "password": "postgres",
    "host": "localhost",
    "port": 35433,
}

SQL_BALANCES = """
WITH tx_effects AS (
   SELECT
       "AccountId",
       "TransactionStatusId",
       CASE WHEN "TransactionDirectionId" = 1 THEN "Amount" ELSE 0 END AS credit_amount,
       CASE WHEN "TransactionDirectionId" = 2 THEN "Amount" ELSE 0 END AS debit_amount
   FROM public."Transactions"
   WHERE "IsDeleted" = false
     AND "AccountId" = %(account_id)s
),
hold_effects AS (
   SELECT
       "AccountId",
       SUM("Amount") AS hold_amount
   FROM public."Holds"
   WHERE "IsDeleted" = false
     AND "HoldStatusId" = 1
     AND "AccountId" = %(account_id)s
   GROUP BY "AccountId"
)
SELECT
   COALESCE(SUM(CASE WHEN "TransactionStatusId" IN (2,3) THEN credit_amount - debit_amount END), 0) AS "LedgerBalance",
   COALESCE(SUM(CASE WHEN "TransactionStatusId" IN (2,3) THEN credit_amount - debit_amount END), 0)
     - COALESCE(SUM(CASE WHEN "TransactionStatusId" = 1 THEN debit_amount END), 0)
     - COALESCE(MAX(h.hold_amount), 0) AS "AvailableBalance",
   COALESCE(SUM(CASE WHEN "TransactionStatusId" = 1 THEN debit_amount END), 0) AS "PendingDebitBalance",
   COALESCE(SUM(CASE WHEN "TransactionStatusId" = 1 THEN credit_amount END), 0) AS "PendingCreditBalance",
   COALESCE(MAX(h.hold_amount), 0) AS "HoldBalance"
FROM tx_effects t
LEFT JOIN hold_effects h ON t."AccountId" = h."AccountId";
"""

lock = threading.Lock()

def new_headers():
    headers = BASE_HEADERS.copy()
    headers["X-Correlation-ID"] = str(uuid.uuid4())

    return headers


def create_account(host, account_name="Locust Account"):
    res = requests.post(f"{host}/accounts", json={
        "accountName": account_name,
        "accountType": "Revenue",
        "currencyCode": "GBP",
        "minimumRequiredBalance": 0
    }, headers=new_headers())

    if res.status_code != 201:
        raise Exception(f"Failed to create account: {res.text}")

    return res.json()["accountId"]


def activate_account(host, account_id):
    res = requests.post(f"{host}/accounts/{account_id}/activate", headers=new_headers())

    if res.status_code != 204:
        raise Exception(f"Failed to activate account {account_id}: {res.text}")


def fund_account(host, account_id, amount=STARTING_BALANCE):
    """Create and post a funding transaction using shared transaction helpers"""

    # Use a requests.Session-like interface to match client signature in helpers
    class DummyClient:
        def post(self, path, **kwargs):
            return requests.post(f"{host}{path}", **kwargs)

    client = DummyClient()
    
    tx_id = create_transaction(client, account_id, direction="Credit", amount=amount)
    if not tx_id:
        raise Exception(f"Failed to create funding transaction for account {account_id}")
    
    if not post_transaction(client, tx_id):
        raise Exception(f"Failed to post funding transaction {tx_id} for account {account_id}")
    
    return tx_id


def create_and_fund_account(host, account_name="Locust Account"):
    """High-level function: create, activate, fund account"""

    account_id = create_account(host, account_name)
    activate_account(host, account_id)
    fund_account(host, account_id)

    return account_id


def verify_balances(account_id, expected_balances, host):
    res = requests.get(f"{host}/accounts/{account_id}/balances", headers=new_headers())
    actual = res.json()

    actual_balances = {
        "ledgerBalance": actual["ledgerBalance"],
        "availableBalance": actual["availableBalance"],
        "pendingDebitBalance": actual["pendingDebitBalance"],
        "pendingCreditBalance": actual["pendingCreditBalance"],
        "holdBalance": actual["holdBalance"],
    }

    with psycopg2.connect(**DB_CONN) as conn:
        with conn.cursor() as cur:
            cur.execute(SQL_BALANCES, {"account_id": account_id})
            row = cur.fetchone()
            db_balances = {
               "ledgerBalance": float(row[0]),
               "availableBalance": float(row[1]),
               "pendingDebitBalance": float(row[2]),
               "pendingCreditBalance": float(row[3]),
               "holdBalance": float(row[4]),
            }

    checks = {k: expected_balances[k] == actual_balances[k] == db_balances[k] for k in expected_balances}
    return actual_balances, db_balances, checks


def create_transaction(client, account_id, direction, amount):
    res = client.post("/transactions", json={
        "accountId": account_id,
        "amount": amount,
        "currencyCode": "GBP",
        "direction": direction,
        "idempotencyKey": str(uuid.uuid4()),
        "type": "InboundFunds",
        "description": "Random Tx",
        "reference": "rand"
    }, headers=new_headers())

    if res.status_code != 201:
        return None

    return res.json()["transactionId"]

def post_transaction(client, tx_id):
    res = client.post(f"/transactions/{tx_id}/post", headers=new_headers())
    return res.status_code == 204


def reverse_transaction(client, tx_id):
    res = client.post(f"/transactions/{tx_id}/reverse", headers=new_headers())
    return res.status_code == 201

def create_hold(client, account_id, amount):
    res = client.post("/holds", json={
        "accountId": account_id,
        "amount": amount,
        "currencyCode": "GBP",
        "idempotencyKey": str(uuid.uuid4()),
        "type": "Regulatory",
        "description": "Random Hold",
        "reference": "randHold"
    }, headers=new_headers())

    if res.status_code != 201:
        return None

    return res.json()["holdId"]


def release_hold(client, hold_id):
    res = client.post(f"/holds/{hold_id}/release", headers=new_headers())
    return res.status_code == 204


def settle_hold(client, hold_id):
    res = client.post(f"/holds/{hold_id}/settle", headers=new_headers())
    return res.status_code == 201


def simulate_transaction(client, account_id, expected_balances):
    roll = random.random()
    if roll < 0.4:
        direction = "Credit" if random.random() < 0.5 else "Debit"
        amount = random.randint(1, MAX_TRANSACTION_AMOUNT)

        with lock:
            tx_id = create_transaction(client, account_id, direction, amount)
            if not tx_id:
                return

            if direction == "Credit":
                expected_balances["pendingCreditBalance"] += amount
            else:
                expected_balances["availableBalance"] -= amount
                expected_balances["pendingDebitBalance"] += amount

            if random.random() < 0.8 and post_transaction(client, tx_id):
                if direction == "Credit":
                    expected_balances["ledgerBalance"] += amount
                    expected_balances["availableBalance"] += amount
                    expected_balances["pendingCreditBalance"] -= amount
                else:
                    expected_balances["ledgerBalance"] -= amount
                    expected_balances["pendingDebitBalance"] -= amount

                if random.random() < 0.1 and reverse_transaction(client, tx_id):
                    if direction == "Credit":
                        expected_balances["ledgerBalance"] -= amount
                        expected_balances["availableBalance"] -= amount
                    else:
                        expected_balances["ledgerBalance"] += amount
                        expected_balances["availableBalance"] += amount


def simulate_hold(client, account_id, expected_balances):
    with lock:
        amount = random.randint(1, MAX_HOLD_AMOUNT)
        hold_id = create_hold(client, account_id, amount)
        if not hold_id:
            return
        expected_balances["holdBalance"] += amount
        expected_balances["availableBalance"] -= amount

        r = random.random()
        if r < 0.4 and release_hold(client, hold_id):
            expected_balances["holdBalance"] -= amount
            expected_balances["availableBalance"] += amount
        elif r < 0.8 and settle_hold(client, hold_id):
            expected_balances["holdBalance"] -= amount
            expected_balances["ledgerBalance"] -= amount


def perform_random_operation(client, account_id, expected_balances):
    """Randomly perform a transaction or hold"""
    if random.random() < 0.7:
        simulate_transaction(client, account_id, expected_balances)
    else:
        simulate_hold(client, account_id, expected_balances)