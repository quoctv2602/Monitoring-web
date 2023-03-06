@echo off

SET PROG="C:/inetpub/wwwroot/MonitoringSchedulerService/Monitoring_wsGetHealth.exe"
set _ServiceName=Monitoring_wsGetHealth

sc query %_ServiceName% | find "does not exist" >nul

if %ERRORLEVEL% EQU 0 GOTO install

if %ERRORLEVEL% EQU 1 GOTO uninstall



:install
 echo Service Does Not Exist.
 sc.exe create Monitoring_wsGetHealth binPath= "%PROG% --contentRoot"
 sc.exe START Monitoring_wsGetHealth
 GOTO end
:uninstall
 echo Service Exist.
 sc.exe STOP Monitoring_wsGetHealth
 sc.exe delete Monitoring_wsGetHealth
 sc.exe create Monitoring_wsGetHealth binPath= "%PROG% --contentRoot"
 sc.exe START Monitoring_wsGetHealth
 GOTO end


:end
 ECHO DONE!!!
 pause