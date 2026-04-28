# Plan: GitHub Actions CI/CD Migration

## Steps

- [x] 1. Add `.github/workflows/build.yml` based on the Tharga.Crawler reference, with:
  - `MAJOR_MINOR: '4.0'`
  - .NET 8/9/10 setup
  - Test step with `--filter "Category!=Database"`
  - Pack steps for `Tharga.Console` and `Tharga.Console.Speech`
  - Warning threshold 15
- [x] 2. Verify local `dotnet build -c Release` and `dotnet test -c Release` still pass. (1/1 test passed; pack works for both packages, 2 pre-existing `NU5048 PackageIconUrl` warnings, well under threshold.)
- [~] 3. Commit on `develop` (per user instruction) with message describing the migration.
- [ ] 4. Hand off remaining manual steps to the user:
  - Configure `NUGET_API_KEY` secret in repo settings
  - Create `release` and `prerelease` GitHub environments
  - After validating the workflow on a PR to `master`, disable the Azure DevOps pipeline
  - Switch to feature-branch flow and delete `develop` after the next merge to `master`
- [ ] 5. After the first successful master run, update Requests.md (mark Tharga.Console GHA migration as Done).

## Notes
- Working directly on `develop` per user request — this is a deviation from the standard "feature branch from develop" rule, but the migration itself flips the branching strategy.
- The existing Azure DevOps pipeline (`azure-pipelines.yml`) is left in the repo for now; remove or disable after the new workflow is verified end-to-end.
