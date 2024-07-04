using System;
using System.Collections.Generic;

namespace LiveMedyAIProject.Models
{
    public class MedicalHistory
    {
        public string TitleTr { get; set; }
        public List<MedicalHistoryQuestionAnswer> MedicalHistoryQuestionAnswers { get; set; }
    }

    public class MedicalHistoryQuestionAnswer
    {
        public string MedicalHistoryQuestionId { get; set; }
        public string MedicalQuestionText { get; set; }
        public string PatientAnswer { get; set; }
        public string AnswerValue { get; set; } // This is nullable
    }

}
