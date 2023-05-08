namespace WebApi.Services;

using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Models;
using dotnet_react_xml_generator.Data.Repositories;
using WebApi.Authorization;

public interface IUserService
{
    AuthenticateResponse? Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User? GetById(int id);
}

public class UserService : IUserService
{
    
    private readonly IJwtUtils _jwtUtils;
    private readonly IUserRepository _users;

    public UserService(IJwtUtils jwtUtils, IUserRepository userRepository)
    {
        _jwtUtils = jwtUtils;
        _users = userRepository;
    }

    public AuthenticateResponse? Authenticate(AuthenticateRequest model)
    {
        //var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);
        var user = _users.GetBy(model.Username);

        // return null if user not found
        if (user == null) return null;
        bool passwordCorrect = PasswordHashing.VerifyPassword(model.Password, user.Password);
        if(!passwordCorrect) return null;

        // authentication successful so generate jwt token
        var token = _jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    public IEnumerable<User> GetAll()
    {
        return _users.GetAll();
    }

    public User? GetById(int id)
    {
        return _users.GetById(id);
    }
}