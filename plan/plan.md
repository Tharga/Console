# Plan: feature/icon-and-docs

Branch: `feature/icon-and-docs` (off `master`). Reference commit: Tharga.Test `d6b9efb` (bundled icon + docs + integrated CI).

## Steps

- [~] **1. Bump PackageIconUrl in three csproj files**
  - [ ] `Tharga.Console/Tharga.Console.csproj`
  - [ ] `Tharga.Console.Standard/Tharga.Console.Standard.csproj`
  - [ ] `Tharga.Console.Speech/Tharga.Console.Speech.csproj`
  - [ ] `dotnet build -c Release` to confirm nothing else breaks (csproj-only change, expected clean).

- [ ] **2. Add docs/ tree**
  - [ ] `docs/CNAME` → `console.tharga.net`.
  - [ ] `docs/docfx.json` (metadata covers all three Console csproj files; `_appName/_appTitle` = `Tharga.Console`; `_appLogoPath` / `_appFaviconPath` = the new asset URL; templates: `default`, `modern`, `templates/thg`).
  - [ ] `docs/index.md` (landing page — package table for all 3 nupkgs, quick start, link to articles).
  - [ ] `docs/toc.yml` (Home / Articles / API).
  - [ ] `docs/articles/index.md` + `docs/articles/toc.yml` (4 articles).
  - [ ] `docs/articles/getting-started.md` — install + ClientConsole + RootCommand + CommandEngine minimal example.
  - [ ] `docs/articles/commands.md` — ContainerCommandBase / ActionCommandBase / AsyncActionCommandBase, naming, `IsVisible` (mention the 4.1 rename).
  - [ ] `docs/articles/consoles.md` — ClientConsole + other console types, output methods.
  - [ ] `docs/articles/dependency-injection.md` — Microsoft.Extensions.DI and Castle Windsor patterns from the README.
  - [ ] `docs/templates/thg/public/main.css` — navbar logo sizing (copy-equivalent of Tharga.Test).

- [ ] **3. Update .gitignore**
  - [ ] Append `/docs/_site/`, `/docs/api/`, `/docs/obj/` block (mirroring Tharga.Test lines 421-424).

- [ ] **4. Wire docs jobs into build.yml**
  - [ ] Add `pages: write` and `id-token: write` to top-level `permissions`.
  - [ ] Add `docs` job — `needs: release`, master-push only, installs DocFX, runs `docfx docs/docfx.json`, uploads Pages artifact via `actions/upload-pages-artifact@v3`.
  - [ ] Add `docs-deploy` job — `needs: docs`, concurrency `group: pages`, `environment: github-pages`, deploys via `actions/deploy-pages@v4`.

- [ ] **5. Update README.md**
  - [ ] Add a docs-site link near the top, in the same style as Mcp/Runtime/Test README diffs.

- [ ] **6. Verify locally**
  - [ ] `dotnet build -c Release` — clean.
  - [ ] `dotnet test -c Release` — 44 pass / 0 fail / 2 skip.
  - [ ] If DocFX is installed, smoke-build `docfx docs/docfx.json` to catch obvious config errors before CI runs.

- [ ] **7. Commit, push, open PR**
  - [ ] One bundled commit: `feat: package icon, docs site, and integrated docs CI` (matches Tharga.Test).
  - [ ] Push branch.
  - [ ] Open PR `feature/icon-and-docs` → `master` (same flow that worked for 4.1).

- [ ] **8. Close-out (after user confirms ready to merge)**
  - [ ] Archive `plan/feature.md` to `$DOC_ROOT/Tharga/plans/Toolkit/Console/done/icon-and-docs.md`.
  - [ ] `git rm -r plan` on the feature branch.
  - [ ] Final commit: `feat: icon-and-docs complete`.
  - [ ] Push; merge via PR.
  - [ ] Update Requests.md: mark both `### Tharga.Console` entries (under `## Move PackageIconUrl …` and `## Documentation sites under tharga.net`) as Done with the merge commit + PR link.

## Notes & decisions

- **Bundled vs. split.** User said one PR. Siblings did the same. Risk: PR is larger than a typical "bug fix", but the file types are disjoint (csproj / docs / yml / md) so review is straightforward.
- **PackageProjectUrl unchanged.** Sibling precedent (Fortnox status note) — kept on GitHub.
- **Docs CI gating.** `docs` depends on `release`. If release fails (e.g. test or pack failure), docs do not publish for that revision. This is the "integrated pattern from day one" referenced in Tharga.Runtime / Tharga.Test status entries.
- **No 4.1.x version bump in this PR.** The release job auto-increments the patch from `MAJOR_MINOR=4.1`, so this PR ships as `4.1.x+1` automatically after merge.
- **`<PackageIconUrl>` deprecation.** Microsoft prefers `<PackageIcon>` (embedded file) over `<PackageIconUrl>` (external URL) since SDK 5.0+; latter is shown only in older NuGet UIs. Siblings still use `<PackageIconUrl>` — match that for consistency rather than introducing a new pattern in this PR.
