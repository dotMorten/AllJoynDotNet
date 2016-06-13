SET AJ_VERSION=15.09

XCOPY AllJoynCAPI.nuspec .\nuget\ /Y 
XCOPY AllJoynCAPI-MonoAndroid10.targets .\nuget\build\native\MonoAndroid10\AllJoynCAPI.targets /Y 

XCOPY %AJ_VERSION%\Android\x86\liballjoyn_c.so nuget\libs\MonoAndroid10\x86\ /Y /S
XCOPY %AJ_VERSION%\Android\armeabi-v7a\liballjoyn_c.so nuget\libs\MonoAndroid10\armeabi-v7a\ /Y /S
XCOPY %AJ_VERSION%\Android\x86-debug\liballjoyn_c.so nuget\libs\MonoAndroid10\x86-debug\ /Y /S
XCOPY %AJ_VERSION%\Android\armeabi-v7a-debug\liballjoyn_c.so nuget\libs\MonoAndroid10\armeabi-v7a-debug\ /Y /S

XCOPY %AJ_VERSION%\Windows\x64\alljoyn_c.dll nuget\runtimes\win7-x64\native\ /Y /S
XCOPY %AJ_VERSION%\Windows\x86\alljoyn_c.dll nuget\runtimes\win7-x86\native\ /Y /S

XCOPY %AJ_VERSION%\WinUWP\x64\alljoyn_c.dll nuget\runtimes\win10-x64\native\ /Y /S
XCOPY %AJ_VERSION%\WinUWP\x86\alljoyn_c.dll nuget\runtimes\win10-x86\native\ /Y /S
XCOPY %AJ_VERSION%\WinUWP\arm\alljoyn_c.dll nuget\runtimes\win10-arm\native\ /Y /S

XCOPY %AJ_VERSION%\iOS\ARM\alljoyn_c.a \staticlib\Xamarin.iOS10\ /Y /S

NUGET PACK nuget\AllJoynCAPI.nuspec

PAUSE