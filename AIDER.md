# Aider rules for this repo
- After any changes, run: powershell -NoProfile -ExecutionPolicy Bypass -File .\\check.ps1
- Fix all build/test errors before continuing.
- Do not commit unless check.ps1 succeeds.
- Keep changes small and focused; prefer minimal diffs.
- Ask questions for more details, do not assume or invent things that does not exist.

- Tharga.Console and Tharga.Console.Speech are nuget packages that is the main part of the project.
- Sample.Cli is used to manually test the features.
- Tharga.Console.Test is used for automated testing.
- Tharga.Console.Standard is legacy and should only be used for reference, it will be deprecated.