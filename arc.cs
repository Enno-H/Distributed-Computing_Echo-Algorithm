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
        Console.WriteLine($"... {Millis():F2} {Node.ThisNode} < {from} {to} {tok} {pay}");

        if (to != 0)
        {
            deliverMessageTo(from, to, tok, pay);
        }

        if (to == 0)
        {
            Console.WriteLine("Finish!!!");
        }
    }

    public static void sendMessageTo(int to, int tok, int pay) {
        WebChannelFactory<INodeService> wcf = null;
        OperationContextScope scope = null;
        try
        {
            String arcUrl = "http://localhost:" + Node.map[to.ToString()].ToString() + "/hello";
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

    static public void deliverMessageTo(int from, int to, int tok, int pay)
    {
        String key = from.ToString()+"-"+to.ToString();
        int delayTime = Node.map["-"];
        if (Node.map.ContainsKey(key)) {
            delayTime = Node.map[key];
        }
        //Console.WriteLine($"the delay of {from} to {to} is {delayTime}");

        Thread.Sleep(delayTime);

        WebChannelFactory<INodeService> wcf = null;
        OperationContextScope scope = null;
        try
        {
            String arcUrl = "http://localhost:" + Node.map[to.ToString()].ToString() + "/hello";
            var myChannelFactory = new WebChannelFactory<INodeService>(new Uri(arcUrl));
            var channel = myChannelFactory.CreateChannel();

            Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;

            scope = new OperationContextScope((IContextChannel)channel);
            Console.WriteLine($"... {Millis():F2} {Node.ThisNode} > {from} {to} {tok} {pay}");
            channel.Message(from, to, tok, pay);
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
        String path = inputList[1].ToString();
        //Console.WriteLine($"Path is :" + path);

        //Step 1: Config.txt -> Map
        //Input 1
        int counter = 0;
        string line;
        map = new Dictionary<string, int>();

        System.IO.StreamReader file = new System.IO.StreamReader(path);

        while ((line = file.ReadLine()) != null)
        {
            //System.Console.WriteLine (line);  
            counter++;
            if (!line.StartsWith("//"))
            {
                var bit = System.Text.RegularExpressions.Regex.Split(line, @"\s{1,}");
                if (bit.Length >= 2)
                {
                    map.Add(bit[0], int.Parse(bit[1]));
                }
            }
        }
        file.Close();


        //Step 3: Create Host
        Console.WriteLine($"Port: {map["0"]}");
        String url = "http://localhost:" + map["0"] + "/hello";
        Console.WriteLine(url);
        Uri baseAddress = new Uri(url);

        try
        {

            host = new WebServiceHost(typeof(NodeService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INodeService), new WebHttpBinding(), "");

            host.Open();
            NodeService.sendMessageTo(1, 1, 0);

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

