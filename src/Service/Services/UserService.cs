using AutoMapper;
using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;

namespace Service.Services;

public class UserService(IRepository<User> repository, IMapper mapper) : IUserService
{
    public async Task<UserDto> AddAsync(UserDto userDto)
    {
        var user = mapper.Map<User>(userDto);
        user = await repository.AddAsync(user);        
        return  mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);    
        return mapper.Map<UserDto>(user);
    }
}