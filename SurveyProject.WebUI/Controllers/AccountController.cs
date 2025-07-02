using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Survey.DTOs.LoginDto;
using Survey.DTOs.RegisterDto;
using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Entities.Entity;
using System.Security.Claims;

namespace SurveyProject.WebUI.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;
        private readonly IToastNotification _toastNotification;
        public readonly UserManager<AppUser> _userManager;
        public readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        public AccountController(IAccountService accountService, IAuthService authService, IToastNotification toastNotification, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _accountService = accountService;
            _authService = authService;
            _toastNotification = toastNotification;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // 1. Kullanıcıyı e-posta adresinden bul
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // 2. Kullanıcı varsa ve e-postası doğrulanmamışsa girişi engelle
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Giriş yapabilmek için e-posta adresinizi doğrulamanız gerekmektedir.");

                    // View'da "Doğrulama e-postasını yeniden gönder" linkini göstermek için
                    ViewBag.ShowResendLink = true;

                    return View(model);
                }

                // Lockout kontrolünü de burada yapabiliriz
                if (await _userManager.IsLockedOutAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Hesabınız çok fazla hatalı deneme nedeniyle kilitlenmiştir. Lütfen daha sonra tekrar deneyin.");
                    return View(model);
                }
            }
            // Kontrollerden geçtiyse, asıl giriş servisini çağır
            var (success, error) = await _accountService.LoginAsync(model);
            if (success)
            {
                _toastNotification.AddSuccessToastMessage("Giriş işleminiz başarılı.", new ToastrOptions { Title = "Başarılı" });

                // Kullanıcı string URL ile geldiyse o URL'e yönlendir.
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Bu kullanıcı "Admin" rolünde mi diye kontrol et.
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // Kullanıcı "Admin" rolündeyse Admin paneline yönlendir.
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });                    
                }
               
            }


            // Servisten dönen hatayı göster
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View(new RegisterDto());
        }
       
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var (isSuccess, message, user) = await _authService.RegisterAsync(model);

            if (isSuccess)
            {
                // 1. E-posta doğrulama token'ı oluştur
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // 2. Geri arama (callback) URL'ini oluştur
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = user.Id, code = code },
                    protocol: Request.Scheme);

                // 3. E-postayı gönder
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Hesabınızı Doğrulayın",
                    $"Lütfen hesabınızı doğrulamak için <a href='{callbackUrl}'>buraya tıklayın</a>.");

                // 4. Kullanıcıyı bilgilendirme sayfasına yönlendir
                TempData["SuccessMessage"] = "Kayıt işleminiz başarıyla tamamlandı. Lütfen e-posta adresinize gönderilen doğrulama linkine tıklayarak hesabınızı etkinleştirin.";

                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        // ADIM 1: Google butonuna tıklandığında bu action tetiklenir.
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("GoogleLoginCallback", "Account", new { ReturnUrl = ReturnUrl });
            var property = _signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);
            return new ChallengeResult("Google", property);
        }


        // ADIM 2: Kullanıcı Google'da kimliğini doğruladıktan sonra Google bu action'a geri yönlendirir.
        [HttpGet]
        public async Task<IActionResult> GoogleLoginCallback(string returnUrl = "/", string remoteError = null)
        {
            if (remoteError != null)
            {
                // Google'dan bir hata dönerse kullanıcıyı bilgilendir
                ModelState.AddModelError(string.Empty, $"Harici sağlayıcıdan hata: {remoteError}");
                return RedirectToAction("Login");
            }

            // Google'dan gelen kullanıcı bilgilerini al
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Bilgi alınamazsa login sayfasına yönlendir
                TempData["ErrorMessage"] = "Google ile giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Login");
            }

            // Alınan Google bilgileriyle kullanıcıyı sisteme giriş yapmayı dene
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                // Eğer kullanıcının daha önce Google ile oluşturulmuş bir hesabı varsa, doğrudan giriş yapılır
                return LocalRedirect(returnUrl);
            }

            // Eğer kullanıcının bizde hesabı yoksa, KAYIT OLUŞTURMA işlemi burada başlar.
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Google hesabınızda e-posta bilgisi bulunamadı.";
                return RedirectToAction("Login");
            }

            // E-posta ile bizde kayıtlı bir kullanıcı var mı diye kontrol et (şifreyle kayıt olmuş olabilir)
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {                
                return RedirectToAction("Index","Home");
            }

            // Kullanıcı bizde hiç yoksa, yeni bir kullanıcı oluştur
            user = new AppUser
            {
                UserName = email,
                Email = email,
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "", // Google'dan adı al
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",    // Google'dan soyadı al
                EmailConfirmed = true // Google'dan geldiği için e-postayı doğrulanmış kabul ediyoruz
            };

            var createUserResult = await _userManager.CreateAsync(user);
            if (createUserResult.Succeeded)
            {
                // Yeni oluşturulan yerel hesabı, kullanıcının Google hesabıyla ilişkilendir.
                // Bu sayede bir sonraki sefer doğrudan giriş yapabilir.
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    // İlişkilendirme başarısız olursa
                    TempData["ErrorMessage"] = "Hesabınız oluşturuldu ancak Google hesabınızla ilişkilendirilemedi.";
                    return RedirectToAction("Login");
                }

                // Yeni oluşturulan kullanıcı için sisteme giriş yap
                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["SuccessMessage"] = "Google hesabınızla başarıyla giriş yaptınız.";
                return LocalRedirect(returnUrl);
            }

            // Kullanıcı oluşturma başarısız olursa hataları göster
            foreach (var error in createUserResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Kullanıcı bulunamadı.");
            }

            // Token'ı doğrula (süre kontrolü dahil)
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "E-posta adresiniz başarıyla doğrulandı. Artık giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "E-posta doğrulama başarısız. Linkin süresi dolmuş veya geçersiz olabilir.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "E-posta adresi gereklidir.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            // 1. Durum: Kullanıcı bulunamadı.
            // Güvenlik nedeniyle, kullanıcıya hesabın var olup olmadığını belli etmeyiz.
            // Başarılı senaryo ile aynı mesajı göstererek yönlendiririz.
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Bu e-posta adresine sahip bir kullanıcı bulunamadı.");
                return View();
            }

            // 2. Durum: E-posta zaten doğrulanmış. (Bu kısım zaten doğruydu)
            if (user.EmailConfirmed)
            {
                TempData["SuccessMessage"] = "Bu e-posta adresi zaten doğrulanmış. Lütfen giriş yapın.";
                return RedirectToAction("Login");
            }

            // 3. Durum (Ana Başarı Senaryosu): Yeni e-posta gönderilecek.
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

            await _emailService.SendEmailAsync(
                email,
                "Hesabınızı Doğrulayın (Yeni İstek)",
                $"Lütfen hesabınızı doğrulamak için <a href='{callbackUrl}'>buraya tıklayın</a>.");
          
            TempData["SuccessMessage"] = "Doğrulama linki gönderilmiştir. Lütfen e-posta kutunuzu kontrol edin.";

            return RedirectToAction("Login");
        }


        public async Task<IActionResult> ForgotPasswordConfirmation()
        {
            return View();
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        // [POST] Şifre sıfırlama talebini işler
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            // Güvenlik Notu: Kullanıcı bulunamasa bile, var olup olmadığını belli etme.
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                // Token oluştur
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Linki oluştur (token'ı URL için güvenli hale getir)
                var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, code = token }, protocol: Request.Scheme);

                // E-postayı gönder
                await _emailService.SendEmailAsync(
                    model.Email,
                    "Şifre Sıfırlama Talebi",
                    $"Şifrenizi sıfırlamak için lütfen <a href='{callbackUrl}'>buraya tıklayın</a>.");
            }

            // Her durumda kullanıcıyı aynı bilgilendirme sayfasına yönlendir.
            return View("ForgotPasswordConfirmation");
        }


        // [GET] E-postadaki linke tıklandığında yeni şifre formunu gösterir
        [HttpGet]
        public IActionResult ResetPassword(string code = null, string email = null)
        {
            if (code == null || email == null)
            {
                ModelState.AddModelError("", "Geçersiz şifre sıfırlama linki.");
                return View("Error"); // Bir hata sayfası gösterilebilir
            }

            var model = new ResetPasswordDto { Token = code, Email = email };
            return View(model);
        }

        // [POST] Yeni şifreyi ayarlar
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Yine kullanıcıyı belli etme, genel bir başarı mesajı ile yönlendir.
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir. Artık yeni şifrenizle giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            // Şifreyi sıfırla (token kontrolü ve süre limiti burada otomatik yapılır)
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir. Artık yeni şifrenizle giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            // Token geçersiz veya süresi dolmuş ise
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }





    }
}
