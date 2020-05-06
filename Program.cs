using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace CosmosDbApi
{
    class Program
    {
        private static readonly string EndpointUri = "https://kirankumarcosmosaccount.documents.azure.com:443/";
      
        private static readonly string PrimaryKey = "fLT2nJrMaQWqizv4fxRCs0m4c4jX7nyuwJPTQNPtjZRhSLYyNYGdwpXDpL7LPSaqBqkuxuU5CLOFVtpSzEyCog==";

        private CosmosClient cosmosClient;

        private Database database;

        private Container container;

        private string databaseId = "FamilyDatabase";
        private string containerId = "FamilyContainer";
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.GetStartedDemo();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }
        public async Task GetStartedDemo()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemsToContainerAsync();
            await this.QueryItemsAsync();
        }
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/LastName");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }
        private async Task AddItemsToContainerAsync()
        {
            // Create a family object for the Andersen family
            Family MunagaFamily = new Family
            {
                Id = "Sivaramakrishna",
                LastName = "Munaga",
                Parents = new Parent[]
                {
            new Parent { FirstName = "Rangarao" },
            new Parent { FirstName = "Sambrajyam" }
                },
                Children = new Child[]
                {
            new Child
            {
                FirstName = "Kirankumar",
                Gender = "male"
            }
                },
                Address = new Address { State = "Andhra Pradesh", County = "India", City = "Guntur" },
                IsRegistered = false
            };

            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Family> MunagaFamilyResponse = await this.container.ReadItemAsync<Family>(MunagaFamily.Id, new PartitionKey(MunagaFamily.LastName));
                Console.WriteLine("Item in database with id: {0} already exists\n", MunagaFamilyResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<Family> MunagaFamilyResponse = await this.container.CreateItemAsync<Family>(MunagaFamily, new PartitionKey(MunagaFamily.LastName));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", MunagaFamilyResponse.Resource.Id, MunagaFamilyResponse.RequestCharge);
            }

            // Create a family object for the Wakefield family
            Family PoturajuFamily = new Family
            {
                Id = "Ramadevi",
                LastName = "Poturaju",
                Parents = new Parent[]
                {
            new Parent { FamilyName = "Poturaju", FirstName = "Venkateswarlu" },
            new Parent { FamilyName = "Poturaju", FirstName = "Aadi Lakshmi" }
                },
                Children = new Child[]
                {
            new Child
            {
                FamilyName = "Munaga",
                FirstName = "Kirankumar",
                Gender = "male",
            },
            new Child
            {
                FamilyName = "Munaga",
                FirstName = "Hemanthkumar",
                Gender = "male",
            }
                },
                Address = new Address { State = "Andhra Pradesh", County = "India", City = "Narsaraopet" },
                IsRegistered = true
            };

            try
            {
                // Read the item to see if it exists
                ItemResponse<Family> PoturajuFamilyResponse = await this.container.ReadItemAsync<Family>(PoturajuFamily.Id, new PartitionKey(PoturajuFamily.LastName));
                Console.WriteLine("Item in database with id: {0} already exists\n", PoturajuFamilyResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
                ItemResponse<Family> PoturajuFamilyResponse = await this.container.CreateItemAsync<Family>(PoturajuFamily, new PartitionKey(PoturajuFamily.LastName));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", PoturajuFamilyResponse.Resource.Id, PoturajuFamilyResponse.RequestCharge);
            }
        }
        private async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Munaga'";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

            List<Family> families = new List<Family>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Family family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
            }
        }
    }
    
}
