csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll node.cs

start node.exe config.txt 1 2 > node1.log 2>&1
start node.exe config.txt 2 1 > node2.log 2>&1
start node.exe config.txt 3 1 2 > node3.log 2>&1
pause
