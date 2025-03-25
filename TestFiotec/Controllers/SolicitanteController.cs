using Microsoft.AspNetCore.Mvc;
using TestFiotec.Model;
using TestFiotec.Services.Interface;

namespace TestFiotec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitanteController : ControllerBase
    {
        private readonly ISolicitanteService _solicitanteService;

        public SolicitanteController(ISolicitanteService solicitanteService)
        {
            _solicitanteService = solicitanteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var solicitantes = await _solicitanteService.GetAllAsync();
            return Ok(solicitantes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var solicitante = await _solicitanteService.GetByIdAsync(id);
            if (solicitante == null)
            {
                return NotFound();
            }
            return Ok(solicitante);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Solicitante solicitante)
        {
            await _solicitanteService.AddAsync(solicitante);
            return CreatedAtAction(nameof(GetById), new { id = solicitante.Id }, solicitante);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Solicitante solicitante)
        {
            if (id != solicitante.Id)
            {
                return BadRequest();
            }

            await _solicitanteService.UpdateAsync(solicitante);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _solicitanteService.DeleteAsync(id);
            return NoContent();
        }
    }
}
