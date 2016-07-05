using Nest;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NestTest
{
    [TestFixture]
    public class SearchDescriptorTests
    {
        [Test]
        public void DifferentAreDifferent()
        {
            var s = new SearchDescriptor<string>().Query(x => x.Ids(new[] { "jemima" }));
            var t = new SearchDescriptor<string>().Query(x => x.Ids(new[] { "puddleduck" }));
            Assert.AreNotEqual(s, t);
        }

        [Test]
        public void SameAreSame()
        {
            var s = new SearchDescriptor<string>().Query(x => x.Ids(new[] { "jemima" }));
            var t = new SearchDescriptor<string>().Query(x => x.Ids(new[] { "jemima" }));
            Assert.AreEqual(s, t);
        }

        [Test]
        public void CanGetThingsBackOutOfASearchDescriptor()
        {
            var s = new SearchDescriptor<string>().Query(x => x.Ids(new[] { "jemima" }));
            var actual = GetThingsBackOut(s);
            Assert.AreEqual("jemima", actual);
        }

        private static string GetThingsBackOut(SearchDescriptor<string> searchDescriptor)
        {
            var searchJson = searchDescriptor.Serialize();
            dynamic d = JsonConvert.DeserializeObject<dynamic>(searchJson);

            return d["query"]["ids"]["values"][0];
        }
    }
}