using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PlayerRoster.Server.Data.Models;  

namespace PlayerRoster.Server.Data
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var conn = "Server=(localdb)\\mssqllocaldb;Database=NBAExplorerDb;Trusted_Connection=True;";
            builder.UseSqlServer(conn);
            return new ApplicationDbContext(builder.Options);
        }
    }
}
