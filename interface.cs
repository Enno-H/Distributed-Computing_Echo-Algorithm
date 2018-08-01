using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

[ServiceContract()]
public interface INodeService {
    [OperationContract(IsOneWay=true)]
    [WebGet()]
    void Message(int from, int to, int tok, int pay);
}
