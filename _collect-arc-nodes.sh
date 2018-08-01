#!where bash

cat arc.log node1.log node2.log node3.log node4.log |
grep "\.\.\." |
gsort -k2n -k8n -k4 |
cat > arc-nodes.log

read -p "Press enter to continue"


