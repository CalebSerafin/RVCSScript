# RVCSScript - Real Virtuality 4 C# Script
Sick of SQF? Want to use another scripting language but Mono only supports the predecessor to .NET. Look no more. Execute your .NET 7 for Arma 3 Today! (Soon™) 

# Problem
SQF was great when it was made for its original purpose. However, we have strayed so far from green pasters that SQF is now used to make large-scale persistent missions such as [A3-Antistasi](https://github.com/official-antistasi-community/A3-Antistasi). SQF is not an appropriate language for this purpose. Yes, it has been gradually improved, but it can be a struggle.<br/>
<br/>
The past alternatives have been loading DLLs directly via Arma 3’s Extension API. But this is a risky endeavour as it opens up servers or clients to trivial malicious attacks by bad actors. Each extension and updates requires its own Battleye whitelisting, preventing server owners from using Battleye with these extensions. These DLL extensions don’t allow hot-reload and require the game to be started and stopped to test changes. If the DLLs are developed outside of Arma, then they can be hot-reloaded, however, this misses out of the integration testing with Arma and SQF.

# Proposed Solution
Firstly, the primary goal is some security to isolate partial-trusted or untrusted code. Past attempts of CAS (Code Access Security) display how it eventually fails, and workarounds are found. CAS is not the way. The plan is to leverage the relevant Operating Systems’ process and memory isolation or virtualisation.<br/>

# Inter-Process Communication
Communication between the untrusted code and Arma will happen through pipes. This allows it to be more flexible to changing where the code is executed, whether it’s in a separate process, in a VM, or on a nearby network device. The cost comes with minor latency due to the abstraction layer. The latency and speed pipes between processes on the same machine should still perform excellently.<br/>
<br/>
The abstraction layer for pipes requires that specific commands and bindings be created to allow the script to communicate with Arma 3. That will require developer effort. The API of a script will also require developer time to dress up so that the end-user-developer does not need to worry about serialisation and how it works under the hood.<br/>
<br/>
In the broad picture, this approach should be friendly to loading any-language or runtime code in the isolated process. From Arma’s perspective, they will all share the same API. However, from each language’s perspective, they will need their own API kept up to date.<br/>

# C#’s Advantage for The Pilot Scripting Language.
In Unity and other engines, C# is usually incorporated via the Mono-Project runtime. However, this only supports .NET Framework, which requires developers to ignore and unlearn any new features and optimisations in .NET 5,6,7+. .NET provides simple APIs to use its Roslyn compiler, allowing user-editable scripts (Similar to Space Engineers)

