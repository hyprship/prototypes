#!/usr/bin/env bash

if [[ -f "$HOME/.dotnet/dotnet" ]]; then
    export DOTNET_ROOT="$HOME/.dotnet"
    "$HOME/.dotnet/dotnet" run ./eng/cli/main.cs "$@"
    exit $?
else
    dotnet run ./eng/cli/main.cs "$@"
    exit $?
fi