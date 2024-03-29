param (
	[string]$ProjectFilePath, 
	[string]$OutputDirectory, 
	[string]$Configuration, 
	[string]$Platform
	)

$THIS_SCRIPTS_DIRECTORY = Split-Path $script:MyInvocation.MyCommand.Path
$fileName = “$THIS_SCRIPTS_DIRECTORY\msbuildparams.xml”;

$xmlDoc = [xml] @"
<params>
  <ProjectFilePath>$ProjectFilePath</ProjectFilePath>
  <OutputDirectory>$OutputDirectory</OutputDirectory>
  <Configuration>$Configuration</Configuration>
  <Platform>$Platform</Platform>
</params>
"@
$xmlDoc.Save($fileName);

Write-Host "params are saved to $fileName"

#read param example
#[xml] $xdoc = get-content $fileName
#Write-Host $xdoc.params.ProjectFilePath
