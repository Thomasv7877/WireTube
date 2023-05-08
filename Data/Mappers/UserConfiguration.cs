using WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet_react_xml_generator.Data.Mappers
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder){
            builder.ToTable("User");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.LastName).IsRequired();
            builder.Property(c => c.FirstName).IsRequired();
            builder.Property(c => c.Username).IsRequired();
            builder.Property(c => c.Password).IsRequired();
        }
    }
}