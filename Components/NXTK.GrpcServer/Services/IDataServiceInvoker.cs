using NXTK.GrpcServer.Models;
using System.Threading.Tasks;

namespace NXTK.GrpcServer.Services
{
    public interface IDataServiceInvoker
    {
        Task InvokeSendIncrementalData(HDPOUIncOdds incData);
    }
}
