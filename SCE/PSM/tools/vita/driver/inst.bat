@echo off
cls

set DPINST_TYPE=x86

if exist "%systemroot%\sysnative\" set DPINST_TYPE=amd64
if exist "%systemroot%\syswow64\" set DPINST_TYPE=amd64


if "%SCE_PSM_SDK%" == "" (
    echo "working directory."
) else (
    echo "use environment variable."
    if exist "%SCE_PSM_SDK%\tools\vita\driver\" cd "%SCE_PSM_SDK%\tools\vita\driver\"
)


copy /Y .\dpinst\%DPINST_TYPE%\dpinst.exe .\ > nul

dpinst.exe /SA /EL

del dpinst.exe
