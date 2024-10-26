using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("consultation")]
public class Consultation
{
    [Key]
    [Column("id")]
    public Guid id { get; set; }
    
    [Column("create_time")]
    public DateTime createTime { get; set; }

    [ForeignKey("speciality_id")]
    public Speciality speciality { get; set; }
    
    [ForeignKey("inspection_id")]
    public Inspection inspection { get; set; }
}