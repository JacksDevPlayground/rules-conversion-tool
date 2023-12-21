## This Repo contains all the scripts needed to convert SSW Rules Content to mdoc format

### Run

Make suure you run the scripts in the following order: (Find more info in #Notes)

1. `image-asset-sanity-check.ps1`
2. `move-images.ps1`
3. `clean-images.ps1`


4. Now run the converter tool. You'll need .net8 installed.
5. dotnet build 
6. `.\rules-conversion-tool.exe convert <FILE_OR_DIRECTORY> <OUTPUT_DIR>`
7. Select **CompleteFile** for both `Blurb` and `Body`
8. Select **All** options to convert all components or select the ones you want to convert individually

### Notes

#### `image-asset-sanity-check.ps1` Info

The `NotFoundLog` file finds all images that aren't found in the folder but are in other places 
The `RedirectsLog` finds all the redirects that still have folders in the repo

#### `move-images.ps1` Info

This script does all the heavy lifting. It moves all the images from the old folder structure to the new one + fixes the links in the markdown files. This also removes all the `assets` from that folder as it is no longer needed.

#### `clean-images.ps1` Info

This script removes all the empty folders from the old folder structure + unused files. 

