namespace MediaTracker
    {
        using System;
        using System.Collections.Generic;
        using System.Data;
        using System.Data.SQLite;
        using System.IO;
        using System.Windows;
        using System.Windows.Controls;


        /// <summary>
        /// Class to handle Login/Registration form that handles database connectivity to user table
        /// </summary>
        public partial class Login
            {

                /// <summary>
                /// Initializes a new instance of the Login class.
                /// </summary>
                public Login()
                    {
                        // Declares this window to be main for datacontext in Message Dialog method
                        Application.Current.MainWindow = this;
                        InitializeComponent();

                        // Hide the tab menu bar that is only needed while developing
                        TabLoginMenu.ItemContainerStyle = Utilities.HideTabBar();
                    }

                private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
                {
                    // Check if the database is in the users Documents folder
                    bool doesDbExist = File.Exists(string.Format("{0}\\database.db", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));

                    if (!doesDbExist)
                    {
                        // Show message and wait for response (that's why we have the unused variable)
                        bool hasRead = await Utilities.ShowMessage("Error", "Database does not exist!");
                        Application.Current.Shutdown(); // close
                    }
                }


                /// <summary>
                /// Try to login to the users account
                /// </summary>
                private void TryLogin()
                    {
                        string inputUsername = TxtUsername.Text;
                        string inputPassword = TxtPassword.Password;
                        bool loginSuccess = false;

                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Query database for user details
                                        DataTable dt =
                                            sh.Select(
                                                "SELECT * FROM user " + "WHERE UPPER(username) = UPPER(@user) " +
                                                "AND password = @pass;",
                                                new[]
                                                    {
                                                        new SQLiteParameter("@user", inputUsername),
                                                        new SQLiteParameter("@pass", inputPassword)
                                                    });

                                        // flag login as success and store user ID
                                        if ( dt.Rows.Count != 0 )
                                            {
                                                loginSuccess = true;
                                                User.CurrentUserId = Convert.ToInt32(dt.Rows[0]["id"]);
                                            }
                                    }
                            }

                        // If login successful start the MainWindow and close this
                        if ( loginSuccess )
                            {
                                MainWindow main = new MainWindow();
                                main.Show();
                                Close();
                            }
                        else Utilities.ShowMessage("Error", "Username or password is incorrect");
                    }

                /// <summary>
                /// Try to register a users account
                /// </summary>
                private void TryRegister()
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        string inputUsername = TxtRegisterUsername.Text;
                                        string inputPassword = TxtRegisterPassword1.Password;
                                        string inputPasswordCheck = TxtRegisterPassword2.Password;

                                        // Check that both passwords match and input isn't empty
                                        if ( inputPassword != inputPasswordCheck || inputPassword == "" ||
                                             inputPasswordCheck == "" || inputUsername == "" )
                                            {
                                                // if no match, or empty, stop
                                                Utilities.ShowMessage("Error", "Passwords do not match!");
                                                return;
                                            }

                                        // Check if username already exists
                                        DataTable dataUserConfirm =
                                            sh.Select("SELECT * FROM User WHERE UPPER(username) = UPPER(@user)",
                                                new[] { new SQLiteParameter("@user", inputUsername) });
                                        if ( dataUserConfirm.Rows.Count > 0 )

                                            // A row getting returned would indicate the user exists
                                            {
                                                Utilities.ShowMessage("Error", "Username has been taken.");
                                                return;
                                            }

                                        // Insert new user into table
                                        Dictionary <string, object> userDict = new Dictionary <string, object>();
                                        userDict["username"] = inputUsername;
                                        userDict["password"] = inputPassword;
                                        sh.Insert("user", userDict);
                                        sh.Execute("INSERT INTO settings DEFAULT VALUES;");

                                        Utilities.ShowMessage("Success", "Account created!");
                                    }
                            }
                    }

                /// <summary>
                /// Creates new instance of Login class to display to user that is logging out
                /// </summary>
                public static void Logout()
                    {
                        // Logging out will display login screen
                        Login logout = new Login();
                        logout.Show();
                    }

                private void Password_OnPasswordChanged(object sender, RoutedEventArgs e)
                    {
                        // Display a default text over the Password Boxes

                        // This could not be implemented into the XAML as a password
                        // box can't be binded to for security reasons.

                        // Get the name of the Watermark control
                        string passwordBoxName = ( (PasswordBox) sender ).Name;
                        string textBlockName = "Watermark" + passwordBoxName;

                        // Find the control using the name
                        TextBlock textBlockWatermark = (TextBlock) FindName(textBlockName);

                        // Stop if control is null
                        if ( textBlockWatermark == null ) return;

                        // Hide the watermark if user started typing
                        if ( ( (PasswordBox) sender ).SecurePassword.Length > 0 ) textBlockWatermark.Visibility = Visibility.Collapsed;
                        else textBlockWatermark.Visibility = Visibility.Visible;
                    }

                #region buttons
                private void BtnRegister_Click(object sender, RoutedEventArgs e) { TryRegister(); }
                private void BtnLogin_Click(object sender, RoutedEventArgs e) { TryLogin(); }

                private void BtnMenuLogin_Click(object sender, RoutedEventArgs e)
                    {
                        TabLoginMenu.SelectedItem = TabLogin;
                    }

                private void BtnMenuRegister_Click(object sender, RoutedEventArgs e)
                    {
                        TabLoginMenu.SelectedItem = TabRegister;
                    }
                #endregion


            }
    }