@echo off

SET PROG="D:\PerformanceMonitoring\service/Monitoring_wsAddCounters.exe"
set _ServiceName=Monitoring_wsAddCounters

sc query %_ServiceName% | find "does not exist" >nul

if %ERRORLEVEL% EQU 0 GOTO install





:install
 echo Service Does Not Exist.
 sc.exe create Monitoring_wsAddCounters binPath= "%PROG% --contentRoot"
 sc.exe START Monitoring_wsAddCounters
 GOTO end

:end
 ECHO DONE!!!
 pause