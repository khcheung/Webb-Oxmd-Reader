using WebbSite.Common.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebbSite.Common.Logic;

public class DbService(StockContext stockContext)
{

    public async Task SaveStockCodeAsync(Int32 stockCode)
    {
        var stock = new Stock()
        {
            StockCode = stockCode
        };
        await stockContext.Stock.AddAsync(stock);
        await stockContext.SaveChangesAsync();
    }

    public async Task SaveCCASSRecordsAsync(Int32 stockCode, List<CCASSRecord> records)
    {
        var stock = await (from s in stockContext.Stock
                           where s.StockCode == stockCode
                           select s).FirstOrDefaultAsync();

        if (stock == null)
        {
            stock = new Stock()
            {
                StockCode = stockCode
            };
            stockContext.Stock.Add(stock);
            await stockContext.SaveChangesAsync();  
            //throw new Exception($"Stock with StockCode {stockCode} not found.");
        }

        if (!records.Any())
        {
            Console.WriteLine($"No records to save for stock code {stockCode}.");
            return;
        }
        

        var minRecordDate = records.Min(r => r.Date);
        var maxRecordDate = records.Max(r => r.Date);

        var existRecords = await (from r in stockContext.StockCCASS
                                  where r.StockCode == stockCode && r.RecordDate >= minRecordDate && r.RecordDate <= maxRecordDate
                                  select r).ToListAsync();

        var insert = (from r in records
                      join d in existRecords on r.Date equals d.RecordDate into ld
                      from d in ld.DefaultIfEmpty()
                      where d == null
                      select new StockCCASS()
                      {
                          StockCode = stockCode,
                          RecordDate = r.Date,
                          TopFive = r.Top5,
                          TopTen = r.Top10,
                          TopTenNCIP = r.Top10NCIP,
                          StakeInCCASS = r.Stake
                      }).ToList();

        stockContext.StockCCASS.AddRange(insert);
        Console.WriteLine($"Inserting {insert.Count} new records.");
        await stockContext.SaveChangesAsync();

        var update = (from r in records
                      join d in existRecords on r.Date equals d.RecordDate
                      select new { r, d }).ToList();

        update.ForEach(u =>
        {
            u.d.TopFive = u.r.Top5;
            u.d.TopTen = u.r.Top10;
            u.d.TopTenNCIP = u.r.Top10NCIP;
            u.d.StakeInCCASS = u.r.Stake;
        });

        await stockContext.SaveChangesAsync();

    }

    public async Task<List<CCASSRecord>>  GetCCASSRecordsAsync(int stockCode)
    {
        var records = await (from r in stockContext.StockCCASS
                             where r.StockCode == stockCode
                             orderby r.RecordDate descending
                             select new CCASSRecord()
                             {
                                 Row = 0, // Row is not stored in the database, so we can set it to 0 or any other value.
                                 Date = r.RecordDate,
                                 Top5 = r.TopFive,
                                 Top10 = r.TopTen,
                                 Top10NCIP = r.TopTenNCIP,
                                 Stake = r.StakeInCCASS
                             }).Take(50).ToListAsync();

        return records;
    }
}