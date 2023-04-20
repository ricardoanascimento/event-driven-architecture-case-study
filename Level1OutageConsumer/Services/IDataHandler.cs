public interface IDataHandler
{

    Task<List<TurbineData>> GetListWithinTimeFrameByTurbineIdAsync(string turbineId, DateTime earliestDate);
    Task InsertAsync(TurbineData document);
}