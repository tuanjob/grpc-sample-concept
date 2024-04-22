namespace gRPCSampleServer.Services
{
    public interface IDataServiceInvoker
    {
        Task InvokeGetIncrementalData1(string clientId, string incData);
        Task InvokeGetIncrementalData1( string incData);
    }

}
