# Aider rules for this repo
- After any changes, run: powershell -NoProfile -ExecutionPolicy Bypass -File .\\check.ps1
- Fix all build/test errors before continuing.
- Do not commit unless check.ps1 succeeds.
- Keep changes small and focused; prefer minimal diffs.
- Ask questions for more details, do not assume or invent things that does not exist.

- Tharga.Console is the main project and the nuget package that I want to build.
- Sample.Cli is a sample project that uses Tharga.Console.