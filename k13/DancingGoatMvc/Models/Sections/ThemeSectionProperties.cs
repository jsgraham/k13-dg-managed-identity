using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DancingGoat.Models.Sections
{
    /// <summary>
    /// Section properties to define the theme.
    /// </summary>
    public class ThemeSectionProperties : ISectionProperties
    {
        /// <summary>
        /// Theme of the section.
        /// </summary>
        [EditingComponent(DropDownComponent.IDENTIFIER, Label = "{$DancingGoatMVC.Section.ColorScheme.Label$}", Order = 1)]
        [EditingComponentProperty(nameof(DropDownProperties.DataSource), ";{$DancingGoatMVC.Section.ColorScheme.None$}\r\nsection-gray;{$DancingGoatMVC.Section.ColorScheme.LightGray$}\r\nsection-red;{$DancingGoatMVC.Section.ColorScheme.Red$}")]
        public string Theme { get; set; }
    }
}