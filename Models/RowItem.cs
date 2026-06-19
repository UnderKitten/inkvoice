using System.ComponentModel.DataAnnotations.Schema;

namespace Inkvoice.Models;

public class RowItem
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string ItemName { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Hours { get; set; }
    public decimal Rate { get; set; }

    [NotMapped]
    public decimal RowTotal => Hours * Rate;
}
