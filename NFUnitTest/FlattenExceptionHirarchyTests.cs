using nanoFramework.TestFramework;
using System;
using ExtensionMethods;

namespace NFUnitTest.LogManagerTests
{
    [TestClass]
    public class FlattenExceptionHirarchyTests
    {
        [TestMethod]
        public void CheckHirarchy()
        {
            var innerException2 = new Exception("Inner exception level 2");
            var innerException1 = new Exception("Inner exception level 1", innerException2);
            var exception = new Exception("Root exception message", innerException1);

            string message = exception.FlattenExceptionMessage();

            var expected = "Root exception message | Inner exception level 1 | Inner exception level 2"; 
            Assert.Equal(message, expected);
        }

        [TestMethod]
        public void CheckSingleException()
        {
            var exception = new Exception("Root exception message");

            string message = exception.FlattenExceptionMessage();

            var expected = "Root exception message";
            Assert.Equal(message, expected);
        }
    }
}
