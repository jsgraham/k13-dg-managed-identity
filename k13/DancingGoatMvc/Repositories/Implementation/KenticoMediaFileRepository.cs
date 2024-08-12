using System;
using System.Collections.Generic;

using CMS.MediaLibrary;
using CMS.SiteProvider;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of media files.
    /// </summary>
    public class KenticoMediaFileRepository : IMediaFileRepository
    {
        private readonly IMediaLibraryInfoProvider mediaLibraryInfoProvider;
        private readonly IMediaFileInfoProvider mediaFileInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoMediaFileRepository"/> class.
        /// </summary>
        /// <param name="mediaLibraryInfoProvider">Provider for <see cref="MediaLibraryInfo"/> management.</param>
        /// <param name="mediaFileInfoProvider">Provider for <see cref="MediaFileInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mediaLibraryInfoProvider"/> or <paramref name="mediaFileInfoProvider"/> is null.</exception>
        public KenticoMediaFileRepository(IMediaLibraryInfoProvider mediaLibraryInfoProvider, IMediaFileInfoProvider mediaFileInfoProvider)
        {
            this.mediaLibraryInfoProvider = mediaLibraryInfoProvider ?? throw new ArgumentNullException(nameof(mediaLibraryInfoProvider));
            this.mediaFileInfoProvider = mediaFileInfoProvider ?? throw new ArgumentNullException(nameof(mediaFileInfoProvider));
        }


        /// <summary>
        /// Returns instance of <see cref="MediaFileInfo"/> specified by library name.
        /// </summary>
        /// <param name="mediaLibraryName">Name of the media library.</param>
        public MediaLibraryInfo GetByName(string mediaLibraryName)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return mediaLibraryInfoProvider.Get(mediaLibraryName, SiteContext.CurrentSiteID);
            }, $"{nameof(KenticoMediaFileRepository)}|{nameof(GetByName)}|{mediaLibraryName}");
        }


        /// <summary>
        /// Returns all media files in the media library.
        /// </summary>
        /// <param name="mediaLibraryName">Name of the media library.</param>
        public IEnumerable<MediaFileInfo> GetMediaFiles(string mediaLibraryName)
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                var mediaLibrary = GetByName(mediaLibraryName);

                if (mediaLibrary == null)
                {
                    throw new InvalidOperationException("Media library not found.");
                }

                return mediaFileInfoProvider.Get()
                    .WhereEquals("FileLibraryID", mediaLibrary.LibraryID);
            }, $"{nameof(KenticoMediaFileRepository)}|{nameof(GetMediaFiles)}|{mediaLibraryName}");
        }


        /// <summary>
        /// Returns media file with given identifier and site name.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the media file.</param>
        /// <param name="siteName">Site ID.</param>
        public MediaFileInfo GetMediaFile(Guid fileIdentifier, int siteId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return mediaFileInfoProvider.Get(fileIdentifier, siteId);
            }, $"{nameof(KenticoMediaFileRepository)}|{nameof(GetMediaFile)}|{fileIdentifier}|{siteId}");
        }
    }
}