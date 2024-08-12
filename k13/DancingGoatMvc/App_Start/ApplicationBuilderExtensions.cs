using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Search;

using Kentico.Web.Mvc;

namespace DancingGoat
{
    public static class ApplicationBuilderExtensions
    {
        private static readonly object indexLock = new object();
        private static bool indexRebuilt = false;


        /// <summary>
        /// Method to ensure proper smart search index initialization after installation or application deployment.
        /// </summary>
        /// <param name="builder">Application builder.</param>
        public static void UseSmartSearchIndexRebuild(this IApplicationBuilder builder)
        {
            RequestEvents.PostAuthenticate.Execute += RebuildIndex;
        }


        private static void RebuildIndex(object sender, EventArgs e)
        {
            if (!indexRebuilt)
            {
                lock (indexLock)
                {
                    if (!indexRebuilt)
                    {
                        var site = Service.Resolve<ISiteService>().CurrentSite;
                        RebuildSiteIndexes(site.SiteName);

                        indexRebuilt = true;
                    }
                }
            }
        }


        private static void RebuildSiteIndexes(SiteInfoIdentifier siteIdentifier)
        {
            var indexes = Service.Resolve<ISearchIndexInfoProvider>().Get().OnSite(siteIdentifier);

            foreach (var index in indexes)
            {
                RebuildIndex(index);
            }
        }


        private static void RebuildIndex(SearchIndexInfo index)
        {
            try
            {
                // After installation or deployment the index is new but no web farm task is issued
                // to rebuild the index. This will ensure that index is rebuilt on instance upon first request.
                if (index.IndexStatusLocal == IndexStatusEnum.NEW)
                {
                    var taskCreationParameters = new SearchTaskCreationParameters
                    {
                        TaskType = SearchTaskTypeEnum.Rebuild,
                        TaskValue = index.IndexName,
                        RelatedObjectID = index.IndexID
                    };

                    taskCreationParameters.TargetServers.Add(WebFarmHelper.ServerName);

                    SearchTaskInfoProvider.CreateTask(taskCreationParameters, true);
                }
            }
            catch (SearchIndexException ex)
            {
                Service.Resolve<IEventLogService>().LogException("SmartSearch", "INDEX REBUILD", ex);
            }
        }
    }
}