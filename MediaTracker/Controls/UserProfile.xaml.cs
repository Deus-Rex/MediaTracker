namespace MediaTracker
    {
        using System;
        using System.Data;
        using System.Data.SQLite;
        using System.Globalization;
        using System.Text.RegularExpressions;
        using System.Windows;
        using System.Windows.Controls;

        /// <summary>
        /// Show the current users profile or a predefined profile
        /// </summary>
        public partial class UserProfile
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="UserProfile"/> class.
                /// </summary>
                public UserProfile()
                    {
                        // Default constructor shows logged in user
                        InitializeComponent();
                        BtnBack.Visibility = Visibility.Collapsed;
                        BtnLogout.Visibility = Visibility.Visible;
                        ExpanderShowMediaButtons.Visibility = Visibility.Collapsed;
                        ShowUser(User.CurrentUserId);
                        ShowStats(User.CurrentUserId);
                    }

                /// <summary>
                /// Initializes a new instance of the <see cref="UserProfile"/> class.
                /// Shows the profile of a user based on their User ID
                /// </summary>
                /// <param name="userid">The userid.</param>
                public UserProfile(int userid)
                    {
                        // Default constructor shows logged in user
                        InitializeComponent();
                        BtnLogout.Visibility = Visibility.Hidden;
                        ShowUser(userid);
                        ShowStats(userid);

                        Userid = userid; // store the userid throughout
                    }

                private int Userid { get; set; }

                /// <summary>
                /// Get the users data and display it
                /// </summary>
                /// <param name="userid">The userid.</param>
                private void ShowUser(int userid)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Setup database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Query database for user details
                                        DataTable dt = sh.Select("SELECT username, registerdate FROM user WHERE id = @user",
                                            new[] { new SQLiteParameter("@user", userid) });

                                        // If data is returned, display it
                                        if ( dt.Rows.Count == 0 ) return;
                                        LblUsername.Content = dt.Rows[0].ItemArray[0].ToString();
                                        LblDateRegistered.Content = dt.Rows[0].ItemArray[1].ToString();
                                    }
                            }
                    }

                /// <summary>
                /// Show the users media stats
                /// </summary>
                /// <param name="userid">The userid.</param>
                private void ShowStats(int userid)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Setup database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Query database for user details
                                        DataTable dt =
                                            sh.Select(
                                                "SELECT type.type, ROUND(SUM(item.length), 2) 'Total Length', SUM(progress.Progress) 'Number Watched', " +
                                                " ROUND(SUM(item.length * progress.Progress)/60.0/24.0, 2) 'Days Total' " +
                                                "FROM progress " + "JOIN item ON progress.itemId = item.id " +
                                                "JOIN type ON item.typeId = type.id " +
                                                "WHERE progress.userId = @user AND progress.statusId IN (0, 1, 2) " +
                                                "GROUP BY item.typeId", new[] { new SQLiteParameter("@user", userid) });

                                        // If data is returned, display it
                                        if ( dt.Rows.Count == 0 ) return;

                                        float movieTime = 0, movieNumber = 0;
                                        float seriesTime = 0, seriesNumber = 0;
                                        float bookTime = 0, bookNumber = 0;

                                        for ( int i = 0; i < dt.Rows.Count; i++ )
                                            {
                                                string type = dt.Rows[i].ItemArray[0].ToString();
                                                
                                                // To parse the data without it being rounded
                                                NumberFormatInfo decimalpoint = CultureInfo.InvariantCulture.NumberFormat;
                                                
                                                // Parse the contents to appropriate variables
                                                switch ( type )
                                                    {
                                                        case "Movie":
                                                            movieTime = float.Parse(dt.Rows[i].ItemArray[3].ToString(),
                                                                decimalpoint);
                                                            movieNumber = float.Parse(
                                                                dt.Rows[i].ItemArray[2].ToString(), decimalpoint);
                                                            break;
                                                        case "Series":
                                                            seriesTime = float.Parse(
                                                                dt.Rows[i].ItemArray[3].ToString(), decimalpoint);
                                                            seriesNumber =
                                                                float.Parse(dt.Rows[i].ItemArray[2].ToString(),
                                                                    decimalpoint);
                                                            break;
                                                        case "Book":
                                                            bookTime = float.Parse(dt.Rows[i].ItemArray[3].ToString(),
                                                                decimalpoint);
                                                            bookNumber = float.Parse(
                                                                dt.Rows[i].ItemArray[2].ToString(), decimalpoint);
                                                            break;
                                                    }
                                            }

                                        // Display the contents
                                        LblStatMovie.Content =
                                            String.Format("{0} Movies Added, spending {1} days in total.", movieNumber,
                                                movieTime);
                                        LblStatSeries.Content =
                                            String.Format("{0} Episodes Added, spending {1} days in total.",
                                                seriesNumber, seriesTime);
                                        LblStatBook.Content = String.Format("{0} Books Added, at {1} pages in total.",
                                            bookNumber, bookTime);
                                    }
                            }
                    }

                private void BtnLogout_Click(object sender, RoutedEventArgs e)
                    {
                        // Open the Login window
                        Login.Logout();

                        // Close the Main window
                        Window parent = Window.GetWindow(this);
                        if ( parent != null ) parent.Close();
                    }

                private void BtnBack_Click(object sender, RoutedEventArgs e) { ( Parent as Grid ).Children.Clear(); }

                /// <summary>
                /// Accesses the MainWindows popup and shows the users media table
                /// </summary>
                /// <param name="type">The type.</param>
                /// <param name="status">The status.</param>
                private void ShowUsersMedia(string type, string status)
                    {
                        MediaTable showTable = new MediaTable(type, status, Userid);
                        ( Application.Current.MainWindow as MainWindow ).ShowPopup(showTable);
                    }

                /// <summary>
                /// Handles the OnClick event of the MediaButton control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void MediaButton_OnClick(object sender, RoutedEventArgs e)
                    {
                        // Get item type and status from the name of button that called event
                        string name = ( (Button) sender ).Name;
                        Regex splitter = new Regex(@"(?<!^)(?=[A-Z])");
                        string[] instruction = splitter.Split(name);
                        string type = instruction[0];
                        string status = instruction[1];

                        ShowUsersMedia(type, status);
                    }
            }
    }