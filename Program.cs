using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace AzureTableStorage
{
    class Program
    {
        public static CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storageaccountfortable;AccountKey=bYRmuA6ijIY2O2TMPaQA+lDQtOwBUk46nAG0IeIo5RpKQZJsOo3gH345wW11ejT7y2npoCCx6Q64di326ZBBgQ==;EndpointSuffix=core.windows.net");
        public static CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        public static CloudTable cloudTable = tableClient.GetTableReference("Fleetmanagementsystem");

        public static void Main(string[] args)
        {
            InsertElement("asd125", "Steve");
            InsertElement("akl098", "Luie");

            RetrieveElement();

            Console.ReadKey();
            
        }
        public static void InsertElement(string id, string name)
        {
            FleetEntity fleetEntity = new FleetEntity(id,name);
            TableOperation tableOperation = TableOperation.Insert(fleetEntity);
            cloudTable.Execute(tableOperation);
        }
        public static void RetrieveElement()
        {
            TableQuery<FleetEntity> query = new TableQuery<FleetEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Model"));

            foreach (FleetEntity fleet in cloudTable.ExecuteQuery(query))
            {
                Console.WriteLine(fleet.RowKey);
            }
        }
        public class FleetEntity : TableEntity
        {
            public FleetEntity()
            {

            }
            public FleetEntity(string id,string name)
            {
                this.PartitionKey = "Model";
                this.RowKey = id;
            }
        }
    }
}
