namespace MediaTracker
    {
        using System;
        using System.Data;
        using System.Windows;
        using System.Windows.Controls;
        using System.Windows.Input;

        /// <summary>
        /// A datagrid that displays a list of registered users
        /// </summary>
        public partial class ListUsers
            {
                private readonly bool returnUserId;

                /// <summary>
                /// Initializes a new instance of the <see cref="ListUsers"/> class.
                /// </summary>
                public ListUsers()
                    {
                        InitializeComponent();

                        ShowUsers();
                    }

                /// <summary>
                /// Initializes a new instance of the <see cref="ListUsers"/> class.
                /// This one is specifically to return an ID from the list, rather than display the user profile
                /// </summary>
                /// <param name="returnId">if set to <c>true</c> [return identifier].</param>
                public ListUsers(bool returnId)
                    {
                        InitializeComponent();
                        ClickedUserId = 0;
                        returnUserId = returnId;
                        ShowUsers();
                    }

                /// <summary>
                /// Gets or sets the clicked user identifier.
                /// </summary>
                /// <value>
                /// The clicked user identifier.
                /// </value>
                public int ClickedUserId { get; private set; }

                /// <summary>
                /// Shows the users ro the datagrid
                /// </summary>
                private void ShowUsers()
                    {
                        // Get the list of users
                        User.GetUsers();

                        // Bind the table to the datagrid 
                        DataGridUsers.ItemsSource = User.UserList.DefaultView;
                    }

                /// <summary>
                /// Handle the data depending on the purpose of this control instance
                /// </summary>
                private void UseData()
                    {
                        if ( returnUserId )
                            {
                                // Call close control method
                                ( Parent as Grid ).Children.Remove(this);
                            }
                        else
                            {
                                // Show User info
                                UserProfile showUserProfile = new UserProfile(ClickedUserId);
                                GridUserProfilePopup.Children.Add(showUserProfile);
                                GridUserProfilePopup.Visibility = Visibility.Visible;
                                GridShowData.Visibility = Visibility.Collapsed;

                                showUserProfile.Unloaded += ShowProfile_Unloaded; // event
                            }
                    }

                /// <summary>
                /// Handles the Unloaded event of the ShowProfile control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void ShowProfile_Unloaded(object sender, RoutedEventArgs e)
                    {
                        GridUserProfilePopup.Visibility = Visibility.Collapsed;
                        GridShowData.Visibility = Visibility.Visible;
                    }

                /// <summary>
                /// Handles the MouseDoubleClick event of the DataGrid control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
                private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
                    {
                        // Get the data from the selected row of datagrid
                        DataRowView drView = DataGridUsers.SelectedItem as DataRowView;
                        if ( drView == null ) return;
                        DataRow item = drView.Row;

                        // Get the ID of the item selected and pass to control
                        ClickedUserId = Convert.ToInt32(item.ItemArray.GetValue(0));

                        UseData();
                    }
            }
    }