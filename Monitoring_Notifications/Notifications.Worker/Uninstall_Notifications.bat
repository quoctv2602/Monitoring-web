@echo off

SET PROG="C:\Workspace\Publish\Monitoring_Notifications/Monitoring_Notifications.exe"
set _ServiceName=Monitoring_Notifications

sc query %_ServiceName% | find "does not exist" >nul



if %ERRORLEVEL% EQU 1 GOTO uninstall



:uninstall
 echo Service Exist.
 sc.exe STOP Monitoring_Notifications
 sc.exe delete Monitoring_Notifications
 GOTO end


:end
 ECHO DONE!!!
 pause