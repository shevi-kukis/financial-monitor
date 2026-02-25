import { useSelector } from "react-redux";
import type { RootState } from "../store/store";
import { TransactionStatus, type Transaction } from "../types/transaction";
import "../styles/monitor.css";

interface Props {
  transactions: Transaction[];
}

function TransactionTable({ transactions }: Props) {
  const lastAddedId = useSelector(
    (state: RootState) => state.transactions.lastAddedId
  );

  return (
    <table className="transaction-table">
      <thead>
        <tr>
          <th>Amount</th>
          <th>Currency</th>
          <th>Status</th>
          <th>Created</th>
        </tr>
      </thead>

      <tbody>
        {transactions.map((t) => {
          const statusClass =
            t.status === TransactionStatus.Failed
              ? "status-failed"
              : t.status === TransactionStatus.Pending
              ? "status-pending"
              : "status-completed";

          const newRowClass =
            t.id === lastAddedId ? "new-transaction" : "";

          return (
            <tr
              key={t.id}
              className={`transaction-row ${statusClass} ${newRowClass}`}
            >
              <td>{t.amount}</td>
              <td>{t.currency}</td>
              <td>{t.status}</td>
              <td>{new Date(t.createdAt).toLocaleTimeString()}</td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
}

export default TransactionTable;