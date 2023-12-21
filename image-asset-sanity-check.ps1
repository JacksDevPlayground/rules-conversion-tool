# Define the base source directory and log file paths
param (
    [string]$assetsDirectory = "SSW.Rules.Content\assets",
    [string]$baseDirectory = "SSW.Rules.Content\rules",
    [string]$notFoundLog = "rules-conversion-tool\NotFoundLog.txt",
    [string]$redirectsLog = "rules-conversion-tool\RedirectsLog.txt"
)
# Define counters for summary
$notFoundCount = 0
$redirectsFoundCount = 0

# Function to get redirects from a rule.md file
function Get-Redirects {
    param ([string]$Content)
    
    $redirects = @()
    $inRedirectsSection = $false

    foreach ($line in $Content -split "\r?\n") {
        if ($line -match '^redirects:\s*$') {
            $inRedirectsSection = $true
        } elseif ($inRedirectsSection -and $line -match '^\s*-\s*(.+)$') {
            $redirects += $matches[1].Trim()
        } elseif ($inRedirectsSection -and $line -match '^\S') {
            break # Exit redirects section
        }
    }
    return $redirects
}

# Process each rule.md file
Get-ChildItem -Path $baseDirectory -Recurse -Filter "rule.md" | ForEach-Object {
    $ruleMdFile = $_
    $content = Get-Content $ruleMdFile.FullName -Raw
    $currentRuleUri = (Split-Path $ruleMdFile.DirectoryName -Leaf)
    $redirects = Get-Redirects -Content $content

    # Check if the redirects exist and log information
    foreach ($redirect in $redirects) {
        $redirectPath = Join-Path $baseDirectory $redirect
        if (Test-Path $redirectPath) {
            $logEntry = "Link: $currentRuleUri ::: To: $redirect"
            Add-Content -Path $redirectsLog -Value $logEntry
            $redirectsFoundCount++
        }
    }

# Check for image references and their existence
$matches = Select-String -InputObject $content -Pattern '!\[.*?\]\(((?!http).+?)\)' -AllMatches
foreach ($match in $matches.Matches) {
    $imagePath = $match.Groups[1].Value -replace '\\_', '_'
    $fullImagePath = Join-Path $ruleMdFile.DirectoryName $imagePath

    # Initialize foundPath as null
    $foundPath = $null

    # Check in the rule's folder
    if ($fullImagePath -and (Test-Path $fullImagePath)) {
        $foundPath = $fullImagePath
    }

    # If not found, check in the assets folder
    if (-not $foundPath) {
        $assetsImagePath = Join-Path $assetsDirectory $imagePath
        if ($assetsImagePath -and (Test-Path $assetsImagePath)) {
            $foundPath = $assetsImagePath
        }
    }

    # If still not found, check in the redirects
    if (-not $foundPath) {
        foreach ($redirect in $redirects) {
            $redirectImagePath = Join-Path (Join-Path $baseDirectory $redirect) $imagePath
            if ($redirectImagePath -and (Test-Path $redirectImagePath)) {
                $foundPath = $redirectImagePath
                break
            }
        }
    }

    # Log the result
    if (-not $foundPath) {
        $logEntry = "Image Path: $imagePath ::: Not Found in rule: $currentRuleUri Found in: None"
        $notFoundCount++
    } else {
        $logEntry = "Image Path: $imagePath ::: Found in rule: $currentRuleUri Found in: $foundPath"
    }
    Add-Content -Path $notFoundLog -Value $logEntry
}
}

# Output summary
Write-Host "Total redirects found: $redirectsFoundCount"
Write-Host "Total images/assets not found: $notFoundCount"
