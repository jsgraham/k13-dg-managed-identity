using System.Linq;
using System.Web;
using System.Web.Mvc;

using DancingGoat.Models.MediaGallery;
using DancingGoat.Repositories;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Controllers
{
    public class MediaController : Controller
    {
        private const string MEDIA_LIBRARY_NAME = "CoffeeGallery";

        private readonly IMediaFileRepository mediaFileRepository;
        private readonly IMediaFileUrlRetriever fileUrlRetriever;


        public MediaController(IMediaFileRepository mediaFileRepository, IMediaFileUrlRetriever fileUrlRetriever)
        {
            this.mediaFileRepository = mediaFileRepository;
            this.fileUrlRetriever = fileUrlRetriever;
        }


        [ChildActionOnly]
        public ActionResult Gallery()
        {
            var mediaLibary = mediaFileRepository.GetByName(MEDIA_LIBRARY_NAME);

            if (mediaLibary == null)
            {
                throw new HttpException(404, "Media library not found.");
            }

            var mediaFiles = mediaFileRepository.GetMediaFiles(MEDIA_LIBRARY_NAME);
            var mediaGallery = new MediaGalleryViewModel(mediaLibary.LibraryDisplayName);
            mediaGallery.MediaFiles = mediaFiles.Select(file => MediaFileViewModel.GetViewModel(file, fileUrlRetriever));

            return PartialView("_Gallery", mediaGallery);
        }
    }
}