using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System;
using Microsoft.Extensions.Logging;

public class OpenAiVisionClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<OpenAiVisionClient> _logger;

    public OpenAiVisionClient(HttpClient httpClient, string apiKey, ILogger<OpenAiVisionClient> logger)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _logger = logger;
    }

    public async Task<string> ProcessImage(byte[] imageBytes)
    {
        // Convert image to base64
        string base64Image = Convert.ToBase64String(imageBytes);

        var payload = new
        {
            model = "gpt-4-turbo",
            messages = new[]
            {
                new {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = "This is an image of a lab test result. I want you to return the name of the tests, results, units and reference values for each test in json format." },
                        new {
                            type = "image_url",
                            image_url = new {
                                url = $"data:image/png;base64,{base64Image}"
                            }
                        }
                    }
                }
            },
            max_tokens = 300
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        _logger.LogInformation("Sending request to OpenAI API.");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);

        var responseString = await response.Content.ReadAsStringAsync();
        _logger.LogInformation($"OpenAI API response: {responseString}");
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"OpenAI API request failed: {response.StatusCode}, {responseString}");
            throw new HttpRequestException($"OpenAI API request failed: {response.StatusCode}, {responseString}");
        }

        return responseString;
    }
}
