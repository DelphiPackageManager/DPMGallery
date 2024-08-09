using DPMGallery.Entities;
using System;

namespace DPMGallery.Models.Account
{
    public record CreateApiKeyModel(string Name,  int ExpiresInDays, string GlobPattern, string Packages, ApiKeyScope Scopes, int PackageOwner);
}
