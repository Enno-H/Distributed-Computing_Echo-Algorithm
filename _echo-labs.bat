set path=C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\Roslyn\;%path%

rem if exist node.exe del node.exe
rem csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs node.cs
rem pause

rem if exist arc.exe del arc.exe
rem csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs arc.cs
rem pause

set config=config4.txt

rem 2^>^&1
start "node1" cmd /k node.exe %config% 1 2 3 4 ^> node1.log 
start "node2" cmd /k node.exe %config% 2 1 3 ^> node2.log
start "node3" cmd /k node.exe %config% 3 1 2 4 ^> node3.log
start "node4" cmd /k node.exe %config% 4 1 3 ^> node4.log

rem 2>&1
arc.exe %config% > arc.log 

pause
