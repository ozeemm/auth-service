using AuthServiceApp.API.Models.Dtos;

namespace AuthServiceApp.API.Interfaces.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDto>> GetUsers();
    }
}
