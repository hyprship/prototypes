
$scriptDir = $PSScriptRoot

if (!($env:PATH -contains "$scriptDir")) {
    $env:PATH = "$scriptDir;$env:PATH"
}

if (Test-Path "$HOME/.dotnet/dotnet") {
    $env:DOTNET_ROOT = "$HOME/.dotnet"
    & "$HOME/.dotnet/dotnet" run ./eng/cli/main.cs @args
    exit $LASTEXITCODE
} else {
    dotnet run ./eng/cli/main.cs @args
    exit $LASTEXITCODE
}