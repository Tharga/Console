<#
Starts Aider from repo root, using:
- .aider.conf.yml automatically (because we run aider from repo root)
- AIDER.md as the instruction file
- Adds "everything" via **/*, while honoring .aiderignore with full .gitignore semantics
#>

$ErrorActionPreference = "Stop"

Set-Location -Path $PSScriptRoot

$InstructionsFile = "AIDER.md"
$AiderIgnoreFile  = ".aiderignore"

# Sanity checks
if (-not (Get-Command aider -ErrorAction SilentlyContinue)) {
  throw "Could not find 'aider' on PATH."
}

if (-not (Test-Path -LiteralPath $InstructionsFile)) {
  throw "Missing $InstructionsFile in repo root. Create it (or update `$InstructionsFile in this script)."
}

# Create a starter .aiderignore if missing (optional)
if (-not (Test-Path -LiteralPath $AiderIgnoreFile)) {
  @"
bin
obj
"@ | Set-Content -Encoding ASCII -LiteralPath $AiderIgnoreFile
  Write-Host "Created $AiderIgnoreFile with default ignores: bin, obj"
}

# Optional: ensure we're in a git repo (helps avoid surprises)
try {
  git rev-parse --is-inside-work-tree | Out-Null
} catch {
  Write-Host "Warning: This directory doesn't look like a git repo. Aider can still run, but git features may be disabled."
}

# Start aider:
# - Include AIDER.md first
# - Add dotfiles in repo root explicitly (globs often skip dotfiles)
# - Add everything else via **/* and let Aider apply .aiderignore with full gitignore semantics
# - --aiderignore explicitly points to the file (Aider also auto-detects it, but this is explicit)
& aider `
  --aiderignore $AiderIgnoreFile `
  $InstructionsFile `
  .gitignore .editorconfig .aider.conf.yml .aiderignore `
  "**/*"
