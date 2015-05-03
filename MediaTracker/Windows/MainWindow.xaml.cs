namespace MediaTracker
    {
        using System.Collections.Generic;
        using System.Windows;
        using System.Windows.Controls;

        /// <summary>
        /// The Main Window that holds the tabcontrol for each section of the program
        /// </summary>
        public partial class MainWindow
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="MainWindow"/> class.
                /// </summary>
                public MainWindow()
                    {
                        InitializeComponent();
                        PopulateSplitSearch(); // Add buttons to the splitButton
                    }

                /// <summary>
                /// Shows a specified control hovering over the MainWindow. 
                /// </summary>
                /// <param name="inputControl">The input control to be displayed.</param>
                public void ShowPopup(UserControl inputControl)
                    {
                        GridPopup.Children.Add(inputControl);
                        GridTabs.Visibility = Visibility.Hidden;
                        GridPopup.Visibility = Visibility.Visible;

                        // Creates event that runs when popup control is unloaded, to reset the grids
                        inputControl.Unloaded += PopupControl_Unloaded;
                    }

                private void PopupControl_Unloaded(object sender, RoutedEventArgs e) { ResetGrid(); }

                /// <summary>
                /// Resets the grid for the next control to use.
                /// </summary>
                public void ResetGrid()
                    {   
                        // Hide the popup and redislay the main content
                        GridPopup.Visibility = Visibility.Hidden;
                        GridTabs.Visibility = Visibility.Visible;

                        // Clear the Grid of any contents it may still have
                        GridPopup.Children.Clear();
                    }

                /// <summary>
                /// Refreshes the content. Default parameter is to refresh all tables, but can be specified.
                /// </summary>
                /// <param name="type">The type of table to refresh.</param>
                public void RefreshContent(string type = "All")
                    {
                        // Calls the method to refresh tables in each table control
                        switch ( type )
                            {
                                case "Movie":
                                    DbDisplayMovies.RefreshMedia();
                                    break;
                                case "Series":
                                    DbDisplayTelevision.RefreshMedia();
                                    break;
                                case "Book":
                                    DbDisplayBook.RefreshMedia();
                                    break;
                                case "All":
                                    DbDisplayMovies.RefreshMedia();
                                    DbDisplayTelevision.RefreshMedia();
                                    DbDisplayBook.RefreshMedia();
                                    break;
                            }
                    }

                /// <summary>
                /// Handles the Loaded event of the MainWindow control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void MainWindow_Loaded(object sender, RoutedEventArgs e)
                    {
                        // Sets this window as main for the ShowMessage function
                        Application.Current.MainWindow = this;

                        // Use the font type chosen by user
                        User.GetFont();

                        // Set users predefined theme
                        User.GetTheme();

                        // Hide the tab bar as it's only needed during design time
                        TabMenu.ItemContainerStyle = Utilities.HideTabBar();
                    }

                /// <summary>
                /// Handles the Unloaded event of the MainWindow control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
                    {
                        // Method that runs automatically when logging out

                        // Show log-off message
                        Utilities.ShowMessage("", "Logging off...");
                        GridPopup.Children.Clear();
                    }

                /// <summary>
                /// Populates the split search splitbutton with the buttons needed.
                /// Can't be done easily in XAML, so has to be run in code-behind at startup.
                /// </summary>
                private void PopulateSplitSearch()
                    {
                        // Create buttons
                        SplitButtonItem btnMovie = new SplitButtonItem { Text = "Movies" };
                        SplitButtonItem btnTv = new SplitButtonItem { Text = "Series" };
                        SplitButtonItem btnBook = new SplitButtonItem { Text = "Books" };

                        // Create List of the buttons
                        List <SplitButtonItem> buttonList = new List <SplitButtonItem> { btnMovie, btnTv, btnBook };

                        // Tell Search SplitButton to use it
                        SplitButtonSearch.ItemsSource = buttonList;
                    }

                /// <summary>
                /// Handles the Click event of the Search control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void Search_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid(); // start off with resetting the grids to remove the Popup if this is clicked

                        // Determine the selected search type from the SplitButtons index
                        string selectedItem = string.Empty;
                        switch ( SplitButtonSearch.SelectedIndex )
                            {
                                case 0:
                                    selectedItem = "Movie";
                                    break;
                                case 1:
                                    selectedItem = "Series";
                                    break;
                                case 2:
                                    selectedItem = "Book";
                                    break;
                            }

                        // Using the instance of ShowMedia created in the XAML
                        MediaControl.GetOnlineItemData(selectedItem, TxtSearch.Text);

                        // Switch to AddEntry tab to display the content
                        TabMenu.SelectedItem = TabAddEntry;
                    }


                /// <summary>
                /// Subclass with property needed to populate the SplitSearch button
                /// </summary>
                private class SplitButtonItem
                    {
                        public string Text { private get; set; }
                    }

                #region Buttons

                // Buttons to change tabs and reset the grid
                private void BtnMovies_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid();
                        TabMenu.SelectedItem = TabMovies;
                    }

                private void BtnTv_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid();
                        TabMenu.SelectedItem = TabTv;
                    }

                private void BtnBooks_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid();
                        TabMenu.SelectedItem = TabBooks;
                    }

                private void BtnAddEntry_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid();
                        TabMenu.SelectedItem = TabAddEntry;
                    }

                private void BtnUser_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid();
                        TabMenu.SelectedItem = TabUser;
                    }

                private void BtnSettings_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid();
                        TabMenu.SelectedItem = TabSettings;
                    }

                private void BtnInbox_Click(object sender, RoutedEventArgs e)
                    {
                        ResetGrid();
                        TabMenu.SelectedItem = TabRecommendations;
                    }
                #endregion Buttons
            }
    }