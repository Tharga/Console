$ErrorActionPreference = "Stop"
Set-Location -Path $PSScriptRoot

$InstructionsFile = "AIDER.md"
$IgnoreFile = ".aiderignore"

if (-not (Get-Command aider -ErrorAction SilentlyContinue)) { throw "aider not found on PATH" }
if (-not (Get-Command git -ErrorAction SilentlyContinue))   { throw "git not found on PATH" }

if (-not (Test-Path -LiteralPath $InstructionsFile)) { throw "Missing $InstructionsFile in repo root." }
if (-not (Test-Path -LiteralPath $IgnoreFile)) {
  @"
bin
obj
"@ | Set-Content -Encoding ASCII -LiteralPath $IgnoreFile
}

$repoRoot = (Resolve-Path $PSScriptRoot).Path
$ignoreAbs = (Resolve-Path (Join-Path $repoRoot $IgnoreFile)).Path

# All files under repo (exclude .git internals)
$all = Get-ChildItem -Path $repoRoot -Recurse -File -Force |
  Where-Object { $_.FullName -notmatch "\\.git\\|/\.git/" }

# Repo-relative paths with forward slashes (what git expects)
$relPaths = $all | ForEach-Object {
  $_.FullName.Substring($repoRoot.Length).TrimStart('\','/') -replace '\\','/'
}

# Ask git which paths match .aiderignore (FULL gitignore semantics)
$ignored = @()
if ($relPaths.Count -gt 0) {
  $stdin = ($relPaths -join "`n") + "`n"
  $ignored = $stdin | git -c core.excludesFile="$ignoreAbs" check-ignore --no-index --stdin 2>$null
}

$ignoredSet = New-Object 'System.Collections.Generic.HashSet[string]'
foreach ($p in $ignored) { $null = $ignoredSet.Add(($p.Trim() -replace '\\','/')) }

# Keep everything NOT ignored
$includedRel = $relPaths | Where-Object { -not $ignoredSet.Contains($_) }

# Always include instructions file first
$instructionFull = (Resolve-Path -LiteralPath $InstructionsFile).Path
$finalArgs = New-Object System.Collections.Generic.List[string]
$finalArgs.Add($InstructionsFile) | Out-Null

foreach ($rp in $includedRel) {
  if ($rp -eq $InstructionsFile) { continue }
  $finalArgs.Add(($rp -replace '/','\')) | Out-Null
}

Write-Host "Total files found : $($relPaths.Count)"
Write-Host "Files ignored     : $($ignoredSet.Count)"
Write-Host "Files added       : $($finalArgs.Count)"

# Start aider using .aider.conf.yml, and DO NOT apply repo .gitignore filtering
& aider --no-gitignore @finalArgs
