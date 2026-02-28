import { configureStore } from "@reduxjs/toolkit";
import transactionsReducer from "./transactionsSlice";

const loggerMiddleware = (storeAPI: any) => (next: any) => (action: any) => {
  console.log("🟡 Action dispatched:", action.type);

  const result = next(action);

  console.log("🟢 New state:", storeAPI.getState().transactions);

  return result;
};

export const store = configureStore({
  reducer: {
    transactions: transactionsReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(loggerMiddleware),
});
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;