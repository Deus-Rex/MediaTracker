namespace MediaTracker
    {
        using System;
        using System.Collections.Generic;
        using System.Data;
        using System.Data.SQLite;

        /// <summary>
        /// Class that contains methods that manipulate data relating to media items
        /// </summary>
        internal static class ManipulateItem
            {

                /// <summary>
                /// Sets the rating.
                /// </summary>
                /// <param name="newRating">The new rating.</param>
                /// <param name="itemId">The item identifier.</param>
                public static void SetRating(double? newRating, int itemId)
                    {
                        string tableName = "progress";
                        string columnName = "rating";
                        string columnValue = newRating.ToString();
                        string condition = "itemId";
                        int conditionValue = itemId;

                        UpdateData(tableName, columnName, columnValue, condition, conditionValue);
                    }

                /// <summary>
                /// Sets the review.
                /// </summary>
                /// <param name="newReview">The new review.</param>
                /// <param name="itemId">The item identifier.</param>
                public static void SetReview(string newReview, int itemId)
                    {
                        string tableName = "Review";
                        string columnName = "review";
                        string columnValue = newReview;
                        string condition = "itemId";
                        int conditionValue = itemId;

                        UpdateData(tableName, columnName, columnValue, condition, conditionValue);
                    }

                /// <summary>
                /// Sets the status.
                /// </summary>
                /// <param name="newStatus">The new status.</param>
                /// <param name="itemId">The item identifier.</param>
                public static void SetStatus(string newStatus, int itemId)
                    {
                        // Change or set the status of an item

                        int statusId = 3; // Default status is Planned

                        // Set statusId
                        switch ( newStatus )
                            {
                                case "Complete":
                                    statusId = 0;
                                    break;

                                case "Ongoing":
                                    statusId = 1;
                                    break;

                                case "Dropped":
                                    statusId = 2;
                                    break;

                                case "Planned":
                                    statusId = 3;
                                    break;
                            }

                        string tableName = "progress";
                        string columnName = "statusId";
                        string columnValue = statusId.ToString();
                        string condition = "itemId";
                        int conditionValue = itemId;

                        UpdateData(tableName, columnName, columnValue, condition, conditionValue);
                    }

                /// <summary>
                /// Sets the progress.
                /// </summary>
                /// <param name="newValue">The new value.</param>
                /// <param name="itemId">The item identifier.</param>
                public static void SetProgress(int newValue, int itemId)
                    {
                        // Set the progress of an item

                        string tableName = "progress";
                        string columnName = "progress";
                        string columnValue = newValue.ToString();
                        string condition = "itemId";
                        int conditionValue = itemId;

                        UpdateData(tableName, columnName, columnValue, condition, conditionValue);
                    }

                /// <summary>
                /// A generic method used by the other methods to update rows in the specified table
                /// </summary>
                /// <param name="tableName">Name of the table.</param>
                /// <param name="columnName">Name of the column.</param>
                /// <param name="columnValue">The column value.</param>
                /// <param name="condition">The condition.</param>
                /// <param name="conditionValue">The condition value.</param>
                private static void UpdateData(string tableName, string columnName, string columnValue, string condition,
                                               int conditionValue)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Update the database
                                        Dictionary <string, object> dicData = new Dictionary <string, object>();
                                        dicData[columnName] = columnValue;
                                        Dictionary <string, object> dicCondition = new Dictionary <string, object>();
                                        dicCondition["userId"] = User.CurrentUserId;
                                        dicCondition[condition] = conditionValue;
                                        sh.Update(tableName, dicData, dicCondition);
                                    }
                            }
                    }

                /// <summary>
                /// Gets the item identifier.
                /// </summary>
                /// <param name="itemTitle">The item title.</param>
                /// <param name="itemType">Type of the item.</param>
                /// <returns></returns>
                public static int GetItemId(string itemTitle, string itemType)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Query table for item name
                                        DataTable mediaData =
                                            sh.Select(
                                                "SELECT item.id FROM item " + "JOIN type ON type.id = item.typeId " +
                                                "WHERE item.title = @title AND type.type = @type;",
                                                new[]
                                                    {
                                                        new SQLiteParameter("@title", itemTitle),
                                                        new SQLiteParameter("@type", itemType)
                                                    });

                                        // Return 0 if there are no rows from query
                                        if ( mediaData.Rows.Count == 0 ) return 0;

                                        // Otherwise return the Items ID
                                        return Convert.ToInt32(mediaData.Rows[0].ItemArray.GetValue(0));
                                    }
                            }
                    }

                /// <summary>
                /// Removes the item.
                /// </summary>
                /// <param name="itemId">The item identifier.</param>
                /// <param name="tableName">Name of the table.</param>
                public static void RemoveItem(int itemId, string tableName = "progress")
                    {
                        // Remove an item from the users list
                        // Can be used to remove recommendations if using the optional parameter
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        // Connect to database
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        string sql = "DELETE FROM " + sh.Escape(tableName) +
                                                  " WHERE itemid = @itemId AND userid = @userId;";

                                        // Remove item
                                        sh.Execute(sql,
                                            new[]
                                                {
                                                    new SQLiteParameter("@tableName", tableName),
                                                    new SQLiteParameter("@itemId", itemId),
                                                    new SQLiteParameter("@userId", User.CurrentUserId)
                                                });
                                    }
                            }
                    }

                /// <summary>
                /// Determines whether the specified item identifier has item.
                /// </summary>
                /// <param name="itemId">The item identifier.</param>
                /// <returns></returns>
                public static bool HasItem(int itemId)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Query table for item name
                                        DataTable itemData =
                                            sh.Select(
                                                "SELECT item.id, item.title " + "FROM item " +
                                                "JOIN progress ON item.id = progress.itemId " +
                                                "WHERE item.id = @item AND progress.userId = @user;",
                                                new[]
                                                    {
                                                        new SQLiteParameter("@item", itemId),
                                                        new SQLiteParameter("@user", User.CurrentUserId)
                                                    });

                                        // Return false if no items queried, else it's true
                                        if ( itemData.Rows.Count == 0 ) return false;
                                        return true;
                                    }
                            }
                    }

                /// <summary>
                /// Adds to database.
                /// </summary>
                /// <returns></returns>
                public static int AddToDatabase()
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Look for item in database and return its ID
                                        DataTable dt =
                                            sh.Select(
                                                "SELECT item.id, item.title, type.type " + "FROM item " +
                                                "JOIN Type ON item.typeId = Type.id " +
                                                "WHERE UPPER(title) = UPPER(@title) AND " + "type = @type ;",
                                                new[]
                                                    {
                                                        new SQLiteParameter("@title", SearchItem.Media.Title),
                                                        new SQLiteParameter("@type", SearchItem.Media.MediaType)
                                                    });

                                        if ( dt.Rows.Count == 0 )
                                            {
                                                // Item doesn't exist in DB - Add it and get ID
                                                Dictionary <string, object> newItemData = GetItemDetails();
                                                sh.Insert("item", newItemData);

                                                return Convert.ToInt32(sh.LastInsertRowId()); // ID of last insert
                                            }

                                        // Item exists - Get ID
                                        int itemId = Convert.ToInt32(dt.Rows[0]["id"]);
                                        Dictionary <string, object> newData = GetItemDetails();
                                        sh.Update("item", newData, "id", itemId);

                                        return itemId;
                                    }
                            }
                    }

                /// <summary>
                /// Gets the item details from the Item class into a dictionary.
                /// </summary>
                /// <returns></returns>
                private static Dictionary <string, object> GetItemDetails()
                    {
                        // Create dictionary of details for the item
                        Dictionary <string, object> itemData = new Dictionary <string, object>();
                        itemData["title"] = SearchItem.Media.Title;
                        itemData["year"] = SearchItem.Media.Year;
                        itemData["synopsis"] = SearchItem.Media.Plot;
                        itemData["imdbrating"] = SearchItem.Media.Rating;
                        itemData["length"] = SearchItem.Media.Length;
                        itemData["posterurl"] = SearchItem.Media.Poster;
                        itemData["author"] = SearchItem.Media.Creator;
                        itemData["ageRating"] = SearchItem.Media.AgeRating;
                        itemData["genre"] = SearchItem.Media.Genre;

                        // Get appropriate foreign key for media type
                        switch ( SearchItem.Media.MediaType )
                            {
                                case "Movie":
                                    itemData["typeid"] = "1";
                                    break;
                                case "Series":
                                    itemData["typeid"] = "2";
                                    break;
                                case "Book":
                                    itemData["typeid"] = "3";
                                    break;
                            }

                        // Add trailer URL if available
                        if ( SearchItem.Media.IsTrailerCorrect ) itemData["TrailerUrl"] = SearchItem.Media.TrailerUrl.ToString();

                        return itemData;
                    }

                /// <summary>
                /// Recommends the item.
                /// </summary>
                /// <param name="itemId">The item identifier.</param>
                /// <param name="userId">The user identifier.</param>
                public static void RecommendItem(int itemId, int userId)
                    {
                        using ( SQLiteConnection conn = new SQLiteConnection(Db.TableLocation) )
                            {
                                using ( SQLiteCommand cmd = new SQLiteCommand() )
                                    {
                                        cmd.Connection = conn;
                                        conn.Open();
                                        SqLiteHelper sh = new SqLiteHelper(cmd);

                                        // Save item details
                                        Dictionary <string, object> recommendationData = new Dictionary <string, object>();
                                        recommendationData["userId"] = userId;
                                        recommendationData["fromId"] = User.CurrentUserId;
                                        recommendationData["itemId"] = itemId;
                                        sh.Insert("recommendation", recommendationData);

                                        Utilities.ShowMessage("Success", "Sending recommendation complete.");
                                    }
                            }
                    }
            }
    }