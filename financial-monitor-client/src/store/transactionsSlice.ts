import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { Transaction } from "../types/transaction";
import { fetchTransactions } from "./transactionsThunks";
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

const transactionsSlice = createSlice({
  name: "transactions",
  initialState,
  reducers: {
    addTransaction(state, action: PayloadAction<Transaction>) {
      const existingIndex = state.transactions.findIndex(
        (t) => t.id === action.payload.id
      );

      if (existingIndex !== -1) {
       
        state.transactions[existingIndex] = action.payload;
      } else {
      
        state.transactions.unshift(action.payload);
        state.lastAddedId = action.payload.id;

        if (state.transactions.length > 1000) {
          state.transactions.pop();
        }
      }
    },

    addTransactionsBatch(state, action: PayloadAction<Transaction[]>) {
      action.payload.forEach((transaction) => {
        const existingIndex = state.transactions.findIndex(
          (t) => t.id === transaction.id
        );

        if (existingIndex !== -1) {
          state.transactions[existingIndex] = transaction;
        } else {
          state.transactions.unshift(transaction);
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
      state.transactions = action.payload;
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
 extraReducers: (builder) => {
    builder.addCase(fetchTransactions.fulfilled, (state, action) => {
      state.transactions = action.payload;
      state.lastAddedId = null;
    });
   }
});

export const {
  addTransaction,
  addTransactionsBatch,
  setFilter,
  clearTransactions,
  setInitialTransactions,
} = transactionsSlice.actions;

export default transactionsSlice.reducer;