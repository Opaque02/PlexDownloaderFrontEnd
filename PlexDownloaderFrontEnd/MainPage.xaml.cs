using Microsoft.Extensions.DependencyInjection;
using Plex.Api.Factories;
using Plex.Library.ApiModels.Accounts;
using Plex.Library.Factories;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Clients;
using Plex.ServerApi;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Plex.Library.ApiModels.Servers;
using Plex.ServerApi.PlexModels.Library;
using Plex.ServerApi.PlexModels.Library.Search;
using Plex.Library.ApiModels.Libraries;
using Plex.Library.ApiModels.Libraries.Filters;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Media;
using Permissions = Microsoft.Maui.ApplicationModel.Permissions;
using System.Net.Http;

namespace PlexDownloaderFrontEnd;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
	}

    int count = 0;

    const string movieCategory = "movie";
    const string showCategory = "show";
    public static List<FilterRequest> filterList = new List<FilterRequest> { };
    public static FilterRequest filter = new FilterRequest();

    static ServiceProvider ServiceProvider { get; set; }

    private void OnCounterClickedTwo(object sender, EventArgs e)
    {
        DisplayAlert("Test", "test", "test");
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
        /*DisplayAlert("Starting now...", "Click here to start now", "OK");*/

        // Runs a function to add all the filters to stop collections showing up as main media
        CollectionSetUp();

        // Connects to a Plex account
        Console.WriteLine("Attempting to connect...");
        PlexAccount account = SetUp();
        Console.WriteLine("Connected to Plex");

        string serverName = "ServerName"; // Server name

        // Finds libraries for the server you're after
        var servers = account.Servers().Result;
        var server = servers.Where(c => c.FriendlyName == serverName).First();
        var libraries = server.Libraries().Result;

        List<string> allMedia = FindMedia(libraries);

        printMedia(allMedia);

        DisplayAlert("Done!", "Done!", "Done!");
        DisplayAlert("First media:", allMedia[0].ToString(), "Yay!");
    }

    public static PlexAccount SetUp()
    {
        var apiOptions = new ClientOptions
        {
            Product = "API_UnitTests",
            DeviceName = "API_UnitTests",
            ClientId = "MyClientId",
            Platform = "Web",
            Version = "v1"
        };

        var services = new ServiceCollection();
        services.AddSingleton(apiOptions);
        services.AddTransient<IPlexServerClient, PlexServerClient>();
        services.AddTransient<IPlexAccountClient, PlexAccountClient>();
        services.AddTransient<IPlexLibraryClient, PlexLibraryClient>();
        services.AddTransient<IApiService, ApiService>();
        services.AddTransient<IPlexFactory, PlexFactory>();
        services.AddHttpClient();
        services.AddLogging();
        services.AddTransient<IPlexRequestsHttpClient, PlexRequestsHttpClient>();

        ServiceProvider = services.BuildServiceProvider();

        var plexFactory = ServiceProvider.GetService<IPlexFactory>();
        PlexAccount account = plexFactory.GetPlexAccount("username", "password"); // auth to Plex
        return account;
    }

    public static void CollectionSetUp()
    {
        // ADDING FILTER FOR "NOT A COLLECTION" //
        filter.Field = "collection";
        filter.Operator = Operator.IsNot;
        List<string> filterValues = new() { "collection" };
        filter.Values = filterValues;
        filterList.Add(filter);
    }

    public static List<string> FindMedia(List<LibraryBase> libraries)
    {
        List<string> foundMedia = new List<string> { };

        /*var librarySearchType = new SearchType();*/
        for (int i = 0; i < libraries.Count(); i++)
        {
            string libraryName = libraries[i].Title;

            var libraryType = libraries[i].Type;

            if (libraryType == movieCategory)
            {
                MovieLibrary movieLibrary = (MovieLibrary)libraries.Single(c => c.Title == libraryName);
                foundMedia = GetAllMedia(movieLibrary, foundMedia);
            }
            else if (libraryType == showCategory)
            {
                ShowLibrary showLibrary = (ShowLibrary)libraries.Single(c => c.Title == libraryName);
                foundMedia = GetAllMedia(showLibrary, foundMedia);

                /*
                var show = showLibrary.Search("Star Wars", string.Empty, SearchType.Show, filterList, 0, showLibrary.Size().Result).Result;
                var mediaExists = show.Media;

                if (mediaExists != null)
                {
                    int showID = int.Parse(show.Media[0].RatingKey);
                    var season = showLibrary.Seasons(showID).Result;

                }*/


            }

            /*var searchLibrary = libraries.Single(c => c.Title == libraryName);
            var librarySize = searchLibrary.Size().Result;

            *//*movieLibrary = (MovieLibrary) libraries.Single(c => c.Title == libraryName);
            var allMovies = movieLibrary.AllMovies(string.Empty, 0, librarySize).Result;*//*



            if (searchLibrary.Type == "movie")
            {
                librarySearchType = SearchType.Movie;
            }
            else if (searchLibrary.Type == "show")
            {
                librarySearchType = SearchType.Show; 
            }

            var items2 = searchLibrary.Search(title: "Harry Potter", sort:"", librarySearchType, filters: filterList, 0, librarySize).Result;

            if (items2.TotalSize > 0)
            {
                var test = items2.Media[0];
            }*/



            /*var items = searchLibrary.Search();*/
        }
        return foundMedia;
    }

    public static List<string> GetAllMedia(MovieLibrary library, List<string> mediaList, bool hideCollection = true)
    {
        List<FilterRequest> searchFilterList = new List<FilterRequest> { };
        if (hideCollection)
        {
            searchFilterList = filterList;
        }
        var allMedia = library.Search(string.Empty, string.Empty, SearchType.Movie, searchFilterList, 0, library.Size().Result).Result;
        if (allMedia.Media != null)
        {
            for (int i = 0; i < allMedia.Media.Count; i++)
            {
                mediaList.Add(allMedia.Media[i].Title);
            }
        }
        return mediaList;
    }

    public static List<string> GetAllMedia(ShowLibrary library, List<string> mediaList, bool hideCollection = true)
    {
        List<FilterRequest> searchFilterList = new List<FilterRequest> { };
        if (hideCollection)
        {
            searchFilterList = filterList;
        }
        var allMedia = library.Search(string.Empty, string.Empty, SearchType.Show, searchFilterList, 0, library.Size().Result).Result;
        if (allMedia.Media != null)
        {
            for (int i = 0; i < allMedia.Media.Count; i++)
            {
                mediaList.Add(allMedia.Media[i].Title);
            }
        }
        return mediaList;
    }

    public static void printMedia(List<string> mediaList)
    {
        for (int i = 0; i < mediaList.Count; i++)
        {
            Console.WriteLine(mediaList[i]);
        }
    }
}

