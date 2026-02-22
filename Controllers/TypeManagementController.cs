using FairMount_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairMount_api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TypeManagementController : ControllerBase
  {
    private readonly ITypeManagementRepository _typeManagementRepository;
    public TypeManagementController(ITypeManagementRepository repo)
    {
      _typeManagementRepository = repo;
    }
    [HttpGet("GetAllPOTypes")]
    public async Task<IActionResult> GetAllPos()
    {
      try
      {
        var result = await _typeManagementRepository.GetAllPOTypesAsync();
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
    [HttpGet("GetAllItemStatus")]
    public async Task<IActionResult> GetAllItemStatus()
    {
      try
      {
        var result = await _typeManagementRepository.GetAllItemStatus();
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
