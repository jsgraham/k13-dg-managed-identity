using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace DancingGoat.Infrastructure
{
    public static class RepositoryCacheHelper
    {
        public static IEnumerable<TObjectType> CacheObjects<TObjectType>(Func<IEnumerable<TObjectType>> getData, string baseCacheKey, string[] dependencyCacheKeys = null)
            where TObjectType : BaseInfo
        {
            if (!IsCacheEnabled())
            {
                return getData?.Invoke();
            }

            var cacheKey = GetCacheItemKey(baseCacheKey);
            var cacheSettings = CreateCacheSettings(cacheKey);
            Func<IEnumerable<TObjectType>> provideData = () =>
            {
                var result = getData?.Invoke();
                if (result == null || !result.Any())
                {
                    cacheSettings.CacheMinutes = 0;
                    return result;
                }

                var objects = result.ToList();
                cacheSettings.CacheDependency = GetCacheDependency(dependencyCacheKeys, objects);
                return objects;
            };

            return CacheHelper.Cache(provideData, cacheSettings);
        }


        public static TObjectType CacheObject<TObjectType>(Func<TObjectType> getData, string baseCacheKey, string[] dependencyCacheKeys = null)
            where TObjectType : BaseInfo
        {
            if (!IsCacheEnabled())
            {
                return getData?.Invoke();
            }

            var cacheKey = GetCacheItemKey(baseCacheKey);
            var cacheSettings = CreateCacheSettings(cacheKey);
            Func<TObjectType> provideData = () =>
            {
                var result = getData?.Invoke();
                if (result == null)
                {
                    cacheSettings.CacheMinutes = 0;
                    return null;
                }

                cacheSettings.CacheDependency = GetCacheDependency(dependencyCacheKeys, new[] { result });
                return result;
            };

            return CacheHelper.Cache(provideData, cacheSettings);
        }


        public static TPageType CachePage<TPageType>(Func<TPageType> getData, string baseCacheKey, string[] dependencyCacheKeys = null)
            where TPageType : TreeNode
        {
            if (!IsCacheEnabled())
            {
                return getData?.Invoke();
            }

            var cacheKey = GetCacheItemKey(baseCacheKey);
            var cacheSettings = CreateCacheSettings(cacheKey);

            Func<TPageType> provideData = () =>
            {
                var result = getData?.Invoke();
                if (result == null)
                {
                    cacheSettings.CacheMinutes = 0;
                    return null;
                }

                cacheSettings.CacheMinutes = GetMinutes(result.DocumentPublishTo, cacheSettings.CacheMinutes);
                cacheSettings.CacheDependency = GetPagesCacheDependency(dependencyCacheKeys, new[] { result });
                return result;
            };

            return CacheHelper.Cache(provideData, cacheSettings);
        }


        public static IEnumerable<TPageType> CachePages<TPageType>(Func<IEnumerable<TPageType>> getData, string baseCacheKey, string[] dependencyCacheKeys = null)
            where TPageType : TreeNode
        {
            if (!IsCacheEnabled())
            {
                return getData?.Invoke();
            }

            var cacheKey = GetCacheItemKey(baseCacheKey);
            var cacheSettings = CreateCacheSettings(cacheKey);

            Func<IEnumerable<TPageType>> provideData = () =>
            {
                var result = getData?.Invoke();
                if (result == null || !result.Any())
                {
                    cacheSettings.CacheMinutes = 0;
                    return result;
                }

                var pages = result.ToList();
                var earliestPublishTo = pages
                    .Where(treeNode => treeNode.DocumentPublishTo >= DateTime.Now)
                    .Min(treeNode => treeNode.DocumentPublishTo);

                cacheSettings.CacheMinutes = GetMinutes(earliestPublishTo, cacheSettings.CacheMinutes);
                cacheSettings.CacheDependency = GetPagesCacheDependency(dependencyCacheKeys, pages);
                return pages;
            };

            return CacheHelper.Cache(provideData, cacheSettings);
        }


        private static CMSCacheDependency GetCacheDependency(string[] dependencyCacheKeys, IEnumerable<BaseInfo> objects)
        {
            var objectType = objects.First().TypeInfo.ObjectType;
            dependencyCacheKeys = dependencyCacheKeys ?? new[]
            {
                CacheDependencyKeyProvider.GetDependencyCacheKeyForObjectType(objectType) }
            ;

            return CacheHelper.GetCacheDependency(dependencyCacheKeys);
        }


        private static CMSCacheDependency GetPagesCacheDependency(string[] dependencyCacheKeys, IEnumerable<TreeNode> pages)
        {
            var className = pages.First().ClassName;
            dependencyCacheKeys = dependencyCacheKeys ?? new[]
            {
                CacheDependencyKeyProvider.GetDependencyCacheKeyForPageType(SiteContext.CurrentSiteName, className),
                CacheDependencyKeyProvider.GetDependencyCacheKeyForObjectType(className)
            };

            var keys = new HashSet<string>(dependencyCacheKeys, StringComparer.OrdinalIgnoreCase);
            var pathKeys = pages.Where(page => page.HasUrl())
                .Select(page => page.NodeAliasPath)
                .SelectMany(path => DocumentDependencyCacheKeysBuilder.GetParentPathsDependencyCacheKeys(SiteContext.CurrentSiteName, path))
                .Distinct();

            foreach (var pathKey in pathKeys)
            {
                keys.Add(pathKey);
            }

            return CacheHelper.GetCacheDependency(keys);
        }


        private static double GetMinutes(DateTime publishTo, double minutes)
        {
            if (publishTo == DateTime.MaxValue || publishTo < DateTime.Now)
            {
                return minutes;
            }

            var minutesToInvalidation = (publishTo - DateTime.Now).TotalMinutes;
            return Math.Min(minutesToInvalidation, minutes);
        }


        private static string GetCacheItemKey(string baseCacheKey)
        {
            var builder = new StringBuilder(127)
                          .Append(baseCacheKey)
                          .Append("|").Append(SiteContext.CurrentSiteName)
                          .Append("|").Append(CultureInfo.CurrentCulture.Name);

            return builder.ToString();
        }


        private static CacheSettings CreateCacheSettings(string cacheKey)
        {
            return new CacheSettings(CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), cacheKey);
        }


        private static bool IsCacheEnabled()
        {
            return !IsPreviewEnabled();
        }


        private static bool IsPreviewEnabled()
        {
            return HttpContext.Current.Kentico().Preview().Enabled;
        }
    }
}