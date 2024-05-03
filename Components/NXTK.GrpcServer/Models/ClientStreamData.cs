﻿using DataServicePackage;
using Grpc.Core;

namespace NXTK.GrpcServer.Models
{
    public class ClientStreamData
    {
        public ClientStreamData(IServerStreamWriter<DataResponse> ServerStreamWriter)
        {
            this.ServerStreamWriter = ServerStreamWriter;
        }

        public bool DisConnected { get; set; } = false;
        public IServerStreamWriter<DataResponse> ServerStreamWriter { get; private set; }
    }
}
