using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
    // only one should exist at a time, otherwise behavior unspecified
#if SILVERLIGHT
    public class SparseMatrix
#else
    public unsafe class SparseMatrix
#endif
    {
#if SILVERLIGHT
        private double[] pData;
        internal double[] pValue;
        private int[] pRow;
        private int[] pCol;
#else
        private double* pData;
        internal double* pValue;
        private int* pRow;
        private int* pCol;
#endif

#if SILVERLIGHT
        public void BeginSafe(double[] pData, double[] pValue, int[] pRow, int[] pCol)
        {
            this.pData = pData;
            this.pValue = pValue;
            this.pRow = pRow;
            this.pCol = pCol;
        }
#else
        public void BeginUnsafe(double* pData, double* pValue, int* pRow, int* pCol)
        {
            this.pData = pData;
            this.pValue = pValue;
            this.pRow = pRow;
            this.pCol = pCol;
        }
#endif

        public void EndUnsafe()
        {
            pData = null;
            pValue = null;
            pRow = null;
            pCol = null;
        }

        private int cols;
        private int rows;
        private int sparseIndex;
        private int lastCol;
        private ArraySet arraySet;

        private bool finalized;

        public bool ReadOnly
        {
            get
            {
                return finalized;
            }
        }

        public int Columns
        {
            get
            {
                return cols;
            }
        }

        public int Rows
        {
            get
            {
                return rows;
            }
        }

        public SparseMatrix(int rows, int maxCols, ArraySet arraySet)
        {
            Initialize(rows, maxCols, arraySet);
        }

        public void Initialize(int rows, int maxCols, ArraySet arraySet)
        {
            this.arraySet = arraySet;
            if (rows > arraySet.SparseMatrixMaxRows || maxCols + 20 > arraySet.SparseMatrixMaxCols)
            {
                arraySet.SparseMatrixMaxRows = Math.Max(rows, arraySet.SparseMatrixMaxRows);
                arraySet.SparseMatrixMaxCols = Math.Max(maxCols + 20, arraySet.SparseMatrixMaxCols); // give some room for AddColumn
                arraySet.RecreateSparseMatrixArrays();
            }
            this.rows = rows;
            this.cols = 0;
            //Array.Clear(data, 0, rows * cols); // only need to clear what will be used

            sparseIndex = 0;
            lastCol = 0;
            finalized = false;
        }

        public double this[int row, int col]
        {
            get
            {
                return arraySet.SparseMatrixData[col * rows + row];
            }
            set
            {
                if (finalized) throw new InvalidOperationException();
                if (col < lastCol) throw new ArgumentException();
                if (value == 0.0) return;
                if (col > lastCol)
                {
                    for (int c = lastCol + 1; c <= col; c++)
                    {
                        arraySet.SparseMatrixCol[c] = sparseIndex;
                    }
                }
                if (sparseIndex >= arraySet.SparseMatrixSparseSize)
                {
                    arraySet.SparseMatrixSparseSize += (int)(rows * cols * 0.1);
                    int[] newRow = new int[arraySet.SparseMatrixSparseSize];
                    Array.Copy(arraySet.SparseMatrixRow, newRow, arraySet.SparseMatrixRow.Length);
                    arraySet.SparseMatrixRow = newRow;
                    double[] newValue = new double[arraySet.SparseMatrixSparseSize];
                    Array.Copy(arraySet.SparseMatrixValue, newValue, arraySet.SparseMatrixValue.Length);
                    arraySet.SparseMatrixValue = newValue;
                }
                arraySet.SparseMatrixRow[sparseIndex] = row;
                arraySet.SparseMatrixValue[sparseIndex] = value;
                arraySet.SparseMatrixData[col * rows + row] = value; // store data by columns, we always access by column so it will have better locality, we also never increase number of rows, only columns, that way we don't have to reposition data
                lastCol = col;
                sparseIndex++;
            }
        }

        public void SetElementUnsafe(int row, int col, double value)
        {
            if (pValue == null)
            {
                this[row, col] = value;
                return;
            }
            if (value == 0.0) return;
            if (col > lastCol)
            {
                for (int c = lastCol + 1; c <= col; c++)
                {
                    pCol[c] = sparseIndex;
                }
            }
            if (sparseIndex >= arraySet.SparseMatrixSparseSize)
            {
                SetElementSafeFromUnsafe(row, col, value);
                return;
            }
            pRow[sparseIndex] = row;
            pValue[sparseIndex] = value;
            pData[col * rows + row] = value; // store data by columns, we always access by column so it will have better locality, we also never increase number of rows, only columns, that way we don't have to reposition data
            lastCol = col;
            sparseIndex++;
        }

        private void SetElementSafeFromUnsafe(int row, int col, double value)
        {
            // C# does not allow to change a pinned reference, if we have to recreate arrays we have to move to safe code
            EndUnsafe();
            arraySet.SparseMatrixSparseSize += (int)(rows * cols * 0.1);
            int[] newRow = new int[arraySet.SparseMatrixSparseSize];
            Array.Copy(arraySet.SparseMatrixRow, newRow, arraySet.SparseMatrixRow.Length);
            arraySet.SparseMatrixRow = newRow;
            double[] newValue = new double[arraySet.SparseMatrixSparseSize];
            Array.Copy(arraySet.SparseMatrixValue, 0, newValue, 0, arraySet.SparseMatrixValue.Length);
            arraySet.SparseMatrixValue = newValue;
            arraySet.SparseMatrixRow[sparseIndex] = row;
            arraySet.SparseMatrixValue[sparseIndex] = value;
            arraySet.SparseMatrixData[col * rows + row] = value; // store data by columns, we always access by column so it will have better locality, we also never increase number of rows, only columns, that way we don't have to reposition data
            lastCol = col;
            sparseIndex++;
        }

        public int AddColumn()
        {
            finalized = false;
            cols++;
            if (cols > arraySet.SparseMatrixMaxCols)
            {
                throw new InvalidOperationException();
            }
            Array.Clear(arraySet.SparseMatrixData, rows * (cols - 1), rows); // only need to clear what will be used
            return cols - 1;
        }

        public void EndConstruction()
        {
            for (int c = lastCol + 1; c <= cols; c++)
            {
                arraySet.SparseMatrixCol[c] = sparseIndex;
            }            
            finalized = true;
        }
    }
}
