using gRPCSample.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace gRPCSampleServer.FakeData
{

    public class OutrightIncMemoryData : IOutrightIncDataService
    {
        private ConcurrentDictionary<string, HDPOUIncOdds> outrightIncOdds = new ConcurrentDictionary<string, HDPOUIncOdds>();
        public OutrightIncMemoryData()
        {
            
        }

        public int MaxIncMatchKey()
        {
            return outrightIncOdds.Keys.Count();
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

            outrightIncOdds[$"{matchId}:{mode.ToString()}"] = data;

            return data;
        }
    }
}
