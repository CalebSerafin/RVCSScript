echo off
REM Expects output file as first argument and solution directory as second argument
echo CopyDLL Copying output file %1 to %2/CallArmaExtension/ArmaExtensions/NativeAOTExtension_x64.dll
copy %1 %2/CallArmaExtension/ArmaExtensions/NativeAOTExtension_x64.dll
