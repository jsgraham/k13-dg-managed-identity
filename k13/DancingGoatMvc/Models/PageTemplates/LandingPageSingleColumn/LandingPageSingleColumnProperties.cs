using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace DancingGoat.Models.PageTemplates
{
    public class LandingPageSingleColumnProperties : IPageTemplateProperties
    {
        /// <summary>
        /// Indicates if logo should be shown.
        /// </summary>
        [EditingComponent(CheckBoxComponent.IDENTIFIER, Label = "{$DancingGoatMVC.PageTemplate.LandingPageSingleColumn.ShowLogo$}", Order = 1)]
        public bool ShowLogo { get; set; } = true;


        /// <summary>
        /// Background color CSS class of the header.
        /// </summary>
        [EditingComponent(DropDownComponent.IDENTIFIER, Label = "{$DancingGoatMVC.PageTemplate.LandingPageSingleColumn.HeaderColor$}", Order = 2)]
        [EditingComponentProperty(nameof(DropDownProperties.DataSource), "first-color;{$DancingGoatMVC.PageTemplate.LandingPageSingleColumn.FirstColor$}\r\nsecond-color;{$DancingGoatMVC.PageTemplate.LandingPageSingleColumn.SecondColor$}\r\nthird-color;{$DancingGoatMVC.PageTemplate.LandingPageSingleColumn.ThirdColor$}")]
        public string HeaderColorCssClass { get; set; } = "first-color";
    }
}