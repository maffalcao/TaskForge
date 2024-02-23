using Domain.Dtos;

namespace Domain.Interfaces.Services;
public interface IUserService
{
    Task<UserDto> AddAsync(CreateUserDto userDto);
    Task<UserDto> GetByIdAsync(int id);

}