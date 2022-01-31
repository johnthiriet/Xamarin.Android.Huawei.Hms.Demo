using Android.App;
using Android.OS;
using Android.Util;
using Huawei.Agconnect.Config;
using Huawei.Hms.Site.Api;
using Huawei.Hms.Site.Api.Model;

namespace HmsDemo
{
    [Activity(Label = "SiteKitActivity")]
    public class SiteKitActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_sitekit);
        }

        protected override void OnResume()
        {
            base.OnResume();

            using var config = AGConnectServicesConfig.FromContext(this);
            var apiKey = config.GetString("client/api_key");
            var encodedApiKey = Java.Net.URLEncoder.Encode(apiKey, "utf-8");
            ISearchService searchService = SearchServiceFactory.Create(this, encodedApiKey);

            QueryAutocompleteRequest request = new QueryAutocompleteRequest();
            request.Query = "Paris";

            Coordinate location = new Coordinate(48.893478, 2.334595);

            request.Location = location;

            request.Radius = (Java.Lang.Integer)1000;
            request.Children = false;

            SearchResultListenerQueryAutocompleteResponse resultListener = new SearchResultListenerQueryAutocompleteResponse();
            searchService.QueryAutocomplete(request, resultListener);
        }

        public class SearchResultListenerQueryAutocompleteResponse : Java.Lang.Object, ISearchResultListener
        {
            public void OnSearchError(SearchStatus status)
            {
                Log.Info("TAG", "Error : " + status.ErrorCode + " " + status.ErrorMessage);
            }

            public void OnSearchResult(Java.Lang.Object p0)
            {
                QueryAutocompleteResponse results = p0 as QueryAutocompleteResponse;
                if (results == null)
                    return;


            }
        }
    }
}
