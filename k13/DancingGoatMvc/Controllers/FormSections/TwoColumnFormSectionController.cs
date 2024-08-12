using System.Web.Mvc;

using Kentico.Forms.Web.Mvc;

using DancingGoat.Controllers.FormSections;

[assembly: RegisterFormSection(TwoColumnFormSectionController.IDENTIFIER, typeof(TwoColumnFormSectionController), "{$dancinggoatmvc.formsection.twocolumn.name$}", Description = "{$dancinggoatmvc.formsection.twocolumn.description$}", IconClass = "icon-l-cols-2")]

namespace DancingGoat.Controllers.FormSections
{
    [FormSectionExceptionFilter]
    public class TwoColumnFormSectionController : Controller
    {
        public const string IDENTIFIER = "DancingGoat.TwoColumnSection";


        // GET: TwoColumnSection
        public ActionResult Index()
        {
            return PartialView("FormSections/_TwoColumnSection");
        }
    }
}