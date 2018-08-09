using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;



[ServiceBehavior(IncludeExceptionDetailInFaults=true,
    InstanceContextMode=InstanceContextMode.PerCall,
    ConcurrencyMode=ConcurrencyMode.Single)]
public class NodeService : INodeService {
    public static AutoResetEvent Done = new AutoResetEvent (false);
    
    public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;
    
    public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;

    //收信反应
    public void Message(int from, int to, int tok, int pay)
    {
        if (Node.Open == true)
        {
            Console.WriteLine($"... {Millis():F2} {Node.ThisNode} < {from} {to} {tok} {pay}");
            Node.payload = Node.payload + pay;

            if (Node.Visited == false)
            {
                Node.Parent = from;
                //Node.Visited = true;
            }

            Node.rec++;
            if (from == 0)
            {
                Node.rec = Node.rec - 1;
            }
            Console.WriteLine("rec = " + Node.rec);

            if (Node.rec >= Node.Adj.Count)
            {
                Node.Open = false;
                Console.WriteLine($"Node {Node.ThisNode} is closed");
                sendToParent(Node.payload + 1);
            }

            if (tok == 1 && Node.Visited == false)
            {
                foreach (object neigh in Node.Adj)
                {
                    if (!Node.Parent.Equals(Convert.ToInt32(neigh)))
                    {
                        //Console.WriteLine($"... {Millis():F2} {Node.ThisNode} > {Node.ThisNode} {neigh} {tok} {pay}");
                        //Console.WriteLine($"{Node.Open}");
                        sendMessageTo(Convert.ToInt32(neigh), 1,Node.payload);
                    }
                }
            }

            Node.Visited = true;
        }
    }

    public static void sendMessageTo(int to, int tok, int pay) {
        WebChannelFactory<INodeService> wcf = null;
        OperationContextScope scope = null;
        try
        {
            String arcUrl = "http://localhost:" + Node.map["0"].ToString() + "/hello";
            var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(arcUrl));
            var channel = myChannelFactory.CreateChannel();

            scope = new OperationContextScope((IContextChannel)channel);
            Console.WriteLine($"... {Millis():F2} {Node.ThisNode} > {Node.ThisNode} {to} {tok} {pay}");
            channel.Message(Node.ThisNode, to, tok, pay);
        }
        catch (Exception ex)
        {
            var msg = ($"*** Exception {ex.Message}");
            Console.Error.WriteLine(msg);
            Console.WriteLine(msg);
            wcf = null;
            scope = null;

        }
        finally
        {
            if (wcf != null) ((IDisposable)wcf).Dispose();
            if (scope != null) ((IDisposable)scope).Dispose();
        }

    }

    public static void sendToParent(int pay)
    {
        WebChannelFactory<INodeService> wcf = null;
        OperationContextScope scope = null;
        try
        {
            String arcUrl = "http://localhost:" + Node.map["0"].ToString() + "/hello";
            var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(arcUrl));
            var channel = myChannelFactory.CreateChannel();

            scope = new OperationContextScope((IContextChannel)channel);
            Console.WriteLine($"... {Millis():F2} {Node.ThisNode} > {Node.ThisNode} {Node.Parent} 2 {pay}");
            channel.Message(Node.ThisNode, Node.Parent, 2, pay);
        }
        catch (Exception ex)
        {
            var msg = ($"*** Exception {ex.Message}");
            Console.Error.WriteLine(msg);
            Console.WriteLine(msg);
            wcf = null;
            scope = null;

        }
        finally
        {
            if (wcf != null) ((IDisposable)wcf).Dispose();
            if (scope != null) ((IDisposable)scope).Dispose();
        }

    }
}

public class Node {

    static public int ThisNode;
    static public ArrayList Adj;
    static public Boolean Visited = false;
    static public Boolean Open = true;
    static public int Parent;
    static public int rec = 0;
    static public int payload = 0;

    static public Dictionary<string, int> map;


    public static void Main (string[] args) {
        WebServiceHost host = null;


        //Step 0: Get input
        ArrayList inputList = new ArrayList();
        var inputs = Environment.GetCommandLineArgs();
        foreach (var input in inputs)
        {
            inputList.Add(input);
        }
        inputList.RemoveAt(0);
        inputList.RemoveAt(0);


        //Step 1: Config.txt -> Map
        //Input 1
        int counter = 0;
        string line;
        map = new Dictionary<string, int>();

        System.IO.StreamReader file = new System.IO.StreamReader("Config4.txt");

        while ((line = file.ReadLine()) != null)
        {
            //System.Console.WriteLine (line);  
            counter++;
            if (!line.StartsWith("//"))
            {
                var bit = System.Text.RegularExpressions.Regex.Split(line, @"\s{1,}");
                if (bit.Length >= 2){
                    map.Add(bit[0], int.Parse(bit[1]));
                }
            }
        }
        file.Close();


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
        Console.WriteLine($"Id: {ThisNode}, port: {map[ThisNode.ToString()]}");
        Console.WriteLine($"Neighbours Num:{Adj.Count}, they are:");
        foreach (object neigh in Node.Adj)
        {
            Console.WriteLine($"--{neigh}");
        }


        //Step 3: Create Host

        try
        {
            String url = "http://localhost:" + map[ThisNode.ToString()] + "/hello";
            var baseAddress = new Uri(url);
            host = new WebServiceHost (typeof(NodeService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint (typeof(INodeService), new WebHttpBinding(), "");

            host.Open();

            var msg = ($"Ping: {baseAddress}Ping?ttl=?");
            //Console.Error.WriteLine (msg);
            //Console.WriteLine (msg);

            //NodeService.sendMessageTo(2);
                        
            Console.Error.WriteLine ("Press <Enter> to stop the service.");
            Console.ReadLine ();
            //PingService.Done.WaitOne ();
            //NodeService.Done.WaitOne();

            host.Close ();
        
        } catch (Exception ex) {
            var msg = ($"*** Exception {ex.Message}");
            Console.Error.WriteLine (msg);
            Console.WriteLine (msg);
            host = null;
        
        } finally {
            if (host != null) ((IDisposable)host).Dispose();
        }
    }
}

