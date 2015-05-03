using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.SQLite
{

    //public class SqLiteColumnList : IList<SqLiteColumn>
    //{
    //    List<SqLiteColumn> _lst = new List<SqLiteColumn>();

    //    private void CheckColumnName(string colName)
    //    {
    //        for (int i = 0; i < _lst.Count; i++)
    //        {
    //            if (_lst[i].ColumnName == colName)
    //                throw new Exception("Column name of \"" + colName + "\" is already existed.");
    //        }
    //    }

    //    public int IndexOf(SqLiteColumn item)
    //    {
    //        return _lst.IndexOf(item);
    //    }

    //    public void Insert(int index, SqLiteColumn item)
    //    {
    //        CheckColumnName(item.ColumnName);

    //        _lst.Insert(index, item);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        _lst.RemoveAt(index);
    //    }

    //    public SqLiteColumn this[int index]
    //    {
    //        get
    //        {
    //            return _lst[index];
    //        }
    //        set
    //        {
    //            if (_lst[index].ColumnName != value.ColumnName)
    //            {
    //                CheckColumnName(value.ColumnName);
    //            }

    //            _lst[index] = value;
    //        }
    //    }

    //    public void Add(SqLiteColumn item)
    //    {
    //        CheckColumnName(item.ColumnName);

    //        _lst.Add(item);
    //    }

    //    public void Clear()
    //    {
    //        _lst.Clear();
    //    }

    //    public bool Contains(SqLiteColumn item)
    //    {
    //        return _lst.Contains(item);
    //    }

    //    public void CopyTo(SqLiteColumn[] array, int arrayIndex)
    //    {
    //        _lst.CopyTo(array, arrayIndex);
    //    }

    //    public int Count
    //    {
    //        get { return _lst.Count; }
    //    }

    //    public bool IsReadOnly
    //    {
    //        get { return false; }
    //    }

    //    public bool Remove(SqLiteColumn item)
    //    {
    //        return _lst.Remove(item);
    //    }

    //    public IEnumerator<SqLiteColumn> GetEnumerator()
    //    {
    //        return _lst.GetEnumerator();
    //    }

    //    Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
    //    {
    //        return _lst.GetEnumerator();
    //    }
    //}

}
