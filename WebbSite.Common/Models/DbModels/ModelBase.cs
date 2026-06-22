using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebbSite.Common.Models.DbModels;
public abstract class ModelBase
{
    [Key]
    [Column("id")]
    public Int64 Id { get; set; }
}