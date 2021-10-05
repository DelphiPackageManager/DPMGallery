namespace DPMGallery.Data
{
    public class PageInfo
    {
        public PageInfo(int skip, int pageSize)
        {
            Skip = skip;
            PageSize = pageSize;
        }

        public int PageSize { get; set; }

        public int Skip { get; set; }

        public static PageInfo Default
        {
            get
            {
                return new PageInfo(0, 25);
            }
        }

        public static PageInfo FromPage(int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            return new PageInfo(skip, pageSize);
        }

        public string ToSQL()
        {
            return string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", Skip, PageSize);
        }
    }
}
