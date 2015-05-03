namespace MediaTracker.Controls
    {
        using System.Data;
        using System.Data.SQLite;
        using System.Windows.Controls;

        /// <summary>
        ///     Interaction logic for ShowReviews.xaml
        /// </summary>
        public partial class ShowReviews : UserControl
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="ShowReviews"/> class.
                /// </summary>
                /// <param name="itemId">The item identifier.</param>
                public ShowReviews(int itemId)
                    {
                        InitializeComponent();

                        GetReviews(itemId);
                        testListBox.ItemsSource = Reviews.DefaultView;

                        UpdateLayout();
                    }

                private DataTable Reviews { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether this instance has reviews.
                /// </summary>
                /// <value>
                /// <c>true</c> if this instance has reviews; otherwise, <c>false</c>.
                /// </value>
                public bool HasReviews { get; private set; }

                /// <summary>
                /// Gets the reviews and stores them in the Reviews property
                /// </summary>
                /// <param name="itemId">The item identifier.</param>
                private void GetReviews(int itemId)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Store the database contents to the UserList datatable
                                        Reviews =
                                            sh.Select(
                                                "SELECT review.id, user.username, progress.rating, review.date, review.review " +
                                                "FROM review " + 
                                                "JOIN user ON user.ID = review.userId " +
                                                "JOIN progress ON progress.UserId = review.userid AND progress.ItemId = review.itemid " +
                                                "WHERE review.review <> '' AND review.itemId = @itemid ;", 
                                                new[] { new SQLiteParameter("@itemid", itemId) });

                                        // Set property to true if item has reviews
                                        int noOfReviews = Reviews.Rows.Count;
                                        HasReviews = noOfReviews > 0;
                                    }
                            }
                    }
            }
    }