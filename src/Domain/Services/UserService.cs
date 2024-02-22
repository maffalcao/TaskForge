
using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Mapster;

namespace Domain.Services;

public class UserService(IRepository<User> repository) : IUserService
{
    public async Task<UserDto> AddAsync(CreateUserDto userDto)
    {
        var user = new User(userDto.Name, userDto.ProfileName);

        user = await repository.AddAsync(user);

        return user.Adapt<UserDto>();
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);
        return user.Adapt<UserDto>();
    }
}