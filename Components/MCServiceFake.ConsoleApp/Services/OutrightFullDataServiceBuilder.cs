using NXTK.GrpcServer.Interfaces;
using NXTK.GrpcServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCServiceFake.ConsoleApp.Services
{
    public class OutrightFullDataServiceBuilder : IOutrightFullDataServiceInvoker
    {
        // TODO: in MC project, then we can inject _dataBuilders and call _dataBuilders.OutrightFullData.Build()
        // private readonly IOddsServerDataBuilders _dataBuilders; 

        public OutrightFullDataServiceBuilder()
        {
            
        }

        public IReadOnlyList<FullOdds> GetOutrightFullData()
        {
            Console.WriteLine($"Request OutrightService at {DateTime.Now.Ticks}");

            var fullOdsList = new List<FullOdds>() { new FullOdds { IsFinish = true, MatchId = 1 } };
            return fullOdsList;
        }
    }
}
