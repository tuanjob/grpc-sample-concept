using NXTK.GrpcServer.Models;
using System.Threading.Tasks;

namespace NXTK.GrpcServer.Interfaces
{
    public interface IOutrightIncDataServiceInvoker
    {
        Task InvokeSendIncrementalData(HDPOUIncOdds incData);
    }
}
