CD ..\..\..\alljoyn\
SET ANDROID_NDK_PATH=c:\ProgramData\Microsoft\AndroidNDK\android-ndk-r10e

c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=android ANDROID_NDK=%ANDROID_NDK_PATH% BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=debug CPU=x86 
xcopy build\android\x86\debug\dist\c\lib\liballjoyn_c.so ..\src\Libs\15.09\Android\x86-debug\ /Y /S
xcopy build\android\x86\debug\dist\c\lib\liballjoyn_c.a ..\src\Libs\15.09\Android\x86-debug\ /Y /S
PAUSE
c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=android ANDROID_NDK=%ANDROID_NDK_PATH% BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=debug CPU=arm
xcopy build\android\arm\debug\dist\c\lib\liballjoyn_c.so ..\src\Libs\15.09\Android\armeabi-v7a-debug\ /Y /S
xcopy build\android\arm\debug\dist\c\lib\liballjoyn_c.a ..\src\Libs\15.09\Android\armeabi-v7a-debug\ /Y /S

c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=android ANDROID_NDK=%ANDROID_NDK_PATH% BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=release CPU=x86 
xcopy build\android\x86\release\dist\c\lib\liballjoyn_c.so ..\src\Libs\15.09\Android\x86\ /Y /S
xcopy build\android\x86\release\dist\c\lib\liballjoyn_c.a ..\src\Libs\15.09\Android\x86\ /Y /S

c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=android ANDROID_NDK=%ANDROID_NDK_PATH% BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=release CPU=arm
xcopy build\android\arm\release\dist\c\lib\liballjoyn_c.so ..\src\Libs\15.09\Android\armeabi-v7a\ /Y /S
xcopy build\android\arm\release\dist\c\lib\liballjoyn_c.a ..\src\Libs\15.09\Android\armeabi-v7a\ /Y /S
