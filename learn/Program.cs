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
        static public ArrayList Adj;
        static public Boolean Visited;
        static public int Parent;

        static public Dictionary<string, int> map;
        
        public static void Main(string[] args)
        {
            /*
            //Step 0: Get input
            var names = Environment.GetCommandLineArgs();
            foreach (var name in names){
                Console.WriteLine(name);
            }
            */
            
            
            //Step 1: Config.txt -> Map
            //Input 1
            int counter = 0;  
            string line;  
            map = new Dictionary<string, int>();

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
            
            /*
            ArrayList tt = new ArrayList();
            tt.Add(1);
            tt.Add(2);
            tt.Add(3);
            tt.Add(4);
          
            Node node = new Node(tt);
            Console.WriteLine(node.Adj.Count);
            */
            
            //Step 2: ArrayList -> Node 
            //Input 2
            Parent = -1;
            ArrayList tt = new ArrayList();
            tt.Add(1);
            tt.Add(2);
            tt.Add(3);
            tt.Add(4);
            
            if (tt.Count < 2)
            {
                Console.WriteLine("Wrong Input!!!");
            }
            else
            {
                ThisNode = (int)tt[0];
                Adj = new ArrayList();
                for (int i = 1; i < tt.Count; i++)
                {
                    Adj.Add((int)tt[i]);
                }
            }
            
            
            
            
            
            
            
            
            
            //Step 3: Create Host
            Uri baseAddress = new Uri("http://localhost:8081/hello");

            WebServiceHost host = null;

            try {
                
                host = new WebServiceHost(typeof(HelloWorldService), baseAddress);
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INodeService), new WebHttpBinding(), "");

                host.Open();
			
                //ClientClass.ClientTo();
                
                
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
                if (to == ThisNode)
                {
                    if (Visited == false && Parent != 0)
                    {
                        Parent = from;
                    }
                    
                    
                }

            }
            
            
            public string SayHello(string name) {
                Console.WriteLine($"[{Tid()}] [{Millis()}] SayHello received: {name}");
                return $"Hello, {name}";
            }
        }
        
        /*
        static public void Client()
        {
            
            using (var wcf = new WebChannelFactory<INodeService>(new Uri("http://localhost:8081/hello"))) {
                var channel = wcf.CreateChannel();
    
                
                var names = Environment.GetCommandLineArgs();
                
                foreach (var name in names ) {
                    
    
                    Console.WriteLine($"Calling SayHello for {name}");
                    var res = channel.SayHello(name);
                    Console.WriteLine($"... Response: {res}");
                    Console.WriteLine("");
                    
                    Thread.Sleep (10);
                }
                
                String name = "Enno";
                Console.WriteLine($"Calling SayHello for {name}");
                var res = channel.SayHello(name);
                Console.WriteLine($"... Response: {res}");
                Console.WriteLine("");
            }
        }    
        */
    
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

    public class ClientClass
    {
        [ServiceContract()]
        public interface INodeService {
            [OperationContract(IsOneWay=true)]
            [WebGet()]
            void Message(int from, int to, int tok, int pay);
            
            [OperationContract()] 
            [WebGet(ResponseFormat=WebMessageFormat.Json)]
            string SayHello(string name);
        }
        
        static public void ClientTo(int NodeNum)
        {

            String url = "http://localhost:"+Program.map[NodeNum.ToString()].ToString()+"/hello";
            
            var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(url));
            
            var channel = myChannelFactory.CreateChannel();
    
            /*
            var names = Environment.GetCommandLineArgs();
            
            foreach (var name in names ) {
                

                Console.WriteLine($"Calling SayHello for {name}");
                var res = channel.SayHello(name);
                Console.WriteLine($"... Response: {res}");
                Console.WriteLine("");
                
                Thread.Sleep (10);
            }
            */
                
            String name = "Enno";
            Console.WriteLine($"Calling SayHello for {name}");
            var res = channel.SayHello(name);
            Console.WriteLine($"... Response: {res}");
            Console.WriteLine("");
            
            
            Console.WriteLine($"This is Node {Program.ThisNode} sending message to {NodeNum}");
            channel.Message(Program.ThisNode, NodeNum, 1,1);
            
        }

        static public void sayHello()
        {
            Console.WriteLine("Hellooooo!!!!");
        }

    }
    
}