using System;
using System.Collections.Generic;

namespace LiveMedyAIProject.Models
{
    public class PatientData
    {
        public Guid PatientId { get; set; }
        public List<MedicalHistory> MedicalHistory { get; set; }
        public List<Complaint> Complaint { get; set; } // Changed to List<Complaint> to match the JSON structure
        public List<UserDetails> UserDetails { get; set; }
        public List<PatientDetail> PatientDetails { get; set; }
        public List<PatientOperationHistory> PatientOperationHistories { get; set; }
        public List<PatientPet> PatientPets { get; set; }
    }
}

public class UserDetails
{
    public Guid UserId { get; set; }
    public Guid BloodTypeId { get; set; }
    public int Height { get; set; }
    public int HeightUnit { get; set; }
    public double Weight { get; set; }
    public int WeightUnit { get; set; }
    public bool HasAllergy { get; set; }
    public string Allergies { get; set; }
    public bool HadBloodTransfusion { get; set; }
}