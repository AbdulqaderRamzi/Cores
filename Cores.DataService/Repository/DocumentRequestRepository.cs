using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class DocumentRequestRepository : Repository<DocumentRequest>, IDocumentRequestRepository
{
    private readonly ApplicationDbContext _db;
    public DocumentRequestRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(DocumentRequest request)
    {
        var requestFromDb = await _db.DocumentRequests
            .FirstOrDefaultAsync(r => r.Id == request.Id);
        if (requestFromDb is null)
            return;
        
        requestFromDb.DocumentType = request.DocumentType;
        requestFromDb.Purpose = request.Purpose;
        requestFromDb.AdditionalDetails = request.AdditionalDetails;
        requestFromDb.Status = request.Status;
        requestFromDb.ApprovedById = request.ApprovedById;
        requestFromDb.ApprovalDate = request.ApprovalDate;
        requestFromDb.Comments = request.Comments;
    }
}