using Microsoft.AspNetCore.Identity;
using Survey.DTOs.RegisterDto;
using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Entities.Entity;

namespace SurveyProject.Business.Services.Managers
{

    public class AuthManager : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool isSuccess, string message, AppUser User)> RegisterAsync(RegisterDto model)
        {
            // 1. Geliştirme: Kullanıcı oluşturmadan önce e-postanın zaten kayıtlı olup olmadığını kontrol et.
            // Bu, gereksiz veritabanı sorgularını önler ve daha net bir hata mesajı sağlar.
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "Bu e-posta adresi zaten kullanılıyor.", null);
            }

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email, // UserName alanı genellikle e-posta ile aynı ayarlanır
                Email = model.Email,
                EmailConfirmed = false,

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                await _signInManager.SignInAsync(user, isPersistent: false);

                return (true, "Hesabınız başarıyla oluşturuldu. Giriş yapabilirsiniz.", user);
            }

            // 3. Geliştirme: Hata mesajlarını daha okunabilir bir formatta birleştir.
            // Birden fazla hata (örn: şifre çok kısa, şifrede rakam olmalı vb.) varsa
            // her birini ayrı bir satırda göstermek kullanıcı için daha anlaşılır olur.
            string errors = string.Join("\n", result.Errors.Select(e => e.Description));
            return (false, errors, null);
        }
    }
}

