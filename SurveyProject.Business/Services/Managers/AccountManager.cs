using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Survey.DTOs.LoginDto;
using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Entities.Entity;

namespace SurveyProject.Business.Services.Managers
{
    public class AccountManager : IAccountService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<IdentityOptions> _identityOptions;
        public AccountManager(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IOptions<IdentityOptions> identityOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identityOptions = identityOptions;
        }


        public async Task<(bool Success, string Error)> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return (false, "E-posta veya şifre hatalı.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                return (true, null);
            }

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remainingTime = lockoutEnd.Value - DateTimeOffset.UtcNow;
                return (false, $"Hesabınız çok sayıda başarısız deneme nedeniyle geçici olarak kilitlenmiştir. Lütfen {Math.Ceiling(remainingTime.TotalMinutes)} dakika sonra tekrar deneyin.");
            }

            // --- DEĞİŞİKLİK BU BÖLÜMDE ---
            // Diğer tüm başarısız durumlar için (yanlış şifre vb.)
            // Kalan deneme hakkını hesapla ve kullanıcıya bildir.

            // 1. Yapılandırmadan maksimum deneme hakkını al
            var maxAttempts = _identityOptions.Value.Lockout.MaxFailedAccessAttempts;

            // 2. Kullanıcının mevcut başarısız deneme sayısını al
            var failedCount = await _userManager.GetAccessFailedCountAsync(user);

            // 3. Kalan hakkı hesapla
            var remainingAttempts = maxAttempts - failedCount;

            if (remainingAttempts > 0)
            {
                return (false, $"E-posta veya şifre hatalı. Hesabınız kilitlenmeden önce {remainingAttempts} deneme hakkınız kaldı.");
            }
            else
            {
                // Bu noktada hesap yeni kilitlenmiş olabilir ve IsLockedOut durumu bir sonraki denemede true dönecektir.
                // Bu yüzden kullanıcıya şimdiden kilitlenme mesajını verelim.
                var lockoutTime = _identityOptions.Value.Lockout.DefaultLockoutTimeSpan.TotalMinutes;
                return (false, $"Hesabınız çok sayıda başarısız deneme nedeniyle kilitlenmiştir. Lütfen {lockoutTime} dakika sonra tekrar deneyin.");
            }
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
