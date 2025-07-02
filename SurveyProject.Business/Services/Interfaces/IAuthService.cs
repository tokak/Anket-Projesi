using Survey.DTOs.RegisterDto;
using SurveyProject.Entities.Entity;

namespace SurveyProject.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool isSuccess, string message, AppUser User)> RegisterAsync(RegisterDto model);
    }
}
