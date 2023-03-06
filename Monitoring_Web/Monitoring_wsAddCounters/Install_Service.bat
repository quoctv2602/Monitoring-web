@echo off

SET PROG="D:/PerformanceMonitoring/service/Monitoring_wsAddCounters.exe"
set _ServiceName=Monitoring_wsAddCounters

sc query %_ServiceName% | find "does not exist" >nul

if %ERRORLEVEL% EQU 0 GOTO install

if %ERRORLEVEL% EQU 1 GOTO uninstall



:install
 echo Service Does Not Exist.
 sc.exe create Monitoring_wsAddCounters binPath= "%PROG% --contentRoot"
 sc.exe START Monitoring_wsAddCounters
 GOTO end
:uninstall
 echo Service Exist.
 sc.exe STOP Monitoring_wsAddCounters
 sc.exe delete Monitoring_wsAddCounters
 sc.exe create Monitoring_wsAddCounters binPath= "%PROG% --contentRoot"
 sc.exe START Monitoring_wsAddCounters
 GOTO end


:end
 ECHO DONE!!!
 pause