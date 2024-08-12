using System;
using System.Web;
using System.Web.Mvc;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.SiteProvider;

using Kentico.PageBuilder.Web.Mvc;

namespace DancingGoat.Controllers.InlineEditors
{
    public class AttachmentImageUploaderController : Controller
    {
        private readonly IPageBuilderDataContextRetriever dataContextRetriever;


        public AttachmentImageUploaderController(IPageBuilderDataContextRetriever dataContextRetriever)
        {
            this.dataContextRetriever = dataContextRetriever;
        }


        [HttpPost]
        public JsonResult Upload(int pageId)
        {
            var dataContext = dataContextRetriever.Retrieve();
            if (!dataContext.EditMode)
            {
                throw new HttpException(403, "It is allowed to upload an image only when the page builder is in the edit mode.");
            }

            var page = DocumentHelper.GetDocument(pageId, null);
            if (!CheckPagePermissions(page))
            {
                throw new HttpException(403, "You are not authorized to upload an image to the page.");
            }

            var imageGuid = Guid.Empty;

            foreach (string requestFileName in Request.Files)
            {
                imageGuid = AddUnsortedAttachment(page, requestFileName);
            }

            return Json(new { guid = imageGuid });
        }


        private Guid AddUnsortedAttachment(TreeNode page, string requestFileName)
        {
            if (!(Request.Files[requestFileName] is HttpPostedFileWrapper file))
            {
                return Guid.Empty;
            }

            return ImageUploaderHelper.Upload(file, path =>
            {
                return DocumentHelper.AddUnsortedAttachment(page, Guid.Empty, path).AttachmentGUID;
            });
        }


        private bool CheckPagePermissions(TreeNode page)
        {
            return page?.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser) ?? false;
        }
    }
}