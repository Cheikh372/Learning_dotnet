using EndpointsIidentity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace EndpointsIidentity.Infrastructures
{
    public class EndpointsIidentityDbContext : IdentityDbContext<User>
    {
        public EndpointsIidentityDbContext(DbContextOptions<EndpointsIidentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
