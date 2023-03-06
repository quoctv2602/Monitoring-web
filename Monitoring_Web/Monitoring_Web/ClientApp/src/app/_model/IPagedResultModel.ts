export interface IPagedResultModel<T> {
  pageIndex: number;
  pageSize: number;
  totalRecords: number;
  pageCount: number;
  listItem: T[];
}
