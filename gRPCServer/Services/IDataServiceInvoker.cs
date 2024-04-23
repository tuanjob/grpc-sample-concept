using gRPCSample.Core.Models;

namespace gRPCSampleServer.Services
{
    public interface IDataServiceInvoker
    {
        Task InvokeSendIncrementalData(string clientId, JsonIncModel incData);
        Task InvokeSendIncrementalData(JsonIncModel incData);
    }

}
