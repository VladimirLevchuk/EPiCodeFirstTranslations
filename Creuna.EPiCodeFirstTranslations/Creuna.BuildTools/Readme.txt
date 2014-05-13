In the build definition
1. Update Pre-build script path with path to 'UpdateVersion.ps1'
2. Update Post-build script path with path to 'BuildAndPushNewPackage.ps1'
3. Update Pre-build script arguments with '-prerelease "true"' for pre-release nuget packages
