using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class AdministrativeRequestRepository : Repository<AdministrativeRequest>, IAdministrativeRequestRepository
{
    private readonly ApplicationDbContext _db;
    public AdministrativeRequestRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(AdministrativeRequest request)
    {
        var requestFromDb = await _db.AdministrativeRequests
            .FirstOrDefaultAsync(r => r.Id == request.Id);
        if (requestFromDb is null)
            return;
        
        requestFromDb.RequestType = request.RequestType;
        requestFromDb.Purpose = request.Purpose;
        requestFromDb.RequiredDate = request.RequiredDate;
        requestFromDb.IsReplacement = request.IsReplacement;
        requestFromDb.ReplacementReason = request.ReplacementReason;
        requestFromDb.AdditionalNotes = request.AdditionalNotes;
        requestFromDb.Status = request.Status;
        requestFromDb.ApprovedById = request.ApprovedById;
        requestFromDb.ApprovalDate = request.ApprovalDate;
        requestFromDb.Comments = request.Comments;
    }
}