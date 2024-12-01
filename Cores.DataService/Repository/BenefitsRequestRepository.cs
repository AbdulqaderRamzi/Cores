using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class BenefitsRequestRepository : Repository<BenefitsRequest>, IBenefitsRequestRepository
{
    private readonly ApplicationDbContext _db;
    public BenefitsRequestRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(BenefitsRequest request)
    {
        var requestFromDb = await _db.BenefitsRequests
            .FirstOrDefaultAsync(r => r.Id == request.Id);
        if (requestFromDb is null)
            return;
        
        requestFromDb.BenefitType = request.BenefitType;
        requestFromDb.EffectiveDate = request.EffectiveDate;
        requestFromDb.Details = request.Details;
        requestFromDb.SupportingDocuments = request.SupportingDocuments;
        requestFromDb.Status = request.Status;
        requestFromDb.ApprovedById = request.ApprovedById;
        requestFromDb.ApprovalDate = request.ApprovalDate;
        requestFromDb.Comments = request.Comments;
    }
}