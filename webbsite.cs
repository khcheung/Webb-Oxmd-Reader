using System;

HttpClient client = new HttpClient();
//client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux)");
var response = await client.GetAsync("https://webbsite.0xmd.com/ccass/cconchist.asp?sc=2513");
   if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
    }
    else
    {
        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
    }
