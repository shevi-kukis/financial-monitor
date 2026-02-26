import { useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import type { RootState } from "../store/store";
import {
  setFilter,
  setInitialTransactions,
} from "../store/transactionsSlice";
import { api } from "../services/api";
import TransactionTable from "../components/TransactionTable";
import {
  TransactionStatus,
  mapStatusToLabel,
} from "../types/transaction";
import "../styles/monitor.css";

function MonitorPage() {
  const dispatch = useDispatch();

  const { transactions, filter } = useSelector(
    (state: RootState) => state.transactions
  );

  useEffect(() => {
    const loadTransactions = async () => {
      try {
        const response = await api.get("/transactions");

        // 👇 כאן אנחנו ממירים את הסטטוס ממספר לטקסט
        const mapped = response.data.map((t: any) => ({
          ...t,
          status: mapStatusToLabel(t.status),
        }));

        dispatch(setInitialTransactions(mapped));
      } catch (error) {
        console.error("Failed to load transactions", error);
      }
    };

    loadTransactions();
  }, [dispatch]);

  const filteredTransactions =
    filter === "all"
      ? transactions
      : transactions.filter((t) => {
          const statusText = String(t.status ?? "").toLowerCase();
          return statusText === filter;
        });

  const failedCount = transactions.filter(
    (t) => t.status === TransactionStatus.Failed
  ).length;

  const pendingCount = transactions.filter(
    (t) => t.status === TransactionStatus.Pending
  ).length;

  const completedCount = transactions.filter(
    (t) => t.status === TransactionStatus.Completed
  ).length;

  return (
    <div className="page-container">
      <h1>Live Financial Monitor</h1>

      <div className="controls">
        <button onClick={() => dispatch(setFilter("all"))}>
          All
        </button>

        <button onClick={() => dispatch(setFilter("pending"))}>
          Pending
        </button>

        <button onClick={() => dispatch(setFilter("completed"))}>
          Completed
        </button>

        <button onClick={() => dispatch(setFilter("failed"))}>
          Failed
        </button>
      </div>

      <div className="stats">
        Total: {transactions.length} | Pending: {pendingCount} |
        Completed: {completedCount} | Failed: {failedCount}
      </div>

      <TransactionTable transactions={filteredTransactions} />
    </div>
  );
}

export default MonitorPage;