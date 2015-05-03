namespace MediaTracker.Controls
    {
        using System;
        using System.Data;
        using System.Data.SQLite;
        using System.Windows;
        using System.Windows.Controls;
        using System.Windows.Input;

        /// <summary>
        ///     Interaction logic for PopularItems.xaml
        /// </summary>
        public partial class PopularItems
            {
                public PopularItems()
                    {
                        InitializeComponent();
                        DataGridPopularItems.Visibility = Visibility.Visible;
                        GetPopularItems();
                    }

                /// <summary>
                /// Gets or sets the popular items data table.
                /// </summary>
                /// <value>
                /// The popular items data table.
                /// </value>
                private DataTable PopularItemsDataTable { get; set; }

                /// <summary>
                /// Gets the top 10 ranked items from the database
                /// </summary>
                private void GetPopularItems()
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
                                        PopularItemsDataTable =
                                            sh.Select("SELECT item.id, item.title, item.year, item.genre, type.type " +
                                                      "FROM item " + "JOIN type ON type.id = item.typeId " +
                                                      "ORDER BY item.imdbRating DESC " + "LIMIT 10");

                                        // Set item source so that DataGrid updates
                                        DataGridPopularItems.ItemsSource = PopularItemsDataTable.DefaultView;
                                    }
                            }
                    }

                /// <summary>
                /// Handles the OnAutoGeneratingColumn event of the DataGridPopularItems control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="DataGridAutoGeneratingColumnEventArgs"/> instance containing the event data.</param>
                private void DataGridPopularItems_OnAutoGeneratingColumn(object sender,
                                                                         DataGridAutoGeneratingColumnEventArgs e)
                    {
                        if ( e.PropertyName == "id" ) e.Cancel = true;

                        e.Column.MinWidth = e.Column.ActualWidth;
                        e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    }

                /// <summary>
                /// Handles the OnMouseDoubleClick event of the DataGridPopularItems control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
                private void DataGridPopularItems_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                        DataRowView drView = DataGridPopularItems.SelectedItem as DataRowView;
                        if ( drView == null ) return;
                        DataRow itemData = drView.Row;
                        int itemId = Convert.ToInt32(itemData.ItemArray.GetValue(0));

                        ShowMedia ShowItem = new ShowMedia(itemId);

                        ( Application.Current.MainWindow as MainWindow ).ShowPopup(ShowItem);
                    }
            }
    }