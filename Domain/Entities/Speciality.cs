using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("speciality")]
public class Speciality
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid id { get; set; }
    
    [Column("create_time")]
    public DateTime CreateTime { get; set; }
    
    [Column("name")]
    public string name { get; set; }
}