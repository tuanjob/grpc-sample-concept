using gRPCSample.Core.Helpers;
using gRPCSample.Core.Models;

namespace gRPCSampleServer.FakeData
{
    public class OutrightFullJsonData : IOutrightFullDataService
    {
        private readonly JsonRepository _repository;
        public OutrightFullJsonData()
        {
            _repository = new JsonRepository("JsonData\\OutrightFull.json");
        }


        public int AddNew()
        {
            var matchId = MaxFullMatchId() + 1;

            // first record
            var data = new FullOdds
            {
                MatchId = matchId,
                Sports = new Dictionary<string, SportClass>()
                {
                    {
                        "S",
                        new SportClass
                        {
                            SportOrder = matchId,
                            SportType = "S"
                        }
                    },
                    {
                        "BB",
                        new SportClass
                        {
                            SportOrder = matchId,
                            SportType = "BB",
                        }
                    }
                },
                Leagues = new Dictionary<int, LeagueClass>()
                {
                    {
                        1,
                        new LeagueClass
                        {
                            LeagueId = 19676,
                            LeagueTipId = 279
                        }
                    },
                    {
                        2,
                        new LeagueClass
                        {
                            LeagueId = 18185,
                            LeagueTipId = 287
                        }
                    }
                },
                MatchGroups = new Dictionary<string, MatchGroupClass>()
                {
                    {
                            "G1",
                            new MatchGroupClass
                            {
                                MatchGroupId = "G1",
                                LeagueId = 1,
                                AwayId = 8275,
                                HomeId = 1803,
                                GameType = "S",
                                MarketType = MarketType.Today,
                                WorkingDate = "20240412",
                                MatchDate = DateTime.Now.AddDays(1),
                            }
                        },
                        {
                            "G2",
                            new MatchGroupClass
                            {
                                MatchGroupId = "G2",
                                LeagueId = 2,
                                AwayId = 1810,
                                HomeId = 14271,
                                GameType = "S",
                                MarketType = MarketType.Today,
                                WorkingDate = "20240412",
                                MatchDate = DateTime.Now.AddDays(1),
                            }
                        }
                },
                Matches = new List<OutrightClass>()
                {
                    {
                            new OutrightClass()
                            {
                                MatchId = matchId,
                                IsHide = false,
                                IsPause = false,
                                StockType = 1,
                                MatchGroupId = "2",
                                Odds = 1L,
                                TicketCount = 11,
                                ValidTime = 145225
                            }
                        }
                },
                LeagueTips = new Dictionary<int, TipClass>()
                {
                    {
                    1,
                    new TipClass
                    {
                        LeagueTipId = 279
                    }
                },
                {
                    2,
                    new TipClass
                    {
                        LeagueTipId = 287
                    }
                }
                }
            };

            var fullAll = _repository.Get<List<FullOdds>>();
            fullAll.Add(data);
            _repository.Set<List<FullOdds>>(fullAll);

            return matchId;
        }

        public void Delete(int matchId)
        {
            var matches = _repository.Get<List<FullOdds>>().Where(r => r.MatchId != matchId).ToList();
            _repository.Set<List<FullOdds>>(matches);
        }

        public List<FullOdds> GetAll()
        {
            return _repository.Get<List<FullOdds>>();
        }

        public int MaxFullMatchId()
        {
            return _repository.Get<List<FullOdds>>().Select(r => r.MatchId).Max();
        }

        public void Update(int matchId)
        {
            Console.WriteLine($"MatchId {matchId} has been updated in FULL");
        }
    }
}
