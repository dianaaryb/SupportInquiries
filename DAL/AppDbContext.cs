using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Inquiry> Inquiries { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
}