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
    [ServiceContract()]
    public interface INodeService
    {
        [OperationContract(IsOneWay = true)]
        [WebGet()]
        void Message(int from, int to, int tok, int pay);

        [OperationContract()]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string SayHello(string name);
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class HelloWorldService : INodeService
    {
        public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;

        public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;


        public void Message(int from, int to, int tok, int pay)
        {
            Console.WriteLine($"... {Millis():F2} {Node.ThisNode} < {from} {to} {tok} {pay}");
            if (to == Node.ThisNode)
            {
                if (Node.Visited == false && Node.Parent != 0)
                {
                    Node.Parent = from;
                }


            }
        }

        public string SayHello(string name)
        {
            Console.WriteLine($"[{Tid()}] [{Millis()}] {Node.ThisNode} received: {name}");
            return $"{Node.ThisNode}: Hello, {name}";
        }
    }
    
    
    public class Node
    {
        static public int ThisNode;
        static public ArrayList Adj;
        static public Boolean Visited;
        static public int Parent;
    
        static public Dictionary<string, int> map;

        public static void Main(string[] args)
        {
    
            //Step 0: Get input
            
            /*
            ArrayList inputList = new ArrayList();
            var inputs = Environment.GetCommandLineArgs();
            foreach (var input in inputs){
                //Console.WriteLine(input);
                inputList.Add(input);
            }
            inputList.RemoveAt(0);
            inputList.RemoveAt(0);
    
            Console.WriteLine("The inputList length is " + inputList.Count+" and the first is "+inputList[0]);
            */
            
            ArrayList inputList = new ArrayList();
            inputList.Add(1);
            inputList.Add(2);
            inputList.Add(3);
            inputList.Add(4);
               
    
    
    
           
    
    
            //Step 1: Config.txt -> Map
            //Input 1
            int counter = 0;
            string line;
            map = new Dictionary<string, int>();
    
            System.IO.StreamReader file = new System.IO.StreamReader(@"/Users/huangxiaolong/RiderProjects/learn/learn/config.txt");
            while ((line = file.ReadLine()) != null)
            {
                //System.Console.WriteLine (line);  
                counter++;
                if (!line.StartsWith("//"))
                {
                    string[] bit = line.Split(' ');
                    map.Add(bit[0], int.Parse(bit[1]));
                }
            }
    
            file.Close();
            Console.WriteLine("There are {0} lines in Config.txt", counter);
            Console.WriteLine("There are "+map.Count+" real data in the Config file");
    
    
            //Step 2: ArrayList -> Node 
            //Input 2
            Parent = -1;
           
            if (inputList.Count < 2)
            {
                Console.WriteLine("Wrong Input!!!");
            }
            else
            {
                //ThisNode = (int)inputList[0];
                ThisNode = Convert.ToInt32(inputList[0]);
    
                Adj = new ArrayList();
    
                for (int i = 1; i < inputList.Count; i++)
                {
                    Adj.Add(inputList[i]);
                }
            }
    
            Console.WriteLine("This Node is: " + ThisNode);
            Console.WriteLine("It has Adj: " + Adj.Count + ", the first is " + Adj[0]);
    
    
    
    
    
    
    
    
    
    
    
            //Step 3: Create Host
            Console.WriteLine($"Id: {ThisNode}, value: {map[ThisNode.ToString()]}");
            String url = "http://localhost:"+map[ThisNode.ToString()] + "/hello";
            Console.WriteLine(url);
            Uri baseAddress = new Uri(url);
    
            WebServiceHost host = null;
    
            try
            {
    
                host = new WebServiceHost(typeof(HelloWorldService), baseAddress);
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INodeService), new WebHttpBinding(), "");
    
                host.Open();
    
                //ClientClass.ClientTo();
    
    
                Console.WriteLine($"The service is ready at {baseAddress}/Hello?name=xyz or /SayHello?name=xyz");
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();
    
    
    
    
                // Close the ServiceHost.
                host.Close();
    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** Exception {ex.Message}");
                host = null;
    
            }
            finally
            {
                if (host != null) ((IDisposable)host).Dispose();
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
        
        static public void Client()
        {
                
                var myChannelFactory = new WebChannelFactory<INodeService>(new Uri("http://localhost:8082/hello"));
         
     
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
            
        }

        static public void sayHello()
        {
            Console.WriteLine("Hellooooo!!!!");
        }

    }
    
}