# locust -f single_balance_test.py --host http://localhost:5000 --headless --skip-log-setup --users 30 --spawn-rate 10 --run-time 60s --stop-timeout 60s

from locust import FastHttpUser, task, constant, events
from utils import STARTING_BALANCE, new_headers, create_and_fund_account, verify_balances, perform_random_operation
import threading

lock = threading.Lock()
account_id = None

expected_balances = {
    "ledgerBalance": 0,
    "availableBalance": 0,
    "pendingDebitBalance": 0,
    "pendingCreditBalance": 0,
    "holdBalance": 0,
}

@events.test_start.add_listener
def on_test_start(environment, **kwargs):
    global account_id, expected_balances
    account_id = create_and_fund_account(environment.host, account_name="Locust Shared Account")
    expected_balances["ledgerBalance"] += STARTING_BALANCE
    expected_balances["availableBalance"] += STARTING_BALANCE

    print(f"[SETUP] Shared Account created: {account_id}")

@events.test_stop.add_listener
def on_test_stop(environment, **kwargs):
    global account_id, expected_balances
    actual, db, checks = verify_balances(account_id, expected_balances, environment.host)

    if all(checks.values()):
        print(f"[Account {account_id}] ✅ Balance match")
    else:
        print(f"\n[Account {account_id}] ❌ Balance mismatch")
        print("Expected:", expected_balances)
        print("Actual (API):", actual)
        print("Database:", db)
        print("Checks:", checks)

    transactions_created = environment.stats.get('/transactions', 'POST').num_requests
    holds_created = environment.stats.get('/holds', 'POST').num_requests
    total_created = transactions_created + holds_created

    print(f"\n[SUMMARY] Total entities created: {total_created}")

class BalanceUser(FastHttpUser):
    wait_time = constant(0)

    @task
    def random_operation(self):
        global account_id, expected_balances, lock
        perform_random_operation(self.client, account_id, expected_balances)
