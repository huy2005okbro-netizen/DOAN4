export const formatDate = (date: string | Date): string =>
  new Intl.DateTimeFormat("vi-VN").format(new Date(date));

export const formatDateTime = (date: string | Date): string =>
  new Intl.DateTimeFormat("vi-VN", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(date));
