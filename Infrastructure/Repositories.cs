using ContractPdfMerger.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContractPdfMerger.Infrastructure;

public interface IDocumentRepository
{
    Task<List<SupplementalDocument>> GetAllAsync();
    Task<SupplementalDocument?> GetByIdAsync(int id);
    Task<int> AddAsync(SupplementalDocument document);
    Task UpdateAsync(SupplementalDocument document);
    Task DeleteAsync(int id);
}

public interface IDocumentTypeRepository
{
    Task<List<DocumentType>> GetAllAsync();
    Task<DocumentType?> GetByCodeAsync(string typeCode);
    Task AddAsync(DocumentType documentType);
    Task UpdateAsync(DocumentType documentType);
    Task DeleteAsync(string typeCode);
    Task<bool> IsTypeInUseAsync(string typeCode);
}

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplementalDocument>> GetAllAsync()
    {
        return await _context.SupplementalDocuments
            .Include(d => d.DocumentType)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<SupplementalDocument?> GetByIdAsync(int id)
    {
        return await _context.SupplementalDocuments
            .Include(d => d.DocumentType)
            .FirstOrDefaultAsync(d => d.ID == id);
    }

    public async Task<int> AddAsync(SupplementalDocument document)
    {
        _context.SupplementalDocuments.Add(document);
        await _context.SaveChangesAsync();
        return document.ID;
    }

    public async Task UpdateAsync(SupplementalDocument document)
    {
        document.UpdatedAt = DateTime.UtcNow;
        _context.SupplementalDocuments.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var document = await _context.SupplementalDocuments.FindAsync(id);
        if (document != null)
        {
            _context.SupplementalDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly AppDbContext _context;

    public DocumentTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentType>> GetAllAsync()
    {
        return await _context.DocumentTypes
            .OrderBy(dt => dt.TypeName)
            .ToListAsync();
    }

    public async Task<DocumentType?> GetByCodeAsync(string typeCode)
    {
        return await _context.DocumentTypes
            .FirstOrDefaultAsync(dt => dt.TypeCode == typeCode);
    }

    public async Task AddAsync(DocumentType documentType)
    {
        _context.DocumentTypes.Add(documentType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DocumentType documentType)
    {
        _context.DocumentTypes.Update(documentType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string typeCode)
    {
        var documentType = await _context.DocumentTypes.FindAsync(typeCode);
        if (documentType != null)
        {
            _context.DocumentTypes.Remove(documentType);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsTypeInUseAsync(string typeCode)
    {
        return await _context.SupplementalDocuments
            .AnyAsync(sd => sd.TypeCode == typeCode);
    }
}