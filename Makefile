# Consul.Configuration.Provider

clean:
	dotnet clean Consul.Configuration.Provider.sln

restore:
	dotnet restore Consul.Configuration.Provider.sln

build: clean restore
	dotnet build -c Release --no-restore Consul.Configuration.Provider.sln

test: build
	dotnet test --no-restore tests/Consul.Configuration.Provider.Tests/Consul.Configuration.Provider.Tests.csproj

coverity-scan:
ifeq ($(coverityScan),true)
	ci/coverity.sh
endif

nuget-pack:
	nuget pack src/Consul.Configuration.Provider/Consul.Configuration.Provider.csproj -Version $(versionNumber)

nuget-push:
    dotnet nuget push Consul.Configuration.Provider.*.nupkg -k $(nugetApiKey) -s https://api.nuget.org/v3/index.json

tag:
	@echo "TAG VERSION:" $(tagNumber)