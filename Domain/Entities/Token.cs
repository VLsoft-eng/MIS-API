using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("banned_tokens")]
public class Token
{
    [Key]
    [Column("id")]
    public Guid id;

    [Column("token_value")]
    public string tokenValue;
}