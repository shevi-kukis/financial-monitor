export const TransactionStatus = {
  Pending: "Pending",
  Completed: "Completed",
  Failed: "Failed",
} as const;

export type TransactionStatus =
  (typeof TransactionStatus)[keyof typeof TransactionStatus];

export type Transaction = {
  id: string;
  amount: number;
  currency: string;
  status: TransactionStatus;
  createdAt: string;
};


