SOURCE_PATH=Source
CSPROJ_PATH=./$(SOURCE_PATH)/MoreBlockDamageOptions.csproj

ROSLYNATOR_VERSION=5.0
ROSLYNATOR_PATH=.nupkg/roslynator/analyzers/dotnet/roslyn$(ROSLYNATOR_VERSION)/cs/

.PHONY: fmt
fmt:
	dotnet csharpier format .

.PHONY: lint
lint: fmt
	dotnet roslynator analyze $(CSPROJ_PATH) -a $(ROSLYNATOR_PATH)

.PHONY: build
build: fmt
	dotnet build $(CSPROJ_PATH) -c Release
