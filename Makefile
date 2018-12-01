# Consul.Configuration.Provider

clean:
	dotnet clean Consul.Configuration.Provider.sln

restore:
	dotnet restore Consul.Configuration.Provider.sln

build: clean restore
	dotnet build -c Release --no-restore Consul.Configuration.Provider.sln

test: build
	dotnet test --no-restore tests/Consul.Configuration.Provider.Tests/Consul.Configuration.Provider.Tests.csproj

version:

nuget-pack:
	nuget pack src/Consul.Configuration.Provider/Consul.Configuration.Provider.csproj

nuget-push:
    dotnet nuget push Consul.Configuration.Provider.0.1.0.nupkg -k $(nugetApiKey) -s https://api.nuget.org/v3/index.json