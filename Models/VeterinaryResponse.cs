using System.Collections.Generic;

public class VeterinaryResponse
{
    public Dictionary<string, DiagnosisDetail> Diagnoses { get; set; }
}

public class DiagnosisDetail
{
    public string Diagnosis { get; set; }
    public List<string> Treatment { get; set; }
    public List<string> Medicine { get; set; }
}
