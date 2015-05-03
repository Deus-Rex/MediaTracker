namespace MediaTracker
    {
        using System;
        using System.Windows;

        /// <summary>
        /// Youtube video player
        /// </summary>
        public partial class VideoPopup
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="VideoPopup"/> class.
                /// It displays a youtube video.
                /// </summary>
                /// <param name="trailerUrl">The trailer URL.</param>
                public VideoPopup(Uri trailerUrl)
                    {
                        InitializeComponent();

                        // WebTrailer is an embedable browser that shows the youtube video
                        WebTrailer.Source = trailerUrl;
                    }

                /// <summary>
                /// Handles the OnUnloaded event of the VideoPopup control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
                private void VideoPopup_OnUnloaded(object sender, RoutedEventArgs e)
                    {
                        // When window closed, kill the video, otherwise the sound will continue
                        WebTrailer.Dispose();                        
                    }
            }
    }