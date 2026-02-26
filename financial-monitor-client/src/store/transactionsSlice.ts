import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { Transaction } from "../types/transaction";
import { mapStatusToLabel } from "../types/transaction";

type FilterType = "all" | "pending" | "completed" | "failed";

interface TransactionsState {
  transactions: Transaction[];
  filter: FilterType;
  lastAddedId: string | null;
}

const initialState: TransactionsState = {
  transactions: [],
  filter: "all",
  lastAddedId: null,
};

const normalizeTransaction = (t: Transaction): Transaction => ({
  ...t,
  status: mapStatusToLabel(t.status),
});

const transactionsSlice = createSlice({
  name: "transactions",
  initialState,
  reducers: {
    addTransaction(state, action: PayloadAction<Transaction>) {
      const normalized = normalizeTransaction(action.payload);

      const existingIndex = state.transactions.findIndex(
        (t) => t.id === normalized.id
      );

      if (existingIndex !== -1) {
        state.transactions[existingIndex] = normalized;
      } else {
        state.transactions.unshift(normalized);
        state.lastAddedId = normalized.id;

        if (state.transactions.length > 1000) {
          state.transactions.pop();
        }
      }
    },

    addTransactionsBatch(state, action: PayloadAction<Transaction[]>) {
      action.payload.forEach((transaction) => {
        const normalized = normalizeTransaction(transaction);

        const existingIndex = state.transactions.findIndex(
          (t) => t.id === normalized.id
        );

        if (existingIndex !== -1) {
          state.transactions[existingIndex] = normalized;
        } else {
          state.transactions.unshift(normalized);
        }
      });

      if (state.transactions.length > 1000) {
        state.transactions.length = 1000;
      }

      if (action.payload.length > 0) {
        state.lastAddedId = action.payload[0].id;
      }
    },

    setInitialTransactions(state, action: PayloadAction<Transaction[]>) {
      state.transactions = action.payload.map(normalizeTransaction);
      state.lastAddedId = null;
    },

    setFilter(state, action: PayloadAction<FilterType>) {
      state.filter = action.payload;
    },

    clearTransactions(state) {
      state.transactions = [];
      state.lastAddedId = null;
    },
  },
});

export const {
  addTransaction,
  addTransactionsBatch,
  setFilter,
  clearTransactions,
  setInitialTransactions,
} = transactionsSlice.actions;

export default transactionsSlice.reducer;