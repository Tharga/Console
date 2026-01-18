$ErrorActionPreference = "Stop"

dotnet --version

dotnet build
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet test
exit $LASTEXITCODE
