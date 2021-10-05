using DPMGallery.Data;
using DPMGallery.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;


namespace DPMGallery.Repositories
{
    public class PackageVersionRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public PackageVersionRepository(ILogger logger, IDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<PackageVersion> GetById(int id, CancellationToken cancellationToken)
        {
            string q = $"select * from {T.PackageVersion} where id = @id";
            return await Context.QueryFirstOrDefaultAsync<PackageVersion>(q, new { id }, cancellationToken: cancellationToken);
        }

        public async Task<PackageVersion> GetByIdAndVersion(int targetPlatformId, string version, CancellationToken cancellationToken)
        {
            string q = @$"select * from {T.PackageVersion} where (targetplatform_id = @targetPlatformId) and (version = @version)";
            return await Context.QueryFirstOrDefaultAsync<PackageVersion>(q, new { targetPlatformId, version }, cancellationToken: cancellationToken);
        }

        public async Task<PageableEnumeration<PackageVersion>> GetByTargetPlatformId(string targetPlatformId, Paging paging, CancellationToken cancellationToken)
        {

            string cq = $"select count(*) from {T.PackageVersion} where targetplatform_id = @targetPlatformId ";
            string q = $"select * from {T.PackageVersion} where targetplatform_id = @targetPlatformId order by version" + PagingSQL;
            int totalCount = await Context.ExecuteScalarAsync<int>(cq, new { targetPlatformId });

            List<PackageVersion> versions;

            if (totalCount > 0)
            {
                versions = (await Context.QueryAsync<PackageVersion>(q, new { targetPlatformId, skip = paging.Skip, take = paging.Take }, cancellationToken: cancellationToken)).ToList();
            }
            else
            {
                versions = new List<PackageVersion>();
            }

            return new PageableEnumeration<PackageVersion>(versions, totalCount);

        }

        public async Task UpdateAsyncStatus(PackageVersion packageVersion, CancellationToken cancellationToken)
        {
            string updateSql = $@"UPDATE {T.PackageVersion} 
	                            SET status = @Status, status_message = @StatusMessage
	                            WHERE id = @id";

            await Context.ExecuteAsync(updateSql, new { id = packageVersion.Id, Status = packageVersion.Status, StatusMessage = packageVersion.StatusMessage }, cancellationToken: cancellationToken);
        }

        public async Task<PackageVersion> InsertAsync(PackageVersion packageVersion, CancellationToken cancellationToken)
        {
            string insertSql = $@"INSERT INTO {T.PackageVersion} (targetplatform_id, version, description, copyright, is_prerelease, is_commercial, is_trial, authors, icon, license, 
                                                            project_url, repository_url, repository_type, repository_branch, repository_commit,read_me, release_notes, filesize, 
                                                            listed, published_utc, downloads, deprecation_state, deprecation_message, alternate_package, status, hash, hash_algorithm)
	                                                        VALUES (@TargetPlatformId, @Version, @Description, @Copyright, @IsPrerelease, @IsCommercial, @IsTrial, @Authors, @Icon, 
                                                                    @License, @ProjectUrl, @RepositoryUrl, @RepositoryType, @RepositoryBranch, @RepositoryCommit, @ReadMe, @ReleaseNotes,
                                                                    @FileSize, @Listed, @PublishedUtc, @Downloads, @DeprecationState, @DeprecatedMessage, @AlternatePackage, @Status, @Hash,
                                                                    @HashAlgorithm) RETURNING id;";

            try
            {
                packageVersion.Id = await Context.ExecuteScalarAsync<int>(insertSql,
                    new
                    {
                        packageVersion.TargetPlatformId,
                        packageVersion.Version,
                        packageVersion.Description,
                        packageVersion.Copyright,
                        packageVersion.IsPrerelease,
                        packageVersion.IsCommercial,
                        packageVersion.IsTrial,
                        packageVersion.Authors,
                        packageVersion.Icon,
                        packageVersion.License,
                        packageVersion.ProjectUrl,
                        packageVersion.RepositoryUrl,
                        packageVersion.RepositoryType,
                        packageVersion.RepositoryBranch,
                        packageVersion.RepositoryCommit,
                        packageVersion.ReadMe,
                        packageVersion.ReleaseNotes,
                        packageVersion.FileSize,
                        packageVersion.Listed,
                        packageVersion.PublishedUtc,
                        packageVersion.Downloads,
                        packageVersion.DeprecationState,
                        packageVersion.DeprecatedMessage,
                        packageVersion.AlternatePackage,
                        packageVersion.Status,
                        packageVersion.Hash,
                        packageVersion.HashAlgorithm

                    }, cancellationToken: cancellationToken);

                if (packageVersion.Dependencies != null && packageVersion.Dependencies.Any())
                {
                    var depInsertSql = $@"INSERT INTO {T.PackageDependency} (packageversion_id, package_id, version_range)
                                        VALUES(@Id, @PackageId, @VersionRange);";
                    foreach (var dep in packageVersion.Dependencies)
                    {
                        dep.PackageVersionId = packageVersion.Id;
                        await Context.ExecuteAsync(depInsertSql, new { packageVersion.Id, dep.PackageId, dep.VersionRange }, cancellationToken: cancellationToken);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[PackageVersionRepository] Failed Adding version {version}");
                throw;
            }

            return packageVersion;
        }

        public async Task<IEnumerable<PackageVersion>> GetPackageVersionsAsync(string packageId, CompilerVersion compilerVersion, Platform platform, bool listed = true, CancellationToken cancellationToken = default)
        {
            string @sql = @"select pv.*
                        from package p
                        inner join package_targetplatform tp on
                        p.id = tp.package_id
                        inner join package_version pv
                        on pv.targetplatform_id = tp.id
                        where 
                        p.packageid = @packageId
                        and tp.compiler_version = @compilerVersion
                        and tp.platform = @platform
                        "; //newline is important!
                        if (listed)
                        {
                            sql = sql + "and pv.listed = true";
                        }

            var versions = await Context.QueryAsync<PackageVersion>(sql, new { packageId, compilerVersion, platform }, cancellationToken: cancellationToken);

            if (versions == null || !versions.Any())
                return null;

            var Ids = versions.Select(m => m.Id).Distinct().ToArray();

            //strange postgresql syntax for where in 
            var dependencies = await Context.QueryAsync<PackageDependency>($"select * from {T.PackageDependency} where packageversion_id = ANY (@Ids)", new { Ids });

            if (dependencies.Any())
            {
                foreach (var version in versions)
                {
                    version.Dependencies = dependencies.Where(x => x.PackageVersionId == version.Id).ToList();
                }
            }

            return versions;

        }

        public async Task<IEnumerable<string>> GetPackageVersionStringsAsync(string packageId, CompilerVersion compilerVersion, Platform platform, bool listed = true, CancellationToken cancellationToken = default)
        {
            string @sql = @"select pv.version
                        from package p
                        inner join package_targetplatform tp on
                        p.id = tp.package_id
                        inner join package_version pv
                        on pv.targetplatform_id = tp.id
                        where pv.listed = true
                        and p.packageid = @packageId
                        and tp.compiler_version = @compilerVersion
                        and tp.platform = @platform";
            var versions = await Context.QueryAsync<string>(sql, new { packageId, compilerVersion, platform }, cancellationToken: cancellationToken);
            return versions.Any() ? versions : null;
        }

        public async Task<bool> GetPackageVersionExistsAsync(string packageId, string version, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken)
        {
            string @sql = @"select count(pv.version)
                        from package p
                        inner join package_targetplatform tp on
                        p.id = tp.package_id
                        inner join package_version pv
                        on pv.targetplatform_id = tp.id
                        where pv.listed = true
                        and p.packageid = @packageId
                        and pv.version = @version
                        and tp.compiler_version = @compilerVersion
                        and tp.platform = @platform";
            var count = await Context.ExecuteScalarAsync<int>(sql, new { packageId, version, compilerVersion, platform }, cancellationToken: cancellationToken);
            return count == 1; //should never be more than 1!!!!
        }

        public async Task<PackageVersion> GetPackageVersionByPackageIdAsync(string packageId, string version, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken)
        {
            string sql = $@"select pv.* from  package_version pv
                            inner join package_targetplatform tp
                            on tp.id = pv.targetplatform_id
                            inner join package p
                            on p.id = tp.package_id
                            where p.packageid = @packageId
                            and tp.compiler_version = @compilerVersion
                            and tp.platform = @platform
                            and pv.version = @version";

            return await Context.QueryFirstOrDefaultAsync<PackageVersion>(sql, new { packageId,  version, compilerVersion, platform }, cancellationToken: cancellationToken);
        }

        public async Task<int> IncrementDownloads(PackageVersion packageVersion, CancellationToken cancellationToken)
        {
            string sql = $@"UPDATE {T.PackageVersion} SET downloads = downloads + 1
                            WHERE id = @id
                            RETURNING ID";
            return await Context.ExecuteScalarAsync<int>(sql, new { id = packageVersion.Id }, cancellationToken: cancellationToken);
        }
    }
}
