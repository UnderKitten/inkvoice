using Inkvoice.Models;
using Microsoft.EntityFrameworkCore;

namespace Inkvoice.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<RowItem> RowItems => Set<RowItem>();
}
