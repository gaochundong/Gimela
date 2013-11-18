@ECHO OFF

SET folderBin=bin
SET folderObj=obj

".\Knifer\rmdir.exe" -d "..\src" -e "%folderBin%" -r -f
".\Knifer\rmdir.exe" -d "..\src" -e "%folderObj%" -r -f

".\Knifer\rmdir.exe" -d "..\tests" -e "%folderBin%" -r -f
".\Knifer\rmdir.exe" -d "..\tests" -e "%folderObj%" -r -f

ECHO.
ECHO   Clean Over!
