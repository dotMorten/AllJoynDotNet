## AllJoyn Core Build steps

Pull AllJoyn submodule.

Install Python 2.x (3.x won't work!) http://www.python.org

Install scons Windows Installer (into 2.x python when asked): http://scons.org/pages/download.html

CD to root of AllJoyn Submodule ( `\alljoyn\` where `SConstruct` file is):


#### Build commands


**ANDROID x86**
```
c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=android ANDROID_NDK=%ANDROID_NDK_PATH% BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=release CPU=x86 
```
Output location: `\alljoyn\build\android\x86\release\dist\c\lib\liballjoyn_c.so`


**ANDROID ARMv7**
```
c:\Python27\python.exe c:\Python27\Scripts\scons.py OS=android ANDROID_NDK=%ANDROID_NDK_PATH% BINDINGS=c BUILD_SERVICES_SAMPLES=off VARIANT=release CPU=arm
```
Output location: `\alljoyn\build\android\arm\release\dist\c\lib\liballjoyn_c.so`

