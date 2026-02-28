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


const statusMap: Record<number, TransactionStatus> = {
  0: TransactionStatus.Pending,
  1: TransactionStatus.Completed,
  2: TransactionStatus.Failed,
};

export const mapStatusToLabel = (status: string | number): TransactionStatus => {

  return statusMap[+status] ?? TransactionStatus.Pending;
};

export const normalizeTransaction = (t: any): Transaction => ({
  ...t,
  status: mapStatusToLabel(t.status),
});