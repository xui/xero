using Xero;

class BusinessLogic
{
    public void SimpleThingNoState()
    {

    }

    public void UpdateTheRecordThings()
    {
    }

    public async Task<bool> UpdateTheRecordsAsync()
    {
        await Task.Delay(1);
        return true;
    }
}
