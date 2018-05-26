
cd

copy /Y "%1%2"  builds\x64\GameData\ksp-advanced-flybywire\

set H=R:\KSP_1.4.3_dev
set GAMEDIR=ksp-advanced-flybywire

echo %H%



mkdir "%H%\GameData\%GAMEDIR%"
xcopy  /E /y builds\x64\GameData\ksp-advanced-flybywire "%H%\GameData\%GAMEDIR%"
pause