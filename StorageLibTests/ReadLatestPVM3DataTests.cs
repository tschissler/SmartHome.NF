using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using StorageLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageLibTests
{
    [TestClass]
    public class ReadLatestPVM3DataTests
    {
        public const string TestTableName = "SmartHomePVM3";
        
        [TestMethod]
        public void ReadItemsFromLast30Sec()
        {
            var timestamp = DateTime.Now.Subtract(new TimeSpan(0, 0, 30));
            var actual = TableStorageConnector.ReadLatestPVM3Data(timestamp, TestTableName, Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString_Prod"));
            actual.Result.Count().Should().BeGreaterThanOrEqualTo(15);
        }
    }
}
