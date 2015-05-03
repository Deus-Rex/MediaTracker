namespace MediaTracker.Controls
    {
        using System;
        using System.Windows;

        public partial class MediaTypes
            {
                private bool isLoaded;
                private Db Media;
                public MediaTypes() { InitializeComponent(); }

                /// <summary>
                /// Handles the OnLoaded event of the DbDisplay control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void DbDisplay_OnLoaded(object sender, RoutedEventArgs e)
                    {
                        // Stop event firing after each tab click
                        if ( isLoaded ) return;
                        isLoaded = true;

                        // Creates a new instance of Db for this media type
                        // this creates a table for each status of the media type
                        Media = new Db(MediaType);
                        GridComplete.Children.Add(Media.Complete);
                        GridOngoing.Children.Add(Media.Ongoing);
                        GridDropped.Children.Add(Media.Dropped);
                        GridPlanned.Children.Add(Media.Planned);
                    }

                /// <summary>
                /// Refreshes the tables.
                /// </summary>
                public void RefreshMedia()
                    {
                        Media.Complete.RefreshTable();
                        Media.Planned.RefreshTable();
                        Media.Dropped.RefreshTable();
                        Media.Ongoing.RefreshTable();
                    }

                #region Dependency Property
                /// <summary>
                /// The media type property
                /// The control has the tag     MediaType="*type*"      which is picked up by this
                /// dependency property to detect what this class should base around
                /// </summary>
                public static readonly DependencyProperty MediaTypeProperty = DependencyProperty.Register("MediaType",
                    typeof (string), typeof (MediaTypes), new PropertyMetadata(""));

                /// <summary>
                /// Gets or sets the type of the media.
                /// </summary>
                /// <value>
                /// The type of the media.
                /// </value>
                public String MediaType
                    {
                        get { return (String) GetValue(MediaTypeProperty); }
                        set { SetValue(MediaTypeProperty, value); }
                    }
                #endregion
            }
    }