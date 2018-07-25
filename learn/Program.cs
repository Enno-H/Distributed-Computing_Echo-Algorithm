using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;

namespace learn
{
    
    public class Program
    {
        static public int ThisNode;
        
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
          
            Node node = new Node(tt);
            Console.WriteLine(node.Adj.Count);
            
            
            
            Uri baseAddress = new Uri("http://localhost:8081/hello");

            WebServiceHost host = null;

            try {
                host = new WebServiceHost(typeof(HelloWorldService), baseAddress);
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INodeService), new WebHttpBinding(), "");

                host.Open();
			
                var names = Environment.GetCommandLineArgs();
                foreach (var name in names){
                    Console.WriteLine(name);
                }

                Console.WriteLine($"The service is ready at {baseAddress}/Hello?name=xyz or /SayHello?name=xyz");
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
        
            } catch (Exception ex) {
                Console.WriteLine($"*** Exception {ex.Message}");
                host = null;
        
            } finally {
                if (host != null) ((IDisposable)host).Dispose();
            }
            
        }
        
        
        
        [ServiceContract()]
        public interface INodeService {
            [OperationContract(IsOneWay=true)]
            [WebGet()]
            void Message(int from, int to, int tok, int pay);
            
            [OperationContract()] 
            [WebGet(ResponseFormat=WebMessageFormat.Json)]
            string SayHello(string name);
        }
        
        [ServiceBehavior(
            InstanceContextMode=InstanceContextMode.Single, 
            ConcurrencyMode=ConcurrencyMode.Multiple)] 
        public class HelloWorldService : INodeService {
            public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;
            
            public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;
            
    
            public void Message(int from, int to, int tok, int pay){
                Console.WriteLine($"... {Millis():F2} {ThisNode} < {from} {to} {tok} {pay}");
            }
            
            
            public string SayHello(string name) {
                Console.WriteLine($"[{Tid()}] [{Millis()}] SayHello received: {name}");
                return $"Hello, {name}";
            }
        }
    
        internal class Node
        {
            private int id;
            private Boolean visited;
            private ArrayList adj;
                
            public Node()
            {
                id = -1;
                visited = false;
                adj = new ArrayList();
            }
    
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
                get => id;
                set => id = value;
            }
                
            public Boolean Visited
            {
                get => visited;
                set => visited = value;
            }
                
            public ArrayList Adj
            {
                get => adj;
                set => adj = value;
            }   
        }   
    }
}