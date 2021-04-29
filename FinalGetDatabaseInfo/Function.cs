using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FinalGetDatabaseInfo
{
    class Books
    {
        public int rank { get; set; }
        public string title { get; set; }
        public string author { get; set; }
    }
    public class Function
    {
        private static AmazonDynamoDBClient client2 = new AmazonDynamoDBClient();
        public static readonly HttpClient client = new HttpClient();
        private string tableName = "FinalBook";
        public async Task<ExpandoObject> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            Table books = Table.LoadTable(client2, tableName);

            HttpResponseMessage httpResponse = await client.GetAsync("https://api.nytimes.com/svc/books/v3/lists/hardcover-fiction.json?api-key=buM7NOsyelhN3OAGrt3oGViXQjIyajlR");
            httpResponse.EnsureSuccessStatusCode();
            string responseBody = await httpResponse.Content.ReadAsStringAsync();

            Document myDoc = Document.FromJson(responseBody);
            dynamic bookObject = JsonConvert.DeserializeObject<ExpandoObject>(myDoc.ToJson());
            for(int i = 0; i < 15; i++)
            {
                var rank = i + 1;
                var title = bookObject.results.books[i].title;
                var author = bookObject.results.books[i].author;
                Books test = new Books();
                test.rank = rank;
                test.title = title;
                test.author = author;

                PutItemOperationConfig config = new PutItemOperationConfig();
                config.ReturnValues = ReturnValues.AllOldAttributes;

                await books.PutItemAsync(Document.FromJson(JsonConvert.SerializeObject(test)), config);
            }
            
            return bookObject;
        }
    }
}
