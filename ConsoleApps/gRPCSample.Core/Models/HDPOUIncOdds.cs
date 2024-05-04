namespace gRPCSample.Core.Models
{
    [Serializable]
    public class HDPOUIncOdds
    {
        public int FTSocOddsId { get; set; }

        //
        // Summary:
        //     Early,Today, Running
        public MarketType MarketType { get; set; }

        public ModType OddsType { get; set; }

        public object Data { get; set; }

        public OddsCommand ParamName { get; set; }

        public int LeagueId { get; set; }

        public string SocOddsListId { get; set; }
    }

    [Serializable]
    public class IncAddArgs
    {
        public List<BetViewOdds> BetViewOddsList { get; set; }
        public bool AddedByAutoMo { get; set; }
    }

    [Serializable]
    public class BetViewOdds
    {
        public MarketType CurrentMarketType { get; set; }

        //
        // Summary:
        //     matchId
        public int FTSocOddsId { get; set; }

        //
        // Summary:
        //     SportType
        public string GameType { get; set; }

        //public Dictionary<Language, string> GameDescList { get; set; }

        //public Dictionary<Language, string> LeagueTipList { get; set; }

        public int LeagueTipId { get; set; }

        public int LeagueId { get; set; }
        public int HomeId { get; set; }

        public int AwayId { get; set; }

    }

    [Serializable]
    public enum MarketType
    {
        Undefined,
        Early,
        Today,
        Running,
        Both
    }


    [Serializable]
    public enum ModType
    {
        ADD = 0,
        MOD = 1,
        SMOD = 3,
        DEL = 2
    }

    [Serializable]
    public enum OddsCommand
    {
        Undifined = -1,
        GameDesc = 1,
        LeagueOrder = 2,
        LeagueCode = 3,
        SetScore = 4,
        HomeTeam = 5,
        AwayTeam = 6,
        FTHdp = 7,
        FTHdpOdds = 8,
        FTOU = 9,
        FTOUOdds = 10,
        FHHdp = 11,
        FHHdpOdds = 12,
        FHOU = 13,
        FHOUOdds = 14,
        FTOEOdds = 15,
        Period = 16,
        IsHomeGive = 17,
        IsRun = 18,
        IsPause = 19,
        IsDanger = 20,
        MatchDate = 21,
        StartTime = 22,
        SetRedCard = 23,
        MatchOrder = 24,
        StockType = 25,
        SportOrder = 26,
        StatsURL = 27,
        IsInetHide = 28,
        IsX12InetHide = 29,
        IsOEInetHide = 30,
        IsCSInetHide = 31,
        IsTGInetHide = 32,
        IsFGLGInetHide = 33,
        IsHTFTInetHide = 34,
        IsOUTInetHide = 35,
        MatchOver = 36,
        MatchOver1H = 37,
        TVLink = 38,
        TeamInfo = 39,
        HasPar = 40,
        SetRedList = 41,
        FHOEOdds = 42,
        TeamName = 43,
        MOOdds = 44,
        FTX12_Odds = 45,
        FHX12_Odds = 46,
        ParDiff = 47,
        AddNewMo = 48,
        DelMo = 49,
        AllSportPanel = 50,
        SetMarketType = 51,
        SetCSOdds = 52,
        SetFTHTOdds = 53,
        SetFGLGOdds = 54,
        SetTGOdds = 55,
        SetOUTOdds = 56,
        SetD1Odds = 57,
        SetD2Odds = 58,
        SetWorkingDatePartition = 59,
        SetLiveDisplay = 60,
        SpecialTeam = 61,
        AddAutoMo = 62,
        OddsDiff = 63,
        SetSmartId = 64,
        LeagueTips = 65,
        ModLeagueTipId = 66,
        SetLeagueType = 67,
        TradeInChgByLeague = 68,
        TradeInChgByMatchGroup = 69,
        SetSpecialBetlimit = 70,
        LiveDataChanged = 71,
        SetMatchDelay = 72,
        SetLeagueMMRSetting = 73,
        DCX12_Odds = 74,
        SepcialName = 75,
        CategoryName = 76,
        SetDataCategory = 77,
        SetSpecialId = 78,
        SetCategoryOrder = 79,
        ModifySpecialOrder = 80,
        SetMatchLiveChange = 81,
        Set3WayHdpOdds = 82,
        SetCleanSheetOdds = 83,
        //
        // Summary:
        //     Command that indicates that some values (e.g. odds) of a Match have been updated
        //
        //
        // Remarks:
        //     The Enum (int) value is forced to 5888
        MatchValuesUpdated = 5888,
        //
        // Summary:
        //     Command that indicates that some values (e.g. odds) of SEVERAL Matches have been
        //     updated
        //
        // Remarks:
        //     The Enum (int) value is forced to 5889
        ManyMatchValuesUpdated = 5889
    }
}
