using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LiveMedyAIProject.Models;
using LiveMedyAIProject.Services;

namespace LiveMedyAIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialistController : ControllerBase
    {
        private readonly IOpenAiService _openAiService;

        public SpecialistController(IOpenAiService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpPost("suggest-specialist")]
        public async Task<IActionResult> SuggestSpecialist([FromBody] PatientData patientData)
        {
            if (patientData == null)
                return BadRequest("Invalid patient data.");

            var specialistSuggestionResponse = await _openAiService.GetSpecialistSuggestion(patientData);
            return Ok(specialistSuggestionResponse);
        }
    }
}
