namespace MediaTracker
    {
        using System;
        using Newtonsoft.Json.Linq;

        /// <summary>
        /// Search the APIs for media content
        /// </summary>
        public class SearchItem
            {
                /// <summary>
                /// Creates an instance of the Item class to extract content into and access
                /// </summary>
                public static Item Media { get; set; }

                /// <summary>
                /// Finds the item by accessing all of the methods required
                /// </summary>
                /// <param name="mediaType">Type of the media.</param>
                /// <param name="inputTitle">The input title.</param>
                /// <returns></returns>
                public bool FindItem(string mediaType, string inputTitle)
                    {

                        if ( string.IsNullOrEmpty(inputTitle) ) return false; // Don't continue if no title given

                        // Get the generated URL for the API with Title attached
                        Uri apiUrl = GetApiUrl(inputTitle, mediaType);

                        // Download the data from the API using the URL
                        string jsonData = GetJsonData(apiUrl);

                        if ( string.IsNullOrEmpty(jsonData) ) return false; // Don#t continue if API fails

                        // Parse data depending on media type
                        switch ( mediaType )
                            {
                                case "Movie":
                                case "Series":
                                case "Book":
                                    {
                                        // Runs if any of above cases are true
                                        Media = new Item(JObject.Parse(jsonData), mediaType);
                                        FindItem("trailer", Media.Title);
                                        return true;
                                    }
                                case "trailer":
                                    Media.AddTrailer(JArray.Parse(jsonData));
                                    return true;
                                default:
                                    return false;
                            }
                    }

                /// <summary>
                /// Gets the API URL.
                /// </summary>
                /// <param name="title">The title.</param>
                /// <param name="type">The type.</param>
                /// <returns></returns>
                private static Uri GetApiUrl(string title, string type)
                    {
                        // Replaces spaces with + for URL friendly name
                        title = title.Replace(" ", "+");

                        // API URLs
                        const string googleApi = "https://www.googleapis.com/books/v1/volumes?q={0}";
                        const string imdbApiMovie = "http://www.omdbapi.com/?t={0}&y=&plot=short&r=json&type=movie";
                        const string imdbApiSeries = "http://www.omdbapi.com/?t={0}&y=&plot=short&r=json&type=series";
                        const string trailerApi = "http://trailersapi.com/trailers.json?movie={0}&limit=5&width=320";

                        // Return the URL after embedding the title to be searched to the API link
                        switch ( type )
                            {
                                case "Book":
                                    return new Uri(String.Format(googleApi, title));
                                case "Movie":
                                    return new Uri(String.Format(imdbApiMovie, title));
                                case "Series":
                                    return new Uri(String.Format(imdbApiSeries, title));
                                case "trailer":
                                    return new Uri(String.Format(trailerApi, title));
                                default:
                                    return null;
                            }
                    }

                /// <summary>
                /// Gets the json data from the API using the URL
                /// </summary>
                /// <param name="apiUrl">The API URL.</param>
                /// <returns></returns>
                private static string GetJsonData(Uri apiUrl)
                    {
                        // TimedWebClient is a control customized by myself to allow for it to
                        //      timeout. The general WebClient can go on for minutes before timing out,
                        //      whereas I need it to timeout sooner to notify of network problem.

                        using ( TimedWebClient web = new TimedWebClient { Timeout = 10000 } )
                            {
                                // Downloads the data from the URL and stores it in a variable
                                try
                                    {
                                        return web.DownloadString(apiUrl);
                                    }
                                catch ( Exception )
                                    {
                                        Utilities.ShowMessage("Error", "Connecting to API server timed out.");
                                        return null;
                                    }
                            }
                    }
            }
    }