using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
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
        public async Task<ActionResult<Requisition>> CreateRequisition(Requisition requisition)
        {
            _context.Requisitions.Add(requisition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequisition), new { id = requisition.RequisitionId }, requisition);
        }

        // PUT: api/requisitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequisition(int id, Requisition requisition)
        {
            if (id != requisition.RequisitionId) return BadRequest();

            _context.Entry(requisition).State = EntityState.Modified;
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

            return NoContent();
        }
    }
}
