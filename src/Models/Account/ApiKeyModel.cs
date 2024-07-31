using DPMGallery.Entities;
using System;

namespace DPMGallery.Models.Account
{
    public record ApiKeyModel(int Id, string Name, string Key, int UserId, DateTimeOffset ExpiresUTC, string GlobPattern, string Packages, ApiKeyScope Scopes, bool Revoked);
}
