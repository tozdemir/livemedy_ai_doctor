using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LiveMedyAIProject.Models;
using LiveMedyAIProject.Services;


[ApiController]
[Route("api/[controller]")]
public class PetController : ControllerBase
{
    private readonly IOpenAiService _openAiService;

    public PetController(IOpenAiService openAiService)
    {
        _openAiService = openAiService;
    }

    [HttpPost("diagnose-pet")]
    public async Task<IActionResult> DiagnosePet([FromBody] PetDetail petDetail)
    {
        if (petDetail == null)
                return BadRequest("Invalid pet data.");

        var response = await _openAiService.GetPetDiagnosis(petDetail);
        return Ok(response);
    }
}
