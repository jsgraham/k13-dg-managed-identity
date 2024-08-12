using System;
using System.Collections.Generic;

using CMS.MediaLibrary;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of media files.
    /// </summary>
    public interface IMediaFileRepository : IRepository
    {
        /// <summary>
        /// Returns a media library with the specified identifier. 
        /// </summary>
        /// <param name="mediaLibraryName">Name of the media library.</param>
        MediaLibraryInfo GetByName(string mediaLibraryName);


        /// <summary>
        /// Returns all media files in specified media library.
        /// </summary>
        /// <param name="mediaLibraryName">Name of the media library.</param>
        IEnumerable<MediaFileInfo> GetMediaFiles(string mediaLibraryName);


        /// <summary>
        /// Returns media file with given identifier and site name.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the media file.</param>
        /// <param name="siteId">Site ID.</param>
        MediaFileInfo GetMediaFile(Guid fileIdentifier, int siteId);
    }
}