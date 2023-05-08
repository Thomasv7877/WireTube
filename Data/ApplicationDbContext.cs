//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using dotnet_react_xml_generator.Data.Mappers;
using WebApi.Entities;
using dotnet_react_xml_generator.Data.Repositories;

namespace dotnet_react_xml_generator.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users {get; set;}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){

        }
        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}