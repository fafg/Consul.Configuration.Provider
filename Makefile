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
	nuget pack src/Consul.Configuration.Provider/Consul.Configuration.Provider.csproj -Version $(versionNumber) -Properties Configuration=Release

nuget-push:
ifeq ($(pushNuget),true)
	find . -iname Consul.Configuration.Provider.*.nupkg -type f | xargs -I package \
    dotnet nuget push package -k $(nugetApiKey) -s https://api.nuget.org/v3/index.json
endif

tag:
	@echo "TAG VERSION:" $(tagVersion)