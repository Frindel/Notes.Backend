using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notes.Domain;

namespace Notes.Persistence.Data.DbEntitiesConfig
{
    internal class UsersConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            SetTableProperties(builder);
            SetLoginProperties(builder);
            SetPasswordProperties(builder);
            SetRefreshTokenProperties(builder);
        }

        EntityTypeBuilder<User> SetTableProperties(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(t => t.Id);

            builder.Property(u => u.Id)
                .HasColumnName("user_id");

            return builder;
        }

        EntityTypeBuilder<User> SetLoginProperties(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Login)
                .HasColumnName("login")
                .IsRequired()
                .HasColumnType("text")
                .HasMaxLength(20);

            return builder;
        }

        EntityTypeBuilder<User> SetPasswordProperties(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Password)
                .HasColumnName("password")
                .IsRequired()
                .HasColumnType("text")
                .HasMaxLength(256);

            return builder;
        }

        EntityTypeBuilder<User> SetRefreshTokenProperties(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.RefreshToken)
                .HasColumnName("refresh_token")
                .IsRequired()
                .HasColumnType("text")
                .HasMaxLength(256);

            return builder;
        }
    }
}
