using System.CommandLine;
using CLI.Broker;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Commands;

public class DeleteRecordCommand : Command
{
    private readonly IAPIBroker _apiBroker;

    public DeleteRecordCommand(IAPIBroker apiBroker) : base("delete-record", "Delete a record from a zone")
    {
        _apiBroker = apiBroker;
        var recordId = new Argument<string>("recordId")
        {
            Name = "recordId",
            Description = "The ID of the record to delete (get from list-zone)"
        };


        AddArgument(recordId);

        this.SetHandler(ExecuteCommand, recordId);
    }


    public async Task ExecuteCommand(string recordId)
    {
        Console.WriteLine($"Trying to delete {recordId} record....");

        var tryGetRecord = await _apiBroker.GetRecordByIDAsync(recordId);


        var record = await tryGetRecord.ProcessHttpResponseAsync<Record>("record");
        if (record == null)
        {
            Console.WriteLine("Could not get record from API");
            return;
        }


        var tryDeleteRecord = await _apiBroker.DeleteRecordAsync(record);


        var deleteInfo = await tryDeleteRecord.ProcessHttpResponseAsync<OperationResult>("delete record");
        if (deleteInfo == null)
        {
            Console.WriteLine("Could delete record");
            return;
        }

        Console.WriteLine($"Successfully deleted record - {deleteInfo.Id}");
    }
}