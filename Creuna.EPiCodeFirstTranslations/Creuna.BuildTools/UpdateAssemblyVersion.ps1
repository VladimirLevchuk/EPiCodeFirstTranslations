Param(
  [string]$pathToSearch = $env:TF_BUILD_SOURCESDIRECTORY,
  [string]$buildNumber = $env:TF_BUILD_BUILDNUMBER,
  [string]$prerelease,#if not empty indicates that it's going to be pre-release version
  [string]$searchFilter = "AssemblyInfo.*",
  [regex]$pattern = "\d+\.\d+\.\d+"  
)

[string]$assemblyInformationalVersionAttributName = "AssemblyInformationalVersion"
[string]$assemblyInformationalVersionPattern = 'AssemblyInformationalVersion\(\"'
[string]$prereleaseSuffixName = "-alpha"

Write-Host "------- Start UpdateVersion.ps1"
Write-Verbose "pathToSearch = $pathToSearch"
Write-Verbose "buildNumber = $buildNumber"
Write-Verbose "prerelease = $prerelease"
Write-Verbose "searchFilter = $searchFilter"
Write-Verbose "pattern = $pattern"

if ($buildNumber -match $pattern -ne $true) {
    Write-Host "Could not extract a version from [$buildNumber] using pattern [$pattern]"
} else {
    $extractedBuildNumber = $Matches[0]
    Write-Host "Using version $extractedBuildNumber"

    gci -Path $pathToSearch -Filter $searchFilter -Recurse | %{
        Write-Host "  -> Changing $($_.FullName)" 
		
		# remove the read-only bit on the file
		sp $_.FullName IsReadOnly $false

		# run the regex replace
        (gc $_.FullName) | % { $_ -replace $pattern, $extractedBuildNumber } | sc $_.FullName
    }    
	
# if pre-release version special suffix will be added to product version
if(! [string]::IsNullOrEmpty($prerelease)){	
	Write-Host "Marking as pre-release version [$extractedBuildNumber$prereleaseSuffixName]"
	
	$pattern = $assemblyInformationalVersionPattern+$pattern	
	$extractedBuildNumber = $assemblyInformationalVersionAttributName + '("' + $extractedBuildNumber + $prereleaseSuffixName
	
	gci -Path $pathToSearch -Filter $searchFilter -Recurse | %{
        Write-Host "  -> Changing $($_.FullName)" 
		
		# remove the read-only bit on the file
		sp $_.FullName IsReadOnly $false

		# run the regex replace
        (gc $_.FullName) | % { $_ -replace $pattern, $extractedBuildNumber } | sc $_.FullName	
	
		}
	}

Write-Host "------- Done UpdateVersion.ps1"
}