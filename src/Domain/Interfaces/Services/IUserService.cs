using Domain.Dtos;
using Domain.Entities;

namespace Domain.Interfaces.Services;
public interface IUserService
{
    Task<UserDto> AddAsync(CreateUserDto userDto);        
    Task<UserDto> GetByIdAsync(int id);    

}