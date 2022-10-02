﻿using System.Runtime.Serialization;

namespace SharedContracts.Data
{
    [DataContract]
    public class PVDataPoints
    {
        [DataMember]
        public DecimalDataPoint PVProduction = new() { Unit = "W", MaxValue = 12000, DecimalDigits = 1, History = new() { DataHistoryLength = 100 } };
        [DataMember]
        public DecimalDataPoint GridSupply = new() { Unit = "W", MaxValue = 12000, DecimalDigits = 1, History = new() { DataHistoryLength = 100 } };
        [DataMember]
        public DecimalDataPoint GridDemand = new() { Unit = "W", MaxValue = 8000, DecimalDigits = 1, History = new() { DataHistoryLength = 100 } };
    }
}