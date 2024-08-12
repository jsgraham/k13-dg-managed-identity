using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Creates a minimum set of ASP.NET output cache dependencies for views that contain data from pages or info objects.
    /// </summary>
    public sealed class OutputCacheDependencies : IOutputCacheDependencies
    {
        private readonly HttpResponseBase mResponse;
        private readonly IContentItemMetadataProvider mContentItemMetadataProvider;
        private readonly bool mCacheEnabled;
        private readonly HashSet<string> mDependencyCacheKeys = new HashSet<string>();


        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCacheDependencies"/> class.
        /// </summary>
        /// <param name="response">HTTP response that will be used to create output cache dependencies.</param>
        /// <param name="contentItemMetadataProvider">object that provides information about pages and info objects using their runtime type.</param>
        /// <param name="cacheEnabled">Indicates whether caching is enabled.</param>
        public OutputCacheDependencies(HttpResponseBase response, IContentItemMetadataProvider contentItemMetadataProvider, bool cacheEnabled)
        {
            mResponse = response;
            mContentItemMetadataProvider = contentItemMetadataProvider;
            mCacheEnabled = cacheEnabled;
        }


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from pages of the specified runtime type.
        /// When any page of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="pages">Pages used to create validation callback to invalidate cache based on <see cref="TreeNode.DocumentPublishTo"/>.</param>
        /// <typeparam name="T">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="pages"/> is <c>null</c>.</exception>
        public void AddDependencyOnPages<T>(IEnumerable<T> pages) where T : TreeNode, new()
        {
            if (pages == null)
            {
                throw new ArgumentNullException(nameof(pages));
            }

            if (!mCacheEnabled)
            {
                return;
            }

            if (!pages.Any())
            {
                var dependencyCacheKey = GetDependencyCacheKeyForEmptyPagesRuntimeType<T>();
                AddCacheItemDependency(dependencyCacheKey);
            }
            else
            {
                var classNames = pages.Select(page => page.ClassName).Distinct();

                foreach (var className in classNames)
                {
                    var dependencyCacheKey = CacheDependencyKeyProvider.GetDependencyCacheKeyForPageType(SiteContext.CurrentSiteName, className);
                    AddCacheItemDependency(dependencyCacheKey);
                }

                AddCacheItemDependency("cms.adhocrelationship|all");
                AddCacheItemDependency("cms.relationship|all");

                AddValidationCallback(pages);
            }
        }


        private string GetDependencyCacheKeyForEmptyPagesRuntimeType<T>() where T : TreeNode, new()
        {
            var className = mContentItemMetadataProvider.GetClassNameFromPageRuntimeType<T>();
            return CacheDependencyKeyProvider.GetDependencyCacheKeyForPageType(SiteContext.CurrentSiteName, className);
        }


        private void AddValidationCallback<T>(IEnumerable<T> pages) where T : TreeNode, new()
        {
            var earliestPublishTo = pages
                .Where(treeNode => treeNode.DocumentPublishTo >= DateTime.Now)
                .Min(treeNode => treeNode.DocumentPublishTo);

            if (earliestPublishTo >= DateTime.Now && earliestPublishTo != DateTime.MaxValue)
            {
                mResponse.Cache.AddValidationCallback(new HttpCacheValidateHandler(ValidateCacheBasedOnPublishTo), earliestPublishTo);
            }
        }


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from info objects of the specified runtime type.
        /// When any info object of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</typeparam>
        public void AddDependencyOnInfoObjects<T>() where T : AbstractInfo<T>, new()
        {
            if (!mCacheEnabled)
            {
                return;
            }

            var objectType = mContentItemMetadataProvider.GetObjectTypeFromInfoObjectRuntimeType<T>();
            var dependencyCacheKey = string.Format("{0}|all", objectType);
            AddCacheItemDependency(dependencyCacheKey);
        }


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from info object of the specified runtime type.
        /// When info object of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="infoGuid">Info object guid used for dependency cache key.</param>
        /// <typeparam name="T">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</typeparam>
        public void AddDependencyOnInfoObject<T>(Guid infoGuid) where T : AbstractInfo<T>, new()
        {
            if (!mCacheEnabled || infoGuid == Guid.Empty)
            {
                return;
            }

            var objectType = mContentItemMetadataProvider.GetObjectTypeFromInfoObjectRuntimeType<T>();
            var dependencyCacheKey = $"{objectType}|byguid|{infoGuid}";
            AddCacheItemDependency(dependencyCacheKey);
        }


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from page attachment.
        /// When specified attachment is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="attachmentGuid">Attachment guid used for dependency cache key.</param>
        public void AddDependencyOnPageAttachmnent(Guid attachmentGuid)
        {
            if (!mCacheEnabled || attachmentGuid == Guid.Empty)
            {
                return;
            }

            var dependencyCacheKey = $"attachment|{attachmentGuid}";
            AddCacheItemDependency(dependencyCacheKey);
        }


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from page of the specified runtime type.
        /// When specified page of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <param name="page">Page used for dependency cache key.</param>
        /// <typeparam name="T">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</typeparam>
        public void AddDependencyOnPage<T>(T page) where T : TreeNode, new()
        {
            if (!mCacheEnabled || page == null)
            {
                return;
            }

            var dependencyCacheKey = $"documentid|{page.DocumentID}";
            AddCacheItemDependency(dependencyCacheKey);

            // Add dependency on NodeAliasPath because DancingGoat uses combine with default culture.
            // When non-existing culture version is accessed the default culture version is cached. In case the version is created cache needs to be flushed.
            dependencyCacheKey = $"node|{page.NodeSiteName}|{page.NodeAliasPath}";
            AddCacheItemDependency(dependencyCacheKey);

            if (page.DocumentPublishTo >= DateTime.Now && page.DocumentPublishTo != DateTime.MaxValue)
            {
                mResponse.Cache.AddValidationCallback(new HttpCacheValidateHandler(ValidateCacheBasedOnPublishTo), page.DocumentPublishTo);
            }
        }


        private static void ValidateCacheBasedOnPublishTo(HttpContext context, object publishTo, ref HttpValidationStatus status)
        {
            if (publishTo == null)
            {
                status = HttpValidationStatus.IgnoreThisRequest;
                return;
            }

            status = (DateTime)publishTo >= DateTime.Now ? HttpValidationStatus.Valid : HttpValidationStatus.Invalid;
        }


        private void AddCacheItemDependency(string dependencyCacheKey)
        {
            dependencyCacheKey = dependencyCacheKey.ToLowerInvariant();

            if (!mDependencyCacheKeys.Contains(dependencyCacheKey))
            {
                mDependencyCacheKeys.Add(dependencyCacheKey);
                CacheHelper.EnsureDummyKey(dependencyCacheKey);
                mResponse.AddCacheItemDependency(dependencyCacheKey);
            }
        }
    }
}