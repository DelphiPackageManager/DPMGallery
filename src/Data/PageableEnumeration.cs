using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.Data
{

	public interface IPageableEnumeration<T> : IEnumerable<T> where T : class
	{
		int HashCode { get; set; }
		bool IsTotalCountCorrect { get; set; }
		List<T> Items { get; }
		int PageSize { get; }
		int TotalCount { get; }
		int PageNo { get; set; }
		Paging Paging { get; }
		PageableEnumeration<T> AsPageable();
		IPageableEnumeration<Dest> ConvertTo<Dest>(Func<T, Dest> converter) where Dest : class;
	}

	[DataContract]
	[Serializable]
	public class PageableEnumeration<T> : IPageableEnumeration<T> where T : class
	{
		public PageableEnumeration()
		{
			Items = new List<T>();
			TotalCount = 0;
			//NOTE: When calling this it kinda makes the pageable information no longer useful.
			PageSize = -1;
			PageNo = -1;
			IsTotalCountCorrect = true;
		}

		public PageableEnumeration(IEnumerable<T> items, int totalCount, int pageSize = 20, int pageNo = 0)
		{
			Items = items?.ToList() ?? new List<T>();

			TotalCount = totalCount;
			PageSize = pageSize;
			PageNo = pageNo;
			IsTotalCountCorrect = true;
		}

		public PageableEnumeration(IEnumerable<T> items, int totalCount, Paging paging)
		{
			Items = items?.ToList() ?? new List<T>();

			TotalCount = totalCount;
			PageSize = paging.PageSize;
			PageNo = paging.Page;
			IsTotalCountCorrect = true;
		}

		[DataMember(Order = 1)]
		public List<T> Items { get; private set; }

		[DataMember(Order = 2)]
		public int TotalCount { get; private set; }

		[DataMember(Order = 3)]
		public int PageSize { get; private set; }

		[DataMember(Order = 4)]
		public bool IsTotalCountCorrect { get; set; }

		[DataMember(Order = 5)]
		public int PageNo { get; set; }

		[DataMember(Order = 6)]
		public int HashCode { get; set; }

		public Paging Paging => new Paging(PageNo, PageSize, string.Empty, string.Empty);

		public PageableEnumeration<T> AsPageable() => this;

		public IEnumerator<T> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}

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

		public IPageableEnumeration<Dest> ConvertTo<Dest>(Func<T, Dest> converter) where Dest : class
		{
			return ConvertTo(this, converter);
		}


		public static PageableEnumeration<Dest> ConvertTo<Dest>(PageableEnumeration<T> original, Func<T, Dest> converter) where Dest : class
		{
			var convertedItems = new List<Dest>();
			foreach (var item in original.Items)
			{
				convertedItems.Add(converter(item));
			}

			var converted = new PageableEnumeration<Dest>(convertedItems, original.TotalCount, original.PageSize, original.PageNo);

			return converted;
		}

		public static PageableEnumeration<T> Empty => new PageableEnumeration<T>(new T[0], 0, 0, 0);
	}

}
