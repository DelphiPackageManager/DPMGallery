using DPMGallery.Entities;
using System;

namespace DPMGallery.Models
{
    public class ApiKeyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public int UserId { get; set; }

        public DateTime ExpiresUTC { get; set; }

        //either a glob pattern .
        public string GlobPattern { get; set; }

        public string Packages { get; set; }

        public int Scopes { get; set; }

        public bool Revoked { get; set; }
    }
}
