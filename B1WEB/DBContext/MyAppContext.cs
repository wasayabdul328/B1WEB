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

    }
}
