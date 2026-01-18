dotnet --version

# Build the solution (change the .sln name if needed)
dotnet build
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Run tests (will succeed even if there are no tests, but may take time)
dotnet test
exit $LASTEXITCODE