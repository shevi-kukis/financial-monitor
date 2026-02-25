import * as signalR from "@microsoft/signalr";
import { store } from "../store/store";
import { addTransaction } from "../store/transactionsSlice";
import type { Transaction } from "../types/transaction";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${import.meta.env.VITE_API_BASE_URL}/transactionHub`)
  .withAutomaticReconnect()
  .build();

export const startSignalR = async () => {
  await connection.start();

  connection.on("ReceiveTransaction", (transaction: Transaction) => {
    store.dispatch(addTransaction(transaction));
  });
};