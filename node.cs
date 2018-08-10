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

    public void Message(int from, int to, int tok, int pay)
    {
        if (Node.Visited == false)
        {
            Console.WriteLine($"... {Millis():F2} {Node.ThisNode} < {from} {to} {tok} {pay}");
            Node.Parent = from;
            if (from != 0)
            {
                Node.rec++;
            }
            Node.Visited = true;
            foreach (object neigh in Node.Adj)
            {
                if (!Node.Parent.Equals(Convert.ToInt32(neigh)))
                {
                    sendMessageTo(Convert.ToInt32(neigh), 1, Node.payload);
                }
            }
            if (Node.Adj.Count == 1) {
                sendToParent(Node.payload);
            }
        }
        else {
            Console.WriteLine($"... {Millis():F2} {Node.ThisNode} < {from} {to} {tok} {pay}");

            Node.rec++;
            if (tok == 2)
            {
                Node.payload = pay+Node.payload;
            }
            if (Node.rec == Node.Adj.Count) {
                sendToParent(Node.payload + 1);
            }
        }
    }

    public static void sendMessageTo(int to, int tok, int pay) {
        WebChannelFactory<INodeService> wcf = null;
        OperationContextScope scope = null;
        try
        {
            String arcUrl = "http://localhost:" + Node.map["0"].ToString();
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
            String arcUrl = "http://localhost:" + Node.map["0"].ToString();
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
        String path = inputList[0].ToString();
        inputList.RemoveAt(0);


        //Step 1: Config.txt -> Map
        int counter = 0;
        string line;
        map = new Dictionary<string, int>();

        System.IO.StreamReader file = new System.IO.StreamReader(path);

        while ((line = file.ReadLine()) != null)
        {
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
        Parent = -1;

        if (inputList.Count < 2)
        {
            Console.WriteLine("Wrong Input!!!");
        }
        else
        {
            ThisNode = Convert.ToInt32(inputList[0]);

            Adj = new ArrayList();

            for (int i = 1; i < inputList.Count; i++)
            {
                Adj.Add(inputList[i]);
            }
        }
        //Step 3: Create Host

        try
        {
            String url = "http://localhost:" + map[ThisNode.ToString()];
            var baseAddress = new Uri(url);
            host = new WebServiceHost (typeof(NodeService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint (typeof(INodeService), new WebHttpBinding(), "");

            host.Open();
            Console.WriteLine($"The service is ready at {baseAddress}/");
            Console.Error.WriteLine ("Press <Enter> to stop the service.");
            Console.ReadLine ();
           
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

