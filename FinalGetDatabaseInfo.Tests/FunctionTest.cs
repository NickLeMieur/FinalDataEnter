using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using FinalGetDatabaseInfo;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System.Dynamic;

namespace FinalGetDatabaseInfo.Tests
{
    public class FunctionTest
    {
        public static readonly HttpClient client = new HttpClient();
        //This test checks if the http is correct and is grabbing the correct information
        [Fact]
        public async void TestHTTPisCorrect()
        {
            var function = new Function();
            var context = new TestLambdaContext();
            HttpResponseMessage httpResponse = await client.GetAsync("https://api.nytimes.com/svc/books/v3/lists/hardcover-fiction.json?api-key=buM7NOsyelhN3OAGrt3oGViXQjIyajlR");
            httpResponse.EnsureSuccessStatusCode();
            string responseBody = await httpResponse.Content.ReadAsStringAsync();

            Document myDoc = Document.FromJson(responseBody);
            dynamic bookObject = JsonConvert.DeserializeObject<ExpandoObject>(myDoc.ToJson());

            Assert.Equal(1, bookObject.results.books[0].rank);
            Assert.Equal("A GAMBLING MAN", bookObject.results.books[0].title);
            Assert.Equal("David Baldacci", bookObject.results.books[0].author);
        }
    }
}
