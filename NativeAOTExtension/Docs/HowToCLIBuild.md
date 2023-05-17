# Build With Dotnet CLI
_[↖ Back to Readme](../../README.md)_

## Select Output Type
To build this library in Debug: `dotnet publish -f net7.0 -c Debug -r win-x64 -p:PublishAot=true -p:NativeLib=Shared -p:SelfContained=true -p:OutputType=Library`<br/>
To build this library in Release: `dotnet publish -f net7.0 -c Release -r win-x64 -p:PublishAot=true -p:NativeLib=Shared -p:SelfContained=true -p:OutputType=Library`<br/>
If you want to update to .NET 8, change the `net7.0` to `net8.0`.<br/>
[Using NativeAOT/Prerequisites, 2021](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT/docs/using-nativeaot/prerequisites.md)<br/>

## 32 Bit on Windows
x86 32bit on Windows is not support. It will not be supported. Arma bearly has enough memory on it.<br/>
There are alot of x86 wierdness that must be worked around for the .NET Team to make native AOT work.<br/>
Most of that wierdness was resolved when x86-64 was made.<br/>

## 32 Bit on Linux
Its possible that the .NET Team will support 32 bit on Linux as they report it has an easier API to work with.
[Support x86 for NativeAOT, 2021](https://github.com/dotnet/runtimelab/issues/1471)<br/>

## See Other Samples
https://github.com/dotnet/runtimelab/tree/feature/NativeAOT/samples/HelloWorld <br/>
https://github.com/dotnet/runtimelab/tree/feature/NativeAOT/samples/NativeLibrary <br/>

---
_[↖ Back to Readme](../../README.md)_