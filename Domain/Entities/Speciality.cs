using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("specialities")]
public class Speciality
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid id { get; set; }
    
    [Column("create_time")]
    public DateTime CreateTime { get; set; }
    
    [Column("name")]
    public string name { get; set; }
}