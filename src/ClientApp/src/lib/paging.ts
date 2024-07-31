export type Paging = {
    pageSize?: number;
    page?: number;
    sort?: string;
    sortDirection?: string;
    filter?: string;
}

export type PagedList<T> = {
    items: Array<T>;
    totalCount: number;
    paging: Paging;
};

