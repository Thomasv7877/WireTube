from mcr.microsoft.com/dotnet/sdk as dotnet-build
workdir /src
copy . /src
run dotnet restore 'dotnet_react_xml_generator.csproj'
run dotnet build 'dotnet_react_xml_generator.csproj' -c release -o /app/build

from dotnet-build as dotnet-publish
run dotnet publish 'dotnet_react_xml_generator.csproj' -c release -o /app/publish

from node as node-builder
workdir /node
copy ./ClientApp /node
run npm install
run npm run build

from mcr.microsoft.com/dotnet/aspnet as final
workdir /app
run mkdir /app/wwwroot
copy --from=dotnet-publish /app/publish .
copy --from=node-builder /node/build ./wwwroot
entrypoint ["dotnet", "dotnet_react_xml_generator.dll"]