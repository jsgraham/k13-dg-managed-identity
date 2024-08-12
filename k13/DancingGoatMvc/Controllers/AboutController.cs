using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using DancingGoat.Controllers;
using DancingGoat.Infrastructure;
using DancingGoat.Models.About;
using DancingGoat.Models.References;
using DancingGoat.Repositories;

[assembly: RegisterPageRoute(AboutUs.CLASS_NAME, typeof(AboutController))]

namespace DancingGoat.Controllers
{
    public class AboutController : Controller
    {
        private readonly IPageDataContextRetriever dataRetriever;
        private readonly IAboutUsRepository aboutUsRepository;
        private readonly IReferenceRepository referenceRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;


        public AboutController(IPageDataContextRetriever dataRetriever, 
            IAboutUsRepository aboutUsRepository, 
            IReferenceRepository referenceRepository, 
            IOutputCacheDependencies outputCacheDependencies,
            IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            this.dataRetriever = dataRetriever;
            this.aboutUsRepository = aboutUsRepository;
            this.referenceRepository = referenceRepository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
        }


        [OutputCache(CacheProfile = "Default")]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var aboutUs = dataRetriever.Retrieve<AboutUs>().Page;

            var sideStories = await aboutUsRepository.GetSideStoriesAsync(aboutUs.NodeAliasPath, cancellationToken);
            outputCacheDependencies.AddDependencyOnPages(sideStories);

            var reference = (await referenceRepository.GetReferencesAsync($"{aboutUs.NodeAliasPath}/References", cancellationToken, 1)).FirstOrDefault();
            outputCacheDependencies.AddDependencyOnPage(reference);

            AboutUsViewModel mode = new AboutUsViewModel()
            {
                Sections = sideStories.Select(story => AboutUsSectionViewModel.GetViewModel(story, attachmentUrlRetriever)),
                Reference = ReferenceViewModel.GetViewModel(reference, attachmentUrlRetriever)
            };

            return View(mode);
        }
    }
}