namespace MediaTracker
    {
        using System;
        using System.Collections.Generic;
        using System.Data;
        using System.Data.SQLite;
        using System.Windows;
        using System.Windows.Controls;
        using System.Windows.Media;
        using MediaTracker.Controls;

        /// <summary>
        /// Show the details for a media item and provide options to save to database
        /// </summary>
        public partial class ShowMedia
            {
                private ShowReviews reviews;

                /// <summary>
                /// Initializes a new instance of the <see cref="ShowMedia"/> class.
                /// For simple searching.
                /// </summary>
                public ShowMedia()
                    {
                        InitializeComponent();
                        HideButtons(); // Hide buttons by default

                        BtnBack.Visibility = Visibility.Collapsed; // Only needed for other constructor
                    }

                /// <summary>
                /// Initializes a new instance of the <see cref="ShowMedia"/> class.
                /// For a specified item based on Item ID
                /// </summary>
                /// <param name="itemIdInput">The item identifier input.</param>
                public ShowMedia(int itemIdInput)
                    {
                        InitializeComponent();
                        HideButtons(); // Hide buttons by default
                        ItemId = itemIdInput; // Store ItemID to be used throughout
                        GetLocalItemData(); // Get and display data from DB
                        BtnSaveContent.Visibility = Visibility.Visible;
                        ComboStatus.Visibility = Visibility.Visible;
                        BtnRecommend.Visibility = Visibility.Visible;
                    }

                private static int ItemId { get; set; }
                private bool DidWindowResize { get; set; } // Check if window was resized, to undo it when reverting
                private ListUsers ShowUsers { get; set; }

                /// <summary>
                /// Method to hide the controls for specific purposes.
                /// </summary>
                private void HideButtons()
                    {
                        // When starting a new search, hide all buttons
                        BtnSaveContent.Visibility = Visibility.Collapsed;
                        BtnTrailer.Visibility = Visibility.Collapsed;
                        BtnReview.Visibility = Visibility.Collapsed;
                        BtnRecommend.Visibility = Visibility.Collapsed;
                        ComboStatus.Visibility = Visibility.Collapsed;
                        ReviewExpander.Visibility = Visibility.Collapsed;
                    }

                /// <summary>
                /// Method called when searching for an item.
                /// </summary>
                /// <param name="type">The type.</param>
                /// <param name="title">The title.</param>
                public void GetOnlineItemData(string type, string title)
                    {
                        HideButtons(); // Hide controls by default

                        // Search for item
                        SearchItem search = new SearchItem();
                        bool searchIsSuccessful = search.FindItem(type, title);
                        if ( !searchIsSuccessful ) return;

                        // Display media, controls and try get reviews/trailer
                        bool canDisplay = ShowItem();
                        if ( !canDisplay ) return;
                        BtnSaveContent.Visibility = Visibility.Visible;
                        ComboStatus.Visibility = Visibility.Visible;
                        BtnRecommend.Visibility = Visibility.Visible;
                        ShowReviews();
                        ShowTrailer();
                    }

                /// <summary>
                /// Method called when displaying a specified item
                /// </summary>
                public void GetLocalItemData()
                    {
                        // Get specified data from database
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Store the database contents to the DtMovie DataTable
                                        DataTable itemData =
                                            sh.Select(
                                                "SELECT item.id, item.title, item.year, item.imdbrating, item.length, item.synopsis, " +
                                                "item.posterUrl,  type.type, " +
                                                "item.trailerurl, item.agerating, item.genre, item.author " +
                                                "FROM item " + "JOIN type ON item.typeId = type.id " +
                                                "WHERE item.id = @item", new[] { new SQLiteParameter("@item", ItemId) });
                                        SearchItem.Media = new Item(itemData);

                                        // Show content and try to get trailer and reviews
                                        ShowItem();
                                        ShowTrailer();
                                        ShowReviews();
                                        BtnReview.Visibility = Visibility.Hidden;
                                        if ( ManipulateItem.HasItem(ItemId) ) BtnReview.Visibility = Visibility.Visible;
                                    }
                            }
                    }

                /// <summary>
                /// Checks if there are reviews and shows them
                /// </summary>
                private void ShowReviews()
                    {
                        // Get Item ID
                        string itemName = SearchItem.Media.Title;
                        string itemType = SearchItem.Media.MediaType;
                        ItemId = ManipulateItem.GetItemId(itemName, itemType);
                        if ( ItemId == 0 ) return; // Stop if no item

                        // Show review button if USER has ITEM
                        BtnReview.Visibility = Visibility.Hidden;
                        if ( ManipulateItem.HasItem(ItemId) ) BtnReview.Visibility = Visibility.Visible;

                        // Check if item has reviews
                        reviews = null;
                        reviews = new ShowReviews(ItemId);
                        if ( reviews.HasReviews == false ) return; // Stop if no reviews

                        // If so, show reviews and make control visible
                        ReviewGrid.Children.Add(reviews);
                        ReviewExpander.Visibility = Visibility.Visible;
                    }

                /// <summary>
                /// Mark user as having particular item in list
                /// </summary>
                /// <param name="newItemId">The new item identifier.</param>
                private void AddToUsersList(int newItemId)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Add item to users list
                                        Dictionary <string, object> mediaItem = new Dictionary <string, object>();
                                        mediaItem["itemid"] = newItemId;
                                        mediaItem["userid"] = User.CurrentUserId;
                                        sh.Insert("progress", mediaItem);

                                        // Set rating and status
                                        ManipulateItem.SetRating(Convert.ToDouble(RatingPersonal.Value), newItemId);
                                        ManipulateItem.SetStatus(ComboStatus.Text, newItemId);

                                        // Create empty review slot in table
                                        Dictionary <string, object> newReviewData = new Dictionary <string, object>();
                                        newReviewData["userid"] = User.CurrentUserId;
                                        newReviewData["itemid"] = newItemId;
                                        sh.Insert("review", newReviewData);
                                    }
                            }
                    }


                /// <summary>
                /// Saves media item details to database
                /// </summary>
                private void SaveMedia()
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Save item to database
                                        ItemId = ManipulateItem.AddToDatabase();

                                        #region Add item to list

                                        // Check if user already has item
                                        DataTable dt2 =
                                            sh.Select(
                                                "SELECT * FROM progress " +
                                                "WHERE itemid = @itemId AND userid = @userId;",
                                                new[]
                                                    {
                                                        new SQLiteParameter("@itemid", ItemId),
                                                        new SQLiteParameter("@userId", User.CurrentUserId)
                                                    });

                                        if ( dt2.Rows.Count == 0 )
                                            {
                                                // User doesn't have item - Add it
                                                AddToUsersList(ItemId);

                                                // Refresh reviews
                                                ShowReviews();

                                                // Refresh Tables
                                                ( (MainWindow) Application.Current.MainWindow ).RefreshContent();

                                                Utilities.ShowMessage("Complete", "Finished adding item to list");
                                            }
                                        else Utilities.ShowMessage("Woops", "You already have this item");
                                        #endregion Add item to list
                                    }
                            }
                    }

                /// <summary>
                /// When item is parsed successfully, show the content
                /// </summary>
                /// <returns></returns>
                private bool ShowItem()
                    {
                        // Check for invalid API response
                        if ( !SearchItem.Media.ApIResponse )
                            {
                                Utilities.ShowMessage("Woops", "Could not find what you were looking for.");
                                return false;
                            }

                        // Show Content
                        LblTitle.Content = SearchItem.Media.Title;
                        LblYear.Content = string.Format("({0})", SearchItem.Media.Year);
                        RatingScore.Value = SearchItem.Media.Rating;
                        LblSynopsis.Text = SearchItem.Media.Plot;
                        ImgPoster.Source = SearchItem.Media.Poster;
                        LblAgeRating.Content = SearchItem.Media.AgeRating;
                        LblRuntime.Content = SearchItem.Media.Length;
                        LblGenre.Content = SearchItem.Media.Genre;
                        LblCreator.Content = SearchItem.Media.Creator;
                        LblMediaType.Content = SearchItem.Media.MediaType;
                        return true; // To notify of success
                    }

                /// <summary>
                /// Shows the trailer if available
                /// </summary>
                private void ShowTrailer()
                    {
                        if ( SearchItem.Media.IsTrailerCorrect ) BtnTrailer.Visibility = Visibility.Visible;
                        else
                            {
                                // Clear content and hide button
                                BtnTrailer.Visibility = Visibility.Collapsed;
                                SearchItem.Media.TrailerUrl = null;
                            }
                    }

                /// <summary>
                /// Handles the Click event of the BtnTrailer control.
                /// Opens a new window to show trailer
                /// </summary>
                private void BtnTrailer_Click(object sender, RoutedEventArgs e)
                    {
                        // Create a pseudo-window with purpose to darken screen
                        Window darkWindow = new Window
                                             {
                                                 Background = Brushes.Black,
                                                 Opacity = 0.7,
                                                 AllowsTransparency = true,
                                                 WindowStyle = WindowStyle.None,
                                                 WindowState = WindowState.Maximized,
                                                 Topmost = true
                                             };

                        // Instance of trailer window, passing through the trailer url
                        VideoPopup playTrailer = new VideoPopup(SearchItem.Media.TrailerUrl);

                        darkWindow.Show();
                        playTrailer.ShowDialog();
                        darkWindow.Close();
                    }

                private void BtnSaveContent_Click(object sender, RoutedEventArgs e) { SaveMedia(); }

                private async void BtnReview_Click(object sender, RoutedEventArgs e)
                    {
                        // Get review input and refresh review control
                        string review =
                            await Utilities.MetroStyleInput("Review", "Write a review for " + SearchItem.Media.Title);
                        ManipulateItem.SetReview(review, ItemId);
                        ShowReviews();
                    }

                /// <summary>
                /// Handles the OnCollapsed event of the ReviewExpander control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void ReviewExpander_OnCollapsed(object sender, RoutedEventArgs e)
                    {
                        // Only resize if it was resized on expand
                        if ( DidWindowResize == false ) return;
                        Application.Current.MainWindow.Height -= ( (Expander) sender ).ActualHeight;
                        DidWindowResize = false; // Reset property
                    }

                /// <summary>
                /// Handles the OnExpanded event of the ReviewExpander control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void ReviewExpander_OnExpanded(object sender, RoutedEventArgs e)
                    {
                        // Get size of review control
                        reviews.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        double reviewHeight = reviews.DesiredSize.Height;
                        ReviewExpander.Height = reviewHeight + 40;

                        // Don't resize if window is large enough
                        double requiredWindowHeight = Application.Current.MainWindow.ActualHeight + reviewHeight;
                        if ( Application.Current.MainWindow.ActualHeight > requiredWindowHeight ) return;

                        // Add review control height to window height
                        Application.Current.MainWindow.Height = requiredWindowHeight;
                        DidWindowResize = true;
                    }

                /// <summary>
                /// Handles the OnValueChanged event of the RatingPersonal control.
                /// </summary>
                private void RatingPersonal_OnValueChanged(object sender, RoutedPropertyChangedEventArgs <double?> e)
                    {
                        if ( !ManipulateItem.HasItem(ItemId) ) return; // Stop if user doesn't have this item
                        ManipulateItem.SetRating(RatingPersonal.Value, ItemId); // Set new rating
                        ( (MainWindow) Application.Current.MainWindow ).RefreshContent(); // Refresh tables
                    }

                private void BtnRecommend_Click(object sender, RoutedEventArgs e)
                    {
                        // Create and show an instance of ListUsers control to get User ID for recommendation
                        ShowUsers = new ListUsers(true);
                        GridShowUsersPopup.Children.Add(ShowUsers);
                        GridShowUsersPopup.Visibility = Visibility.Visible;
                        ShowUsers.Unloaded += Recommend_Unloaded;
                    }

                private void Recommend_Unloaded(object sender, RoutedEventArgs e)
                    {
                        // After control closes, use ID to send recommendation
                        GridShowUsersPopup.Visibility = Visibility.Hidden;
                        GridShowUsersPopup.Children.Clear();
                        if ( ShowUsers.ClickedUserId == 0 ) return; // Stop if ID is 0
                        ItemId = ManipulateItem.AddToDatabase(); // Save item
                        ManipulateItem.RecommendItem(ItemId, ShowUsers.ClickedUserId); // Send recommendation
                    }

                private void BtnBack_Click(object sender, RoutedEventArgs e)
                    {
                        // Close grid that this control is displayed on
                        ( Application.Current.MainWindow as MainWindow ).ResetGrid();
                    }
            }
    }