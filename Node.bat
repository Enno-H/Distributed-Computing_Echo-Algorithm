csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll node.cs


start node.exe config.txt 2 1 3 > node2.log 2>&1
pause
start node.exe config.txt 3 1 2 4 > node3.log 2>&1
pause
start node.exe config.txt 4 1 3 > node4.log 2>&1
pause
start node.exe config.txt 1 2 3 4 > node1.log 2>&1
pause