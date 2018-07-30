using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;

// How to: Create a Basic WCF Web HTTP Service
// https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-create-a-basic-wcf-web-http-service

// How to: Expose a Contract to SOAP and Web Clients
// https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-expose-a-contract-to-soap-and-web-clients




[ServiceContract()]
public interface INodeService
{
    [OperationContract(IsOneWay = true)]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
    void Message(int from, int to, int tok, int pay);

    [OperationContract()]
    [WebInvoke]
    string SayHello(string name);
}

[ServiceBehavior(
    InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
public class NodeService : INodeService
{
    public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;

    public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;


    //收信反应
    public void Message(int from, int to, int tok, int pay)
    {
        Console.WriteLine($"... {Millis():F2} {Node.ThisNode} < {from} {to} {tok} {pay}");
        Console.WriteLine($"*Receive: Node {Node.ThisNode} ({to}) has received message from {from}");
        if (to == Node.ThisNode && Node.Open == true)
        {
            //选parents
            if (Node.Visited == false)
            {
                Node.Parent = from;
                Node.Visited = true;
            }

            Node.rec++;

            if (Node.rec >= Node.Adj.Count)
            {
                Node.Open = false;
            }

            /*
            if (Node.ThisNode == 2){
                Console.WriteLine("This is Node 2 so it will send message to 3 directly");
                ClientClass.directSendMessageTo(3);
               

            }
            */
            

            foreach (object neigh in Node.Adj)
            {
                Console.WriteLine("hhhh");
                if (neigh != Node.Parent)
                {
                    ClientClass.sendMessageTo(neigh);
                    Console.WriteLine($"{Node.ThisNode} is sending message to its neigh: {neigh}");
                }
            }
        }
    }

    public string SayHello(string name)
    {
        Console.WriteLine($"[{Tid()}] [{Millis()}] {Node.ThisNode} SayHello received: {name}");

        if (Node.ThisNode == 2)
        {
            Console.WriteLine("This is Node 2 so it will say hello to 3");
            ClientClass.sayTo(3);
        }

        return $"{Node.ThisNode} Hello, {name}";
    }
}

public class Node
{
    static public int ThisNode;
    static public ArrayList Adj;
    static public Boolean Visited = false;
    static public Boolean Open = true;
    static public int Parent;
    static public int rec = 0;

    static public Dictionary<string, int> map;

    public static void Main(string[] args)
    {

        //Step 0: Get input
        ArrayList inputList = new ArrayList();
        var inputs = Environment.GetCommandLineArgs();
        foreach (var input in inputs){
            //Console.WriteLine(input);
            inputList.Add(input);
        }
        inputList.RemoveAt(0);
        inputList.RemoveAt(0);

        Console.WriteLine("The inputList length is " + inputList.Count+" and the first is "+inputList[0]);


        //Step 1: Config.txt -> Map
        //Input 1
        int counter = 0;
        string line;
        map = new Dictionary<string, int>();

        System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Enno\RiderProjects\Learn\Learn\Config.txt");
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

            host = new WebServiceHost(typeof(NodeService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INodeService), new WebHttpBinding(), "");

            host.Open();

            //ClientClass.ClientTo();
            //ClientClass.sendMessageTo(2);
            //ClientClass.sendMessageTo(3);

            if (Node.ThisNode == 1) {

                Console.WriteLine("!!!This is Node 1 so it will send message to Node 2");
                ClientClass.directSendMessageTo(2);
                //ClientClass.sayTo(2);
            }

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
    public interface INodeService
    {
        [OperationContract(IsOneWay = true)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        void Message(int from, int to, int tok, int pay);

        [OperationContract()]
        [WebInvoke]
        string SayHello(string name);
    }

    
    static public void sendMessageTo(int NodeNum)
    {
        String arcUrl = "http://localhost:" + Node.map["0"].ToString() + "/hello";

        Console.WriteLine(arcUrl);

        var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(arcUrl));

        var channel = myChannelFactory.CreateChannel();

        Console.WriteLine($"Node {Node.ThisNode} is sending message to Node {NodeNum} ");

        channel.Message(Node.ThisNode, NodeNum, 1, 1);

    }

    static public void directSendMessageTo(int NodeNum)
    {
        String Url = "http://localhost:" + Node.map[NodeNum.ToString()].ToString() + "/hello";

        //Console.WriteLine(Url);

        var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(Url));

        var channel = myChannelFactory.CreateChannel();

        Console.WriteLine($"&&Send Begin: Node {Node.ThisNode} is directly sending message to Node {NodeNum} +{Url}");

        channel.Message(Node.ThisNode, NodeNum, 1, 1);

        Console.WriteLine("&&Send Over");

    }

    static public void sayTo(int NodeNum) {
        String Url = "http://localhost:" + Node.map[NodeNum.ToString()].ToString() + "/hello";

        var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(Url));

        var channel = myChannelFactory.CreateChannel();

        Console.WriteLine($"&&Send Begin: Node {Node.ThisNode} says Hello to Node {NodeNum}");

        channel.SayHello("Enno");

        Console.WriteLine("&&Send Over");
    }
}
