using System.Net;
using System.Text.RegularExpressions;

public class WebbSiteClient : IDisposable
{
    private bool disposedValue;

    private String BaseUrl { get; set; } = "https://webbsite.0xmd.com/";
    private HttpClient client { get; set; } = null!;
    private HttpClientHandler handler { get; set; } = null!;

    public WebbSiteClient()
    {
        this.CreateClient();
    }

    private void CreateClient()
    {
        handler = new HttpClientHandler();
        handler.CookieContainer = new CookieContainer();

        client = new HttpClient(handler);
        client.BaseAddress = new Uri(BaseUrl);

        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux)");
    }

    public async Task<List<CCASSRecord>> GetCCASSAsync(Int32 stockCode)
    {
        var result = new List<CCASSRecord>();
        var path = $"/ccass/cconchist.asp?sc={stockCode}";
        using (var response = await client.GetAsync(path))
        {
            var content = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(content);


            //    <tr>
            //             <td class="colHide1">4676</td>
            //             <td><a href="choldings.asp?i=1088&amp;d=2007-06-28">2007-06-28</a></td>
            //             <td>59.67</td>
            //             <td>71.45</td>
            //             <td>71.80</td>
            //             <td>18.03</td>
            //     </tr>        

            Regex rxRecord = new Regex(@"<tr>\s*<td class(?:[^>]*)>(?<row>\d+)</td>\s*<td><a(?:[^>]*)>(?<date>[\d-]+)</a></td>\s*<td>(?<top5>[\d.]+)</td>\s*<td>(?<top10>[\d.]+)</td>\s*<td>(?<top10ncip>[\d.]+)</td>\s*<td>(?<stake>[\d.]+)</td>\s*</tr>", RegexOptions.IgnoreCase);

            var matchesRecord = rxRecord.Matches(content);
            foreach (Match match in matchesRecord)
            {
                var row = match.Groups["row"].Value;
                var date = match.Groups["date"].Value;
                var top5 = match.Groups["top5"].Value;
                var top10 = match.Groups["top10"].Value;
                var top10ncip = match.Groups["top10ncip"].Value;
                var stake = match.Groups["stake"].Value;

                //Console.WriteLine($"Row: {row}, Date: {date}, Top5: {top5}, Top10: {top10}, Top10NCIP: {top10ncip}, Stake: {stake}");
                result.Add(new CCASSRecord()
                {
                    Row = Int32.Parse(row),
                    Date = DateTime.Parse(date),
                    Top5 = Decimal.Parse(top5),
                    Top10 = Decimal.Parse(top10),
                    Top10NCIP = Decimal.Parse(top10ncip),
                    Stake = Decimal.Parse(stake)
                });
            }
        }

        return result;
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (this.client != null)
                {
                    this.client.Dispose();
                }

                if (this.handler != null)
                {
                    this.handler.Dispose();
                }
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
