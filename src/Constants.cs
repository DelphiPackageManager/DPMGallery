using System;
using System.Linq;

namespace DPMGallery
{
    public partial class Constants
    {
        public const string DefaultSiteUrl = "https://delphipm.org";
        public static class Claims
        {
            public const string UserName = "claim.username";
            public const string UserId = "claim.userid";
            public const string ApiKeyId = "claim.apikey-id";
            public const string ApiKeyExpires = "claim.apikey-expires";
            public const string ApiKeyGlob = "claim.apikey-glob";
            public const string ApiKeyPackages = "claim.apikey-packages";
        }

        public static class Database
        {
            public const string Collation = "collat_ci";
            public static class FieldLength
            {

                public const int Short = 128;
                public const int Medium = 256;
                public const int Long = 1024;
                public const int VeryLong = 10240;
                public const int Max = 10485760; // Postgres maximum varying character length 
            }
            public static class TableNames
            {
                //Note these are premised on using snakecase namimg

                public const string Users = "asp_net_users";
                public const string Roles = "asp_net_roles";

                public const string UserRoles = "asp_net_user_roles";
                public const string UserClaims = "asp_net_user_claims";
                public const string UserLogins = "asp_net_user_logins";
                public const string UserTokens = "asp_net_user_tokens";

                public const string RoleClaims = "asp_net_role_claims";

                public const string ApiKey = "api_key";
                public const string OrganisationMember = "organisation_member";
                public const string Package = "package";
                public const string PackageTargetPlatform = "package_targetplatform";
                public const string PackageOwner = "package_owner";
                public const string PackageVersion = "package_version";
                public const string PackageVersionProcess = "package_version_process";
                public const string PackageDependency = "package_dependency";
                public const string ReservedPrefix = "reserved_prefix";
                public const string ReservedPrefixOwner = "reserved_prefix_owner";
                public static string PrimaryKeyNamer(string tableName, params string[] columns)
                {
                    string name = $"pk_{tableName}";
                    foreach (string column in columns)
                    {
                        name += $"_{column}";
                    }

                    return name;
                }


                public static string ForeignKeyNamer(string tableName, string from, string to)
                {
                    return "fk_" + tableName + "_" + from + "_to_" + to;
                }

                public static string RelationshipNamer(string has, string many)
                {
                    return has + many;
                }


            }

            public static class ViewNames
            {
                public const string SearchStableVersion = "search_stable_version";
                public const string SearchLatestVersion = "search_latest_version";

            }
        }
    }
}
