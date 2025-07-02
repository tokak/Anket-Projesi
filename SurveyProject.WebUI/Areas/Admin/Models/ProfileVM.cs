namespace SurveyProject.WebUI.Areas.Admin.Models
{
    public class ProfileVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string RoleName { get; set; }

        public string Initials { get; set; }
        public string AvatarBackgroundColor { get; set; }
    }
}
