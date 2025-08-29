using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HiringPipelineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequisitionsController : ControllerBase
    {
        private readonly IRequisitionService _requisitionService;

        public RequisitionsController(IRequisitionService requisitionService)
        {
            _requisitionService = requisitionService;
        }

        // GET: api/requisitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Requisition>>> GetRequisitions()
        {
            var requisitions = await _requisitionService.GetAllAsync();
            return Ok(requisitions);
        }

        // GET: api/requisitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Requisition>> GetRequisition(int id)
        {
            var requisition = await _requisitionService.GetByIdAsync(id);

            if (requisition == null) return NotFound();

            return requisition;
        }

        // POST: api/requisitions
        [HttpPost]
        public async Task<ActionResult<Requisition>> CreateRequisition(CreateRequisitionDto createDto)
        {
            // Check if this will be the first requisition and reset identity seed if needed
            if (!await _requisitionService.AnyAsync())
            {
                _requisitionService.ResetIdentitySeed();
            }

            var requisition = new Requisition
            {
                Title = createDto.Title,
                Department = createDto.Department,
                JobLevel = createDto.JobLevel,
                Status = "Open",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdRequisition = await _requisitionService.CreateAsync(requisition);
            return CreatedAtAction(nameof(GetRequisition), new { id = createdRequisition.RequisitionId }, createdRequisition);
        }

        // PUT: api/requisitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequisition(int id, UpdateRequisitionDto updateDto)
        {
            var requisition = await _requisitionService.GetByIdAsync(id);
            if (requisition == null)
                return NotFound();

            if (updateDto.Title != null)
                requisition.Title = updateDto.Title;
            if (updateDto.Department != null)
                requisition.Department = updateDto.Department;
            if (updateDto.JobLevel != null)
                requisition.JobLevel = updateDto.JobLevel;
            if (updateDto.Status != null)
                requisition.Status = updateDto.Status;

            requisition.UpdatedAt = DateTime.UtcNow;

            var updated = await _requisitionService.UpdateAsync(id, requisition);
            if (updated == null) return NotFound();

            return NoContent();
        }

        // DELETE: api/requisitions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequisition(int id)
        {
            var deleted = await _requisitionService.DeleteAsync(id);
            if (!deleted) return NotFound();

            // Check if this was the last requisition and reset identity seed if so
            if (!await _requisitionService.AnyAsync())
            {
                _requisitionService.ResetIdentitySeed();
            }

            return NoContent();
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllRequisitions()
        {
            await _requisitionService.DeleteAllAsync();
            _requisitionService.ResetIdentitySeed();
            return NoContent();
        }
    }
}
