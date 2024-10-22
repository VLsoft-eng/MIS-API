using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain;

[Table("doctors")]
public class Doctor
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid id { get; set; }
    
    [Column("create_time")]
    public DateTime createTime { get; set; }
    
    [Column("hashed_password")]
    public string hashedPassword { get; set; }

    [Column("email")]
    [EmailAddress]
    public string email { get; set; }

    [Column("name")]
    public string name { get; set; }
    
    [Column("birthday")]
    public DateTime birthday { get; set; }

    [Column("gender")]
    public Gender gender { get; set; }
    
    [Column("phone")]
    public string phone { get; set; }
    
    public Speciality speciality { get; set; }
}