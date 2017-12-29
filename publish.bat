SET version=1
echo %version%

nuget restore src;

SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"

%MSBUILD% src\Arragro.sln /property:OutputPath=bin\Release;Configuration=Release;Version=%version%
