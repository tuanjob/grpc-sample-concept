using gRPCSample.Core.Models;

namespace gRPCSampleServer.FakeData
{
    public interface IOutrightIncDataService
    {
        HDPOUIncOdds AddNew(int matchId, ModType mode, MarketType marketType, OddsCommand oddsCommand);
        int MaxIncMatchKey();
    }
}
