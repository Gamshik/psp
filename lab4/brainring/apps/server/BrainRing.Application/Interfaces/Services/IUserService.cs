using BrainRing.Application.Params.User;
using BrainRing.Domain.Entities;

namespace BrainRing.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(CreateUserParams @params, CancellationToken token = default);
        Task<User?> GetUserByIdAsync(GetUserByIdParams @params, CancellationToken token = default);
        User? GetUserByName(GetUserByNameParams @params);
    }
}
