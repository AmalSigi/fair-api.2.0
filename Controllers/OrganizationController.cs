using FairMount_api.Interfaces;
using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FairMount_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
    private readonly IOrganizationRepository _repository;

    public OrganizationController(IOrganizationRepository repository)
    {
      _repository = repository;
    }

    [HttpGet("Org")]
    public async Task<IActionResult> GetAll()
    {
      var orgs = await _repository.GetAllOrgs();
      return Ok(orgs);
    }

    [HttpGet("Org/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var org = await _repository.GetOrgByIdAsync(id);
      if (org == null)
        return NotFound("Organization not found");

      return Ok(org);
    }

    [HttpPost("Org")]
    public async Task<IActionResult> Add([FromBody] Organizations org)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var newOrg = await _repository.AddOrganizationAsync(org);
      return CreatedAtAction(nameof(GetById), new { id = newOrg.OrganizationId }, newOrg);
    }

    [HttpPost("UpdateOrg")]
    public async Task<IActionResult> Update( [FromBody] Organizations org)
    {
      var updatedOrg = await _repository.UpdateOrganizationAsync(org);
      if (updatedOrg == null)
        return NotFound("Organization not found");

      return Ok(updatedOrg);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var deleted = await _repository.DeleteOrganizationAsync(id);
      if (!deleted)
        return NotFound("Organization not found");

      return NoContent();
    }
    [HttpPost("Address")]
    public async Task<IActionResult> Create([FromBody] OrganizationAddresses address)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var created = await _repository.AddAddressAsync(address);
      if (created == null) return BadRequest("Could not create address. Ensure organization exists.");
      return CreatedAtAction(nameof(GetById), new { id = created.AddressId }, created);
    }

    // PUT: api/address/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrganizationAddresses address)
    {
      if (id != address.AddressId) return BadRequest("ID mismatch");
      var updated = await _repository.UpdateAddressAsync(address);
      if (!updated) return NotFound();
      return Ok();
    }

    // DELETE: api/address/{id}
    [HttpPost("Address/{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
      var deleted = await _repository.DeleteAddressAsync(id);
      if (!deleted) return NotFound();
      return NoContent();
    }

  }
}
