@echo off
chcp 65001 >nul
setlocal EnableDelayedExpansion
title Русификатор SkyFactory 5 v5.0.8 by Qiota

set "SRC=%~dp0.."
set "PACK=SF5_Russian-5.0.8"

echo ============================================
echo  Русификатор SkyFactory 5 (v5.0.8) by Qiota
echo ============================================
echo.

if not "%~1"=="" (
  set "INSTANCE=%~1"
  goto verify
)

set "INSTANCE="
if exist "%USERPROFILE%\.minecraftx\instances\SkyFactory 5-5.0.8\mods" set "INSTANCE=%USERPROFILE%\.minecraftx\instances\SkyFactory 5-5.0.8"
if not defined INSTANCE (
  for /d %%D in ("%USERPROFILE%\curseforge\minecraft\Instances\*SkyFactory*") do set "INSTANCE=%%D"
)
if not defined INSTANCE (
  if exist "%APPDATA%\.minecraft\mods" set "INSTANCE=%APPDATA%\.minecraft"
)
if defined INSTANCE goto verify

:ask
echo Не нашёл папку instance автоматически.
echo Укажи путь к папке instance (где лежат mods, resourcepacks, config):
set /p INSTANCE="Путь: "

:verify
if not exist "%INSTANCE%\mods" (
  echo [ОШИБКА] В "%INSTANCE%" нет папки mods. Проверь путь.
  pause
  exit /b 1
)
echo Папка instance: %INSTANCE%
echo.

:pack
echo [1/5] Ресурспак...
if not exist "%INSTANCE%\resourcepacks" mkdir "%INSTANCE%\resourcepacks"
xcopy "%SRC%\resourcepacks\%PACK%" "%INSTANCE%\resourcepacks\%PACK%\" /E /I /Y >nul
echo       Готово.

:scripts
echo [2/5] Скрипты (тултипы цветов, мешки, подсказки)...
call :copyfile "scripts-patch\scripts\tooltips.zs" "scripts\tooltips.zs"
call :copyfile "scripts-patch\scripts\globals.zs" "scripts\globals.zs"
call :copyfile "scripts-patch\scripts\colors\content\registry\item_registry.zs" "scripts\colors\content\registry\item_registry.zs"
for %%F in ("%SRC%\scripts-patch\scripts\colors\items\*.zs") do call :copyfile "scripts\colors\items\%%~nxF" "scripts\colors\items\%%~nxF"
echo       Готово.

:tasks
echo [3/5] Книга заданий...
call :copyfile "config-patch\config\checklist\tasks.txt" "config\checklist\tasks.txt"
echo       Готово.

:data
echo [4/5] Датапак (ачивки Forcecraft, JEI-инфо)...
xcopy "%SRC%\datapack-patch\data" "%INSTANCE%\global_packs\required_data\skyfactory_5\data\" /E /I /Y >nul
echo       Готово.

:enable
echo [5/5] Включение пака...
if not exist "%INSTANCE%\options.txt" (
  echo       options.txt нет (игра не запускалась) — включи пак вручную в меню.
  goto done
)
powershell -NoProfile -ExecutionPolicy Bypass -Command "$p='%INSTANCE%\options.txt'; $t=[IO.File]::ReadAllText($p); if ($t -notmatch 'SF5_Russian-5.0.8') { $t = $t -replace '(\"resourcePacks\":\[[^\]]*)(\])', '$1,\"file/SF5_Russian-5.0.8\"$2'; [IO.File]::WriteAllText($p, $t); Write-Output '      Пак включён.' } else { Write-Output '      Пак уже был включён.' }"

:done
echo.
echo ============================================
echo  Готово! Перезапусти игру.
echo  В игре: Настройки -^> Язык: Русский.
echo  Старые мешки в инвентаре сохранят имена.
echo  Откат: файлы .en.bak рядом с заменёнными.
echo ============================================
pause
exit /b 0

:copyfile
set "REL1=%~1"
set "REL2=%~2"
for %%P in ("%INSTANCE%\%REL2%") do set "PDIR=%%~dpP"
if not exist "%PDIR%" mkdir "%PDIR%" 2>nul
if exist "%INSTANCE%\%REL2%" (
  if not exist "%INSTANCE%\%REL2%.en.bak" copy /Y "%INSTANCE%\%REL2%" "%INSTANCE%\%REL2%.en.bak" >nul
)
copy /Y "%SRC%\%REL1%" "%INSTANCE%\%REL2%" >nul
exit /b 0
