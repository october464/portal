using Finportal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Data
{
    public class ApplicationsDbContext : IdentityDbContext<FPUser>
    {
        public ApplicationsDbContext(DbContextOptions<ApplicationsDbContext> options)
            : base(options)
        {
        }
        public DbSet<Finportal.Models.BankAccount> BankAccount { get; set; }
        public DbSet<Finportal.Models.Category> Category { get; set; }
        public DbSet<Finportal.Models.CategoryItem> CategoryItem { get; set; }
        public DbSet<Finportal.Models.Household> Household { get; set; }
        public DbSet<Finportal.Models.Invitation> Invitation { get; set; }
        public DbSet<Finportal.Models.Notification> Notification { get; set; }
        public DbSet<Finportal.Models.Transaction> Transaction { get; set; }
    }
}
