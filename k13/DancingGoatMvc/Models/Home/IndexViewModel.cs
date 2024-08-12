using System.Collections.Generic;
using DancingGoat.Models.References;

namespace DancingGoat.Models.Home
{
    public class IndexViewModel
    {
        public IEnumerable<HomeSectionViewModel> HomeSections { get; set; }

        public ReferenceViewModel Reference { get; set; }
    }
}