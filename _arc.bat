set config=config4.txt

if exist arc.exe del arc.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs arc.cs
pause

arc.exe %config%
pause
