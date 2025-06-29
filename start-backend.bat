@echo off

echo Checking D drive
d:

echo Changing directory to backend project
cd "D:\projs\Fin2025\backend"

echo Building 
dotnet build

echo Running 
dotnet run --project .\Fin.Api\Fin.Api.csproj

pause