# Plan: 4.1 release

Branch: `feature/fix-delimited-params` (off `master`, GitHub Actions strategy).

## Steps

- [x] **1. Fix #9 — delimited-string param bug**
  - [x] Wrote 5 failing/regression tests in [ParamExtensions_tests.cs](Tharga.Console.Tests/ParamExtensions_tests.cs): quoted-with-spaces, mixed quoted/unquoted, unquoted, empty, single-quoted.
  - [x] Confirmed 3 tests failed (GUIDs returned in place of token content).
  - [x] Rewrote `ToInput` to always split on space then map captured GUIDs back to their values. The unquoted-only fast-path is preserved.
  - [x] All 5 new tests pass; full suite 42 pass / 0 fail / 2 pre-existing skips.

- [x] **2. Fix #10 — buffer insert position negative**
  - [x] Decision: deterministic integration repro requires multi-thread coordination against private `_startLocation`. Took the plan's authorized fallback — extracted a tiny pure helper `InputInstance.RecoverNegativeBufferPosition` that encapsulates the recovery decision, and unit-tested it in isolation. Production verification via user testing of the pushed branch.
  - [x] Replaced the throw at [InputInstance.cs:130](Tharga.Console.Standard/Helpers/InputInstance.cs#L130) with a recovery: re-anchor `_startLocation` to `currentScreenLocation` under `_locationLock` and reset `currentBufferPosition` to 0. Added `Debug.WriteLine` for diagnosis.
  - [x] Added `InputInstance_recovery_tests` with 2 tests (no-op and recover cases).
  - [x] Full suite green: 44/0/2.

- [~] **3. Rename `IsHidden` → `IsVisible`**
  - [ ] Update [ICommand.cs:15](Tharga.Console.Standard/Commands/ICommand.cs#L15) — `bool IsVisible { get; }`; remove the in-code TODO.
  - [ ] Update [CommandBase.cs:18,29](Tharga.Console.Standard/Commands/Base/CommandBase.cs#L18) — property + constructor param `bool visible = true`. Internally store `IsVisible` directly (no inversion).
  - [ ] Update all consumers in `ContainerCommandBase.cs` (8 sites — lines 204-236) — flip the `!x.IsHidden` checks to `x.IsVisible` and the `command.IsHidden ? "*" : ""` to `!command.IsVisible ? "*" : ""`.
  - [ ] Search for any subclass passing `: base(..., hidden: true)` or positional-`true` for hidden and flip them.
  - [ ] Run `dotnet build -c Release` and `dotnet test -c Release`.
  - [ ] If [README.md](README.md) documents `IsHidden`, update it.
  - [ ] Commit with `refactor!: rename ICommand.IsHidden to IsVisible` (breaking change).

- [ ] **4. Version bump**
  - [ ] Change `MAJOR_MINOR: '3.7'` → `'4.1'` in [.github/workflows/build.yml](.github/workflows/build.yml#L9).
  - [ ] If any csproj `<Version>` tags carry a real version (not the placeholder `1.0.0`), align them too. Otherwise leave them.
  - [ ] Commit with `chore: bump version to 4.1`.

- [ ] **5. Push for user testing**
  - [ ] Run the full test suite one more time.
  - [ ] Push `feature/fix-delimited-params` to origin.
  - [ ] Ask the user to test from the pushed branch. **Do not open the PR yet** (per feature workflow — PR opens after close-out commit).

- [ ] **6. Close-out (only after user confirms)**
  - [ ] Archive `plan/feature.md` to `$DOC_ROOT/Tharga/plans/Toolkit/Console/done/4.1-fix-delimited-params.md`.
  - [ ] `git rm -r plan` on the feature branch.
  - [ ] Final commit: `fix: 4.1-fix-delimited-params complete`.
  - [ ] Push, then open PR from `feature/fix-delimited-params` → `master`.

## Notes & decisions

- Issue #10's reproducer is uncertain. If a deterministic repro proves too costly, fall back to a targeted unit test of the recovery branch only and rely on user testing of the pushed branch for in-the-wild verification.
- The `IsHidden`→`IsVisible` rename includes flipping the constructor parameter name and default. Downstream consumers (Tharga.Crawler, Quilt4Net, etc.) will see compile errors when they bump — that's the point of the major version bump. Capture a Follow-up entry in `$DOC_ROOT/Tharga/Requests.md` when the feature closes so consumers know to update.
