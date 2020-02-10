using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobRead
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Storage_Account_Connection_String";

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference($"device1");

            ReadBlobHierarchical(container,DateTime.Now.ToString("yyyy/MM/dd")).Wait();

            Console.ReadLine();
        }
        
        private static async Task ReadBlobHierarchical(CloudBlobContainer container, string prefix)
        {
            CloudBlobDirectory dir;
            CloudBlob cloudBlob;
            CloudBlockBlob cloudBlockBlob;
            BlobContinuationToken continuationToken = null;

            string contents;

            try
            {
                // Call the listing operation and enumerate the result segment.
                // When the continuation token is null, the last segment has been returned and
                // execution can exit the loop.
                do
                {
                    BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(prefix,
                        false, BlobListingDetails.Metadata, null, continuationToken, null, null);
                    foreach (var blobItem in resultSegment.Results)
                    {
                        // A hierarchical listing may return both virtual directories and blobs.
                        if (blobItem is CloudBlobDirectory)
                        {
                            dir = (CloudBlobDirectory)blobItem;

                            // Write out the prefix of the virtual directory.
                            Console.WriteLine("Virtual directory prefix: {0}", dir.Prefix);

                            // Call recursively with the prefix to traverse the virtual directory.
                            await ReadBlobHierarchical(container, dir.Prefix);
                        }
                        else
                        {
                            // Write out the name of the blob.
                            cloudBlob = (CloudBlob)blobItem;
                            cloudBlockBlob = container.GetBlockBlobReference(cloudBlob.Name);
                            contents = cloudBlockBlob.DownloadTextAsync().Result;

                            Console.WriteLine("Blob name: {0}", cloudBlob.Name);
                            Console.WriteLine(contents);
                        }
                        Console.WriteLine();
                    }

                    // Get the continuation token and loop until it is null.
                    continuationToken = resultSegment.ContinuationToken;

                } while (continuationToken != null);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
