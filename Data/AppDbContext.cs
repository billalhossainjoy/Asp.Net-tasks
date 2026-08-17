using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity=>
        {
            entity.ToTable("Users");

            entity.HasIndex(x => x.Email)
            .IsUnique().HasDatabaseName("Email_Users");
        });
    }
}