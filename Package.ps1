[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$SPTPath,
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release'
)
$ErrorActionPreference = 'Stop'
$repo = $PSScriptRoot
$spt = (Resolve-Path -LiteralPath $SPTPath).Path
[xml]$clientProject = Get-Content -Raw -LiteralPath (Join-Path $repo 'Client/MoeTradeMarker.Client.csproj')
[xml]$serverProject = Get-Content -Raw -LiteralPath (Join-Path $repo 'Server/MoeTradeMarker.Server.csproj')
$version = [string]$clientProject.Project.PropertyGroup.Version
if ($version -ne [string]$serverProject.Project.PropertyGroup.Version) { throw 'Client/server project versions differ.' }
$dist = Join-Path $repo 'dist'
$work = Join-Path $dist ('package-work/' + [guid]::NewGuid().ToString('N'))
$output = Join-Path $work 'build'
$staging = Join-Path $work 'staging'
$temporaryZip = Join-Path $work 'package.zip'
$destination = Join-Path $dist "Moe-TradeMarker-$version.zip"
New-Item -ItemType Directory -Path $staging -Force | Out-Null
try {
    # A fresh output tree prevents failed or partial builds from packaging old binaries.
    $outputProperty = '-p:MoeTradeMarkerOutputRoot=' + $output.Replace('\', '/') + '/'
    & dotnet build (Join-Path $repo 'MoeTradeMarker.sln') -t:Rebuild -c $Configuration "-p:SPTPath=$spt" $outputProperty
    if ($LASTEXITCODE -ne 0) { throw 'Build failed; existing release archive was not changed.' }
    & dotnet test (Join-Path $repo 'tests/Server/MoeTradeMarker.Server.Tests.csproj') -c $Configuration --no-build --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Tests failed; existing release archive was not changed.' }

    $clientRoot = 'BepInEx/plugins/Moe-TradeMarker'
    $serverRoot = 'SPT_Runtime/user/mods/Moe-TradeMarker'
    $clientDll = Join-Path $output "$clientRoot/MoeTradeMarker.Client.dll"
    $serverDll = Join-Path $output "$serverRoot/MoeTradeMarker.Server.dll"
    foreach ($dll in @($clientDll, $serverDll)) {
        if ([Reflection.AssemblyName]::GetAssemblyName($dll).Version.ToString() -ne "$version.0") {
            throw "Unexpected assembly version: $dll"
        }
    }
    Add-Type -Path (Join-Path $spt 'BepInEx/core/Mono.Cecil.dll')
    $assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($clientDll)
    try {
        $plugin = $assembly.MainModule.Types | Where-Object FullName -eq 'MoeTradeMarker.Client.Plugin'
        $metadata = $plugin.CustomAttributes | Where-Object { $_.AttributeType.FullName -eq 'BepInEx.BepInPlugin' }
        if ($metadata.ConstructorArguments[2].Value -ne $version) { throw 'BepInEx plugin version differs.' }
    } finally { $assembly.Dispose() }
    $assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($serverDll)
    try {
        $metadata = $assembly.MainModule.Types | Where-Object FullName -eq 'MoeTradeMarker.Server.ModMetadata'
        $constructor = $metadata.Methods | Where-Object { $_.Name -eq '.ctor' -and $_.Parameters.Count -eq 0 }
        if (-not ($constructor.Body.Instructions | Where-Object { $_.OpCode.Name -eq 'ldstr' -and $_.Operand -eq $version })) {
            throw 'Server mod metadata version differs.'
        }
    } finally { $assembly.Dispose() }

    $files = @("$clientRoot/MoeTradeMarker.Client.dll", "$clientRoot/MoeTradeMarker.Client.pdb")
    $files += @('config.json', 'MoeTradeMarker.Server.dll', 'MoeTradeMarker.Server.pdb',
        'MoeTradeMarker.Server.deps.json', 'MoeTradeMarker.Server.staticwebassets.endpoints.json',
        'MoeTradeMarker.Shared.dll', 'MoeTradeMarker.Shared.pdb') | ForEach-Object { "$serverRoot/$_" }
    foreach ($relative in $files) {
        $target = Join-Path $staging $relative
        New-Item -ItemType Directory -Path (Split-Path $target) -Force | Out-Null
        Copy-Item -LiteralPath (Join-Path $output $relative) -Destination $target
    }
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [IO.Compression.ZipFile]::CreateFromDirectory($staging, $temporaryZip)
    [IO.File]::Move($temporaryZip, $destination, $true)
    Write-Output "Verified package: $destination"
} finally {
    # The work directory is a unique child of this repository's dist/package-work.
    $expectedParent = [IO.Path]::GetFullPath((Join-Path $dist 'package-work'))
    if ((Test-Path -LiteralPath $work) -and [IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($work)) -eq $expectedParent) {
        Remove-Item -LiteralPath $work -Recurse -Force
    }
}
