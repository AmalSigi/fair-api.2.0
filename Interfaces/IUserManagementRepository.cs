using FairMount_api.Models;
using FairMount_api.Models.Tables;

namespace FairMount_api.Interfaces
{
  public interface IUserManagementRepository
  {
    Task<int> Login(UserDto user);
    Task<int> CreateUser(User user);
    
  }
}
