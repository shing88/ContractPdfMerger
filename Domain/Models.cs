using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractPdfMerger.Domain;

public class DocumentType
{
    [Key]
    [MaxLength(50)]
    public string TypeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string TypeName { get; set; } = string.Empty;

    public virtual ICollection<SupplementalDocument> SupplementalDocuments { get; set; } = new List<SupplementalDocument>();
}

public class SupplementalDocument
{
    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string TypeCode { get; set; } = string.Empty;

    [Required]
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    [ForeignKey(nameof(TypeCode))]
    public virtual DocumentType? DocumentType { get; set; }
}

public class MergeTargetItem
{
    public string FileName { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public bool IsFromDatabase { get; set; }
    public int? DatabaseId { get; set; }
}