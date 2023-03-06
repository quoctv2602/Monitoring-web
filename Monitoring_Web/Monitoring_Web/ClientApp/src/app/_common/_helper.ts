import { ServiceStatusEnum } from './_enum';
export class Helper {
  static getIconServiceStatus(status: string): string {
    switch (status) {
      case ServiceStatusEnum.Running:
        return 'success';
      case ServiceStatusEnum.Stopped:
        return 'danger';
      case ServiceStatusEnum.Paused:
        return 'warning';
      case ServiceStatusEnum.Nothing:
        return 'light';
      case ServiceStatusEnum.Starting:
        return 'info';
      default:
        return 'dark';
    }
  }

  static emailRegex: RegExp = RegExp(
    '[A-Za-z0-9._%-]+@[A-Za-z0-9._%-]+\\.[a-z]{2,3}'
  );

  static defaultNodeType: number = 2;

  static getPager(
    totalItems: number,
    currentPage: number = 1,
    pageSize: number = 10
  ) {
    // default to first page
    currentPage = currentPage || 1;

    // default page size is 100
    pageSize = pageSize || 10;

    // calculate total pages
    var totalPages = Math.ceil(totalItems / pageSize);
    var startPage, endPage;
    // less than 10 total pages so show all
    startPage = 1;
    endPage = totalPages;

    // calculate start and end item indexes
    var startIndex = (currentPage - 1) * pageSize;
    var endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

    // create an array of pages to ng-repeat in the pager control
    var pages = this.range(startPage, endPage);

    // return object with all pager properties required by the view
    return {
      totalItems: totalItems,
      currentPage: currentPage,
      pageSize: pageSize,
      totalPages: totalPages,
      startPage: startPage,
      endPage: endPage,
      startIndex: startIndex,
      endIndex: endIndex,
      pages: pages,
    };
  }

  static range(min: number, max: number, step: number = 1): Array<any> {
    var input = [];
    for (var i = min; i <= max; i += step) {
      input.push(i);
    }
    return input;
  }
}
