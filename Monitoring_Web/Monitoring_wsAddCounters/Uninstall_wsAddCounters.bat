@echo off

SET PROG="D:/PerformanceMonitoring/service/Monitoring_wsAddCounters.exe"
set _ServiceName=Monitoring_wsAddCounters

sc query %_ServiceName% | find "does not exist" >nul



if %ERRORLEVEL% EQU 1 GOTO uninstall



:uninstall
 echo Service Exist.
 sc.exe STOP Monitoring_wsAddCounters
 sc.exe delete Monitoring_wsAddCounters
 GOTO end


:end
 ECHO DONE!!!
 pause