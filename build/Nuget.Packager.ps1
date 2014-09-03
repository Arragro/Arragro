param ( 
	[parameter(mandatory=$true, valuefrompipeline=$true)]
	[string]$buildDirectory,
	[parameter(mandatory=$true, valuefrompipeline=$true)]
	[string]$version,
	[parameter(mandatory=$true, valuefrompipeline=$true)]
	[string]$nugetApiKey
)

$nugetPath = "$($buildDirectory)\nuget"
if (Test-Path $nugetPath) { Remove-Item $nugetPath -Force -Recurse }
New-Item $nugetPath -ItemType Directory -Force

function Invoker($exp) {
    $er = (Invoke-Expression $exp) 2>&1
	    if ($lastexitcode) {throw $er}
}

function SetNugetApiKey($nugetApiKey) {
	$exp = "& `"$($buildDirectory)\src\.nuget\Nuget.exe`" setApiKey $($nugetApiKey)"
    Invoker $exp
}

function PackageNuget($buildDirectory, $packPath, $nuspec, $version) {
    $exp =  "& `"$($buildDirectory)\src\.nuget\Nuget.exe`" pack '$packPath\$nuspec' -OutputDirectory '$packPath' -Version '$($version)' -Symbols"
    Invoker $exp
}

function PushNuget($buildDirectory, $packPath, $project, $version) {

    $exp = "& `"$($buildDirectory)\src\.nuget\Nuget.exe`" push '$packPath\$project.$($version).nupkg'"
    Invoker $exp
    #Invoke-Expression "& `"$($buildDirectory)\src\.nuget\Nuget.exe`" push '$packPath\$project.$($version).symbols.nupkg'"
}

function BuildNugetCommon($buildDirectory, $version, $project) {
    
    $projPath = "$($buildDirectory)\src\$($project)"
    $nugetPath = "$($buildDirectory)\nuget"
    $nugetProjPath = "$($nugetPath)\$($project)"
    $dllsPath = "$($buildDirectory)\src\dlls\"
    $libNet45 = "$($nugetProjPath)\lib\net45"
    $dllsPath = "$($buildDirectory)\src\dlls"

    if (Test-Path $nugetProjPath) { Remove-Item $nugetProjPath -Force -Recurse }
    New-Item $libNet45 -ItemType Directory -Force

    Copy-Item "$($projPath)\bin\Release\$($project).dll" $libNet45
    Copy-Item "$($projPath)\bin\Release\$($project).pdb" $libNet45
    Copy-Item "$($buildDirectory)\build\$($project).nuspec" $nugetProjPath

    PackageNuget $buildDirectory $nugetProjPath "$($project).nuspec" $version
    PushNuget $buildDirectory $nugetProjPath "$($project)" $version
}

Try
{
    SetNugetApiKey $nugetApiKey
    BuildNugetCommon $buildDirectory $version "Arragro.Common"
}
Catch [System.Exception]
{
    Write-Error $_.Exception;
    Write-Output $_.Exception;
    exit 1;
}