if exist node.exe del node.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs node.cs
pause

if exist arc.exe del arc.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs arc.cs
pause

set config=config5.txt

rem 2^>^&1
start "node1" cmd /k node.exe %config% 1 2 3 4 
start "node2" cmd /k node.exe %config% 2 1 5 6
start "node3" cmd /k node.exe %config% 3 1 4 6 7
start "node4" cmd /k node.exe %config% 4 1 3 5 6
start "node5" cmd /k node.exe %config% 5 2 4 8 
start "node6" cmd /k node.exe %config% 6 2 3 4 7 8
start "node7" cmd /k node.exe %config% 7 3 6 8
start "node8" cmd /k node.exe %config% 8 5 6 7

rem 2>&1
arc.exe %config%

pause
