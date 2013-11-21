@echo off
msbuild /t:Rebuild /p:Configuration=Release C5.csproj
cmd /c echo F | xcopy /Y bin\Release\C5.dll "package\lib\portable-net4+sl5+wp8+win8\C5.dll"
cmd /c echo F | xcopy /Y bin\Release\C5.xml "package\lib\portable-net4+sl5+wp8+win8\C5.xml"

msbuild /t:Rebuild /p:Configuration=Release;PCL=false C5.csproj
cmd /c echo F | xcopy /Y bin\Release\C5.dll "package\lib\net40\C5.dll"
cmd /c echo F | xcopy /Y bin\Release\C5.xml "package\lib\net40\C5.xml"

..\.nuget\NuGet.exe pack C5.nuspec -OutputDirectory bin/Release -BasePath package