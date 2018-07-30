csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll arc.cs
start arc.exe config.txt > arcs.log 2>&1
pause