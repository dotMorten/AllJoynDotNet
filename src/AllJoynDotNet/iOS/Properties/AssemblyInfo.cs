using ObjCRuntime;
using System.Runtime.InteropServices;

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("75dd2887-d200-4a3f-aeaa-04caf991c3b7")]

[assembly: LinkWith("liballjoyn.a", LinkTarget.Simulator | LinkTarget.ArmV7 | LinkTarget.Arm64, LinkerFlags = "-lc++", Frameworks = "CoreText", SmartLink = true)]

