@echo off

SET PROG="C:/inetpub/wwwroot/MonitoringSchedulerService/Monitoring_wsGetHealth.exe"
set _ServiceName=Monitoring_wsGetHealth

sc query %_ServiceName% | find "does not exist" >nul



if %ERRORLEVEL% EQU 1 GOTO uninstall



:uninstall
 echo Service Exist.
 sc.exe STOP Monitoring_wsGetHealth
 sc.exe delete Monitoring_wsGetHealth
 GOTO end


:end
 ECHO DONE!!!
 pause