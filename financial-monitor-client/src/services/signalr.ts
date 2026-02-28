import * as signalR from "@microsoft/signalr";
import { store } from "../store/store";
import { normalizeTransaction } from "../types/transaction";
import { addTransaction } from "../store/transactionsSlice";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${import.meta.env.VITE_API_BASE_URL}/transactionHub`)
  .withAutomaticReconnect()
  .build();

export const startSignalR = async () => {
  await connection.start();

  connection.on("ReceiveTransaction", (transaction: any) => {
    console.log("🟢 SignalR received:", transaction);

    const normalized = normalizeTransaction(transaction);

    store.dispatch(addTransaction(normalized));
  });
};