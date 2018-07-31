using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;

[ServiceContract()]
public interface INodeService
{
    [OperationContract(IsOneWay = true)]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
    void Message(int from, int to, int tok, int pay);
}



[ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
public class NodeService : INodeService
{
    public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;

    public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;


    public void Message(int from, int to, int tok, int pay)
    {
        Console.WriteLine($"... {Millis():F2} {Node.ThisNode} < {from} {to} {tok} {pay}");

        if (to != 0) {
            ClientClass.deliverMessageTo(from, to, tok, pay);
        }

        if (to == 0) {
            Console.WriteLine("Finish!!!");
        }
        

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
        foreach (var input in inputs)
        {
            //Console.WriteLine(input);
            inputList.Add(input);
        }

        Console.WriteLine("The inputList length is " + inputList.Count + " and the first is " + inputList[0]);







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
        Console.WriteLine("There are " + map.Count + " real data in the Config file");














        //Step 3: Create Host
        Console.WriteLine($"Port: {map["0"]}");
        String url = "http://localhost:" + map["0"] + "/hello";
        Console.WriteLine(url);
        Uri baseAddress = new Uri(url);

        WebServiceHost host = null;

        try
        {

            host = new WebServiceHost(typeof(NodeService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INodeService), new WebHttpBinding(), "");

            host.Open();
            ClientClass.sendMessageTo(1);

            Console.WriteLine($"The service is ready at {baseAddress}/");
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
    }

    static public void deliverMessageTo(int from, int to, int tok, int pay)
    {
        Thread.Sleep(10);

        String arcUrl = "http://localhost:" + Node.map[to.ToString()].ToString() + "/hello";

        var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(arcUrl));

        var channel = myChannelFactory.CreateChannel();

        Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;

        Console.WriteLine($"... {Millis():F2} {Node.ThisNode} > {from} {to} {tok} {pay}");

        channel.Message(from, to, 1, 1);

    }

    static public void sendMessageTo(int NodeNum)
    {
        String arcUrl = "http://localhost:" + Node.map[NodeNum.ToString()].ToString() + "/hello";

        var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(arcUrl));

        var channel = myChannelFactory.CreateChannel();

        Console.WriteLine($"*****Node {Node.ThisNode} is sending message to Node {NodeNum} ");

        channel.Message(Node.ThisNode, NodeNum, 1, 1);

    }
}
