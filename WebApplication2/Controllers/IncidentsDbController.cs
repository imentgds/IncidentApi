using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.model;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsDbController : ControllerBase
    {
        private readonly IncidentsDbContext _context;

        public IncidentsDbController(IncidentsDbContext context)
        {
            _context = context;
        }

        // GET: api/IncidentsDb
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            return await _context.Incidents.ToListAsync();
        }

        // GET: api/IncidentsDb/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncident(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);

            if (incident == null)
            {
                return NotFound();
            }

            return incident;
        }

        // PUT: api/IncidentsDb/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncident(int id, Incident incident)
        {
            if (id != incident.id)
            {
                return BadRequest();
            }

            _context.Entry(incident).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncidentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/IncidentsDb
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Incident>> PostIncident(Incident incident)
        {

            incident.status = "OPEN";
            incident.createdat = DateTime.Now;

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIncident", new { id = incident.id }, incident);
        }

        // DELETE: api/IncidentsDb/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IncidentExists(int id)
        {
            return _context.Incidents.Any(e => e.id == id);
        }


        [HttpGet("severity/{severity}")]
        public ActionResult<IEnumerable<Incident>> FilterBySeverity(string severity)
        {
            var incidents = _context.Incidents
                .Where(i => i.severity.Contains(severity))
                .ToList();

            if (!incidents.Any())
            {
                return NotFound($"Aucun incident trouvé avec la gravité contenant '{severity}'");
            }

            return Ok(incidents);
        }


        [HttpGet("status/{status}")]
        public ActionResult<IEnumerable<Incident>> FilterByStatus(string status)
        {
            var incidents = _context.Incidents
                .Where(i => i.status.Contains(status))
                .ToList();

            if (!incidents.Any())
            {
                return NotFound($"Aucun incident trouvé avec le statut contenant '{status}'");
            }

            return Ok(incidents);
        }

        private static readonly string[] AllowedSeverities = { "LOW", "MEDIUM", "HIGH", "CRITICAL" };
        private static readonly string[] AllowedStatuses = { "OPEN", "IN_PROGRESS", "RESOLVED" };
    }

}
