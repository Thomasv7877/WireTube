using WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_react_xml_generator.Data.Repositories;

namespace dotnet_react_xml_generator.Data
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        public ApplicationDbSeeder (ApplicationDbContext dbContext, IUserRepository userRepository){
            _dbContext = dbContext;
            _userRepository = userRepository;
        }
        public async Task InitializeData(){
            _dbContext.Database.EnsureDeleted();
            if(_dbContext.Database.EnsureCreated()){
                User testUser = new User {
                    Id = 1,
                    FirstName = "test",
                    LastName = "test",
                    Username = "test",
                    Password = "test"
                };
                _userRepository.Add(testUser);
                Console.WriteLine("UserAdded?");
                _userRepository.SaveChanges();
            }
        }

    }
}