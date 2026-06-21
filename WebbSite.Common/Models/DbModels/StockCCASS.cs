namespace WebbSite.Common.Models.DbModels;

public class StockCCASS : ModelBase
{
    public Int32 StockCode { get; set; }
    public DateTime RecordDate { get; set; }
    public Decimal TopFive { get; set; }
    public Decimal TopTen { get; set; }
    public Decimal TopTenNCIP { get; set; }
    public Decimal StakeInCCASS { get; set; }

}