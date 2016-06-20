RMDIR nuget /S /Q
XCOPY dotMorten.AllJoyn.CAPI.nuspec .\nuget\ /Y /F
echo f | XCOPY /Y /F AllJoynCAPI-MonoAndroid10.targets .\nuget\build\MonoAndroid10\dotMorten.AllJoyn.CAPI.targets

XCOPY AllJoyn_C\Android\x86\liballjoyn_c.so nuget\libs\MonoAndroid10\x86\ /Y /S
XCOPY AllJoyn_C\Android\armeabi-v7a\liballjoyn_c.so nuget\libs\MonoAndroid10\armeabi-v7a\ /Y /S
XCOPY AllJoyn_C\Android\x86-debug\liballjoyn_c.so nuget\libs\MonoAndroid10\x86-debug\ /Y /S
XCOPY AllJoyn_C\Android\armeabi-v7a-debug\liballjoyn_c.so nuget\libs\MonoAndroid10\armeabi-v7a-debug\ /Y /S


echo f | XCOPY /Y /F AllJoynCAPI-net452.targets .\nuget\build\net452\dotMorten.AllJoyn.CAPI.targets
rem XCOPY AllJoyn_C\Win7\x64\alljoyn_c.dll nuget\runtimes\win7-x64\native\ /Y /S
rem XCOPY AllJoyn_C\Win7\x86\alljoyn_c.dll nuget\runtimes\win7-x86\native\ /Y /S
XCOPY AllJoyn_C\Win7\x64\alljoyn_c.dll nuget\libs\net452\x64\ /Y /S
XCOPY AllJoyn_C\Win7\x86\alljoyn_c.dll nuget\libs\net452\x86\ /Y /S


REM XCOPY AllJoyn_C\WinUWP\x64\alljoyn_c.dll nuget\runtimes\win10-x64\native\ /Y /S
REM XCOPY AllJoyn_C\WinUWP\x86\alljoyn_c.dll nuget\runtimes\win10-x86\native\ /Y /S
REM XCOPY AllJoyn_C\WinUWP\arm\alljoyn_c.dll nuget\runtimes\win10-arm\native\ /Y /S

XCOPY AllJoyn_C\iOS\ARM\alljoyn_c.a \staticlib\Xamarin.iOS10\ /Y /S

NUGET PACK nuget\dotMorten.AllJoyn.CAPI.nuspec

PAUSE