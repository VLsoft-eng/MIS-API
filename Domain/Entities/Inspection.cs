using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain;

[Table("inspection")]
public class Inspection
{
    [Key]
    [Column("id")]
    public Guid id { get; set; }
    
    [Column("date")]
    public DateTime date { get; set; }
    
    [Column("anamnesis")]
    public string anamnesis { get; set; }
    
    [Column("complaints")]
    public string complaints { get; set; }
    
    [Column("treatment")]
    public string treatment { get; set; }
    
    [Column("conclustion")]
    public Conclusion conclusion { get; set; }
    
    [Column("next_visit_date")]
    public DateTime? nextVisitDate { get; set; }
    
    [Column("death_date")]
    public DateTime? deathDate { get; set; }
    
    [Column("create_time")]
    public DateTime createTime { get; set; }
    
    [Column("is_notified")]
    public bool isNotified { get; set; }
    
    [ForeignKey("doctor_id")]
    public Doctor doctor { get; set; }
    
    [ForeignKey("patient_id)")]
    public Patient patient { get; set; }
    
    [ForeignKey("previous_inspection_id")]
    public Inspection? previousInspection { get; set; }
}
