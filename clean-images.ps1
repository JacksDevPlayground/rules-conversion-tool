# Define the paths for the assets and rules directories
param (
    [string]$assetsDirectory = "SSW.Rules.Content\assets",
    [string]$rulesDirectory = "SSW.Rules.Content\rules"
)
# Define the file types to check
$fileTypes = @('.bmp', '.docx', '.drawio', '.gif', '.ics', '.jpeg', '.jpg', '.m4a', '.mp3', '.mp4', '.msg', '.oft', '.pdf', '.png', '.pptx', '.sql', '.svg', '.txt', '.webp', '.xlsx', '.zip')

# Define the log file paths
$logFile = "C:\Users\jpr17\src\SSW\SSW.Rules.Content\DeletionLog.txt"
$debugLogFile = "C:\Users\jpr17\src\SSW\SSW.Rules.Content\DebugLog.txt"

# Function to filter files by defined types and log information for debugging
function Filter-FilesByType {
    param (
        [string]$Directory
    )

    Get-ChildItem -Path $Directory -Recurse | ForEach-Object {
        if (!$_.PSIsContainer -and $_.Extension -in $fileTypes) {
            # File matches the criteria, add to output
            $_
        } else {
            # Log the skipped item for debugging
            Add-Content -Path $debugLogFile -Value "Skipped item: $($_.FullName)"
        }
    }
}

# Find all files of specified types in the assets and rules directories
$filesInAssets = Filter-FilesByType -Directory $assetsDirectory
$filesInRules = Filter-FilesByType -Directory $rulesDirectory

$allFiles = $filesInAssets + $filesInRules
Write-Host "Found $($allFiles.Count) files of specified types."

# Function to check if an item is a file
function Is-File($item) {
    return $item -and ($item.Attributes -notmatch 'Directory')
}

# Filter out directories from $allFiles
$filesToProcess = $allFiles | Where-Object { Is-File $_ }

# Log and remove the files
foreach ($file in $filesToProcess) {
    # Log the file path for deletion
    Add-Content -Path $logFile -Value "File to be deleted: $($file.FullName)"

    Remove-Item -Path $file.FullName -Force
}

Write-Host "All leftover files of specified types have been logged and removed."
