using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Survey.DTOs;
using Survey.Service.Describers;
using SurveyProject.Business.Abstract;
using SurveyProject.Business.Concrete;
using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Business.Services.Managers;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.DataAccess.Context;
using SurveyProject.DataAccess.EntityFramework;
using SurveyProject.Entities.Entity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNToastNotifyToastr(new ToastrOptions()
    {
        PositionClass = ToastPositions.TopRight,
        TimeOut = 3000
    });


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, AppRole>(options =>
    {
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // E-posta do�rulama token'� i�in 3 saat ge�erlilik s�resi belirle
    options.TokenLifespan = TimeSpan.FromHours(3);
});

builder.Services.Configure<MailSettingsDto>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddScoped<IAccountService, AccountManager>();
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<IEmailService, EmailManager>();

builder.Services.AddScoped<ISurveyDal, EfSurveyDal>();


builder.Services.AddScoped<ISurveyDal, EfSurveyDal>();
builder.Services.AddScoped<ISurveyService, SurveyManager>();

builder.Services.AddScoped<IQuestionsDal, EfQuestionDal>();
builder.Services.AddScoped<IQuestionsService, QuestionsManager>();
builder.Services.AddScoped<IAnswerOptionsDal, EfAnswerOptionsDal>();
builder.Services.AddScoped<IAnswerOptionsService, AnswerOptionsManager>();
builder.Services.AddScoped<IUserAnswerDal, EfUserAnswerDal>();
builder.Services.AddScoped<IUserAnswerService, UserAnswerManager>();

builder.Services.AddSession();

builder.Services.AddAuthentication()
   .AddGoogle(opt =>
   {
       // Yap�land�rmadan ClientId ve ClientSecret'� al�yoruz
       opt.ClientId = builder.Configuration["Authentication:Google:ClientId"];
       opt.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
   });



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Kullan�c� giri� yapmam��ken yetkilendirilmi� bir sayfaya eri�meye �al��t���nda y�nlendirilecek sayfa
        options.LoginPath = "/Account/Login";

        // Kullan�c�n�n yetkisi olmad���nda (�rn: rol tabanl� yetkilendirme) y�nlendirilecek sayfa
        options.AccessDeniedPath = "/Account/AccessDenied";

        // Oturumun ne kadar s�re ge�erli kalaca��
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        // Her istekte oturumun �mr�n� uzat�r (belirtilen s�re i�inde aktif kald��� s�rece)
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseNToastNotify();
app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
