using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DPMGallery.Models
{
    [Description("Representation of how the list of models are returned. To reduce bandwidth, and server load pagination is implemented to limit the number of models returned to those required in a paged view by the user.")]
    public class PaginationModel
    {

        [JsonRequired]
        [Description("The size of the paging returned. The paging determines the maximum number of items to return in a single page. It also affects how the previous, and next page links are constructed.")]
        //[Range(1, HttpConfig.MaxPageSize)]
        public int PageSize { get; set; }

        [JsonRequired]
        [Description("The page in the list of pages available. The page number is determined by the number of items skipped, and then taken from the total number available.")]
        public int Page { get; set; }

        [JsonRequired]
        [Description("The total number of items available to be paged through.")]
        public int TotalItems { get; set; }
    }
}
