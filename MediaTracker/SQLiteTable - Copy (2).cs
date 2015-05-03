using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.SQLite
{
    public class SqLiteTable
    {
        public string TableName = "";
        public SqLiteColumnList Columns = new SqLiteColumnList();

        public SqLiteTable() { }

        public SqLiteTable(string name)
        {
            TableName = name;
        }
    }
}