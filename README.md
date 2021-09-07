# BOF-Builder
C# .Net 5.0 project to build BOF (Beacon Object Files) in mass

## Pre Req/ Install these first
Install Linux subsystems for windows
Install a Linux OS say ubuntu or kali form windows store
`apt install gcc-mingw-w64-x86-64 i686-w64-mingw32-gcc make -y`
Install Visual Studio for the 'x64 Native Tools Command Prompt for VS 2019'

# How to Use
Run tool from 'x64 Native Tools Command Prompt for VS 2019' (if your goal is to install x64)

# Example Usage
./BuildBOFs.exe C:\Path\To\BOF\Files\Dir

# Errors/ My stuff wont build
- Why? Its often that the repo is missing something or has errors (errors like they coded it wrong). I can tell you i tested this on ~90% of BOF's on Github as of 09/2021 and most of the have errors as is. I suggest you fix them or send them an issue. The compiler output is shown to screen.
