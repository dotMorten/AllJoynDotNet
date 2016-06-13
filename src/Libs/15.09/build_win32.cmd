CD ..\..\..\alljoyn\
c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=win7 BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=debug CPU=x86 
xcopy build\win7\x86\debug\dist\c\lib\alljoyn_c.dll ..\src\Libs\15.09\Windows\x86-debug\ /Y /S

c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=win7 BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=debug CPU=x86_64 
xcopy build\win7\x64\debug\dist\c\lib\alljoyn_c.dll ..\src\Libs\15.09\Windows\x64-debug\ /Y /S

c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=win7 BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=release CPU=x86 
xcopy build\win7\x86\release\dist\c\lib\alljoyn_c.dll ..\src\Libs\15.09\Windows\x86\ /Y /S

c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=win7 BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=release CPU=x86_64
xcopy build\win7\x64\release\dist\c\lib\alljoyn_c.dll ..\src\Libs\15.09\Windows\x64\ /Y /S
