using Microsoft.AspNetCore.Identity;

namespace SurveyProject.Entities.Entity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
