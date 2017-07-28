using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Tests
{
    [TestClass]
    public class CompassCardTests
    {
        public const string Url = @"https://compasscard.511sd.com";
        CompassCard scraper;
        public CompassCardTests()
        {
            scraper = new CompassCard();
        }

        [ClassInitialize]
        public static void TestClassInit(TestContext testContext)
        {
            Console.WriteLine("Class Init!");
        }

        [TestInitialize]
        public void TestInit()
        {
            Console.WriteLine("Test Init!");
        }

        [TestMethod]
        public void FrontPageTest()
        {
            string content = scraper.GetFrontPage();
            Assert.IsTrue(content.Contains(@"<span id=""navText"">Welcome to Online Ticketing</span>"));
        }

        [TestMethod]
        public void LoginTest()
        {
            float content = scraper.LoginToSummaryPage();
            //Assert.IsTrue(content.Contains(@"<span id=""navText"">Summary of my card</span>"));
            Assert.AreEqual(9.25, content);
        }
    }
}
