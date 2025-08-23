using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fin.Domain.Entities;

public class Recurrence
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Frequency { get; set; }
    public string BaseDescription { get; set; }
    public int BaseDay { get; set; }
    public string StartMonthYear { get; set; }
    public string EndMonthYear { get; set; }
}
