using FairMount_api.Models.Dtos;
using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SLIController : ControllerBase
{
    private readonly ISLIRepository _sliService;

    public SLIController(ISLIRepository sliService)
    {
        _sliService = sliService;
    }

    // GET: api/SLI (Get All)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sli_Document>>> GetAllSLIs()
    {
        var slis = await _sliService.GetAllAsync();
        return Ok(slis);
    }
    // GET: api/SLI/5 (Get By ID)
    [HttpGet("{id}")]
    public async Task<ActionResult<Sli_Document>> GetSLI(int id)
    {
        var sli = await _sliService.GetByIdAsync(id);
        if (sli == null) return NotFound();
        return Ok(sli);
    }


    // POST: api/SLI (Create)
    [HttpPost]
    public async Task<ActionResult<Sli_Document>> CreateSLI([FromBody] Sli_Document sli)
    {
        try
        {
            // The model is passed directly to the service
            var slis = await _sliService.CreateAsync(sli);
            return CreatedAtAction(nameof(GetSLI), new { id = slis.SliId }, slis);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: api/SLI/commercial/5
    [HttpGet("commercial/{id}")]
    public async Task<ActionResult<Sli_Document>> GetByCommercial(int id)
    {
        var sli = await _sliService.GetByComercialIdAsync(id);
        if (sli == null) return NotFound();
        return Ok(sli);
    }
}