using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;

namespace Cores.DataService.Repository;

public class MessagePayloadRepository : Repository<MessagePayload>, IMessagePayloadRepository
{
    private readonly ApplicationDbContext _db;
        
    public MessagePayloadRepository(ApplicationDbContext db) : base(db) => _db = db;
    
}