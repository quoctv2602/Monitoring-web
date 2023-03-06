@echo off

SET PROG="C:\Workspace\Publish\Monitoring_Notifications/Monitoring_Notifications.exe"
set _ServiceName=Monitoring_Notifications

sc query %_ServiceName% | find "does not exist" >nul

if %ERRORLEVEL% EQU 0 GOTO install





:install
 echo Service Does Not Exist.
 sc.exe create Monitoring_Notifications binPath= "%PROG% --contentRoot"
 sc.exe START Monitoring_Notifications
 GOTO end

:end
 ECHO DONE!!!
 pause