namespace MediaTracker.Controls
    {
        using System;
        using System.Data;
        using System.Data.SQLite;
        using System.Windows;
        using System.Windows.Controls;
        using System.Windows.Input;

        /// <summary>
        ///     Interaction logic for ShowRecommendations.xaml
        /// </summary>
        public partial class Recommendations
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="Recommendations"/> class.
                /// </summary>
                public Recommendations()
                    {
                        InitializeComponent();
                        LblNoRecs.Visibility = Visibility.Collapsed;
                        DataGridRecs.Visibility = Visibility.Visible;
                        GetRecommendations();
                    }

                /// <summary>
                /// Gets or sets the recommendations data table.
                /// </summary>
                /// <value>
                /// The recommendations data table.
                /// </value>
                private DataTable RecommendationsDataTable { get; set; }
                private bool HasRecommendations { get; set; }

                /// <summary>
                /// Gets the recommendations.
                /// </summary>
                private void GetRecommendations()
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
                                        RecommendationsDataTable =
                                            sh.Select(
                                                "SELECT recommendation.itemId, item.title, item.year, type.type, recommendation.date, user.username 'Sent by' " +
                                                "FROM recommendation " + "JOIN user ON recommendation.fromId = user.id " +
                                                "JOIN item ON recommendation.itemid = item.id " +
                                                "JOIN type ON item.typeId = type.id " + "WHERE userid = @userid",
                                                new[] { new SQLiteParameter("@userid", User.CurrentUserId) });

                                        DataGridRecs.ItemsSource = RecommendationsDataTable.DefaultView;

                                        // Set property to true if item has reviews
                                        int noOfRecommendations = RecommendationsDataTable.Rows.Count;
                                        HasRecommendations = noOfRecommendations > 0;

                                        // If no reviews, hide table and show message
                                        if ( !HasRecommendations )
                                            {
                                                DataGridRecs.Visibility = Visibility.Collapsed;
                                                LblNoRecs.Visibility = Visibility.Visible;
                                            }
                                    }
                            }
                    }

                /// <summary>
                /// Handles the OnAutoGeneratingColumn event of the DataGridRecs control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="DataGridAutoGeneratingColumnEventArgs"/> instance containing the event data.</param>
                private void DataGridRecs_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
                    {
                        if ( e.PropertyName == "itemid" ) e.Cancel = true;
                        e.Column.MinWidth = e.Column.ActualWidth;
                        e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    }

                /// <summary>
                /// Handles the OnMouseDoubleClick event of the DataGridRecs control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
                private void DataGridRecs_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
                    {
                        // Continue only if a ROW was clicked
                        DataGridRow row =
                            ItemsControl.ContainerFromElement((DataGrid) sender, e.OriginalSource as DependencyObject)
                            as DataGridRow;
                        if ( row == null ) return;

                        // Continue only if a valid COLUMN was clicked
                        string columnClicked = ( (DataGrid) sender ).CurrentCell.Column.Header.ToString();
                        if ( columnClicked.IsAnyOf("X") ) return;

                        // Get ID of item from row
                        DataRowView drView = DataGridRecs.SelectedItem as DataRowView;
                        if ( drView == null ) return;
                        DataRow itemData = drView.Row;
                        int itemId = Convert.ToInt32(itemData.ItemArray.GetValue(0));

                        ShowMedia showItem = new ShowMedia(itemId);
                        ( Application.Current.MainWindow as MainWindow ).ShowPopup(showItem);
                    }

                /// <summary>
                /// Removes the item.
                /// </summary>
                /// <param name="sender">The sender.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void RemoveItem(object sender, RoutedEventArgs e)
                    {
                        // Get itemID of the selected row
                        DataRow selectedItem = ( (DataRowView) DataGridRecs.SelectedItem ).Row;
                        int itemId = Convert.ToInt32(selectedItem.ItemArray.GetValue(0));

                        // Get the DataTable row of the row selected in the DataGrid
                        int index = DataGridRecs.Items.IndexOf(DataGridRecs.SelectedItem);
                        if ( index <= -1 ) return;

                        RecommendationsDataTable.Rows.RemoveAt(index); // Remove from datatable
                        ManipulateItem.RemoveItem(itemId, "recommendation"); // Remove from database
                    }

                /// <summary>
                /// Handles the OnAutoGeneratedColumns event of the DataGridRecs control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
                private void DataGridRecs_OnAutoGeneratedColumns(object sender, EventArgs e)
                    {
                        DataGridRecs.Columns[0].DisplayIndex = DataGridRecs.Columns.Count - 1;
                    }
            }
    }