using BrainRing.Application.Interfaces.Repositories;
using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Params.User;
using BrainRing.Domain.Entities;

namespace BrainRing.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;

        public UserService(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> CreateUserAsync(CreateUserParams @params, CancellationToken token = default )
        {
            var existing = _userRepository.FindByCondition(u => u.Name == @params.Name).FirstOrDefault();
            if (existing != null) throw new InvalidOperationException("User already exists.");

            var user = new User { Id = Guid.NewGuid(), Name = @params.Name };
            await _userRepository.CreateAsync(user, token);

            return user;
        }

        public Task<User?> GetUserByIdAsync(GetUserByIdParams @params, CancellationToken token = default) => _userRepository.FindByIdAsync(@params.Id, token);

        public User? GetUserByName(GetUserByNameParams @params)
        {
            var user = _userRepository.FindByCondition(u => u.Name == @params.Name).FirstOrDefault();
            return user;
        }
    }

}
