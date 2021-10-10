using FluentMigrator;

using FL = DPMGallery.Constants.Database.FieldLength;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;

using DB = DPMGallery.Constants.Database;

namespace DPMGallery.DBMigration.Conventions
{


	[Migration(1, "Initial Schema creation")]
	public class DPMGallery_1_001 : Migration
	{
		private const string LatestStableVersionView = @"create view " + V.SearchStableVersion + @" as
														select 
															p.id,
															pv.id as versionid,
															p.packageid, 
															tp.compiler_version, 
															tp.platform,
															pv.version as latest_version,
															pv.version as latest_stable_version,
															pv.is_prerelease, 
															pv.is_commercial, 
															pv.is_trial, 
															p.reserved_prefix_id is not null as is_reserved,
															pv.description,
															pv.authors,
															pv.icon,
															pv.read_me,
															pv.release_notes,
															pv.license, 
															pv.project_url,
															pv.repository_url,
															pv.repository_type,
															pv.repository_branch,
															pv.repository_commit,
															pv.listed,
															pv.status,
															pv.tags,
															pv.published_utc,
															pv.deprecation_state,
															pv.deprecation_message,
															pv.alternate_package,
															pv.hash,
															pv.hash_algorithm,
															p.downloads as total_downloads,                           
															pv.downloads as version_downloads   
														from package p 
														left  join package_targetplatform tp on
														p.id = tp.package_id
														left join package_version pv
														on tp.latest_version = pv.id							
														where pv.status = 200
														and pv.listed = true
														order by p.id, tp.compiler_version, tp.platform";

		private const string LatestVersionView = @"create view " + V.SearchLatestVersion + @" as
													select 
														p.id,
														pvl.id as versionid,
														p.packageid, 
														tp.compiler_version, 
														tp.platform,
														pvl.version as latestversion,
														pv.version as lateststableversion,
														pvl.is_prerelease, 
														pvl.is_commercial, 
														pvl.is_trial, 
														p.reserved_prefix_id is not null as is_reserved,
														pvl.description,
														pvl.authors,
														pvl.icon,
														pvl.read_me,
														pvl.release_notes,
														pvl.license, 
														pvl.project_url,
														pvl.repository_url,
														pvl.repository_type,
														pvl.repository_branch,
														pvl.repository_commit,
														pvl.listed,
														pvl.status,
													    pvl.tags,
														pvl.published_utc,
														pvl.deprecation_state,
														pvl.deprecation_message,
														pvl.alternate_package,
														pvl.hash,
														pvl.hash_algorithm,
														p.downloads as totaldownloads,
														pvl.downloads as versiondownloads
													from package p 
													left join package_targetplatform tp on
													p.id = tp.package_id
													left join package_version pvl
													on tp.latest_version = pvl.id
													left join package_version pv
													on tp.latest_stable_version = pv.id
													where pvl.status = 200
													and pvl.listed = true
													order by p.id, tp.compiler_version, tp.platform";

		public override void Up()
		{
			// Case insentive collation for use in equals statements
			// NOTE : This collation cannot be used with LIKE! - Use ILIKE
			// packageid ILIKE '%ndy%' COLLATE "C" 
			Execute.Sql($@"CREATE COLLATION IF NOT EXISTS {DB.Collation} (
						  provider = 'icu',
						  locale = '@colStrength=secondary',
						  deterministic = false
						);");

			Create.Table(T.Users)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("user_name").AsString(FL.Medium, DB.Collation).NotNullable()
				.WithColumn("normalized_user_name").AsString(FL.Medium).NotNullable()
				.WithColumn("email").AsString(FL.Medium).NotNullable()
				.WithColumn("normalized_email").AsString(FL.Medium).NotNullable()
				.WithColumn("email_confirmed").AsBoolean().NotNullable()
				.WithColumn("password_hash").AsString(FL.Long).Nullable()
				.WithColumn("security_stamp").AsString(FL.Long).NotNullable()
				.WithColumn("concurrency_stamp").AsString(FL.Long).Nullable()
				.WithColumn("phone_number").AsString(FL.Medium).Nullable()
				.WithColumn("phone_number_confirmed").AsBoolean()
				.WithColumn("two_factor_enabled").AsBoolean().NotNullable()
				.WithColumn("lockout_end").AsDateTime2().WithDefaultValue(null).Nullable()
				.WithColumn("lockout_enabled").AsBoolean().NotNullable()
				.WithColumn("access_failed_count").AsInt32().WithDefaultValue(0).NotNullable()
				.WithColumn("is_organisation").AsBoolean().NotNullable()
				.WithColumn("account_suspended").AsBoolean().NotNullable();

			Create.Index("ix_users_user_name").OnTable(T.Users).OnColumn("user_name").Ascending().WithOptions().Unique();
			Create.Index("ix_users_email").OnTable(T.Users).OnColumn("email").Ascending().WithOptions().Unique().WithOptions().NonClustered();

			Create.Table(T.UserClaims)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("user_id").AsInt32().NotNullable()
				.WithColumn("claim_type").AsString(FL.Medium, DB.Collation).NotNullable()
				.WithColumn("claim_value").AsString(FL.Max, DB.Collation).NotNullable();

			Create.Index("ix_user_claims_user_id").OnTable(T.UserClaims).OnColumn("user_id").Ascending();
			Create.ForeignKey($"fk_{T.UserClaims}_{T.Users}_user_id").FromTable(T.UserClaims).ForeignColumn("user_id").ToTable(T.Users).PrimaryColumn("id");


			Create.Table(T.UserLogins)
				.WithColumn("login_provider").AsString(FL.Short, DB.Collation).NotNullable()
				.WithColumn("provider_key").AsString(FL.Short, DB.Collation).NotNullable()
				.WithColumn("provider_display_name").AsString(FL.Medium, DB.Collation).NotNullable()
				.WithColumn("user_id").AsInt32().NotNullable();

			Create.PrimaryKey(T.PrimaryKeyNamer(T.UserLogins, "login_provider", "provider_key")).OnTable(T.UserLogins).Columns("login_provider", "provider_key");
			Create.Index("ix_user_logins_user_id").OnTable(T.UserLogins).OnColumn("user_id").Ascending();
			Create.ForeignKey($"fk_{T.UserLogins}_{T.Users}_user_id").FromTable(T.UserLogins).ForeignColumn("user_id").ToTable(T.Users).PrimaryColumn("id");

			Create.Table(T.UserTokens)
				.WithColumn("user_id").AsInt32().NotNullable()
				.WithColumn("login_provider").AsString(FL.Short, DB.Collation).NotNullable()
				.WithColumn("name").AsString(FL.Short, DB.Collation).NotNullable()
				.WithColumn("value").AsString(FL.Max);

			Create.PrimaryKey(T.PrimaryKeyNamer(T.UserTokens, "user_id", "login_provider", "name")).OnTable(T.UserTokens).Columns("user_id", "login_provider", "name");
			Create.ForeignKey($"fk_{T.UserTokens}_{T.Users}_user_id").FromTable(T.UserTokens).ForeignColumn("user_id").ToTable(T.Users).PrimaryColumn("id");


			Create.Table(T.Roles)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("name").AsString(FL.Medium, DB.Collation).NotNullable()
				.WithColumn("normalized_name").AsString(FL.Medium).NotNullable()
				.WithColumn("concurrency_stamp").AsString(FL.Long).Nullable();

			Create.Index("ix_roles_name").OnTable(T.Roles).OnColumn("normalized_name").Ascending().WithOptions().Unique();

			Create.Table(T.UserRoles)
				.WithColumn("user_id").AsInt32().NotNullable()
				.WithColumn("role_id").AsInt32().NotNullable();

			Create.PrimaryKey(T.PrimaryKeyNamer(T.UserRoles, "user_id", "role_id")).OnTable(T.UserRoles).Columns("user_id", "role_id");
			Create.Index("ix_user_roles_role_id").OnTable(T.UserRoles).OnColumn("role_id").Ascending();

			Create.ForeignKey($"fk_{T.UserRoles}_{T.Users}_user_id").FromTable(T.UserRoles).ForeignColumn("user_id").ToTable(T.Users).PrimaryColumn("id");
			Create.ForeignKey($"fk_{T.UserRoles}_{T.Roles}_role_id").FromTable(T.UserRoles).ForeignColumn("role_id").ToTable(T.Roles).PrimaryColumn("id");

			Create.Table(T.RoleClaims)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("role_id").AsInt32().NotNullable()
				.WithColumn("claim_type").AsString(FL.Medium, DB.Collation).NotNullable()
				.WithColumn("claim_value").AsString(FL.Max).Nullable();

			Create.Index("ix_roles_claims_role_id").OnTable(T.RoleClaims).OnColumn("role_id").Ascending();
			Create.ForeignKey($"fk_{T.RoleClaims}_{T.Roles}_role_id").FromTable(T.RoleClaims).ForeignColumn("role_id").ToTable(T.Roles).PrimaryColumn("id");

			Create.Table(T.OrganisationMember)
				.WithColumn("org_id").AsInt32().PrimaryKey().NotNullable()
				.WithColumn("member_id").AsInt32().PrimaryKey().NotNullable()
				.WithColumn("role").AsInt32().WithDefaultValue(0).NotNullable();

			Create.ForeignKey($"fk_{T.OrganisationMember}_{T.Users}_org_id").FromTable(T.OrganisationMember).ForeignColumn("org_id").ToTable(T.Users).PrimaryColumn("id");
			Create.ForeignKey($"fk_{T.OrganisationMember}_{T.Users}_member_id").FromTable(T.OrganisationMember).ForeignColumn("member_id").ToTable(T.Users).PrimaryColumn("id");


			Create.Table(T.ApiKey)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("user_id").AsInt32().NotNullable()
				.WithColumn("name").AsString(FL.Long, DB.Collation).NotNullable()
				.WithColumn("key_hashed").AsString(FL.Max).NotNullable()
				.WithColumn("expires_utc").AsDateTime2().NotNullable()
				.WithColumn("glob_pattern").AsString(FL.Long).WithDefaultValue("*")
				.WithColumn("package_list").AsString(FL.Max).Nullable()
				.WithColumn("scopes").AsInt32().NotNullable();

			Create.Index("ix_apikeys_user_id").OnTable(T.ApiKey).OnColumn("user_id").Ascending();
			Create.ForeignKey($"fk_{T.ApiKey}_{T.Users}_user_id").FromTable(T.ApiKey).ForeignColumn("user_id").ToTable(T.Users).PrimaryColumn("id");
			Create.Index("ix_apikeys_user_id_name").OnTable(T.ApiKey).OnColumn("user_id").Ascending().OnColumn("name").Unique();

			Create.Table(T.Package)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("packageid").AsString(FL.Medium, DB.Collation).NotNullable().Unique()
				.WithColumn("downloads").AsInt64().WithDefaultValue(0).NotNullable().NotNullable()
				.WithColumn("reserved_prefix_id").AsInt32().Nullable();

			Create.Table(T.PackageOwner)
				.WithColumn("package_id").AsInt32().NotNullable().PrimaryKey()
				.WithColumn("owner_id").AsInt32().NotNullable().PrimaryKey();

			Create.ForeignKey($"fk_{T.PackageOwner}_{T.Package}_package_id").FromTable(T.PackageOwner).ForeignColumn("package_id").ToTable(T.Package).PrimaryColumn("id");

			Create.Table(T.PackageTargetPlatform)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("package_id").AsInt32().NotNullable()
				.WithColumn("compiler_version").AsInt32().NotNullable()
				.WithColumn("platform").AsInt32().NotNullable()
				.WithColumn("latest_version").AsInt32().Nullable() //needs to be nullable due to circular ref
				.WithColumn("latest_stable_version").AsInt32().Nullable(); // ""

			Create.Index("ix_targetplatforn_unique")
							.OnTable(T.PackageTargetPlatform).WithOptions().Unique()
							.OnColumn("package_id").Ascending()
							.OnColumn("compiler_version").Ascending()
							.OnColumn("platform").Ascending();

			Create.ForeignKey($"fk_{T.PackageTargetPlatform}_{T.Package}_package_id")
				.FromTable(T.PackageTargetPlatform).ForeignColumn("package_id")
				.ToTable(T.Package).PrimaryColumn("id");

			Create.Table(T.PackageVersion)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("targetplatform_id").AsInt32().NotNullable()
				.WithColumn("version").AsString(FL.Medium, DB.Collation).NotNullable()
				.WithColumn("description").AsString(FL.Long).NotNullable()
				.WithColumn("copyright").AsString(FL.Medium).NotNullable()
				.WithColumn("is_prerelease").AsBoolean().NotNullable()
				.WithColumn("is_commercial").AsBoolean().NotNullable()
				.WithColumn("is_trial").AsBoolean().NotNullable()
				.WithColumn("authors").AsString(FL.Long, DB.Collation).Nullable()
				.WithColumn("icon").AsString(FL.Long).Nullable()
				.WithColumn("license").AsString(FL.Short, DB.Collation).Nullable()
				.WithColumn("project_url").AsString(FL.Long).Nullable()
				.WithColumn("repository_url").AsString(FL.Long).Nullable()
				.WithColumn("repository_type").AsString(FL.Short).Nullable()
				.WithColumn("repository_branch").AsString(FL.Medium).Nullable()
				.WithColumn("repository_commit").AsString(FL.Medium).Nullable()
				.WithColumn("read_me").AsString(FL.Max).Nullable()
				.WithColumn("release_notes").AsString(FL.Max).Nullable()
				.WithColumn("filesize").AsInt64().WithDefaultValue(0).NotNullable()
				.WithColumn("listed").AsBoolean().NotNullable()
				.WithColumn("published_utc").AsDateTime2().Nullable()
				.WithColumn("downloads").AsInt64().WithDefaultValue(0).NotNullable()
				.WithColumn("deprecation_state").AsInt32().WithDefaultValue(0).NotNullable()
				.WithColumn("deprecation_message").AsString(FL.Long).Nullable()
				.WithColumn("alternate_package").AsString(FL.Long, DB.Collation).Nullable()
				.WithColumn("status").AsInt32().WithDefaultValue(0).NotNullable()
				.WithColumn("status_message").AsString(FL.VeryLong).Nullable()
				.WithColumn("tags").AsString(FL.Long).Nullable()
				.WithColumn("hash").AsString(FL.Long).Nullable()
				.WithColumn("hash_algorithm").AsString(FL.Short).Nullable();

			Create.ForeignKey($"fk_{T.PackageVersion}_{T.PackageTargetPlatform}_targetplatform_id").FromTable(T.PackageVersion).ForeignColumn("targetplatform_id").ToTable(T.PackageTargetPlatform).PrimaryColumn("id");

			Create.Index("ix_packageversion_targetplatform")
				.OnTable(T.PackageVersion)
				.OnColumn("targetplatform_id").Ascending()
				.OnColumn("version").Ascending();


			Create.Table(T.ReservedPrefix)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("prefix").AsString(FL.Short, DB.Collation).NotNullable().Unique();

			Create.Table(T.ReservedPrefixOwner)
				.WithColumn("prefix_id").AsInt32().NotNullable().PrimaryKey()
				.WithColumn("owner_id").AsInt32().NotNullable().PrimaryKey();

			Create.Table(T.PackageDependency)
				.WithColumn("packageversion_id").AsInt32().NotNullable().PrimaryKey()
				.WithColumn("package_id").AsString(FL.Medium, DB.Collation).NotNullable().PrimaryKey()
				.WithColumn("version_range").AsString(FL.Short).NotNullable();
			
			Create.ForeignKey($"fk_{T.PackageDependency}_{T.PackageVersion}_package_version_id")
				.FromTable(T.PackageDependency).ForeignColumn("packageversion_id")
				.ToTable(T.PackageVersion).PrimaryColumn("id");

			Create.Table(T.PackageVersionProcess)
				.WithIntPrimaryKeyColumn().Identity()
				.WithColumn("packageversion_id").AsInt32().NotNullable().Unique()
				.WithColumn("package_filename").AsString(FL.Long).NotNullable().Unique()
				.WithColumn("completed").AsBoolean().NotNullable()
				.WithColumn("last_updated_utc").AsDateTime2().NotNullable()
				.WithColumn("retry_count").AsInt32().NotNullable();

			Create.ForeignKey($"fk_{T.PackageVersionProcess}_{T.PackageVersion}__packageversion_id")
				.FromTable(T.PackageVersionProcess).ForeignColumn("packageversion_id")
				.ToTable(T.PackageVersion).PrimaryColumn("id");

			Execute.Sql(LatestStableVersionView);
			Execute.Sql(LatestVersionView);

			//TODO : Add tables for statistics - downloads per week, month

		}

		public override void Down()
		{

		}

	}
}
