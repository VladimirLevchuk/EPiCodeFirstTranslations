set ApiKey=harderwork
set Server= http://localhost/Nuget/

rem echo nuget push "%~1" %ApiKey% -s %Server%
nuget push "%~1" %ApiKey% -s %Server%