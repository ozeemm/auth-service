using AuthServiceApp.API.Entities;

namespace AuthServiceApp.API.Interfaces.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetUsers();
    }
}
