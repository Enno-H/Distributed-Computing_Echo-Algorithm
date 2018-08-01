set config=config4.txt

if exist node.exe del node.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs node.cs
pause

node.exe %config% 1 2 3 4
pause
