using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace DPMGallery.Data
{
    [Serializable]
    public class PagedList<T> : IPagedList<T> where T : class
    {
        public PagedList()
        {
            Items = [];
            TotalCount = 0;
            Paging = Paging.Default;
        }

        public PagedList(List<T> items, int totalCount, int pageSize = 20, int pageNo = 0) : this()
        {
            Items = items ?? [];

            TotalCount = totalCount;

            Paging.PageSize = pageSize;
            Paging.Page = pageNo;
            Paging.EnsureValidPage(TotalCount);
        }

        public PagedList(List<T> items, int totalCount, Paging paging)
        {
            Items = items ?? [];

            TotalCount = totalCount;
            Paging = paging;
            Paging.EnsureValidPage(TotalCount);
        }

        public List<T> Items { get; private set; }

        public Paging Paging { get; private set; }
        public int TotalCount { get; private set; }


        [JsonIgnore]
        public int HashCode { get; set; }

        public PagedList<T> AsPageable() => this;

        /*public IEnumerator<T> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}*/

        /*
		public PageableEnumeration<DTO> MapTo<DTO>() where DTO : class
		{
			var mapped = Mapping<T, DTO>.Map(Items);

			return new PageableEnumeration<DTO>(mapped, TotalCount, PageSize, PageNo);
		}

		public PageableEnumeration<DTO> MapTo<DTO>(Action<T, DTO> modifier) where DTO : class
		{
			var mapped = Mapping<T, DTO>.Map(Items, modifier);

			return new PageableEnumeration<DTO>(mapped, TotalCount, PageSize, PageNo);
		}
		*/

        public IPagedList<Dest> ConvertTo<Dest>(Func<T, Dest> converter) where Dest : class
        {
            return ConvertTo(this, converter);
        }


        public static PagedList<Dest> ConvertTo<Dest>(PagedList<T> original, Func<T, Dest> converter) where Dest : class
        {
            var convertedItems = new List<Dest>();
            foreach (T item in original.Items)
                convertedItems.Add(converter(item));

            var converted = new PagedList<Dest>(convertedItems, original.TotalCount, original.Paging);

            return converted;
        }

        [JsonIgnore]
        public static PagedList<T> Empty => new([], 0, 0, 0);
    }

}
