# BOF-Builder
C# .Net 5.0 project to build BOF (Beacon Object Files) in mass based on them all being in a folder directory struct somewhere.

Useful for building and I guess 'testing if they compile' BOF locally or in a pipeline ;)

# Pre Req/ Install these first
- Install Linux subsystems for windows

- Install a Linux OS say ubuntu or kali form windows store (wsl.exe/bash.exe)
 
- Install these on Linux (WSL) `apt install gcc-mingw-w64-x86-64 gcc-mingw-w64-i686 make -y`

- Install Visual Studio for the 'x64 Native Tools Command Prompt for VS 2019' or 'x86 Native Tools Command Prompt for VS 2019'

# How to Use
Run tool from 'x64 Native Tools Command Prompt for VS 2019' (if your goal is to build x64)

It runs like this from cmd.exe (or powershell) `%comspec% /k "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat"`

Its normally got a short cut here `C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Visual Studio 2019\Visual Studio Tools\VC`

Its normally got a bat file `C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build`

## Usage 
    Example: BuildBOFs.exe -rootdir C:\Path\to\BOFs

    Commands:
    -rootdir
        The folder that contains the BOF files to build (REQUIRED)
    -x86
        Tell to compile linux bins for x86 (i686-w64-mingw32-gcc)
    -x64
        Tell to compile linux bins for x64 (86_64-w64-mingw32-gcc) (DEFAULT)
    -timeout
        Sets the timeout for the process who is building bin (DEFAULT 3 seconds) (time in milliseconds)
    -wsl
        Tell app to use wsl.exe instead of bash.exe to compile linux bins

# Errors/ My stuff wont build
- Why? Its often that the repo is missing something or has errors (errors like syntax or they coded it wrong). I can tell you i tested this on ~90% of BOF's on Github as of 09/2021 and most of them have errors as is. I suggest you fix them or send them an issue. The compiler output is shown to screen.

- My fav error ` 1 | #include <Windows.h>` for a linux compile target .....yep... literally used the wrong import. Change to all lowercase to fix. I rest my case for statement above. But im glad you found the time to test my software and it tested there stuff for you, wait you tested there stuff to right?

# Things to know

- No, i did not check your BOF for lets call it malware
- All this does it build the files. Thats it. 
