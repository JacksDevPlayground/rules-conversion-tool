# Define the base source directory and the destination directory
param (
    [string]$baseDirectory = "SSW.Rules.Content\rules",
    [string]$publicDirectory = "SSW.Rules.Content\public"
)

# Define the file types to move
$fileTypes = @('.bmp', '.docx', '.drawio', '.gif', '.ics', '.jpeg', '.jpg', '.m4a', '.mp3', '.mp4', '.msg', '.oft', '.pdf', '.png', '.pptx', '.sql', '.svg', '.txt', '.webp', '.xlsx', '.zip')

# Get all rule.md files recursively from the base directory
$ruleMdFiles = Get-ChildItem -Path $baseDirectory -Recurse -Filter "rule.md"

foreach ($ruleMdFile in $ruleMdFiles) {
    $ruleName = Split-Path $ruleMdFile.DirectoryName -Leaf
    $content = Get-Content $ruleMdFile.FullName -Raw

    # Normalize file paths in the content
    $content = $content -replace '\\_', '_'

    $matches = Select-String -InputObject $content -Pattern '!\[.*?\]\(((?!http).+?)\)' -AllMatches

    if ($matches.Matches.Count -gt 0) {
        $filesToUpdate = @()
        foreach ($match in $matches.Matches) {
            $relativePath = $match.Groups[1].Value -replace '\\_', '_'
            $fullPath = Resolve-Path (Join-Path $ruleMdFile.DirectoryName $relativePath) -ErrorAction SilentlyContinue

            if (-not $fullPath -or -not (Test-Path $fullPath)) {
                # Check in the ../../assets folder
                $assetsPath = Resolve-Path (Join-Path $ruleMdFile.DirectoryName "../../assets/$relativePath") -ErrorAction SilentlyContinue
                if ($assetsPath -and (Test-Path $assetsPath)) {
                    $fullPath = $assetsPath
                }
            }

            if ($fullPath -and ($fileTypes -contains (Get-Item $fullPath).Extension)) {
                $filesToUpdate += @{
                    FullPath = $fullPath
                    FileName = [System.IO.Path]::GetFileName($fullPath)
                    OldRelativePath = $relativePath
                }
            }
        }

        if ($filesToUpdate.Count -gt 0) {
            $newRuleDir = Join-Path (Join-Path $publicDirectory "rules") $ruleName
            if (-not (Test-Path $newRuleDir)) {
                New-Item -Path $newRuleDir -ItemType Directory
                Write-Host "Created directory: $newRuleDir"
            }

            foreach ($fileInfo in $filesToUpdate) {
                $newFilePath = Join-Path $newRuleDir $fileInfo.FileName
                Move-Item -Path $fileInfo.FullPath -Destination $newFilePath -Force
                Write-Host "Moved file from $($fileInfo.FullPath) to $newFilePath"

                $oldRelativePath = [regex]::Escape($fileInfo.OldRelativePath)
                $newRelativePath = "/rules/$ruleName/$($fileInfo.FileName)"
                $content = $content -replace $oldRelativePath, $newRelativePath
            }

            Set-Content $ruleMdFile.FullName -Value $content
            Write-Host "Updated rule.md file at $($ruleMdFile.FullName)"
        }
    }
}

