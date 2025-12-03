msbuild .\EndUser.csproj /P:Configuration=Release /P:CreateNugetPackage=true /P:IncludeSymbols=true /P:SymbolPackageFormat=snupkg
Copy-Item ..\nugets\*.enduser.*nupkg -Destination $env:localNugets -Force
