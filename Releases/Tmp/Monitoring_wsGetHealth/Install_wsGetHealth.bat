@echo off

SET PROG="C:\inetpub\wwwroot\MonitoringSchedulerService/Monitoring_wsGetHealth.exe"
set _ServiceName=Monitoring_wsGetHealths

sc query %_ServiceName% | find "does not exist" >nul
if %ERRORLEVEL% EQU 0 GOTO install

sc query %_ServiceName%| find "%_ServiceName%" >nul
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