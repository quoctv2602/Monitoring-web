export interface IPaginationModel {
  pageNumber: number | 1;
  pageSize: number | 50;
  pageSizeList: number[] | [];
  totalPage: number | 0;
  listPageNumber: number[] | [];
  totalItem: number | 0;
  itemFrom: number | 1;
  itemTo: number | 50;
}
