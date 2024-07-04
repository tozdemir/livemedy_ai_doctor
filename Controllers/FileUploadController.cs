using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using LiveMedyAIProject.Models;
using LiveMedyAIProject.Services;
using Microsoft.Extensions.Logging;

namespace LiveMedyAIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly PdfToImageConverter _pdfToImageConverter;
        private readonly OpenAiVisionClient _visionClient;
        private readonly IOpenAiService _openAiService;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(PdfToImageConverter pdfToImageConverter, OpenAiVisionClient visionClient, IOpenAiService openAiService, ILogger<FileUploadController> logger)
        {
            _pdfToImageConverter = pdfToImageConverter;
            _visionClient = visionClient;
            _openAiService = openAiService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] string patientData, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            PatientData patientDataObject;
            try
            {
                patientDataObject = JsonConvert.DeserializeObject<PatientData>(patientData);
            }
            catch (JsonException)
            {
                return BadRequest("Invalid patient data.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var imageTexts = new List<string>();

            if (fileExtension == ".pdf")
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset the stream position to the beginning

                var images = _pdfToImageConverter.ConvertPdfToImages(stream);

                foreach (var image in images)
                {
                    try
                    {
                        var imageBytes = _pdfToImageConverter.BitmapToByteArray(image);
                        var result = await _visionClient.ProcessImage(imageBytes);
                        imageTexts.Add(result);
                    }
                    finally
                    {
                        image.Dispose();
                    }
                }
            }
            else if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var imageBytes = stream.ToArray();
                var result = await _visionClient.ProcessImage(imageBytes);
                imageTexts.Add(result);
            }
            else
            {
                return BadRequest("Unsupported file type.");
            }

            var combinedText = string.Join("\n", imageTexts);
            var specialistSuggestionResponse = await _openAiService.GetSpecialistSuggestion(patientDataObject, combinedText);
            return Ok(specialistSuggestionResponse);
        }
    }
}
