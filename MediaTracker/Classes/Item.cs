namespace MediaTracker
    {
        using System;
        using System.Data;
        using System.Linq;
        using System.Text.RegularExpressions;
        using System.Windows.Media.Imaging;
        using Newtonsoft.Json.Linq;

        /// <summary>
        ///     Class to Parse and store data for media
        /// </summary>
        public class Item
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="Item"/> class.
                /// </summary>
                /// <param name="apiData">The API data.</param>
                /// <param name="type">The type.</param>
                public Item(JObject apiData, string type)
                    {
                        // Store the chosen media type in property
                        MediaType = type;

                        // Determine how to parse apiData
                        if ( type == "Movie" || type == "Series" ) ParseVideoFromJson(apiData);
                        else if ( type == "Book" ) ParseBookFromJson(apiData);
                    }

                // Properties to hold media items details
                public Item(DataTable sqlData) { ParseMediaFromSql(sqlData); }
                public string Title { get; set; }
                public int Year { get; set; }
                public string AgeRating { get; private set; }
                public string Length { get; private set; }
                public string Genre { get; private set; }
                public string Creator { get; private set; }
                public string Plot { get; private set; }
                public BitmapImage Poster { get; private set; }
                public double Rating { get; private set; }
                public string ImdbId { get; set; } // Not implemented
                public string MediaType { get; private set; }
                public bool ApIResponse { get; private set; }
                public Uri TrailerUrl { get; set; }
                public bool IsTrailerCorrect { get; private set; }

                /// <summary>
                /// Called from other class to try parse the Trailer apidata
                /// </summary>
                /// <param name="apiData">The API data.</param>
                public void AddTrailer(JArray apiData)
                    {
                        // Pass the data to the Trailer data parsing method
                        ParseTrailerFromJson(apiData);
                    }

                /// <summary>
                /// Parses the video details from json.
                /// </summary>
                /// <param name="apiData">The API data.</param>
                private void ParseVideoFromJson(JObject apiData)
                    {
                        // Check for valid response
                        ApIResponse = bool.Parse(apiData["Response"].ToString());

                        // If valid response, parse data
                        if ( !ApIResponse ) Utilities.ShowMessage("Error", "Item not found in API");
                        else
                            {
                                // Validate data and assign them to properties

                                // NOTE: ======= Error handling ========
                                // Using the ?: conditional operator, which is like an If-Else
                                // condition ? first_expression : second_expression;
                                // If condition is true then first_expression, else second_expression

                                Title = apiData["Title"].ToString();
                                Length = apiData["Runtime"].ToString();
                                Plot = apiData["Plot"].ToString();

                                // Some items return lists seperated by commas, so need to be parsed differently
                                var genres = apiData["Genre"].ToString().Split(',').ToList();
                                Genre = genres[0];
                                var ageRatings = apiData["Rated"].ToString().Split(',').ToList();
                                AgeRating = ageRatings[0];

                                // Store poster URL, or assign placeholder if none exists
                                var defaultPoster =
                                    new BitmapImage(new Uri(@"/Resources/posterPlaceholder.jpg",
                                        UriKind.RelativeOrAbsolute));
                                var imdbPoster = apiData["Poster"].ToString();
                                Poster = imdbPoster == "N/A"
                                             ? defaultPoster
                                             : new BitmapImage(new Uri(imdbPoster, UriKind.RelativeOrAbsolute));

                                // Store only the Year and not a range
                                var imdbYear = apiData["Year"].ToString();
                                if ( imdbYear == "N/A" ) Year = 0;
                                else Year = int.Parse(imdbYear.Substring(0, 4));

                                // Ensures that Director isn't stored as "N/A"
                                var imdbDirector = apiData["Director"].ToString();
                                Creator = imdbDirector == "N/A" ? "" : imdbDirector;

                                // Convert IMDB rating to decimal for compatibility with Rating Control
                                var imdbRating = apiData["imdbRating"].ToString();
                                Rating = imdbRating == "N/A" ? ' ' : ( double.Parse(imdbRating) / 10 );
                            }
                    }

                /// <summary>
                /// Parses the book data from json.
                /// </summary>
                /// <param name="apiData">The API data.</param>
                private void ParseBookFromJson(JObject apiData)
                    {
                        // Need to clean this up and remove the Try/Catch

                        // Response = True if there is an item available
                        ApIResponse = (int) apiData["totalItems"] > 0;

                        if ( ApIResponse )
                            {
                                //  These are to simplify the paths used
                                var volumeInfo = apiData["items"][0]["volumeInfo"];
                                var imageLinks = volumeInfo["imageLinks"];
                                MediaType = "Book";

                                Title = volumeInfo["title"].ToString();
                                AgeRating = ""; // Note: books don't have age ratings

                                // Sometimes the API doesn't return any categories, which can't be checked
                                // before using it (which breaks), so try-catch to find out.
                                try
                                    {
                                        Genre = volumeInfo["categories"][0].ToString();
                                    }
                                catch ( Exception )
                                    {
                                        Genre = "";
                                    }

                                Creator = volumeInfo["authors"][0].ToString();

                                var bookPlot = volumeInfo["description"].ToString();
                                Plot = bookPlot == "" ? "" : bookPlot;

                                var bookRating = volumeInfo.Value <double?>("averageRating") ?? 0;
                                Rating = bookRating / 10 * 2;

                                // Ensures Year data is first 4 digits and not full date
                                var bookYear = volumeInfo["publishedDate"].ToString();
                                Year = bookYear == "" ? 0 : int.Parse(bookYear.Substring(0, 4));

                                // Stores poster URL as image for simpler implementation
                                var bookPoster = imageLinks["thumbnail"].ToString();
                                Poster = new BitmapImage(new Uri(bookPoster, UriKind.RelativeOrAbsolute));

                                // Include 'pages' in the string
                                //var bookLength = volumeInfo["pageCount"].ToString();
                                var bookLength = volumeInfo.Value <string>("pageCount") ?? "";

                                Length = string.Format("{0} pages", bookLength);
                            }
                        else Utilities.ShowMessage("Error", "Item not found in API");
                    }

                /// <summary>
                /// Parses the trailer data from json.
                /// </summary>
                /// <param name="apiData">The API data.</param>
                private void ParseTrailerFromJson(JArray apiData)
                    {
                        if ( apiData != null && apiData.Count > 0 )
                            {
                                // Get trailer title and embed code from API data
                                var trailerTitle = apiData[0]["title"].ToString();
                                var trailerCode = apiData[0]["code"].ToString();

                                // Sanitize each title to ignore grammar and punctuation
                                trailerTitle = Regex.Replace(trailerTitle.ToUpper(), "[ ().-]+", "");
                                var mediaTitle = Regex.Replace(SearchItem.Media.Title.ToUpper(), "[ ().-]+", "");

                                // True if trailers title matches the media title
                                IsTrailerCorrect = trailerTitle.Contains(mediaTitle); // Check for match

                                // If trailer is wrong, then stop
                                if ( !IsTrailerCorrect ) return;

                                //Extract the youtube ID from the embed code
                                var regexPattern = new Regex(@"src=\""\S+/embed/(?<videoId>\w+)");
                                var matchVideoId = regexPattern.Match(trailerCode); // Checks for match
                                var youtubeId = string.Empty;
                                if ( matchVideoId.Success ) youtubeId = matchVideoId.Groups["videoId"].Value;

                                // Add the ID to the youtube URL
                                const string youtubeUrl = "http://www.youtube.com/v/{0}";
                                TrailerUrl = new Uri(String.Format(youtubeUrl, youtubeId));
                            }
                        else IsTrailerCorrect = false;
                    }

                /// <summary>
                /// Parses the media from the database.
                /// </summary>
                /// <param name="itemData">The item data.</param>
                private void ParseMediaFromSql(DataTable itemData)
                    {
                        var row = itemData.Rows[0];
                        
                        Title = row["Title"].ToString();
                        Year = Convert.ToInt32(row["Year"]);
                        AgeRating = row["AgeRating"].ToString();
                        Genre = row["Genre"].ToString();
                        Creator = row["Author"].ToString();
                        Length = row["Length"].ToString();
                        Plot = row["Synopsis"].ToString();
                        MediaType = row["Type"].ToString();
                        Poster = new BitmapImage(new Uri(row["PosterUrl"].ToString(), UriKind.RelativeOrAbsolute));
                        Rating = double.Parse(row["ImdbRating"].ToString());

                        IsTrailerCorrect = !string.IsNullOrEmpty(row["TrailerUrl"].ToString());
                        if ( IsTrailerCorrect ) TrailerUrl = new Uri(row["TrailerUrl"].ToString());

                        ApIResponse = true;
                    }
            }
    }