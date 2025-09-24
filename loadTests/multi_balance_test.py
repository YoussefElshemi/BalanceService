# locust -f multi_balance_test.py --host http://localhost:5000 --headless --skip-log-setup --users 30 --spawn-rate 10 --run-time 60s --stop-timeout 60s

from locust import FastHttpUser, task, constant, events
import uuid
import random
import threading
import requests
import psycopg2

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


def new_headers():
    headers = BASE_HEADERS.copy()
    headers["X-Correlation-ID"] = str(uuid.uuid4())
    return headers


class BalanceUser(FastHttpUser):
    wait_time = constant(0)

    def on_start(self):
        """
        Runs once per VU:
        - Create account
        - Activate it
        - Pre-fund with 100k credit
        """
        self.expected_balances = {
            "ledgerBalance": 0,
            "availableBalance": 0,
            "pendingCreditBalance": 0,
            "pendingDebitBalance": 0,
            "holdBalance": 0,
        }
        self.lock = threading.Lock()
        self.account_id = None

        # 1. Create account
        res = requests.post(f"{self.environment.host}/accounts", json={
            "accountName": "Locust Account",
            "accountType": "Revenue",
            "currencyCode": "GBP",
            "minimumRequiredBalance": 0
        }, headers=new_headers())
        self.account_id = res.json()["accountId"]

        # 2. Activate account
        requests.post(f"{self.environment.host}/accounts/{self.account_id}/activate", headers=new_headers())

        # 3. Prefund with 100k credit
        res = requests.post(f"{self.environment.host}/transactions", json={
            "accountId": self.account_id,
            "amount": 100000,
            "currencyCode": "GBP",
            "direction": "Credit",
            "idempotencyKey": str(uuid.uuid4()),
            "type": "Transfer",
            "description": "Initial load",
            "reference": "init"
        }, headers=new_headers())
        tx_id = res.json()["transactionId"]

        requests.post(f"{self.environment.host}/transactions/{tx_id}/post", headers=new_headers())

        with self.lock:
            self.expected_balances["ledgerBalance"] += 100000
            self.expected_balances["availableBalance"] += 100000

        print(f"[VU {self.environment.runner.user_count}] Setup done. Account ID: {self.account_id}")

    def on_stop(self):
        """
        Teardown for each VU: fetch balances and compare with expected and DB values
        """
        # --- API balances ---
        res = requests.get(f"{self.environment.host}/accounts/{self.account_id}/balances", headers=new_headers())
        actual = res.json()

        actual_balances = {
            "ledgerBalance": actual["ledgerBalance"],
            "availableBalance": actual["availableBalance"],
            "pendingCreditBalance": actual["pendingCreditBalance"],
            "pendingDebitBalance": actual["pendingDebitBalance"],
            "holdBalance": actual["holdBalance"],
        }

        # --- DB balances ---
        sql = """
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
              AND "HoldStatusId" = 1 -- active holds only
              AND "AccountId" = %(account_id)s
            GROUP BY "AccountId"
        )
        SELECT
            -- Ledger = posted credits - posted debits
            COALESCE(SUM(CASE WHEN "TransactionStatusId" IN (2,3) THEN credit_amount - debit_amount END), 0) AS "LedgerBalance",

            -- Available = ledger - pending debits - active holds
            COALESCE(SUM(CASE WHEN "TransactionStatusId" IN (2,3) THEN credit_amount - debit_amount END), 0)
              - COALESCE(SUM(CASE WHEN "TransactionStatusId" = 1 THEN debit_amount END), 0)
              - COALESCE(MAX(h.hold_amount), 0) AS "AvailableBalance",

            -- Pending draft balances
            COALESCE(SUM(CASE WHEN "TransactionStatusId" = 1 THEN credit_amount END), 0) AS "PendingCreditBalance",
            COALESCE(SUM(CASE WHEN "TransactionStatusId" = 1 THEN debit_amount END), 0) AS "PendingDebitBalance",

            -- Active holds
            COALESCE(MAX(h.hold_amount), 0) AS "HoldBalance"
        FROM tx_effects t
        LEFT JOIN hold_effects h ON t."AccountId" = h."AccountId";
        """

        with psycopg2.connect(**DB_CONN) as conn:
            with conn.cursor() as cur:
                cur.execute(sql, {"account_id": self.account_id})
                row = cur.fetchone()
                db_balances = {
                    "ledgerBalance": float(row[0]),
                    "availableBalance": float(row[1]),
                    "pendingCreditBalance": float(row[2]),
                    "pendingDebitBalance": float(row[3]),
                    "holdBalance": float(row[4]),
                }

        checks = {
            "ledgerBalance": self.expected_balances["ledgerBalance"] == actual_balances["ledgerBalance"] == db_balances["ledgerBalance"],
            "availableBalance": self.expected_balances["availableBalance"] == actual_balances["availableBalance"] == db_balances["availableBalance"],
            "pendingDebitBalance": self.expected_balances["pendingDebitBalance"] == actual_balances["pendingDebitBalance"] == db_balances["pendingDebitBalance"],
            "pendingCreditBalance": self.expected_balances["pendingCreditBalance"] == actual_balances["pendingCreditBalance"] == db_balances["pendingCreditBalance"],
            "holdBalance": self.expected_balances["holdBalance"] == actual_balances["holdBalance"] == db_balances["holdBalance"],
        }
        
        if all(checks.values()):
            print(f"[Account {self.account_id}] ✅ Balance match")
        else:
            print(f"\n[Account {self.account_id}] ❌ Balance mismatch")
            print("Expected:", self.expected_balances)
            print("Actual (API):", actual_balances)
            print("Database:", db_balances)
            print("Checks:", checks)


    @task
    def random_operation(self):
        if self.account_id is None:
            return

        roll = random.random()

        # ---------------- Transactions ----------------
        if roll < 0.4:
            direction = "Credit" if random.random() < 0.5 else "Debit"
            amount = random.randint(1, 100)

            tx_res = self.client.post("/transactions", json={
                "accountId": self.account_id,
                "amount": amount,
                "currencyCode": "GBP",
                "direction": direction,
                "idempotencyKey": str(uuid.uuid4()),
                "type": "InboundFunds",
                "description": "Random Tx",
                "reference": "rand"
            }, headers=new_headers())

            if tx_res.status_code == 201:
                tx_id = tx_res.json()["transactionId"]

                with self.lock:
                    if direction == "Credit":
                        self.expected_balances["pendingCreditBalance"] += amount
                    else:
                        self.expected_balances["availableBalance"] -= amount
                        self.expected_balances["pendingDebitBalance"] += amount

                if random.random() < 0.8:
                    self.client.post(f"/transactions/{tx_id}/post", headers=new_headers())
                    with self.lock:
                        if direction == "Credit":
                            self.expected_balances["ledgerBalance"] += amount
                            self.expected_balances["availableBalance"] += amount
                            self.expected_balances["pendingCreditBalance"] -= amount
                        else:
                            self.expected_balances["ledgerBalance"] -= amount
                            self.expected_balances["pendingDebitBalance"] -= amount

                    if random.random() < 0.1:
                        self.client.post(f"/transactions/{tx_id}/reverse", headers=new_headers())
                        with self.lock:
                            if direction == "Credit":
                                self.expected_balances["ledgerBalance"] -= amount
                                self.expected_balances["availableBalance"] -= amount
                            else:
                                self.expected_balances["ledgerBalance"] += amount
                                self.expected_balances["availableBalance"] += amount

        # ---------------- Holds ----------------
        elif roll < 0.7:
            amount = random.randint(1, 50)

            hold_res = self.client.post("/holds", json={
                "accountId": self.account_id,
                "amount": amount,
                "currencyCode": "GBP",
                "idempotencyKey": str(uuid.uuid4()),
                "type": "Regulatory",
                "description": "Random Hold",
                "reference": "randHold"
            }, headers=new_headers())

            if hold_res.status_code == 201:
                hold_id = hold_res.json()["holdId"]
                with self.lock:
                    self.expected_balances["holdBalance"] += amount
                    self.expected_balances["availableBalance"] -= amount

                r = random.random()
                if r < 0.4:
                    # Release
                    self.client.post(f"/holds/{hold_id}/release", headers=new_headers())
                    with self.lock:
                        self.expected_balances["holdBalance"] -= amount
                        self.expected_balances["availableBalance"] += amount
                elif r < 0.8:
                    # Settle
                    self.client.post(f"/holds/{hold_id}/settle", headers=new_headers())
                    with self.lock:
                        self.expected_balances["holdBalance"] -= amount
                        self.expected_balances["ledgerBalance"] -= amount
                else:
                    # Do nothing
                    pass


@events.test_stop.add_listener
def _(environment, **kwargs):
    transactions_created = environment.stats.get('/transactions', 'POST').num_requests
    holds_created = environment.stats.get('/holds', 'POST').num_requests
    total_created = transactions_created + holds_created

    print(f"\n[SUMMARY] Total entities created: {total_created}")
