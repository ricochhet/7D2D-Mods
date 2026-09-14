APP_NAME=MoreBlockDamageOptions

SOURCE_PATH=Source
BUILD_PATH=build
BUILD_NAME=zzzzz_MoreBlockDamageOptions
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

.PHONY: release
release: build
	mkdir -p $(BUILD_PATH)/$(BUILD_NAME)
	cp "$(SOURCE_PATH)/bin/Release/$(APP_NAME).dll" "$(BUILD_PATH)/$(BUILD_NAME)/"
	cp "$(SOURCE_PATH)/GearsAPI.dll" "$(BUILD_PATH)/$(BUILD_NAME)/"
	cp ModInfo.xml "$(BUILD_PATH)/$(BUILD_NAME)/"
	cp ModSettings.xml "$(BUILD_PATH)/$(BUILD_NAME)/"
