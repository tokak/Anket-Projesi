using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Survey.DTOs.UserProfileDto;
using Survey.Service.Const;
using SurveyProject.Entities.Entity;

namespace SurveyProject.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class UserProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IToastNotification _toastNotification; // IToastNotification enjekte edilmiş

        public UserProfileController(UserManager<AppUser> userManager, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _toastNotification = toastNotification; // Enjekte edilen nesne atanıyor
        }

        // GET:Account/Profile
        [HttpGet]
        public async Task<IActionResult> ProfileForm()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Kullanıcı bulunamazsa veya oturum sona ermişse login sayfasına yönlendir
                _toastNotification.AddErrorToastMessage("Kullanıcı bulunamadı veya oturumunuz sona ermiş. Lütfen tekrar giriş yapın.");
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            UserProfile userProfileDto = new()
            {
                Email = currentUser.Email,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName
            };
            return View(userProfileDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileForm(UserProfile model)
        {
            // 1. Model doğrulamasını kontrol et
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lütfen formdaki hataları düzeltin.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage("Profiliniz başarıyla güncellendi!", new ToastrOptions { Title = "Başarılı" });
                return View(model);
            }
            else
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }



        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _toastNotification.AddErrorToastMessage("Kullanıcı bulunamadı.");
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            if (model.OldPassword == model.NewPassword)
            {
                TempData["ErrorMessage"] = "Yeni şifre, mevcut şifrenizden farklı olmalıdır.";
                return View(model);
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (changePasswordResult.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage("Şifreniz başarıyla değiştirildi.", new ToastrOptions { Title = "Başarılı" });

                return RedirectToAction("Index", "Survey", new { area = "Admin" }); 
            }

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
