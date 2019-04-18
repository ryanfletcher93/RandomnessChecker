using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomnessChecker;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void isSameUriReturned()
        {
            Uri uri = new Uri("https://www.reddit.com/");

            GetWebPage page = new GetWebPage();
            String result = page.GetRandomUnit(uri);

            Assert.AreEqual(uri.ToString(), result);
        }

        [TestMethod]
        public void isRandomUriReturned()
        {
            Uri uri = new Uri("https://www.reddit.com/r/random/");
            String result = (new GetWebPage()).GetRandomUnit(uri);

            Assert.AreNotEqual(uri.ToString(), result);
        }

        [TestMethod]
        public void isResultInvalid()
        {
            Uri uri = new Uri("https://www.reddit.com/random/");
            String result = (new GetWebPage()).GetRandomUnit(uri);

            Assert.AreEqual(result, null);
        }
    }
}
