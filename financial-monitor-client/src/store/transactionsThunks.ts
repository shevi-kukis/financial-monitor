import { createAsyncThunk } from "@reduxjs/toolkit";

import type { Transaction } from "../types/transaction";
import { transactionsService } from "../services/transactionsService";

export const fetchTransactions = createAsyncThunk<
  Transaction[]
>(
  "transactions/fetchTransactions",
  async () => {
    return await transactionsService.getAll();
  }
);