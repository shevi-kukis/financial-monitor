import { api } from "../services/api";
import "./../styles/monitor.css";

function AddPage() {
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

  return (
    <div className="page-container">
      <h1>Transaction Simulator</h1>

      <div className="controls">
        <button onClick={generateOne}>
          Generate 1
        </button>

        <button onClick={generateHundred}>
          Generate 100
        </button>
      </div>
    </div>
  );
}

export default AddPage;