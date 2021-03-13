using System.Collections;
using System.Collections.Generic;

namespace LFAOIReview
{
    class Matrix<T> : IEnumerable<T>, IEnumerator<T>
    {
        public Matrix(int rowCount, int columnCount)
        {
            _Matrix = new T[rowCount, columnCount];
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        private T[,] _Matrix;

        public T this[int rowIndex, int columnIndex]
        {
            get
            {
                return _Matrix[rowIndex, columnIndex];
            }
            set
            {
                _Matrix[rowIndex, columnIndex] = value;
            }
        }

        /// <summary>
        /// 行总数
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// 列总数
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// 盘号
        /// </summary>
        public string FrameName { get; set; }

        private int PositionRow = 0;
        private int PositionColumn = -1;

        public object Current
        {
            get
            {
                return _Matrix[PositionRow, PositionColumn];
            }
        }

        T IEnumerator<T>.Current
        {
            get
            {
                return (T)Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if (PositionColumn < ColumnCount - 1)
            {
                PositionColumn++;
                return true;
            }
            else
            {
                if (PositionRow < RowCount - 1)
                {
                    PositionRow++;
                    PositionColumn = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Reset()
        {
            PositionRow = 0;
            PositionColumn = -1;
        }


        public void Dispose()
        {
            PositionRow = 0;
            PositionColumn = -1;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }
    }
}
