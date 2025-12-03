msbuild .\EndUser.Wpf.csproj /P:Configuration=Release /P:CreateNugetPackage=true /P:IncludeSymbols=true
Copy-Item ..\nugets\*.enduser.*.*nupkg -Destination $env:localNugets -Force
