using B1WEB.AppModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace B1WEB.DBContext
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options)
        {
        }
        public DbSet<PortalUsers> PortalUsers { get; set; }
        public DbSet<CompanyConfiguration> CompanyConfiguration { get; set; }
        public DbSet<UserCompany> UserCompany { get; set; }
        public DbSet<UserPermission> UserPermission { get; set; }

    }
}
