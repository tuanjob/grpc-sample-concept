using System;
using System.Collections.Generic;

namespace NXTK.GrpcServer.Models
{


    [Serializable]
    public class FullOdds
    {
        public List<OddsDiff> OddsDiffs { get; set; }

        public Dictionary<string, SportClass> Sports { get; set; }

        public Dictionary<int, LeagueClass> Leagues { get; set; }

        public Dictionary<string, MatchGroupClass> MatchGroups { get; set; }

        public Dictionary<int, TipClass> LeagueTips { get; set; }

        public object Matches { get; set; }

        public bool IsFinish { get; set; }

        public int Sequence { get; set; }

        public int MatchId { get; set; }

    }

    [Serializable]
    public class OutrightClass
    {
        public int MatchId { get; set; }

        public bool IsHide { get; set; }

        public bool IsPause { get; set; }

        public int StockType { get; set; }

        public string MatchGroupId { get; set; }

        public decimal Odds { get; set; }

        public int ValidTime { get; set; }

        public int TicketCount { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }
    }

    [Serializable]
    public class TipClass
    {
        public int LeagueTipId { get; set; }

        // public Dictionary<Language, string> LeagueTips { get; set; }
    }

    [Serializable]
    public class MatchGroupClass
    {
        public string GameType { get; set; }

        public int LeagueId { get; set; }

        public string MatchGroupId { get; set; }

        public int RunHomeScore { get; set; }

        public int RunAwayScore { get; set; }

        //public Dictionary<Language, string> HomeTeam { get; set; }
        //public Dictionary<Language, string> AwayTeam { get; set; }

        public DateTime MatchDate { get; set; }

        public int Period { get; set; }

        public bool IsDanger { get; set; }

        public DateTime StartTime { get; set; }

        public int RCHome { get; set; }

        public int RCAway { get; set; }

        public int MatchOrder { get; set; }

        public string StatsURL { get; set; }

        public int OT { get; set; }

        public bool IsET { get; set; }

        public string TVLink { get; set; }

        public int TeamInfo { get; set; }

        public MarketType MarketType { get; set; }

        // public LeagueType LeagueType { get; set; }

        public string WorkingDate { get; set; }

        public string LiveDisplay { get; set; }

        public int SpecialOrder { get; set; }

        public string SmartId { get; set; }

        public bool IsTradeIn { get; set; }

        //
        // Summary:
        //     Trade In ticket赢的百分比
        public int WinningPercent { get; set; }

        //
        // Summary:
        //     Trade In ticket 输的百分比
        public int LosePercent { get; set; }

        //
        // Summary:
        //     是否为SpecialMatch
        public bool IsSpecialMatch { get; set; }

        public bool IsDelay { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public string SpecialId { get; set; }

        public bool IsLiveStatistics { get; set; }

        public bool IsLiveCenter { get; set; }

        public bool IsMatchTracker { get; set; }

        // More
    }

    [Serializable]
    public class LeagueClass
    {
        public int LeagueId { get; set; }

        public int LeagueTipId { get; set; }

        public int RunningLeagueOrder { get; set; }

        public int TodayLeagueOrder { get; set; }

        public int EarlyLeagueOrder { get; set; }

        // public Dictionary<Language, string> LeagueCode { get; set; }


        // More
    }

    [Serializable]
    public class SportClass
    {
        public int SportOrder { get; set; }

        public string SportType { get; set; }

        // public Dictionary<Language, string> SportDesc { get; set; }


        // More
    }

    [Serializable]
    public class OddsDiff
    {
        public int MatchId { get; set; }

        public string Brand { get; set; }


        // TODO
    }
}
