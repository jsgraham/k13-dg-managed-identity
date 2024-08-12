using Kentico.Membership;

namespace DancingGoat.Models.Account
{
    public class YourAccountViewModel
    {
        public User User { get; set; }

        public bool AvatarUpdateFailed { get; set; }
    }
}