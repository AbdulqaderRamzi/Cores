using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IDocumentRequestRepository : IRepository<DocumentRequest>
{
    Task Update(DocumentRequest documentRequest);
}