# Consul.Configuration.Provider

clean:
	dotnet clean Consul.Configuration.Provider.sln

restore:
	dotnet restore Consul.Configuration.Provider.sln

build: clean restore
	dotnet build --no-restore Consul.Configuration.Provider.sln

test: build
	dotnet test tests/Consul.Configuration.Provider.Tests/Consul.Configuration.Provider.Tests.csproj

pack: build
	nuget pack src/Consul.Configuration.Provider/Consul.Configuration.Provider.csproj