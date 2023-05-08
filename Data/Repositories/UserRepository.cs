using System.Linq;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApi.Authorization;


namespace dotnet_react_xml_generator.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbSet<User> _users;
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext){
            _dbContext = dbContext;
            _users = _dbContext.Users;
        }
        public User? GetBy(string userName){
            return _users.SingleOrDefault(u => u.Username == userName);
        }
        public void Add(User user){
            user.Password = PasswordHashing.HashPassword(user.Password);
            _users.Add(user);
        }
        public IEnumerable<User> GetAll()
        {
            return _users.ToList();
        }
        public User? GetById(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }
        public void SaveChanges(){
            _dbContext.SaveChanges();
        }
    }
}