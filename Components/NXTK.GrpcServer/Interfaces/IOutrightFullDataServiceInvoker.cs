using NXTK.GrpcServer.Models;
using System.Collections.Generic;

namespace NXTK.GrpcServer.Interfaces
{
    public interface IOutrightFullDataServiceInvoker
    {
        /// <summary>
        /// Must be implemented at any place that will install <see cref="NXTKGrpcServer"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IReadOnlyList<FullOdds> GetOutrightFullData();
    }

}
