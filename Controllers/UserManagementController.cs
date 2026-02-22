using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models;
using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairMount_api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserManagementController : ControllerBase
  {
    private readonly IUserManagementRepository _userManagementRepository;
    public UserManagementController(IUserManagementRepository userManagementRepository)
    {
      _userManagementRepository = userManagementRepository;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto data)
    {
      var result = await _userManagementRepository.Login(data);
      if (result == -1)
      {
        return BadRequest();
      }
      else if (result == 0)
      {
        return NotFound();
      }
      else
      {
        return Ok(result);
      }
    }
    [HttpPost("user")]
    public async Task<IActionResult> Create(User user)
    {
      var result =await _userManagementRepository.CreateUser(user);
      return Ok(result);
    }
  }
}
