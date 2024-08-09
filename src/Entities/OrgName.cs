using DPMGallery.Data;

namespace DPMGallery.Entities
{
    public class OrgName
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

    }
}
