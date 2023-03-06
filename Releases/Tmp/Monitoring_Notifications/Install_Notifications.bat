@echo off

SET PROG="C:\inetpub\wwwroot\MonitoringNotificationService\Monitoring_Notifications.exe"
set _ServiceName=Monitoring_Notification

sc query %_ServiceName% | find "does not exist" >nul
if %ERRORLEVEL% EQU 0 GOTO install

sc | find %_ServiceName% >nul
if %ERRORLEVEL% EQU 0 GOTO start

:install
 echo Service Does Not Exist.
 sc.exe create %_ServiceName% binPath= "%PROG% --contentRoot"
 GOTO start
 
 :start
 echo Service was be started.
 sc.exe START %_ServiceName%
 GOTO end

:end
 ECHO DONE!!!
 pause