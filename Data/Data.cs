using Corridadecavalos.Models;
using Microsoft.EntityFrameworkCore;

namespace Corridadecavalos.Data;

public class CavalosDbContext(
    DbContextOptions<CavalosDbContext> options
) : DbContext(options)
{
    public DbSet<Cavalo> Cavalos => Set<Cavalo>();
}