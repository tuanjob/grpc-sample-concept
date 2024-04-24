using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;

namespace gRPCSampleServer.FakeData
{
    public class OutrightIncJsonDataService : IOutrightIncDataService
    {
        private readonly JsonRepository _repository;
        public OutrightIncJsonDataService()
        {
            _repository = new JsonRepository("JsonData\\OutrightInc.json");
        }

        public HDPOUIncOdds AddNew(int matchId, ModType mode, MarketType marketType, OddsCommand oddsCommand)
        {
            var data = new HDPOUIncOdds
            {
                FTSocOddsId = matchId,
                MarketType = marketType,
                OddsType = mode,
                ParamName = oddsCommand,//OddsCommand.AwayTeam,
                Data = new IncAddArgs
                {
                    BetViewOddsList = new List<BetViewOdds>
                        {
                            new BetViewOdds
                            {
                                FTSocOddsId = matchId,
                                LeagueId = 16903,
                                AwayId = 8276,
                                HomeId = 12879
                            }
                        },
                    AddedByAutoMo = true,
                },
            };

            var incAll = _repository.Get<List<HDPOUIncOdds>>();
            incAll.Add(data);

            _repository.Set<List<HDPOUIncOdds>>(incAll);


            return data;
        }

        public int MaxIncMatchKey()
        {
            return _repository.Get<List<HDPOUIncOdds>>().Select(r => r.FTSocOddsId).Max();
        }
    }
}
