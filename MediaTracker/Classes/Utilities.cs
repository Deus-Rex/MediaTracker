namespace MediaTracker
    {
        using System.Linq;
        using System.Threading.Tasks;
        using System.Windows;
        using System.Windows.Controls;
        using System.Windows.Media;
        using MahApps.Metro.Controls;
        using MahApps.Metro.Controls.Dialogs;

        /// <summary>
        ///  Class to store specific utilities that are used throughout the program
        /// </summary>
        public static class Utilities
            {
                /// <summary>
                /// Hides the tab header bar, for whichever tab bar sets this as its style.
                /// This becomes useful as I am using tabs throughout which don't need the tab headers.
                /// </summary>
                /// <returns></returns>
                public static Style HideTabBar()
                    {
                        // Set and return new style
                        Style hideTabBar = new Style();
                        hideTabBar.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
                        return hideTabBar;
                    }

                /// <summary>
                /// This is a helper method to simplify accessing the MetroStyleDialog method below
                /// </summary>
                /// <param name="title">The title.</param>
                /// <param name="message">The message.</param>
                public static async Task <bool> ShowMessage(string title, string message)
                    {
                        await
                            MetroStyleDialog(title, message, MessageDialogStyle.AffirmativeAndNegative).
                                ConfigureAwait(false);

                        return true;
                    }

                /// <summary>
                /// This displays a popop messagebox contained within the window, resembling that of
                /// Windows 8 applications.
                /// </summary>
                /// <param name="title">The title.</param>
                /// <param name="message">The message.</param>
                /// <param name="dialogStyle">The dialog style.</param>
                /// <returns></returns>
                private static async Task <MessageDialogResult> MetroStyleDialog(string title, string message,
                                                                                 MessageDialogStyle dialogStyle)
                    {
                        // This can't be accessed properly from other classes
                        // so will be accessed via the ShowMessage method

                        // Get the window set as Main
                        MetroWindow metroWindow = ( Application.Current.MainWindow as MetroWindow );

                        // Set dialog to use the theme accent
                        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;

                        // Show message
                        return
                            await
                            metroWindow.ShowMessageAsync(title, message, dialogStyle, metroWindow.MetroDialogOptions);
                    }

                /// <summary>
                /// This displays a popop input box contained within the window, resembling that of
                /// Windows 8 applications.                /// </summary>
                /// <param name="title">The title.</param>
                /// <param name="message">The message.</param>
                /// <returns></returns>
                public static async Task <string> MetroStyleInput(string title, string message)
                    {
                        // Get the window set as Main
                        MetroWindow metroWindow = ( Application.Current.MainWindow as MetroWindow );
                        return await metroWindow.ShowInputAsync(title, message);
                    }

                /// <summary>
                /// Created a custom Extension method that makes comparing values simpler
                /// It will return true if a variable matches any from a list of text
                /// i.e. ( variable == "a" || variable == "b" || variable == "c" ) but a lot shorter
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="obj">The object.</param>
                /// <param name="collection">The collection.</param>
                /// <returns></returns>
                public static bool IsAnyOf <T>(this T obj, params T[] collection)
                    {
                        return collection.Contains(obj);
                    }
            }
    }