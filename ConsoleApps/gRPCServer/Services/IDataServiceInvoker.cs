using gRPCSample.Core.Models;
using gRPCSampleServer.Models;

namespace gRPCSampleServer.Services
{
    public interface IDataServiceInvoker
    {
        Task InvokeSendIncrementalData(string clientId, HDPOUIncOdds incData);
        Task InvokeSendIncrementalData(HDPOUIncOdds incData);
    }

}
