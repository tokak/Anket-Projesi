using Survey.DTOs.LoginDto;

namespace SurveyProject.Business.Services.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Success, string Error)> LoginAsync(LoginDto model);
        Task LogoutAsync();
    }
}
