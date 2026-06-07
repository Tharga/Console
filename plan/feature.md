# Feature: package icon, docs site, and integrated docs CI

## Goal

Bundle two `2026-06-01` Internal requests into one PR, modeled directly on Tharga.Test commit `d6b9efb`:

1. **Move PackageIconUrl to thargelion.net/assets** — three csproj files (Tharga.Console, Tharga.Console.Standard, Tharga.Console.Speech) currently point at the old `http://thargelion.se/wp-content/uploads/2019/11/Thargelion-White-Icon-150.png`. Update them all to `https://thargelion.net/assets/component-console.png`.
2. **Documentation site under console.tharga.net** — add a DocFX `docs/` tree, wire `docs` + `docs-deploy` jobs into the existing `.github/workflows/build.yml` (gated on `needs: release` so they only run for published versions), and add a `CNAME` for `console.tharga.net`. Sibling repos (Mcp, Runtime, Test) bundled these two requests onto a single `feature/icon-and-docs` branch — we follow that pattern.

Pre-reqs verified at start of work:
- `https://thargelion.net/assets/component-console.png` returns `HTTP/1.1 200 OK` (Cloudflare HIT).
- `console.tharga.net` resolves via DNS.

## Scope

In:
- Bump `<PackageIconUrl>` in `Tharga.Console.csproj`, `Tharga.Console.Standard.csproj`, `Tharga.Console.Speech.csproj` → `https://thargelion.net/assets/component-console.png`.
- Add `docs/` DocFX source tree mirroring Tharga.Test:
  - `docs/docfx.json` (metadata: all three Console csproj files for API ref; globalMetadata: app name + logo + favicon pointing at `component-console.png`; templates: `default`, `modern`, `templates/thg`).
  - `docs/CNAME` → `console.tharga.net`.
  - `docs/index.md` (landing page with package table, quick start, links).
  - `docs/toc.yml` (Home / Articles / API).
  - `docs/articles/index.md` (article index).
  - `docs/articles/toc.yml`.
  - Four articles: `getting-started.md`, `commands.md`, `consoles.md`, `dependency-injection.md`.
  - `docs/templates/thg/public/main.css` (constrains navbar logo to 32px height).
- Update `.gitignore` to ignore DocFX-generated output (`/docs/_site/`, `/docs/api/`, `/docs/obj/`).
- Wire docs jobs into `.github/workflows/build.yml`:
  - Add `pages: write` and `id-token: write` to top-level `permissions`.
  - Add `docs` job (`needs: release`, master push only) that installs DocFX, builds the site, uploads the Pages artifact.
  - Add `docs-deploy` job (`needs: docs`, master push only) that publishes to GitHub Pages with the `github-pages` environment.
- Update `README.md` with a single-line link to the new docs site (sibling pattern: add link near the top, leave inline examples in README untouched).

Out of scope:
- Changing `<PackageProjectUrl>` — siblings (Fortnox, Mcp, Runtime, Test) kept it pointing at GitHub for source-first parity with Tharga.MongoDB. Same here.
- Restructuring README to push consumers toward the docs site — that can come later if it adds value.
- New article topics beyond the four planned. Keep parity with siblings (3-4 articles + index).

## Acceptance criteria

- All three csproj files use the new `component-console.png` URL.
- `docs/docfx.json` builds cleanly with `docfx docs/docfx.json` (local smoke if DocFX is installed; otherwise rely on CI).
- `docs/CNAME` contains exactly `console.tharga.net`.
- `.gitignore` ignores `/docs/_site/`, `/docs/api/`, `/docs/obj/`.
- `.github/workflows/build.yml` has `pages: write` + `id-token: write` and the two new jobs gated `if: github.ref == 'refs/heads/master' && github.event_name == 'push'` with `needs: release` and `needs: docs` respectively.
- README has a docs link near the top.
- `dotnet build -c Release` clean.
- `dotnet test -c Release` clean (44 pass / 0 fail / 2 pre-existing skips — no regressions).
- After PR merge: CI publishes a 4.1.x patch release, then `docs` + `docs-deploy` jobs run and `console.tharga.net` serves the new site.

## Done condition

User confirms the release is good (PR opened, CI green). Close-out commit removes `plan/`, then PR is opened against `master` per the same flow as the 4.1 feature.
