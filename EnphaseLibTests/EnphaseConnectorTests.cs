using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnphaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnphaseLibTests
{
    [TestClass()]
    public class EnphaseConnectorTests
    {
        [TestMethod()]
        public void GetMeterReadingsTest()
        {
            var result = EnphaseGatewayConnector.GetMeterReadings();
        }
    }
}