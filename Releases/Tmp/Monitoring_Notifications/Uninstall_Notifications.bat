@echo off

set _ServiceName=Monitoring_Notification
sc | find %_ServiceName% >nul
if %ERRORLEVEL% EQU 0 GOTO stop

:stop
 echo Service Exist.
 sc.exe STOP %_ServiceName%

 GOTO end

:end
 ECHO DONE!!!
 pause