using DancingGoat;
using DancingGoat.Models.PageTemplates;
using DancingGoat.Models.Sections;
using DancingGoat.Models.Widgets;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

// Widgets
[assembly: RegisterWidget(ComponentIdentifiers.TESTIMONIAL_WIDGET, "{$dancinggoatmvc.widget.testimonial.name$}", typeof(TestimonialWidgetProperties), Description = "{$dancinggoatmvc.widget.testimonial.description$}", IconClass = "icon-right-double-quotation-mark")]
[assembly: RegisterWidget(ComponentIdentifiers.CTA_BUTTON_WIDGET, "{$dancinggoatmvc.widget.ctabutton.name$}", typeof(CTAButtonWidgetProperties), Description = "{$dancinggoatmvc.widget.ctabutton.description$}", IconClass = "icon-rectangle-a")]

// Sections
[assembly: RegisterSection(ComponentIdentifiers.SINGLE_COLUMN_SECTION, "{$dancinggoatmvc.section.singlecolumn.name$}", typeof(ThemeSectionProperties), Description = "{$dancinggoatmvc.section.singlecolumn.description$}", IconClass = "icon-square")]
[assembly: RegisterSection(ComponentIdentifiers.TWO_COLUMN_SECTION, "{$dancinggoatmvc.section.twocolumn.name$}", typeof(ThemeSectionProperties), Description = "{$dancinggoatmvc.section.twocolumn.description$}", IconClass = "icon-l-cols-2")]
[assembly: RegisterSection(ComponentIdentifiers.THREE_COLUMN_SECTION, "{$dancinggoatmvc.section.threecolumn.name$}", typeof(ThreeColumnSectionProperties), Description = "{$dancinggoatmvc.section.threecolumn.description$}", IconClass = "icon-l-cols-3")]
[assembly: RegisterSection(ComponentIdentifiers.SECTION_25_75, "{$dancinggoatmvc.section.twocolumn2575.name$}", typeof(ThemeSectionProperties), Description = "{$dancinggoatmvc.section.twocolumn2575.description$}", IconClass = "icon-l-cols-70-30")]
[assembly: RegisterSection(ComponentIdentifiers.SECTION_75_25, "{$dancinggoatmvc.section.twocolumn7525.name$}", typeof(ThemeSectionProperties), Description = "{$dancinggoatmvc.section.twocolumn7525.description$}", IconClass = "icon-l-cols-30-70")]

// Page templates
[assembly: RegisterPageTemplate(ComponentIdentifiers.LANDING_PAGE_SINGLE_COLUMN_TEMPLATE, "{$dancinggoatmvc.pagetemplate.landingpagesinglecolumn.name$}", typeof(LandingPageSingleColumnProperties), Description = "{$dancinggoatmvc.pagetemplate.landingpagesinglecolumn.description$}")]
