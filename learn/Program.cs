using System;
using System.Collections.Generic;

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
            // Suspend the screen.  
            Console.ReadLine();  
        }
    }
}