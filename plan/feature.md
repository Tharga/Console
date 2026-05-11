# Feature: 4.1 release — bug fixes + IsVisible rename

## Goal

Ship Tharga.Console 4.1 with two bug fixes filed by external users and one breaking API rename that justifies the major bump (3.7 → 4.1; 4.0 was already burned).

## Scope

In:
1. **Fix [#9](https://github.com/Tharga/Console/issues/9) — delimited string parameters.** `ParamExtensions.ToInput` currently returns GUID placeholders and drops unquoted verbs when any quoted argument is present. Rewrite to map placeholders back to their captured values after splitting on space.
2. **Fix [#10](https://github.com/Tharga/Console/issues/10) — "Buffer insert position cannot be less than zero."** `InputInstance` throws `InvalidOperationException` and gets stuck throwing on every keypress once the cursor ends up above `_startLocation` (window resize, scroll, or stdout from another thread). Replace the throw with a recovery that re-anchors `_startLocation` to the current cursor and resets the buffer position so input continues.
3. **Rename `ICommand.IsHidden` → `ICommand.IsVisible`.** Breaking API change. Flip default and constructor parameter accordingly. Update all call sites and subclasses in this repo.
4. **Bump `MAJOR_MINOR` in `.github/workflows/build.yml` from `3.7` to `4.1`.**

Out of scope:
- Other in-code TODOs (BufferWidth abstraction, Write/WriteLine merge, voice console items, etc.) — tracked separately in the backlog.
- The remaining open GitHub issues (#11 alignment, #16 Unix support).

## Acceptance criteria

- Issue #9 reproducer works: `transmit "Hello World"` yields the two tokens `transmit` and `Hello World` (no GUIDs). Existing non-quoted paths unchanged.
- Issue #10 no longer throws — instead, when `currentBufferPosition < 0` is detected, `_startLocation` is reset to current cursor location and input continues. A unit test covers the negative-position branch.
- `IsHidden` is gone from the public surface. `IsVisible` is the property; constructor parameter and defaults are flipped (default = visible).
- Every existing in-repo consumer compiles and passes against the renamed API.
- `dotnet build -c Release` and `dotnet test -c Release` are clean.
- README updated to reflect the rename if it documents the property.
- `MAJOR_MINOR` is `4.1` in build.yml.

## Done condition

User has tested the pushed feature branch and confirms the release is good. Close-out commit removes `plan/`, then PR is opened against `master` (GitHub Actions branching rule — no local merge).
