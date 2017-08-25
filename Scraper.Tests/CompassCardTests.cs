using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Scraper.Tests
{
    [TestClass]
    public class CompassCardTests
    {
        public const string Url = @"https://compasscard.511sd.com";
        private readonly string password = ConfigurationManager.AppSettings["secretPassword"];
        private readonly List<string> cardNumbers = new List<string>(ConfigurationManager.AppSettings["cardNumberCsv"].Split(','));
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
            float bal1 = scraper.GetRemainingBalance(cardNumbers[0], password);
            Assert.AreEqual(9.25, bal1);
            float bal2 = scraper.GetRemainingBalance(cardNumbers[1], password);
            Assert.AreEqual(5.50, bal2);
        }
    }
}
