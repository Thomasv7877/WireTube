using WebApi.Entities;
using System.Collections.Generic;

namespace dotnet_react_xml_generator.Data.Repositories
{
    public interface IUserRepository
    {
        User? GetBy(string userName);
        void Add(User user);
        IEnumerable<User> GetAll();
        User? GetById(int id);
        void SaveChanges();
    }
}