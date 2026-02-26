import { useState } from "react";
import { api } from "../services/api";
import "./../styles/monitor.css";

function AddPage() {
  const [amount, setAmount] = useState("");
  const [currency, setCurrency] = useState("USD");

  const generateOne = async () => {
    await api.post("/transactions", {
      amount: Math.floor(Math.random() * 1000),
      currency: "USD",
    });
  };

  const generateHundred = async () => {
    const requests = Array.from({ length: 100 }, () =>
      api.post("/transactions", {
        amount: Math.floor(Math.random() * 1000),
        currency: "USD",
      })
    );

    await Promise.all(requests);
  };

  const submitManual = async () => {
    if (!amount) return;

    await api.post("/transactions", {
      amount: Number(amount),
      currency,
    });

    setAmount("");
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
        />

        <select
          value={currency}
          onChange={(e) => setCurrency(e.target.value)}
        >
          <option value="USD">USD</option>
          <option value="EUR">EUR</option>
        </select>

        <button onClick={submitManual}>
          Add Transaction
        </button>
      </div>

      <div className="card">
        <h3>Random Generator</h3>

        <button onClick={generateOne}>
          Generate 1
        </button>

        <button onClick={generateHundred} style={{ marginLeft: "10px" }}>
          Generate 100
        </button>
      </div>
    </div>
  );
}

export default AddPage;