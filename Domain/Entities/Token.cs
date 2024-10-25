using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("banned_tokens")]
public class Token
{
    [Column("id")]
    public Guid id;

    [Column("token_value")]
    public string tokenValue;
}