namespace MediaTracker
    {
        using System;
        using System.Windows;

        /// <summary>
        ///     Class to store the database location and the datagrid property instances
        ///     This class is instantiated four times, to have a table for each status, for each media type
        /// </summary>
        public class Db
            {
                /// <summary>
                ///     Location of the database table along with option to enable foreign keys.
                ///     This can be accessed as if it was static.
                /// </summary>
               // public const string TableLocation = "Data Source=C:\\database.db;foreign keys=true;";
                public static readonly string TableLocation = string.Format("Data Source={0}\\database.db;foreign keys=true;", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

                /// <summary>
                ///     Control to display the Completed items of a media type
                /// </summary>
                public readonly MediaTable Complete;

                /// <summary>
                ///     Control to display the Dropped items of a media type
                /// </summary>
                public readonly MediaTable Dropped;

                /// <summary>
                ///     Control to display the Ongoing items of a media type
                /// </summary>
                public readonly MediaTable Ongoing;

                /// <summary>
                ///     Control to display the Planned items of a media type
                /// </summary>
                public readonly MediaTable Planned;

                /// <summary>
                ///     Initializes a new instance of the class, while setting the properties
                ///     to become instances for the appropriate media type and status
                /// </summary>
                /// <param name="mediaType">Type of the media to create instances for.</param>
                public Db(string mediaType)
                    {
                        Complete = new MediaTable(mediaType, "Complete");
                        Ongoing = new MediaTable(mediaType, "Ongoing");
                        Dropped = new MediaTable(mediaType, "Dropped");
                        Planned = new MediaTable(mediaType, "Planned");
                    }
            }
    }