# BOF-Builder
C# .Net 5.0 project to build BOF (Beacon Object Files) in mass based on them all being in a folder directory stuct somewhere.

Useful for building BOF locally or in a pipeline ;)

# Pre Req/ Install these first
- Install Linux subsystems for windows

- Install a Linux OS say ubuntu or kali form windows store
 
- `apt install gcc-mingw-w64-x86-64 i686-w64-mingw32-gcc make -y`

- Install Visual Studio for the 'x64 Native Tools Command Prompt for VS 2019'

# How to Use
Run tool from 'x64 Native Tools Command Prompt for VS 2019' (if your goal is to install x64)

Its normally got a short cut here `C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Visual Studio 2019\Visual Studio Tools\VC`

Its normally got a bat file `C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build`

It runs like this `%comspec% /k "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat"`

# Example Usage
`./BuildBOFs.exe C:\Path\To\BOF\Files\Dir`

# Errors/ My stuff wont build
- Why? Its often that the repo is missing something or has errors (errors like syntax or they coded it wrong). I can tell you i tested this on ~90% of BOF's on Github as of 09/2021 and most of them have errors as is. I suggest you fix them or send them an issue. The compiler output is shown to screen.

# Things to know

- No, i did not check your BOF for lets call it malware
- All this does it build the files. Thats it. 
