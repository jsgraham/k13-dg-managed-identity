using System.Collections.Generic;

using DancingGoat.Models.Contacts;

namespace DancingGoat.Models.Cafes
{
    public class IndexViewModel
    {
        public IEnumerable<CafeViewModel> CompanyCafes { get; set; }


        public Dictionary<string, List<ContactViewModel>> PartnerCafes { get; set; }
    }
}