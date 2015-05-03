namespace MediaTracker
    {
        using System;
        using System.Collections.Generic;
        using System.Data;
        using System.Data.SQLite;
        using System.Windows;
        using System.Windows.Media;
        using MahApps.Metro;

        public static class User
            {
                /// <summary>
                /// Gets or sets the current user identifier.
                /// </summary>
                public static int CurrentUserId { get; set; } // This is a c# auto-property, a shorter getter/setter
                
                /// <summary>
                /// Gets/sets the list of users registered
                /// </summary>
                public static DataTable UserList { get; private set; }

                /// <summary>
                /// Queries the database for list of users and stores it in the UserList property
                /// </summary>
                public static void GetUsers()
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
                                        UserList =
                                            sh.Select(
                                                "SELECT id, username, registerdate " + "FROM user " +
                                                "WHERE id != @currentuser;",
                                                new[] { new SQLiteParameter("@currentuser", CurrentUserId) });
                                        conn.Close();
                                    }
                            }
                    }

                /// <summary>
                /// Sets the theme
                /// </summary>
                /// <param name="themeAccent">The theme accent.</param>
                /// <param name="themeBg">The theme bg.</param>
                private static void UseTheme(string themeAccent, string themeBg)
                    {
                        ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(themeAccent),
                            ThemeManager.GetAppTheme(themeBg));
                    }

                /// <summary>
                /// Gets the theme from the users settings table
                /// </summary>
                public static void GetTheme()
                    {
                        // Get the users preferred theme from the database

                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Setup database connection
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Get users settings from database
                                        DataTable dt = sh.Select("SELECT * FROM settings WHERE id = @userid;",
                                            new[] { new SQLiteParameter("@userid", CurrentUserId) });

                                        if ( dt.Rows.Count != 0 )
                                            {
                                                // Get users theme
                                                string themeAccent = dt.Rows[0]["themeAccent"].ToString();
                                                string themeBg = dt.Rows[0]["themeBG"].ToString();

                                                // Apply theme
                                                SetTheme(themeAccent, themeBg);
                                            }
                                    }
                            }
                    }

                /// <summary>
                /// Saves the theme to the users settins table
                /// </summary>
                /// <param name="themeAccent">The theme accent.</param>
                /// <param name="themeBg">The theme bg.</param>
                public static void SetTheme(string themeAccent, string themeBg)
                    {
                        // Save the users theme to database

                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Setup database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Upload data to database
                                        Dictionary <string, object> settings = new Dictionary <string, object>();
                                        settings["themeAccent"] = themeAccent;
                                        settings["themeBG"] = themeBg;
                                        sh.Update("settings", settings, "id", CurrentUserId);

                                        // Apply theme
                                        UseTheme(themeAccent, themeBg);
                                    }
                            }
                    }

                /// <summary>
                /// Gets the font from the users settings table
                /// </summary>
                public static void GetFont()
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Setup database connection
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Get users settings from database
                                        DataTable dt = sh.Select("SELECT * FROM settings WHERE id = @userid;",
                                            new[] { new SQLiteParameter("@userid", CurrentUserId) });

                                        if ( dt.Rows.Count == 0 ) return;

                                        // Get users theme
                                        int fontSize = Convert.ToInt32(dt.Rows[0]["fontSize"]);
                                        string fontType = dt.Rows[0]["fontType"].ToString();

                                        UseFont(fontSize, fontType);
                                    }
                            }
                    }

                /// <summary>
                /// Stores the font in the users settings table
                /// </summary>
                /// <param name="fontSize">Size of the font.</param>
                /// <param name="fontType">Type of the font.</param>
                public static void SetFont(int fontSize, string fontType)
                    {
                        // Save the users theme to database

                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Setup database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Upload data to database
                                        Dictionary <string, object> settings = new Dictionary <string, object>();
                                        settings["fontSize"] = fontSize;
                                        settings["fontType"] = fontType;
                                        sh.Update("settings", settings, "id", CurrentUserId);

                                        // Apply font
                                        UseFont(fontSize, fontType);
                                    }
                            }
                    }

                /// <summary>
                /// Uses the font provided
                /// </summary>
                /// <param name="fontSize">Size of the font.</param>
                /// <param name="fontType">Type of the font.</param>
                private static void UseFont(int fontSize, string fontType)
                    {
                        ( Application.Current.MainWindow as MainWindow ).FontSize = fontSize;
                        ( Application.Current.MainWindow as MainWindow ).FontFamily = new FontFamily(fontType);
                    }
            }
    }