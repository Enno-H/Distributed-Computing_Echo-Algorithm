using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace learn
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int counter = 0;  
            string line;  
            Dictionary<string,int> map = new Dictionary<string, int>();

            System.IO.StreamReader file = new System.IO.StreamReader(@"/Users/huangxiaolong/RiderProjects/learn/learn/config.txt");  
            while((line = file.ReadLine()) != null)  
            {  
                //System.Console.WriteLine (line);  
                counter++;
                if (!line.StartsWith("//"))
                {
                    string[] bit = line.Split(' ');
                    map.Add(bit[0],int.Parse(bit[1]));
                }
            }  

            file.Close();  
            Console.WriteLine("There were {0} lines.", counter);  
            Console.WriteLine(map.Count);
            
            ArrayList tt = new ArrayList();
            tt.Add(1);
            tt.Add(2);
            tt.Add(3);
            tt.Add(4);
            
            Node node1 = new Node(tt);
            
            Console.WriteLine(node1.Adj.Count);

            
            
            
            
            // Suspend the screen.  
            Console.ReadLine();  
        }

        public void readFile()
        {
            int counter = 0;  
            string line;  
            Dictionary<string,int> map = new Dictionary<string, int>();

            System.IO.StreamReader file = new System.IO.StreamReader(@"/Users/huangxiaolong/RiderProjects/learn/learn/config.txt");  
            while((line = file.ReadLine()) != null)  
            {  
                //System.Console.WriteLine (line);  
                counter++;
                if (!line.StartsWith("//"))
                {
                    string[] bit = line.Split(' ');
                    map.Add(bit[0],int.Parse(bit[1]));
                }
            }  

            file.Close();  
            Console.WriteLine("There were {0} lines.", counter);  
            Console.WriteLine(map.Count);
            // Suspend the screen.  
            Console.ReadLine();
        }
        
        
        internal class Node
        {
            private int id;
            private Boolean visited;
            private ArrayList adj;
            
            /*
            public Node()
            {
                id = -1;
                visited = false;
                adj = new ArrayList();
            }
            */

            public Node(ArrayList al)
            {
                if (al.Count < 2)
                {
                    Console.WriteLine("Wrong Input!!!");
                }
                else
                {
                    id = (int)al[0];
                    adj = new ArrayList();
                    for (int i = 1; i < al.Count; i++)
                    {
                        adj.Add((int)al[i]);
                    }
                }
            }
            
            
            public int Id   
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }
            
            public Boolean Visited   
            {
                get
                {
                    return visited;
                }
                set
                {
                    visited = value;
                }
            }
            
            public ArrayList Adj   
            {
                get
                {
                    return adj;
                }
                set
                {
                    adj = value;
                }
            }

        }
    }
}