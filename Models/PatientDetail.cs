using System;
using System.Collections.Generic;

namespace LiveMedyAIProject.Models
{
    public class PatientDetail
    {
        public string Allergies { get; set; }
        public string BloodTypeId { get; set; }
        public bool HadBloodTransfusion { get; set; }
        public bool HasAllergy { get; set; }
        public int Height { get; set; }
        public int HeightUnit { get; set; }
        public double Weight { get; set; }
        public int WeightUnit { get; set; }
    }
}