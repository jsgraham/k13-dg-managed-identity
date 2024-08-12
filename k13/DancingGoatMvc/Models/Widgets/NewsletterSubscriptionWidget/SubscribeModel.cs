using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Widgets
{
    public class SubscribeModel
    {
        [Required(ErrorMessage = "DancingGoatMvc.Email.Required")]
        [EmailAddress(ErrorMessage = "DancingGoatMvc.General.InvalidEmail")]
        [DisplayName("DancingGoatMvc.News.SubscriberEmail")]
        [MaxLength(250, ErrorMessage = "DancingGoatMvc.News.LongEmail")]
        public string Email { get; set; }
    }
}