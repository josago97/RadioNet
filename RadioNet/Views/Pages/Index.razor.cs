using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using RadioBrowserNet;
using RadioNet.Views.Components;

namespace RadioNet.Views.Pages;

public partial class Index
{
    [Inject] private HttpClient HttpClient { get; set; }
    private RadioBrowserClient Client { get; set; }
    private Dictionary<string, string> Countries { get; set; }
    private string Country { get; set; }
    private string Query { get; set; }
    private bool IsSearching { get; set; }
    private CancellationTokenSource SearchCancellationSource { get; set; }
    private IEnumerable<StationInfo> Stations { get; set; }
    private Player Player { get; set; }
    private string PlayerCss => Player?.Station == null ? "hide" : null;

    protected override void OnInitialized()
    {
        Client = new RadioBrowserClient("nl1.api.radio-browser.info");

        RegionInfo regionInfo = new RegionInfo(CultureInfo.CurrentCulture.Name);
        Country = regionInfo.TwoLetterISORegionName;
        Query = string.Empty;
        IsSearching = true;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Countries = await HttpClient.GetFromJsonAsync<Dictionary<string, string>>("raw/countries.json");

        _ = SearchAsync();
    }

    private async Task SearchAsync()
    {
        SearchCancellationSource?.Cancel();
        SearchCancellationSource = new CancellationTokenSource();
        CancellationToken cancellation = new CancellationToken();
        IsSearching = true;

        AdvancedSearchOptions options = new AdvancedSearchOptions()
        {
            Name = Query,
            Countrycode = Country,
            Order = StationOrders.Votes,
            Reverse = true,
        };

        IEnumerable<StationInfo> stations;

        try
        {
            stations = await Client.Stations.AdvancedSearchAsync(options);
        }
        catch (Exception ex)
        {
            stations = null;
        }

        if (!cancellation.IsCancellationRequested)
        {
            Stations = stations;
            IsSearching = false;
            StateHasChanged();
        }
    }

    private async Task OnStationSelectedAsync(StationInfo station)
    {
        await Player.PlayAsync(station);
    }
}