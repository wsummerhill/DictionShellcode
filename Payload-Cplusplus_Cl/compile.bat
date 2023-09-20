@ECHO OFF

cl.exe /nologo /Ox /MT /W0 /GS- /DNDEBUG /TcCPP_DictionShellcode.cpp /link /OUT:DictionShellcode.exe /SUBSYSTEM:CONSOLE /MACHINE:x64