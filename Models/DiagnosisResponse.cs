using System;
using System.Collections.Generic;

public class DiagnosisResponse
{
    public string Diagnosis { get; set; }
    public List<string> Specialists { get; set; }
    public List<string> Tests { get; set; }
}
