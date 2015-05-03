namespace MediaTracker
    {
        using System;
        using System.ComponentModel;
        using System.Data;
        using System.Data.SQLite;
        using System.Windows;
        using System.Windows.Controls;
        using System.Windows.Input;

        /// <summary>
        /// Shows the datagrid for a specified status of a media type
        /// 
        /// Implements INotify, so that objects are automatically updated when the property is set
        /// </summary>
        public partial class MediaTable : INotifyPropertyChanged 
            {
                private bool isLoaded;
                private DataTable mediaDataTable;

                /// <summary>
                /// Initializes a new instance of the <see cref="MediaTable"/> class.
                /// </summary>
                /// <param name="inputType">Type of the input.</param>
                /// <param name="inputStatus">The input status.</param>
                public MediaTable(string inputType, string inputStatus)
                    {
                        InitializeComponent();

                        // Set the media type and status for this instance
                        Type = inputType;
                        Status = inputStatus;
                        UserId = User.CurrentUserId;

                        // Get appropriate content from database
                        RefreshTable();
                    }

                /// <summary>
                /// Initializes a new instance of the <see cref="MediaTable"/> class.
                /// </summary>
                /// <param name="inputType">Type of the input.</param>
                /// <param name="inputStatus">The input status.</param>
                /// <param name="inputUserid">The input userid.</param>
                public MediaTable(string inputType, string inputStatus, int inputUserid)
                    {
                        InitializeComponent();

                        // Set the media type and status for this instance
                        Type = inputType;
                        Status = inputStatus;
                        UserId = inputUserid;

                        RefreshTable();
                    }

                /// <summary>
                /// Gets or sets the media data table.
                /// Activates PropertyChanged to update any object linked to this to update
                /// </summary>
                /// <value>
                /// The media data table.
                /// </value>
                private DataTable MediaDataTable
                    {
                        get { return mediaDataTable; }
                        set
                            {
                                mediaDataTable = value;
                                OnPropertyChanged("mediaDataTable"); // Activate PropertyChanged notifier
                            }
                    }

                private string Type { get; set; }
                private string Status { get; set; }
                private int UserId { get; set; }

                /// <summary>
                /// Occurs when a property value changes.
                /// This tells the table control to refresh if the source is changed
                /// </summary>
                public event PropertyChangedEventHandler PropertyChanged;

                private void OnPropertyChanged(String info)
                    {
                        if ( PropertyChanged != null ) PropertyChanged(this, new PropertyChangedEventArgs(info));
                    }

                /// <summary>
                /// Handles the OnLoaded event of the DbControl control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void DbControl_OnLoaded(object sender, RoutedEventArgs e)
                    {
                        // Stop event firing after each tab click
                        if ( isLoaded ) return;
                        isLoaded = true;

                        RefreshTable();
                    }

                /// <summary>
                /// Refreshes or set the table.
                /// </summary>
                public void RefreshTable()
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Store the database contents to the DtMovie DataTable
                                        MediaDataTable =
                                            sh.Select(
                                                "select item.id, item.title, item.year, progress.rating, progress.progress 'itemprogress', status.id as 'statusid' " +
                                                "from progress " + "join user on progress.userId = user.id " +
                                                "JOIN item on progress.itemId = item.id " +
                                                "JOIN status on progress.statusId = status.id " +
                                                "JOIN type on item.typeId = type.id " +
                                                "WHERE user.id = @user AND type.type = @type AND status.status = @status " +
                                                "ORDER BY item.title ASC;",
                                                new[]
                                                    {
                                                        new SQLiteParameter("@user", UserId),
                                                        new SQLiteParameter("@type", Type),
                                                        new SQLiteParameter("@status", Status)
                                                    });
                                        DataGridItem.ItemsSource = MediaDataTable.DefaultView;
                                    }
                            }
                    }

                /// <summary>
                /// Handles the ColumnGeneration event of the DbControl control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="DataGridAutoGeneratingColumnEventArgs"/> instance containing the event data.</param>
                private void DbControl_ColumnGeneration(object sender, DataGridAutoGeneratingColumnEventArgs e)
                    {
                        // This event is started after the generation of each individual column

                        switch ( e.PropertyName )
                            {
                                case "id":
                                case "Rating":
                                case "itemprogress":
                                case "statusid":
                                    {
                                        // Cancel generation of thee columns as they are binded to controls
                                        e.Cancel = true;
                                        break;
                                    }
                                case "Year":
                                    e.Column.CanUserResize = false;
                                    e.Column.MinWidth = 45;
                                    e.Column.MaxWidth = 45;
                                    break;
                            }
                    }

                /// <summary>
                /// Handles the TableGeneration event of the DbControl control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
                private void DbControl_TableGeneration(object sender, EventArgs eventArgs)
                    {
                        // This event is started after the generation of the entire table

                        // Move controls to end and set Title column width
                        DataGridItem.Columns[0].DisplayIndex = DataGridItem.Columns.Count - 1;
                        DataGridItem.Columns[1].DisplayIndex = DataGridItem.Columns.Count - 1;
                        DataGridItem.Columns[2].DisplayIndex = DataGridItem.Columns.Count - 1;
                        DataGridItem.Columns[3].DisplayIndex = DataGridItem.Columns.Count - 1;

                        // Hide PROGRESS column from Movie and Book tables
                        if ( Type == "Movie" || Type == "Book" ) DataGridItem.Columns[1].Visibility = Visibility.Collapsed;

                        // Hide controls if current table doesn't belong to logged in user
                        if ( UserId != User.CurrentUserId )
                            {
                                DataGridItem.Columns[0].Visibility = Visibility.Collapsed;
                                DataGridItem.Columns[1].Visibility = Visibility.Collapsed;
                                DataGridItem.Columns[2].Visibility = Visibility.Collapsed;
                                DataGridItem.Columns[3].Visibility = Visibility.Collapsed;
                            }

                        // Assign all text columns with the Word Wrapping style
                        foreach ( DataGridColumn column in DataGridItem.Columns )
                            {
                                // Set style only if column is for text (not a control)
                                if ( column is DataGridTextColumn )
                                    {
                                        DataGridTextColumn textColumn = column as DataGridTextColumn;
                                        textColumn.ElementStyle = DataGridItem.Resources["WordWrapStyle"] as Style;

                                        // Set columns to resize with window
                                        textColumn.MinWidth = column.ActualWidth;
                                        textColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                                    }
                            }
                    }

                /// <summary>
                /// Updates the progress.
                /// </summary>
                /// <param name="sender">The sender.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void UpdateProgress(object sender, RoutedEventArgs e)
                    {
                        // Get row title, instruction and the datagrid row
                        string title = ( (Button) sender ).CommandParameter.ToString();
                        string instruction = ( (Button) sender ).Tag.ToString();
                        int currentRow = DataGridItem.Items.IndexOf(DataGridItem.CurrentItem);

                        DataRow item = ( (DataRowView) DataGridItem.SelectedItem ).Row;
                        int itemId = Convert.ToInt32(item.ItemArray.GetValue(0));

                        // Set new progress
                        if ( itemId == 0 ) Utilities.ShowMessage("ERROR", "Can't find the item");
                        else
                            {
                                int newProgress = 0;
                                int currentProgress = Convert.ToInt32(MediaDataTable.Rows[currentRow]["itemprogress"]);

                                if ( instruction == "Increment" ) newProgress = currentProgress + 1;
                                else if ( instruction == "Decrement" && currentProgress != 0 ) newProgress = currentProgress - 1;

                                // Submit to database and update local datatable
                                ManipulateItem.SetProgress(newProgress, itemId);
                                MediaDataTable.Rows[currentRow]["itemprogress"] = newProgress;
                            }
                    }

                /// <summary>
                /// Removes the item.
                /// </summary>
                /// <param name="sender">The sender.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void RemoveItem(object sender, RoutedEventArgs e)
                    {
                        // Get itemID of the selected row
                        DataRow selectedItem = ( (DataRowView) DataGridItem.SelectedItem ).Row;
                        int itemId = Convert.ToInt32(selectedItem.ItemArray.GetValue(0));

                        // Get the DataTable row of the row selected in the DataGrid
                        int index = DataGridItem.Items.IndexOf(DataGridItem.SelectedItem);
                        if ( index <= -1 ) return;

                        MediaDataTable.Rows.RemoveAt(index); // Remove from datatable
                        ManipulateItem.RemoveItem(itemId); // Remove from database
                    }

                /// <summary>
                /// Handles the MouseDoubleClick event of the DataGrid control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
                private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
                    {
                        // Continue only if a ROW was clicked
                        DataGridRow row =
                            ItemsControl.ContainerFromElement((DataGrid) sender, e.OriginalSource as DependencyObject)
                            as DataGridRow;
                        if ( row == null ) return;

                        // Continue only if a valid COLUMN was clicked
                        string columnClicked = ( (DataGrid) sender ).CurrentCell.Column.Header.ToString();
                        if ( columnClicked.IsAnyOf("", "Progress", "Score", "X") ) return;

                        // Get ID of item from row
                        DataRowView drView = DataGridItem.SelectedItem as DataRowView;
                        if ( drView == null ) return;
                        DataRow itemData = drView.Row;
                        int itemId = Convert.ToInt32(itemData.ItemArray.GetValue(0));

                        ShowMedia showItem = new ShowMedia(itemId);
                        ( Application.Current.MainWindow as MainWindow ).ShowPopup(showItem);
                    }

                /// <summary>
                /// Handles the OnValueChanged event of the RatingScore control.
                /// </summary>
                private void RatingScore_OnValueChanged(object sender, RoutedPropertyChangedEventArgs <double?> e)
                    {
                        // Get data from row to be updated
                        DataRowView drView = DataGridItem.SelectedItem as DataRowView;
                        if ( drView == null ) return;
                        DataRow itemData = drView.Row;
                        if ( itemData == null ) return;

                        // Get ID of item to be updated
                        int itemId = Convert.ToInt32(itemData.ItemArray.GetValue(0));

                        // Update rating
                        ManipulateItem.SetRating(e.NewValue, itemId);
                    }

                /// <summary>
                /// Handles the OnSelectionChanged event of the Selector control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
                private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
                    {
                        // Get the new seleted status and the item ID
                        DataRowView drView = DataGridItem.SelectedItem as DataRowView;
                        if ( drView == null ) return;
                        DataRow itemData = drView.Row;
                        if ( itemData == null ) return;
                        ComboBox statusBox = (ComboBox) sender;
                        ComboBoxItem statusBoxItem = (ComboBoxItem) statusBox.SelectedItem;
                        string newStatus = statusBoxItem.Name;
                        int itemId = Convert.ToInt32(itemData.ItemArray.GetValue(0));

                        // Update database and table
                        ManipulateItem.SetStatus(newStatus, itemId);
                        if ( ( Type == "Movie" || Type == "Book" ) && newStatus == "Completed" )
                            {
                                // Set progress of movie to 1 for statistical purposes
                                ManipulateItem.SetProgress(1, itemId);
                            }
                        else if ( ( Type == "Movie" || Type == "Book" ) && newStatus != "Completed" )
                            {
                                // Set progress of movie or book to 0 if not completed
                                ManipulateItem.SetProgress(0, itemId);
                            }
                        ( (MainWindow) Application.Current.MainWindow ).RefreshContent(Type);
                    }
            }
    }