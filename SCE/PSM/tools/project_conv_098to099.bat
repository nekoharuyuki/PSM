@echo off

if {%1} == {} goto error

cd /d %~dp0

python\python.exe pss2psm_rename_tool.py %1
python\python.exe cfg2xml_replace_tool.py %1

goto end

:error
@echo.
@echo Usage:
@echo   project_conv_098to099.bat [directory for convert]
@echo.

pause

:end
