using System.ComponentModel.DataAnnotations.Schema;

namespace Inkvoice.Models;

public class Document
{
    public int Id { get; set; }
    public DocumentType Type { get; set; }
    public string Number { get; set; } = "";
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Today;
    public DateTime? DueDate { get; set; }
    public string? Comments { get; set; }
    public decimal TaxRate { get; set; } = 0.13m; // TODO: Make this configurable
    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;

    public List<RowItem> Items { get; set; } = new();

    [NotMapped]
    public decimal Subtotal => Items.Sum(i => i.RowTotal);

    [NotMapped]
    public decimal TaxAmount => Math.Round(Subtotal * TaxRate, 2);

    [NotMapped]
    public decimal Total => Subtotal + TaxAmount;
}

public enum DocumentType
{
    Estimate,
    Invoice,
}

public enum DocumentStatus
{
    Draft,
    Sent,
    Paid,
}
