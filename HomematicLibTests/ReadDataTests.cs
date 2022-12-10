using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomematicLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace HomematicLib.Tests
{
    [TestClass()]
    public class ReadDataTests
    {
        [TestMethod()]
        public void ReadDataTest()
        {
            var result = HomematicConnector.ReadData(new Uri("https://srz23.homematic.com:6969/hmip/home/getCurrentState")).Result;
        }

        [TestMethod]
        public void CheckFrequentCallsTest()
        {
            for (int i = 0; i < 50; i++)
            {
                var result = HomematicConnector.ReadData(new Uri("https://srz23.homematic.com:6969/hmip/home/getCurrentState")).Result;
                result.devices.Should().NotBeNull();
                Thread.Sleep(500);
            }
        }
    }
}