namespace MediaTracker
    {
        using System;
        using System.Windows;

        /// <summary>
        /// Settings window to bind the User class to the UI
        /// </summary>
        public partial class Settings
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="Settings"/> class.
                /// </summary>
                public Settings() { InitializeComponent(); }

                private void Submit_Click(object sender, RoutedEventArgs e)
                    {
                        // Set new theme
                        string themeAccent = ComboAccent.Text;
                        string themeBg = ComboBg.Text;
                        User.SetTheme(themeAccent, themeBg);

                        // Set new font
                        int fontSize = Convert.ToInt32(ComboFontSize.Text);
                        string fontType = ComboFontType.Text;
                        User.SetFont(fontSize, fontType);
                    }
            }
    }