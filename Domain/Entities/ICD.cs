using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("ICD_10")]
public class Icd
{
    [Key]
    [Column("id")]
    public Guid id { get; set; }
    
    [Column("code")]
    public string сode { get; set; }

    [Column("name")]
    public string name { get; set; }
    
    [Column("createTime")]
    public DateTime createTime { get; set; }
    
    [ForeignKey("parentId")]
    public Icd? parent { get; set; }
}