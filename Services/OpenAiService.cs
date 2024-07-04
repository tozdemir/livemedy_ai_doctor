using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiveMedyAIProject.Models; // Ensure this directive is present
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Net.Http.Json;

namespace LiveMedyAIProject.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAiService> _logger;

        public OpenAiService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<SpecialistSuggestionResponse> GetSpecialistSuggestion(PatientData patientData, string additionalInfo = "")
        {
            var openAiRequest = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = GeneratePrompt(patientData, additionalInfo) }
                },
                max_tokens = 1000
            };

            var content = new StringContent(JsonConvert.SerializeObject(openAiRequest), Encoding.UTF8, "application/json");

            // Add the API key to the Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["OpenAi:ApiKey"]);

            var response = await _httpClient.PostAsync(_configuration["OpenAi:ChatApiUrl"], content);
            var responseString = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"OpenAI API response: {responseString}"); // Log the response string to the terminal

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"OpenAI API request failed: {response.StatusCode}, {responseString}");
            }


            var parsedResponse = ParseResponse(responseString);
            return parsedResponse;

        }

        private string GeneratePrompt(PatientData patientData, string additionalInfo = "")
        {
            var prompt = new StringBuilder();
            prompt.AppendLine("Patient Medical Background and Complaints:");

            if (patientData.MedicalHistory != null)
            {
                foreach (var history in patientData.MedicalHistory)
                {
                    prompt.AppendLine($"Category: {history.TitleTr}");
                    if (history.MedicalHistoryQuestionAnswers != null)
                    {
                        foreach (var answer in history.MedicalHistoryQuestionAnswers)
                        {
                            prompt.AppendLine($"{answer.MedicalQuestionText}: {answer.PatientAnswer}");
                            if (!string.IsNullOrEmpty(answer.AnswerValue))
                            {
                                prompt.AppendLine($"Answer Details: {answer.AnswerValue}");
                            }
                        }
                    }
                }
            }

            if (patientData.Complaint != null)
            {
                prompt.AppendLine("Complaints:");
                foreach (var complaint in patientData.Complaint)
                {
                    prompt.AppendLine($"Title: {complaint.Title}");
                    prompt.AppendLine($"Details: {complaint.Details}");
                }
            }

            if (patientData.PatientDetails != null)
            {
                prompt.AppendLine("Patient Details:");
                foreach (var detail in patientData.PatientDetails)
                {
                    prompt.AppendLine($"Allergies: {detail.Allergies}");
                    prompt.AppendLine($"Blood Type ID: {detail.BloodTypeId}");
                    prompt.AppendLine($"Had Blood Transfusion: {detail.HadBloodTransfusion}");
                    prompt.AppendLine($"Has Allergy: {detail.HasAllergy}");
                    prompt.AppendLine($"Height: {detail.Height} (Unit: {detail.HeightUnit})");
                    prompt.AppendLine($"Weight: {detail.Weight} (Unit: {detail.WeightUnit})");
                }
            }

            if (patientData.PatientOperationHistories != null)
            {
                prompt.AppendLine("Operation Histories:");
                foreach (var operation in patientData.PatientOperationHistories)
                {
                    prompt.AppendLine($"Name: {operation.Name}");
                    prompt.AppendLine($"Year: {operation.Year}");
                }
            }

            if (patientData.PatientPets != null)
            {
                prompt.AppendLine("Pets:");
                foreach (var pet in patientData.PatientPets)
                {
                    prompt.AppendLine($"Pet Name: {pet.FullName}");
                    prompt.AppendLine($"About Pet: {pet.AboutPet}");
                    prompt.AppendLine($"Age: {pet.AgeYear} years and {pet.AgeMonth} months");
                    prompt.AppendLine($"Gender: {(pet.Gender == 0 ? "Male" : "Female")}");
                }
            }

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                prompt.AppendLine("Additional Information:");
                prompt.AppendLine(additionalInfo);
            }

            prompt.AppendLine("Based on the above information, please provide the following:");
            prompt.AppendLine("1. Possible 5 reasons for the complaint.");
            prompt.AppendLine("2. Two specialists at least the patient should consult for each reason.");
            prompt.AppendLine("3. Recommended tests the patient should undergo for each reason.");
            prompt.AppendLine("Return response in Turkish.");
            prompt.AppendLine("Return response in JSON format with the following structure:");
            prompt.AppendLine("{");
            prompt.AppendLine("  \"Diagnosis 1\": { \"Diagnosis\": \"Diagnosis description\", \"Specialists\": [\"Specialist 1\", \"Specialist 2\"], \"Tests\": [\"Test 1\", \"Test 2\"] },");
            prompt.AppendLine("  \"Diagnosis 2\": { \"Diagnosis\": \"Diagnosis description\", \"Specialists\": [\"Specialist 1\", \"Specialist 2\"], \"Tests\": [\"Test 1\", \"Test 2\"] },");
            prompt.AppendLine("  \"Diagnosis 3\": { \"Diagnosis\": \"Diagnosis description\", \"Specialists\": [\"Specialist 1\", \"Specialist 2\"], \"Tests\": [\"Test 1\", \"Test 2\"] }");
            prompt.AppendLine("}");

            return prompt.ToString();
        }



        private SpecialistSuggestionResponse ParseResponse(string responseString)
        {
            dynamic responseJson = JsonConvert.DeserializeObject(responseString);
            var responseContent = (string)responseJson.choices[0].message.content;

            // Log the response content
            _logger.LogInformation($"Response content: {responseContent}");

            // Strip markdown markers if present
            if (responseContent.StartsWith("```json"))
            {
                responseContent = responseContent.Substring(7); // Remove the initial "```json\n"
            }
            if (responseContent.EndsWith("```"))
            {
                responseContent = responseContent.Substring(0, responseContent.Length - 3); // Remove the trailing "```"
            }

            // Log the cleaned response content
            _logger.LogInformation($"Cleaned response content: {responseContent}");

            // Deserialize the JSON content directly into SpecialistSuggestionResponse
            var diagnoses = JsonConvert.DeserializeObject<Dictionary<string, DiagnosisResponse>>(responseContent);

            return new SpecialistSuggestionResponse
            {
                Diagnoses = diagnoses
            };
        }



    }
}