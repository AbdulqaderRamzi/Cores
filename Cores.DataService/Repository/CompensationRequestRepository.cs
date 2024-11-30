using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class CompensationRequestRepository : Repository<CompensationRequest>, ICompensationRequestRepository
{
    private readonly ApplicationDbContext _db;
    public CompensationRequestRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(CompensationRequest request)
    {
        var requestFromDb = await _db.CompensationRequests
            .FirstOrDefaultAsync(r => r.Id == request.Id);
        if (requestFromDb is null)
            return;
        
        requestFromDb.RequestType = request.RequestType;
        requestFromDb.RequestedAmount = request.RequestedAmount;
        requestFromDb.PeriodStart = request.PeriodStart;
        requestFromDb.PeriodEnd = request.PeriodEnd;
        requestFromDb.Justification = request.Justification;
        requestFromDb.SupportingDocuments = request.SupportingDocuments;
        requestFromDb.Status = request.Status;
        requestFromDb.ApprovedById = request.ApprovedById;
        requestFromDb.ApprovalDate = request.ApprovalDate;
        requestFromDb.Comments = request.Comments;
    }
}