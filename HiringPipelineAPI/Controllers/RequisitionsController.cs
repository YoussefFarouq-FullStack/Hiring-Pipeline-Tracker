using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequisitionsController : ControllerBase
    {
        private readonly HiringPipelineDbContext _context;

        public RequisitionsController(HiringPipelineDbContext context)
        {
            _context = context;
        }

        // GET: api/requisitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Requisition>>> GetRequisitions()
        {
            return await _context.Requisitions.ToListAsync();
        }

        // GET: api/requisitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Requisition>> GetRequisition(int id)
        {
            var requisition = await _context.Requisitions.FindAsync(id);

            if (requisition == null) return NotFound();

            return requisition;
        }

        // POST: api/requisitions
        [HttpPost]
        public async Task<ActionResult<Requisition>> CreateRequisition(CreateRequisitionDto createDto)
        {
            // Check if this will be the first requisition and reset identity seed if needed
            if (!await _context.Requisitions.AnyAsync())
            {
                ResetIdentitySeed();
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

            _context.Requisitions.Add(requisition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequisition), new { id = requisition.RequisitionId }, requisition);
        }

        // PUT: api/requisitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequisition(int id, UpdateRequisitionDto updateDto)
        {
            var requisition = await _context.Requisitions.FindAsync(id);
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

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/requisitions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequisition(int id)
        {
            var requisition = await _context.Requisitions.FindAsync(id);
            if (requisition == null) return NotFound();

            _context.Requisitions.Remove(requisition);
            await _context.SaveChangesAsync();

            // Check if this was the last requisition and reset identity seed if so
            if (!await _context.Requisitions.AnyAsync())
            {
                ResetIdentitySeed();
            }

            return NoContent();
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllRequisitions()
        {
            var requisitions = await _context.Requisitions.ToListAsync();
            if (requisitions.Any())
            {
                _context.Requisitions.RemoveRange(requisitions);
                await _context.SaveChangesAsync();
                ResetIdentitySeed();
            }

            return NoContent();
        }

        private void ResetIdentitySeed()
        {
            // Reset the identity seed for Requisitions table using constant values (safe from SQL injection)
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Requisitions', RESEED, 0)");
        }
    }
}
