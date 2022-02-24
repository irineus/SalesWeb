#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWeb.Models.Entities;

namespace SalesWeb.Data
{
    public class SalesWebContext : DbContext
    {
        public SalesWebContext (DbContextOptions<SalesWebContext> options)
            : base(options)
        {
        }

        public DbSet<SalesWeb.Models.Entities.Department> Department { get; set; }

        public DbSet<SalesWeb.Models.Entities.Seller> Seller { get; set; }

        public DbSet<SalesWeb.Models.Entities.SalesRecord> SalesRecord { get; set; }
    }
}
