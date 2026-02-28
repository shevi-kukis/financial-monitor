import { useState } from "react";
import { transactionsService } from "../services/transactionsService";
import "./../styles/monitor.css";

function AddPage() {
  const [amount, setAmount] = useState("");
  const [currency, setCurrency] = useState("USD");
  const [loading, setLoading] = useState(false);

  const generateOne = async () => {
    setLoading(true);
    await transactionsService.create(
      Math.floor(Math.random() * 1000),
      "USD"
    );
    setLoading(false);
  };

  const generateHundred = async () => {
    setLoading(true);
    const requests = Array.from({ length: 100 }, () =>
      transactionsService.create(
        Math.floor(Math.random() * 1000),
        "USD"
      )
    );
    await Promise.all(requests);
    setLoading(false);
  };

  const submitManual = async () => {
    if (!amount) return;

    setLoading(true);
    await transactionsService.create(Number(amount), currency);
    setAmount("");
    setLoading(false);
  };

  return (
    <div className="page-container">
      <h1>Transaction Simulator</h1>

      <div className="card">
        <h3>Manual Transaction</h3>

        <input
          type="number"
          placeholder="Amount"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
          disabled={loading}
        />

        <select
          value={currency}
          onChange={(e) => setCurrency(e.target.value)}
          disabled={loading}
        >
          <option value="USD">USD</option>
          <option value="EUR">EUR</option>
        </select>

        <button onClick={submitManual} disabled={loading}>
          Add Transaction
        </button>
      </div>

      <div className="card">
        <h3>Random Generator</h3>

        <button onClick={generateOne} disabled={loading}>
          Generate 1
        </button>

        <button
          onClick={generateHundred}
          style={{ marginLeft: "10px" }}
          disabled={loading}
        >
          Generate 100
        </button>
      </div>
    </div>
  );
}

export default AddPage;