using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyProject.Entities.Entity;
using SurveyProject.WebUI.Areas.Admin.Models;
using System.Security.Claims;

namespace SurveyProject.WebUI.Areas.Admin.ViewComponents
{
    public class ProfileViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly string[] _avatarColors = new string[]
        {
        "#FFC107", "#0D6EFD", "#198754", "#DC3545", "#6F42C1", "#FD7E14", "#20C997"
        };

        // Dependency Injection ile UserManager servisini alıyoruz.
        public ProfileViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // ViewComponent'in ana metodu budur.
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);

            if (user == null)
            {
                return Content(string.Empty);
            }

            var roles = await _userManager.GetRolesAsync(user);

            // --- YENİ AVATAR MANTIĞI ---

            // 1. Baş Harfleri Oluşturma
            string initials = "";
            if (!string.IsNullOrEmpty(user.FirstName))
            {
                initials += user.FirstName[0];
            }
            if (!string.IsNullOrEmpty(user.LastName))
            {
                initials += user.LastName[0];
            }
            if (string.IsNullOrEmpty(initials) && !string.IsNullOrEmpty(user.UserName))
            {
                initials += user.UserName[0];
            }

            // 2. Kullanıcı ID'sine göre sabit bir renk seçme
            // GetHashCode() yerine bu basit yöntem, her zaman aynı sonucu verir.
            int hash = user.Id.ToString().Sum(c => c);
            string backgroundColor = _avatarColors[hash % _avatarColors.Length];

            // --- BİTTİ ---

            var model = new ProfileVM
            {
                Id = user.Id.ToString(),
                Name = user.FirstName,
                Surname = user.LastName,
                RoleName = roles.FirstOrDefault(),
                Initials = initials.ToUpper(),
                AvatarBackgroundColor = backgroundColor
            };

            return View(model);
        }
    }
}
