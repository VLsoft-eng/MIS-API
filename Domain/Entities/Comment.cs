using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table(("Comment"))]
public class Comment
{
    [Key]
    [Column("id")]
    public Guid id { get; set; }
    
    [Column("create_time")]
    public DateTime createTime { get; set; }
    
    [Column("modified_date")]
    public DateTime modifiedDate { get; set; }
    
    [Column("content")]
    public string content { get; set; }
    
    [ForeignKey("author_id")]
    public Doctor author { get; set; }
    
    [ForeignKey("consultation_id")]
    public Consultation consultation { get; set; }
    
    [ForeignKey("parent_id")]
    public Comment? parent { get; set; }
}