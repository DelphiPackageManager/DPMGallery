using System.Collections.Generic;
using System;

namespace DPMGallery.Data
{
    public interface IPagedList<T> where T : class //: IEnumerable<T> where T : class
    {
        int HashCode { get; set; }
        List<T> Items { get; }
        int TotalCount { get; }
        Paging Paging { get; }
        PagedList<T> AsPageable();
        IPagedList<Dest> ConvertTo<Dest>(Func<T, Dest> converter) where Dest : class;
    }
}
