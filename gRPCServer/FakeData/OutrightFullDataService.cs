using gRPCSample.Core.Models;
using System.Collections.Concurrent;

namespace gRPCSampleServer.FakeData
{

    public class OutrightFullDataService : IOutrightFullDataService
    {
        private ConcurrentDictionary<int, FullOdds> outrightFullOdds = new ConcurrentDictionary<int, FullOdds>();
        public OutrightFullDataService()
        {
            AddNewData(0);
        }

        public int MaxFullMatchId()
        {
            var keys = outrightFullOdds.Keys;

            return keys.Any() ? keys.Max() : 0;
        }

        public List<FullOdds> GetAll()
        {
            return outrightFullOdds.Values.ToList();
        }

        public int AddNew()
        {
            var maxId = outrightFullOdds.Keys.Max();
            AddNewData(maxId + 1);

            return maxId + 1;
        }
        public void Update(int matchId)
        {
            Console.WriteLine($"MatchId {matchId} has been updated in FULL");
        }

        public void Delete(int matchId)
        {
            outrightFullOdds.TryRemove(matchId, out _);
            Console.WriteLine($"MatchId {matchId} has been deleted in FULL");
        }


        private void AddNewData(int matchId)
        {
            matchId = matchId == 0 ? 1 : matchId;

            // first record
            var first = new FullOdds
            {
                MatchId = matchId,
                Sports = new Dictionary<string, SportClass>()
                {
                    {
                        "S",
                        new SportClass
                        {
                            SportOrder = matchId,
                            SportType = "S",
                            //SportDesc = new Dictionary<DataStruct.MatchEntities.Entity.Language, string>
                            //{
                            //    {
                            //        DataStruct.MatchEntities.Entity.Language.en_US, "EN"
                            //    },
                            //    {
                            //        DataStruct.MatchEntities.Entity.Language.zh_CN, "CN"
                            //    }
                            //}
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
                            LeagueTipId = 279,
                            //LeagueCode = new Dictionary<DataStruct.MatchEntities.Entity.Language, string>
                            //{
                            //    {
                            //        DataStruct.MatchEntities.Entity.Language.en_US, "EN"
                            //    },
                            //    {
                            //        DataStruct.MatchEntities.Entity.Language.zh_CN, "CN"
                            //    }
                            //}
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
                                //HomeTeam = new Dictionary<DataStruct.MatchEntities.Entity.Language, string>
                                //{
                                //    {
                                //        DataStruct.MatchEntities.Entity.Language.en_US, "EN"
                                //    },
                                //    {
                                //        DataStruct.MatchEntities.Entity.Language.zh_CN, "CN"
                                //    }
                                //},
                                //AwayTeam = new Dictionary<DataStruct.MatchEntities.Entity.Language, string>
                                //{
                                //    {
                                //        DataStruct.MatchEntities.Entity.Language.en_US, "EN"
                                //    },
                                //    {
                                //        DataStruct.MatchEntities.Entity.Language.zh_CN, "CN"
                                //    }
                                //},
                                WorkingDate = "20240412",
                                // LeagueType = DataStruct.MatchEntities.Entity.LeagueType.Outright,
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
                        LeagueTipId = 279,
                        //LeagueTips =  new Dictionary<DataStruct.MatchEntities.Entity.Language, string>
                        //{
                        //    {
                        //        DataStruct.MatchEntities.Entity.Language.en_US, "EN"
                        //    },
                        //    {
                        //        DataStruct.MatchEntities.Entity.Language.zh_CN, "CN"
                        //    }
                        //}
                    }
                },
                {
                    2,
                    new TipClass
                    {
                        LeagueTipId = 287,
                        //LeagueTips =  new Dictionary<DataStruct.MatchEntities.Entity.Language, string>
                        //{
                        //    {
                        //        DataStruct.MatchEntities.Entity.Language.en_US, "EN"
                        //    },
                        //    {
                        //        DataStruct.MatchEntities.Entity.Language.zh_CN, "CN"
                        //    }
                        //}
                    }
                }
                }
            };

            outrightFullOdds[matchId] = first;
        }


    }
}
