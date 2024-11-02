using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("banned_tokens")]
public class Token
{
    [Key]
    [Column("id")]
    public Guid id { get; set; }

    [Column("token_value")]
    public string tokenValue { get; set; }
    
    [Column("expired_at")]
    public DateTime expiresAt { get; set; }
}