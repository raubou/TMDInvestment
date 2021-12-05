using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDInvestment.Models;
using TMDInvestment.Repository;

namespace TMDInvestment.DBContext
{
    public class TMDInvestmentContext : DbContext
    {
        public TMDInvestmentContext()
        {

        }
        public TMDInvestmentContext(DbContextOptions<TMDInvestmentContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            var DBConnectionStrings = configuration.GetSection("ConnectionStrings");
            var DBConnection = DBConnectionStrings.GetSection("DefaultConnection");
            optionsBuilder.UseSqlServer(
                    DBConnection.Value);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        //entities
        public DbSet<Users> users { get; set; }
        public DbSet<RulesEngine> rulesEngine { get; set; }
        public DbSet<PurchaseHistory> purchaseHistory { get; set;}
        public DbSet<AccessKeys> accessKeys { get; set; }
        public DbSet<CryptoProducts> cryptoProducts { get; set; }
}
}
