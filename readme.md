# Background

An edit on the 'react' dotnet template, added jwt authentication instead of identity server and some basic database communication for persisting users.

# Setup

Development
```shell
dotnet restore
dotnet build
dotnet run
```

Production
```shell
# multi platform, runtime required
dotnet publish
# Linux, stand alone
dotnet publish -c linuxrelease --self-contained --runtime linux-x64
# Windows, stand alone
dotnet publish -c windowsrelease --self-contained true --runtime win10-x64
```

Docker
```yml
#todo
```

# Sources

- authentication:  
https://jasonwatmore.com/net-7-csharp-jwt-authentication-tutorial-without-aspnet-core-identity  
https://jasonwatmore.com/post/2019/04/06/react-jwt-authentication-tutorial-example  
https://www.bezkoder.com/react-login-example-jwt-hooks/  
- docker deployment:  
https://medium.com/@mustafamagdy1/netcore-react-docker-1d19f051942c  