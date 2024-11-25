using System.Security.Claims;
using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ProblemRepository : Repository<Problem>, IProblemRepository
{
    private readonly ApplicationDbContext _db;
    
    public ProblemRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Problem problem)
    {
        var problemFromDb = await _db.Problems.FirstOrDefaultAsync(p => p.Id == problem.Id);
        if (problemFromDb is null)
            return;
        problemFromDb.ProblemTypeId = problem.ProblemTypeId;
        problemFromDb.Severity = problem.Severity;
        problemFromDb.Status = problem.Status;
        problemFromDb.Description = problem.Description;
        problemFromDb.Resolution = problem.Resolution;
        problemFromDb.Note = problem.Note;
        problemFromDb.ContactId = problem.ContactId;
        if (problem.ModifiedById is not null)
        {
            problemFromDb.ModifiedById = problem.ModifiedById;

        }
    }

    public async Task<(int OpenProblems, int PendingProblems, int ClosedProblems)> GetStatusCounts()
    {
        var openProblems = await _db.Problems.CountAsync(p => p.Status == "open");
        var pendingProblems = await _db.Problems.CountAsync(p => p.Status == "pending");
        var closedProblems = await _db.Problems.CountAsync(p => p.Status == "closed");
        return (OpenProblems: openProblems, PendingProblems: pendingProblems, ClosedProblems: closedProblems);
    }


}