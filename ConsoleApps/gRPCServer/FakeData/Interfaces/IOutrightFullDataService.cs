using gRPCSample.Core.Models;

namespace gRPCSampleServer.FakeData
{
    public interface IOutrightFullDataService
    {
        List<FullOdds> GetAll();
        int AddNew();
        void Update(int matchId);
        void Delete(int matchId);
        int MaxFullMatchId();
    }
}
