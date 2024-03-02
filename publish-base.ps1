<#
    .SYNOPSIS
    Build and Publish project

    .PARAMETER Configuration
    Configuration Debug, Dev, Qa or Prod

    .PARAMETER PublishSource
    PublishSource "website" or "xconnect"
#>

function PublishContent
{
    param(
        [String] $publishSource,
        [String] $configuration = "Debug",
        [String] $websiteFolderPath
    )

    $msbuildPaths = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
    if ($msbuildPaths.Length -eq 0) {
        $msbuildPaths = Resolve-Path "${env:ProgramFiles}\Microsoft Visual Studio\*\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
    }
    $msbuildPath = $msbuildPaths[$msbuildPaths.Length-1]
    $projectFolder = (get-item "$PSScriptRoot\src\Project\$($publishSource)\EditorsCopilot.Project.Deploy.csproj").FullName

    $solutionDir = $PSScriptRoot + "\"

    Write-Host "websiteFolderPath" $websiteFolderPath
    if (-not $websiteFolderPath)
    {
        $websiteFolderPath = "$PSScriptRoot\docker\deploy\$( $publishSource )"
    }
    If(!(test-path $websiteFolderPath))
    {
        New-Item -ItemType Directory -Force -Path $websiteFolderPath
    }
    $websiteFolder = (get-item $websiteFolderPath).FullName

    Write-Host "msbuild path:" -ForegroundColor Green
    Write-Host $msbuildPath -ForegroundColor Green

    Write-Host "$websiteFolder cleaning..." -ForegroundColor Green
    Get-ChildItem -Path $websiteFolder -Exclude ".gitkeep" -Recurse | Remove-Item -Force -Recurse -Verbose

    cmd.exe /c $msbuildPath $projectFolder /restore /p:Configuration=$configuration /p:DeployOnBuild=True /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:PublishUrl=$websiteFolder /p:SolutionDir=$solutionDir

    Write-Host "Published project:" -ForegroundColor Green
    Write-Host $projectFolder -ForegroundColor Green
    Write-Host "to folder:" -ForegroundColor Green
    Write-Host $websiteFolder -ForegroundColor Green
    Write-Host "successfully." -ForegroundColor Green
}