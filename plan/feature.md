# Feature: GitHub Actions CI/CD Migration

## Goal
Migrate Tharga.Console build/test/publish from Azure DevOps to GitHub Actions, matching the canonical Tharga.Crawler workflow.

## Scope
- Add `.github/workflows/build.yml` with build, security (CodeQL), release, and prerelease jobs.
- Pack two NuGet packages: `Tharga.Console` and `Tharga.Console.Speech`.
- Use `MAJOR_MINOR=4.0` (matches the existing Azure DevOps pipeline; latest tag is `4.0.0-pre.6`).
- Target frameworks: net8.0, net9.0, net10.0.
- Carry over the `Category!=Database` test filter from the Azure DevOps pipeline.
- Warning threshold: 15 (default).

## Out of scope (manual user steps in the GitHub/Azure DevOps UIs)
- Configuring `NUGET_API_KEY` secret in the GitHub repo.
- Creating `release` and `prerelease` GitHub environments.
- Disabling the existing Azure DevOps pipeline.
- Switching the branching strategy from `develop` → `master` to `feature/<name>` → `master` and deleting `develop` after the migration commit lands on `master`.

## Acceptance criteria
- `.github/workflows/build.yml` exists and matches the Crawler structure.
- Local `dotnet build -c Release` and `dotnet test -c Release` still pass.
- The Requests.md "Tharga.Console" entry under "GitHub Actions CI/CD" is updated to Done with date and summary once the workflow runs successfully on master.

## Done condition
The first GitHub Actions run on `master` succeeds, publishes both NuGet packages, and the Azure DevOps pipeline is disabled.
