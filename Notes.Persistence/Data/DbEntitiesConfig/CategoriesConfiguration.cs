using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notes.Domain;

namespace Notes.Persistence.Data.DbEntitiesConfig
{
    internal class CategoriesConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            SetTableProperties(builder);
            SetPersonalIdProperties(builder);
            SetNameProperties(builder);
        }

        EntityTypeBuilder<Category> SetTableProperties(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            builder.HasKey(t => new { t.Id, t.PersonalId});

            builder.Property(c => c.Id)
                .HasColumnName("category_id");

            builder.HasOne(c => c.User);

            return builder;
        }

        EntityTypeBuilder<Category> SetPersonalIdProperties(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.PersonalId)
                .HasColumnName("personal_id")
                .IsRequired()
                .HasColumnType("integer");

            return builder;
        }

        EntityTypeBuilder<Category> SetNameProperties(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasColumnType("text")
                .HasMaxLength(30);

            return builder;
        }
    }
}
