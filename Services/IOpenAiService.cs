using System.Threading.Tasks;
using LiveMedyAIProject.Models;

namespace LiveMedyAIProject.Services
{
    public interface IOpenAiService
    {
        Task<SpecialistSuggestionResponse> GetSpecialistSuggestion(PatientData patientData, string extractedFileText = "");
        Task<VeterinaryResponse> GetPetDiagnosis(PetDetail petDetail);
    }
}
