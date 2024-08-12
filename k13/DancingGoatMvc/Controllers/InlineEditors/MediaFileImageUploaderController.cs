using System;
using System.Web;
using System.Web.Mvc;

using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;

using Kentico.PageBuilder.Web.Mvc;

namespace DancingGoat.Controllers.InlineEditors
{
    public class MediaFileImageUploaderController : Controller
    {
        private readonly IPageBuilderDataContextRetriever dataContextRetriever;


        public MediaFileImageUploaderController(IPageBuilderDataContextRetriever dataContextRetriever)
        {
            this.dataContextRetriever = dataContextRetriever;
        }


        [HttpPost]
        public JsonResult Upload(string libraryName)
        {
            var dataContext = dataContextRetriever.Retrieve();
            if (!dataContext.EditMode)
            {
                throw new HttpException(403, "It is allowed to upload an image only when the page builder is in the edit mode.");
            }

            var library = MediaLibraryInfo.Provider.Get(libraryName, SiteContext.CurrentSiteID);
            if (library == null)
            {
                throw new InvalidOperationException($"The '{libraryName}' media library doesn't exist.");
            }

            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(library, "FileCreate", MembershipContext.AuthenticatedUser))
            {
                throw new HttpException(403, "You are not authorized to upload an image to the media library.");
            }

            var imageGuid = Guid.Empty;

            foreach (string requestFileName in Request.Files)
            {
                imageGuid = AddMediaFile(requestFileName, library);
            }

            return Json(new { guid = imageGuid });
        }


        private Guid AddMediaFile(string requestFileName, MediaLibraryInfo library)
        {
            if (!(Request.Files[requestFileName] is HttpPostedFileWrapper file))
            {
                return Guid.Empty;
            }

            return ImageUploaderHelper.Upload(file, path =>
            {
                var mediaFile = new MediaFileInfo(path, library.LibraryID);
                MediaFileInfo.Provider.Set(mediaFile);

                return mediaFile.FileGUID;
            });
        }
    }
}