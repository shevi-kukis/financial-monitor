import { api } from "./api";
import { normalizeTransaction } from "../types/transaction";
import type { Transaction } from "../types/transaction";

export const transactionsService = {
  async getAll(): Promise<Transaction[]> {
    const response = await api.get("/transactions");
    return response.data.map(normalizeTransaction);
  },

  async create(amount: number, currency: string) {
    return api.post("/transactions", { amount, currency });
  },
};