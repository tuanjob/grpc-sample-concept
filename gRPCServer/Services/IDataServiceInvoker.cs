using DataServicePackage;

namespace gRPCSampleServer.Services
{
    public interface IDataServiceInvoker
    {
        //Task InvokeGetIncrementalData(string incData);

        //void EnqueueUpdate(DataResponse updateMessage);

        Task InvokeGetIncrementalData1(string incData);
    }

}
