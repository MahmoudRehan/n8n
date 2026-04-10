using Microsoft.EntityFrameworkCore;
using n8n.Data.Entities;

namespace n8n.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Sender).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(4000).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
