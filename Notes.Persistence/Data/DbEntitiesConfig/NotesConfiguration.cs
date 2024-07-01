using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notes.Domain;

namespace Notes.Persistence.Data.DbEntitiesConfig
{
    internal class NotesConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            SetTableProperties(builder);
            SetPersonalIdProperties(builder);
            SetNameProperties(builder);
            SetDescriptionProperties(builder);
            SetTimeProperties(builder);
            SetIsCompletedProperties(builder);
        }

        EntityTypeBuilder<Note> SetTableProperties(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("notes");
            builder.HasKey(t => new { t.Id, t.PersonalId });

            builder.Property(u => u.Id)
                .HasColumnName("note_id");

            // создание промежуточной таблицы для реализации связи
            // многое ко многому между таблицами "Notes" и "Categories"
            builder
                .HasMany(n => n.Categories)
                .WithMany(u => u.Notes)
                .UsingEntity(j => j.ToTable("note_categories"));

            builder.HasOne(n => n.User);
            
            return builder;
        }

        EntityTypeBuilder<Note> SetPersonalIdProperties(EntityTypeBuilder<Note> builder)
        {
            builder.Property(u => u.PersonalId)
                .HasColumnName("personal_id")
                .IsRequired()
                .HasColumnType("integer");

            return builder;
        }

        EntityTypeBuilder<Note> SetNameProperties(EntityTypeBuilder<Note> builder)
        {
            builder.Property(u => u.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasColumnType("text")
                .HasMaxLength(80);

            return builder;
        }

        EntityTypeBuilder<Note> SetDescriptionProperties(EntityTypeBuilder<Note> builder)
        {
            builder.Property(u => u.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasColumnType("text")
                .HasMaxLength(400);

            return builder;
        }

        EntityTypeBuilder<Note> SetTimeProperties(EntityTypeBuilder<Note> builder)
        {
            builder.Property(u => u.Time)
                .HasColumnName("time")
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            return builder;
        }

        EntityTypeBuilder<Note> SetIsCompletedProperties(EntityTypeBuilder<Note> builder)
        {
            builder.Property(u => u.IsCompleted)
                .HasColumnName("is_completed")
                .IsRequired()
                .HasColumnType("boolean");

            return builder;
        }
    }
}
