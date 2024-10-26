using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain;

public class Patient
{
    [Key]
    [Column("id")]
    public Guid id { get; set; }
    
    [Column("create_time")]
    public DateTime createTime { get; set; }
    
    [Column("name")]
    public string name { get; set; }
    
    [Column("birthday")]
    public DateTime birtday { get; set; }

    [Column("gender")]
    public Gender gender { get; set; }
}