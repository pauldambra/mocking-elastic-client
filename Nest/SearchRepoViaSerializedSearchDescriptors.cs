using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Nest;
using NUnit.Framework;

namespace NestTest
{
    [TestFixture]
    public class SearchRepoViaSerializedSearchDescriptors
    {
        [Test]
        public void MatchAlwaysReturns()
        {
            var orig = new SearchDescriptor<string>().Query(q => q.Ids(new[] { "jemima" }));

            var mockResponse = new Mock<ISearchResponse<string>>();
            mockResponse.Setup(x => x.Documents).Returns(new[] { "left", "right" });

            var thingy = new Mock<IElasticClient>();
            thingy.Setup(x => x.Search<string>(It.Is<SearchDescriptor<string>>(sd => sd.Serialize() == orig.Serialize())))
                .Returns(mockResponse.Object);

            var r = new Repo(thingy.Object);

            var result = r.Search<string>("jemima");

            result.Should().Be("left");
        }

        [Test]
        public void UnmatchedReturnsNull()
        {
            var orig = new SearchDescriptor<string>().Query(q => q.Ids(new[] { "jemima" }));

            var mockResponse = new Mock<ISearchResponse<string>>();
            mockResponse.Setup(x => x.Documents).Returns(new[] { "left", "right" });

            var thingy = new Mock<IElasticClient>();
            var serializedSearchDescriptor = orig.Serialize();
            thingy.Setup(x => x.Search<string>(It.Is<SearchDescriptor<string>>(sd => sd.Serialize() == serializedSearchDescriptor)))
                .Returns(mockResponse.Object);

            var r = new Repo(thingy.Object);

            var result = r.Search<string>("puddleduck");
            result.Should().BeNull();
        }
    }
}
