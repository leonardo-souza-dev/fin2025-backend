#!/bin/bash

echo "Building"
dotnet build

echo "Running"
dotnet run --project ./Fin.Api/Fin.Api.csproj

read -p "Press Enter to continue..."
