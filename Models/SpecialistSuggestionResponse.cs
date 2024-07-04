using System.Collections.Generic;

namespace LiveMedyAIProject.Models
{
    public class SpecialistSuggestionResponse
    {
        public Dictionary<string, DiagnosisResponse> Diagnoses { get; set; }
    }
}
