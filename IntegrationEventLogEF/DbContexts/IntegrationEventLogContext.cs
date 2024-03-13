
using IntegrationEventLogEF.Entities;

namespace IntegrationEventLogEF.DbContexts;
public class IntegrationEventLogContext : DbContext

{

    public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options) : base(options)
    {
    }
    public DbSet<IntegrationEventOutbox> IntegrationEventLogs { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IntegrationEventOutbox>(ConfigureIntegrationEventLogEntry);
    }

    void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventOutbox> builder)
    {
        builder.ToTable("IntegrationEventLog");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.CreationTime)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder.Property(e => e.EventTypeName)
            .IsRequired();

 
        builder.Property(e => e.EventType)
    .IsRequired();

    }
}
