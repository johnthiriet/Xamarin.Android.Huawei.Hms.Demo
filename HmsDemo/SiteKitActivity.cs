using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.OS;
using Android.Util;
using Huawei.Agconnect.Config;
using Huawei.Hms.Site.Api;
using Huawei.Hms.Site.Api.Model;
using System.Linq;

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
            Log.Debug("SiteKit", $"Encoded ApiKey {encodedApiKey}");
            ISearchService searchService = SearchServiceFactory.Create(this, encodedApiKey);

            QueryAutocompleteRequest request = new QueryAutocompleteRequest();
            request.Query = "5 rue de";

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

                StringBuilder sb = new StringBuilder();
                if (results != null)
                {
                    Site[] sites = results.GetSites();
                    if (sites != null && sites.Length > 0)
                    {
                        int count = 1;
                        foreach (Site site in sites)
                        {
                            AddressDetail addressDetail = site.Address;
                            Coordinate location = site.Location;
                            Poi poi = site.Poi;
                            CoordinateBounds viewport = site.Viewport;

                            sb.AppendFormat(
                                "[{0}] siteId: '{1}', name: {2}, formatAddress: {3}, country: {4}, countryCode: {5}, location: {6}, distance: {7}, poiTypes: {8}, viewport is {9}, streetNumber: {10}, postalCode: {11} , tertiaryAdminArea: {12}",
                                count++, site.SiteId, site.Name, site.FormatAddress,
                                addressDetail == null ? "" : addressDetail.Country,
                                addressDetail == null ? "" : addressDetail.CountryCode,
                                location == null ? "" : (location.Lat + "," + location.Lng),
                                site.Distance, poi == null ? "" : string.Join(";", poi.GetPoiTypes()),
                            viewport == null ? "" : viewport.Northeast + "," + viewport.Southwest,
                                addressDetail == null ? "" : addressDetail.StreetNumber,
                                addressDetail == null ? "" : addressDetail.PostalCode,
                                addressDetail == null ? "" : addressDetail.TertiaryAdminArea);
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        sb.Append("sites 0 results");
                        sb.AppendLine();
                    }

                    AutoCompletePrediction[] predictions = results.GetPredictions();
                    if (predictions != null && predictions.Length > 0)
                    {
                        int count = 1;
                        foreach (AutoCompletePrediction mPrediction in predictions)
                        {
                            sb.AppendFormat("[{0}] Prediction,description = {1} ,", count++, mPrediction.Description);

                            Word[] matchedKeywords = mPrediction.GetMatchedKeywords() ?? new Word[0];
                            foreach (Word matchedKeyword in matchedKeywords)
                            {
                                sb.AppendFormat("matchedKeywords: {0}", matchedKeyword.ToString());
                            }

                            Word[] matchedWords = mPrediction.GetMatchedWords() ?? new Word[0];
                            foreach (Word matchedWord in matchedWords)
                            {
                                sb.AppendFormat(",matchedWords: {0}", matchedWord.ToString());
                            }

                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        sb.Append("Predictions 0 results");
                    }
                }
                var stringResult = sb.ToString();
                Log.Debug("TAG", stringResult);
            }
        }
    }
}
