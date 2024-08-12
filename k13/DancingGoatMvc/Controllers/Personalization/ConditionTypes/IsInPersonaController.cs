using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Personas;

using Kentico.PageBuilder.Web.Mvc.Personalization;

using DancingGoat.Models.Personalization;
using DancingGoat.Personalization;
using DancingGoat.Repositories;

namespace DancingGoat.Controllers.Personalization
{
    public class IsInPersonaController : ConditionTypeController<IsInPersonaConditionType>
    {
        private readonly IPersonaRepository mPersonaRepository;
        private readonly IPersonaPictureUrlCreator mPictureCreator;


        public IsInPersonaController(IPersonaRepository personaRepository, IPersonaPictureUrlCreator pictureCreator)
        {
            mPersonaRepository = personaRepository;
            mPictureCreator = pictureCreator;
        }


        [HttpPost]
        public ActionResult Index()
        {
            var conditionTypeParameters = GetParameters();
   
            var viewModel = new IsInPersonaViewModel
            {
                PersonaCodeName = conditionTypeParameters.PersonaName,
                AllPersonas = GetAllPersonas(conditionTypeParameters.PersonaName)
            };
            return PartialView("Personalization/ConditionTypes/_IsInPersonaConfiguration", viewModel);
        }


        [HttpPost]
        public ActionResult Validate(IsInPersonaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AllPersonas = GetAllPersonas();
                return PartialView("Personalization/ConditionTypes/_IsInPersonaConfiguration", viewModel);
            }

            var parameters = new IsInPersonaConditionType
            {
                PersonaName = viewModel.PersonaCodeName
            };

            return new ConditionTypeValidationResult(parameters);
        }


        private List<IsInPersonaListItemViewModel> GetAllPersonas(string selectedPersonaName = "")
        {
            var allPersonas = mPersonaRepository.GetAll().Select(persona => new IsInPersonaListItemViewModel
            {
                CodeName = persona.PersonaName,
                DisplayName = persona.PersonaDisplayName,
                ImagePath = mPictureCreator.CreatePersonaPictureUrl(persona, 50),
                Selected = persona.PersonaName == selectedPersonaName
            }).ToList();

            if (allPersonas.Count > 0 && !allPersonas.Exists(x => x.Selected))
            {
                allPersonas.First().Selected = true;
            }

            return allPersonas;
        }
    }
}