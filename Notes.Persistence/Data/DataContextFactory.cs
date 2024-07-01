using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Notes.Persistence.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            string connectionStr = GetConnectionString();

            var dbOptionsBuilder = new DbContextOptionsBuilder<DataContext>();
            dbOptionsBuilder.UseNpgsql(connectionStr);

            return new DataContext(dbOptionsBuilder.Options);
        }

        string GetConnectionString()
        {
            IConfigurationRoot configs = GetConfigs();

            string? connectionStr = configs.GetSection("localConnectionDbString").Value;
            if (connectionStr == null)
                throw new ArgumentNullException(nameof(connectionStr));

            return connectionStr;
        }

        IConfigurationRoot GetConfigs()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("migrationsSettings.json");

            return builder.Build();
        }
    }
}
