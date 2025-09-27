# locust -f multi_balance_test.py --host http://localhost:5000 --headless --skip-log-setup --users 30 --spawn-rate 10 --run-time 60s --stop-timeout 60s

from locust import FastHttpUser, task, constant, events
import threading
import requests
from utils import STARTING_BALANCE, new_headers, create_and_fund_account, verify_balances, perform_random_operation

class BalanceUser(FastHttpUser):
    wait_time = constant(0)

    def on_start(self):
        """Runs per VU: create and fund account"""
        self.expected_balances = {
            "ledgerBalance": 0,
            "availableBalance": 0,
            "pendingCreditBalance": 0,
            "pendingDebitBalance": 0,
            "holdBalance": 0,
        }

        self.lock = threading.Lock()
        self.account_id = create_and_fund_account(self.environment.host)
        self.expected_balances["ledgerBalance"] += STARTING_BALANCE
        self.expected_balances["availableBalance"] += STARTING_BALANCE

        print(f"[VU {self.environment.runner.user_count}] Setup done. Account ID: {self.account_id}")

    def on_stop(self):
        """Compare API balances with expected and DB values"""
        actual, db, checks = verify_balances(self.account_id, self.expected_balances, self.environment.host)

        if all(checks.values()):
            print(f"[Account {self.account_id}] ✅ Balance match")
        else:
            print(f"\n[Account {self.account_id}] ❌ Balance mismatch")
            print("Expected:", self.expected_balances)
            print("Actual (API):", actual)
            print("Database:", db)
            print("Checks:", checks)

    @task
    def random_operation(self):
        perform_random_operation(self.client, self.account_id, self.expected_balances)

@events.test_stop.add_listener
def _(environment, **kwargs):
    transactions_created = environment.stats.get('/transactions', 'POST').num_requests
    holds_created = environment.stats.get('/holds', 'POST').num_requests
    total_created = transactions_created + holds_created

    print(f"\n[SUMMARY] Total entities created: {total_created}")
          