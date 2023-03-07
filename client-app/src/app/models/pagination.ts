export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}


// We're also going to want to get access to the pagination data that we get back inside the header.
// because this is a class we need to create constructor and give it initial value
export class PaginatedResult<T> { 
  data: T;
  pagination: Pagination;

  constructor(data: T, pagination: Pagination) {
      this.data = data;
      this.pagination = pagination;
  }
}



export class PagingParams {
  pageNumber;
  pageSize;

  constructor(pageNumber = 1, pageSize = 2) {
      this.pageNumber = pageNumber;
      this.pageSize = pageSize;
  }
}