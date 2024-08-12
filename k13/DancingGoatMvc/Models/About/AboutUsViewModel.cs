using System.Collections.Generic;

using DancingGoat.Models.References;

namespace DancingGoat.Models.About
{
    public class AboutUsViewModel
    {
        public IEnumerable<AboutUsSectionViewModel> Sections { get; set; }

        public ReferenceViewModel Reference {get; set;}
    }
}