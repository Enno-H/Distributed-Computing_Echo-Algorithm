csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll node.cs
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll arc.cs

start node.exe config.txt 1 2 3 4 > node1.log 2>&1
start node.exe config.txt 2 1 3 > node2.log 2>&1
start node.exe config.txt 3 1 2 4 > node3.log 2>&1
start node.exe config.txt 4 1 3 > node4.log 2>&1
pause
start arc.exe config.txt > arcs.log 2>&1
