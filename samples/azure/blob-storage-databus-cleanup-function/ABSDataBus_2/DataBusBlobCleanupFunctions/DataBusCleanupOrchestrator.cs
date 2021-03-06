using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

public static class DataBusCleanupOrchestrator
{
    static DataBusCleanupOrchestrator()
    {
        var storageConnectionString = Environment.GetEnvironmentVariable("DataBusStorageAccount");
        var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
        cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
    }

    #region DataBusCleanupOrchestratorFunction
    [FunctionName(nameof(DataBusCleanupOrchestrator))]
    public static async Task RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context, TraceWriter log)
    {
        var blobData = context.GetInput<DataBusBlobData>();

        log.Info($"Orchestrating deletion for blob at {blobData.Path} with ValidUntilUtc of {blobData.ValidUntilUtc}");

        var validUntilUtc = DataBusBlobTimeoutCalculator.ToUtcDateTime(blobData.ValidUntilUtc);

        DateTime timeoutUntil;

        //Timeouts currently have a 7 day limit, use 6 day loops until the wait is less than 6 days
        do
        {
            timeoutUntil = validUntilUtc > context.CurrentUtcDateTime.AddDays(2) ? context.CurrentUtcDateTime.AddDays(2) : validUntilUtc;
            log.Info($"Waiting until {timeoutUntil}/{validUntilUtc} for blob at {blobData.Path}. Currently {context.CurrentUtcDateTime}.");
            await context.CreateTimer(DataBusBlobTimeoutCalculator.ToUtcDateTime(blobData.ValidUntilUtc), CancellationToken.None);
        } while (validUntilUtc > timeoutUntil);

        var blob = await cloudBlobClient.GetBlobReferenceFromServerAsync(new Uri(blobData.Path));
        log.Info($"Deleting blob at {blobData.Path}");
        await blob.DeleteIfExistsAsync();
    }
    #endregion

    static CloudBlobClient cloudBlobClient;
}