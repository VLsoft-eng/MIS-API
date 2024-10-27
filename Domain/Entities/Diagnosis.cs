using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain;

[Table("diagnosis")]
public class Diagnosis
{
    [Key]
    [Column("id")]
    public Guid id { get; set; }

    [ForeignKey("icd_diagnosis_id")]
    public Icd icd { get; set; }
    
    [Column("description")]
    public string description { get; set; }
    
    [Column("diagnosis_type ")]
    public DiagnosisType diagnosisType { get; set; }
    
    [ForeignKey("inspection_id")]
    public Inspection inspection { get; set; }
}