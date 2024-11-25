using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Data;

public class HangfireDbContext(DbContextOptions<HangfireDbContext> options) : DbContext(options);   