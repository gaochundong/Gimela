@ECHO OFF

SET framework=v4.0.30319

"%SystemDrive%\Windows\Microsoft.NET\Framework\%framework%\MSBuild.exe" foundation.proj /t:Build /p:Configuration=Debug /v:m

"%SystemDrive%\Windows\Microsoft.NET\Framework\%framework%\MSBuild.exe" rukbat_service.proj /t:Build /p:Configuration=Debug /v:m

"%SystemDrive%\Windows\Microsoft.NET\Framework\%framework%\MSBuild.exe" rukbat_gui.proj /t:Build /p:Configuration=Debug /v:m

"%SystemDrive%\Windows\Microsoft.NET\Framework\%framework%\MSBuild.exe" test.proj /t:Build /p:Configuration=Debug /v:m

ECHO.
ECHO   Build Finished!
