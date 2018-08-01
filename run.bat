if exist node.exe del node.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs node.cs

if exist arc.exe del arc.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs arc.cs

set config=config4.txt

start node.exe %config% 1 2 3 4
start node.exe %config% 2 1 3
start node.exe %config% 3 1 2 4
start node.exe %config% 4 1 3

start arc.exe %config% > arc.log 

pause
