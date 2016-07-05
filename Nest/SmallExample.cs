using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Nest;
using NUnit.Framework;

namespace NestTest
{
    public static class SearchdescriptorExtensions
    {
        internal static string Serialize(this SearchDescriptor<string> searchDescriptor)
        {
            var client = new ElasticClient();
            var bytes = client.Serializer.Serialize(searchDescriptor);
            var searchJson = Encoding.UTF8.GetString(bytes);
            return searchJson;
        }
    }

    public class Repo
    {
        private readonly IElasticClient _elasticClient;

        public Repo(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public T Search<T>(string id) where T : class
        {
            var result = _elasticClient.Search<T>(s => s.Query(q => q.Ids(new[] {id.ToString()})));
            return result == null ? null : result.Documents.FirstOrDefault();
        }
    }

    public class MatchMockWithSerializedSearchDescriptors
    {
        [Test]
        public void MatchAlwaysReturns()
        {
            var expectedSearchDescriptor = new SearchDescriptor<string>().Query(q => q.Ids(new[] {"jemima"}));
            var serializedSearchDescriptor = expectedSearchDescriptor.Serialize();

            var mockResponse = new Mock<ISearchResponse<string>>();
            mockResponse.Setup(x => x.Documents).Returns(new[] {"left", "right"});

            var mockElasticClient = new Mock<IElasticClient>();
        
            mockElasticClient.Setup(
                x =>
                    x.Search<string>(
                        It.Is<SearchDescriptor<string>>(sd => sd.Serialize() == serializedSearchDescriptor)))
                .Returns(mockResponse.Object);

            var r = new Repo(mockElasticClient.Object);

            var result = r.Search<string>("jemima");


            mockElasticClient.Verify(s=>s.Search<string>(It.IsAny<Func<SearchDescriptor<string>, SearchDescriptor<string>>>()));
            result.Should().Be("left");
        }
    }
}
