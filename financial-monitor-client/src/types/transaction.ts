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


export const mapStatusToLabel = (status: string | number) => {
  switch (status) {
    case 0:
    case "0":
    case "Pending":
      return TransactionStatus.Pending;

    case 1:
    case "1":
    case "Completed":
      return TransactionStatus.Completed;

    case 2:
    case "2":
    case "Failed":
      return TransactionStatus.Failed;

    default:
      return TransactionStatus.Pending;
  }
};