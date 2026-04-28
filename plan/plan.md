# Plan: GitHub Actions CI/CD Migration

## Steps

- [x] 1. Add `.github/workflows/build.yml` based on the Tharga.Crawler reference, with:
  - `MAJOR_MINOR: '4.0'`
  - .NET 8/9/10 setup
  - Test step with `--filter "Category!=Database"`
  - Pack steps for `Tharga.Console` and `Tharga.Console.Speech`
  - Warning threshold 15
- [x] 2. Verify local `dotnet build -c Release` and `dotnet test -c Release` still pass. (1/1 test passed; pack works for both packages, 2 pre-existing `NU5048 PackageIconUrl` warnings, well under threshold.)
- [x] 3. Commit on `develop` (per user instruction) with message describing the migration. (Commit `dfa336a`.)
- [~] 4. Hand off remaining manual steps to the user:
  - Configure `NUGET_API_KEY` secret in repo settings
  - Create `release` and `prerelease` GitHub environments
  - After validating the workflow on a PR to `master`, disable the Azure DevOps pipeline
  - Switch to feature-branch flow and delete `develop` after the next merge to `master`
- [ ] 5. After the first successful master run, update Requests.md (mark Tharga.Console GHA migration as Done).

## Notes
- Working directly on `develop` per user request — this is a deviation from the standard "feature branch from develop" rule, but the migration itself flips the branching strategy.
- The existing Azure DevOps pipeline (`azure-pipelines.yml`) is left in the repo for now; remove or disable after the new workflow is verified end-to-end.

## Last session (2026-04-28)
- Added `.github/workflows/build.yml` (CI/security/release/prerelease jobs, Crawler-based) and committed on `develop` as `dfa336a`.
- Local `dotnet test -c Release --filter "Category!=Database"` passes (1/1). Both packages pack cleanly (2 pre-existing `NU5048 PackageIconUrl` deprecation warnings, well under the 15 threshold — could be cleaned up separately).
- Develop is now 2 commits ahead of origin (`b948874 update nuget packages` + `dfa336a ci: migrate ...`). Not pushed.
- **Next**: user needs to (1) push, (2) open PR develop → master, (3) configure `NUGET_API_KEY` + `release`/`prerelease` environments in GitHub, (4) merge to validate the workflow, (5) disable Azure DevOps pipeline, (6) switch to feature-branch flow and delete `develop`. Once successful on master, mark Requests.md "Tharga.Console" GHA migration as Done.
