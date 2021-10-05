using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Data
{
	public interface ISort
	{
		string PropertyName { get; set; }
		bool Ascending { get; set; }
	}
	public enum SortDirection
	{
		Default,
		Asc,
		Desc
	}
	public class Paging
	{
		public static int DefaultPageSize = 20; //not const, so it can be changed by consumer.
		public Paging()
			: this(1, DefaultPageSize)
		{
		}

		public Paging(int page, int pageSize)
			: this(page, pageSize, null, null)
		{
		}

		public Paging(int page, int pageSize, string sort, string filter) : this(page, pageSize, sort, filter, null)
		{
		}

		//note order changed here to disambiguate calls above
		public Paging(List<ISort> sorting, int page, int pageSize, string filter) : this(page, pageSize, null, filter, sorting)
		{

		}


		public Paging(int page, int pageSize, string sort, string filter, List<ISort> sorting)
		{
			Page = page;
			PageSize = pageSize;
			Sort = sort;
			Filter = filter;
			Sorting = sorting ?? new List<ISort>();
		}

		private int? _skipCount;

		public static Paging FromSkipTake(int skip, int take, string sort, string filter, SortDirection sortDirection)
		{
			//Absolute value will round us down to the page we are on. We are one paged in paging. 
			var pageNo = Math.Abs(skip / take) + 1;

			var paging = new Paging(pageNo, take, sort, filter)
			{
				Skip = skip,
				SortDirection = sortDirection
			};

			return paging;
		}

		public int Skip
		{
			get
			{
				//If we have a non page size skip count return this.
				if (_skipCount.HasValue)
					return _skipCount.Value;

				if (Page <= 1)
					return 0;

				//We are one based in pages, so the number to skip is the number of
				//pages before time page, times the page size. 
				return (Page - 1) * (int)PageSize;
			}
			set => _skipCount = value;
		}


		public int PageSize { get; set; }

		public int Take => PageSize;

		public int Page { get; set; }

		public IList<ISort> Sorting { get; set; }

		public string Sort { get; set; }

		public SortDirection SortDirection { get; set; }

		/// <summary>
		/// Used as a search field
		/// </summary>
		public string Filter { get; set; }

		public override string ToString()
		{
			return $"Page #{Page}";
		}

		public static Paging None => new Paging(1, int.MaxValue);

		public static Paging Default => new Paging(1, DefaultPageSize);
	}
}
