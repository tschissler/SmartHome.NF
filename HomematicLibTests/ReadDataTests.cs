using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomematicLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}