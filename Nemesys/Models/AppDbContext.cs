using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Nemesys.Models;

namespace Nemesys.Models
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Investigation> Investigation { get; set; }
        public DbSet<Upvotes> Upvotes { get; set; }
    }
}
