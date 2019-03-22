using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
#if SILVERLIGHT
    public class LP
#else
    public unsafe class LP
#endif
    {
        internal SparseMatrix A;
        private int[] extraConstraintsUsed;
        private int numExtraConstraints;
        private int baseRows;
        private int rows;
        internal int cols;
        private int super;

        private int[] _B;
        private int[] _V;
        private int[] _S;        
        private LU lu;

        internal ArraySet arraySet;

        private bool costWorkingDirty;
        private bool shifted;
        private bool osbDirty; // true if superbasis changed since last ordered superbasis update
        private int osbStructural; // number of structural variables in superbasis

        private int[] _flags;
        internal double[] _lb;
        internal double[] _ub;
        internal double[] _mb;

        private const int flagNLB = 0x1; // variable nonbasic at lower bound
        private const int flagNUB = 0x2; // variable nonbasic at upper bound
        private const int flagNMid = 0x400; // variable nonbasic in middle (quadratic program only)
        private const int flagN = flagNLB | flagNUB | flagNMid; // variable nonbasic
        private const int flagB = 0x4; // variable basic
        private const int flagDis = 0x8; // variable blacklisted
        private const int flagFix = 0x10; // variable fixed
        private const int flagLB = 0x20; // variable has finite lower bound
        private const int flagUB = 0x40; // variable has finite upper bound
        private const int flagPivot = 0x80; // variable is eligible for pivot
        private const int flagFlip = 0x100; // variable bounds must be flipped
        private const int flagPivot2 = 0x200; // variable is in reduced pivot candidate set
        private const int flagSB = flagB | flagNMid;

        private bool disabledDirty;

        // quadratic parameters
        private int mpsRow; 
        private int[] sort;
        //private int[] sortinv;
        private double Qk;

#if SILVERLIGHT
        private double[] a;
        private double[] U;
        private double[] d;
        private double[] x;
        private double[] w;
        private double[] ww;
        private double[] wd;
        private double[] qx;
        private double[] qv;
        private double[] vd;
        private double[] c;
        private double[] u;
        private double[] cost;
        private double[] spellDps;
        private double[] sparseValue;
        private double[] D;
        private ArrayOffset<double>[] pD;
        private int[] B;
        private int[] V;
        private int[] S;
        private int[] OSB;
        private int[] sparseRow;
        private int[] sparseCol;
        private int[] flags;
        private double[] lb;
        private double[] ub;
        private double[] mb;

        private double[] cg_p;
        private double[] cg_x;
        private double[] cg_w;
        private double[] cg_r;
        private double[] cg_ww;
        private double[] cg_rr;
        private double[] cg_qp;

#else
        private double* a;
        private double* U;
        private double* d;
        private double* x;
        private double* w;
        private double* ww;
        private double* wd;
        private double* qx;
        private double* qv;
        private double* vd;
        private double* c;
        private double* u;
        private double* cost;
        private double* spellDps;
        private double* sparseValue;
        private double* D;
        private double** pD;
        private int* B;
        private int* V;
        private int* S;
        private int* OSB;
        private int* sparseRow;
        private int* sparseCol;
        private int* flags;
        private double* lb;
        private double* ub;
        private double* mb;

        private double* cg_p;
        private double* cg_x;
        private double* cg_w;
        private double* cg_r;
        private double* cg_ww;
        private double* cg_rr;
        private double* cg_qp;
#endif

        private const double epsPrimal = 1.0e-7;
        private const double epsPrimalLow = 1.0e-6;
        private const double epsPrimalRel = 1.0e-9;
        private const double epsDual = 1.0e-7;
        private const double epsDualI = 1.0e-8;
        private const double epsPivot = 1.0e-5;
        private const double epsPivot2 = 1.0e-7;
        private const double epsZero = 1.0e-12;
        private const double epsDrop = 1.0e-14;
        private const double epsCG = 1.0e-5;

#if SILVERLIGHT
        public static void Copy(double[] dest, double[] source, int size)
        {
            Array.Copy(source, 0, dest, 0, size);
        }

        public static void Zero(double[] array, int size)
        {
            Array.Clear(array, 0, size);
        }
#else
        public static unsafe void Zero(double* array, int size)
        {            
            /*long* arr = (long*)array;
            long* arrend = arr + size;
            for (; arr < arrend; arr++)
            {
                *arr = 0;
            }*/

            const int c = ~3;
            int trunc = size & c;
            long* arr = (long*)array;
            long* arr2 = arr + size;
            long* arr1 = arr + trunc;
            for (; arr < arr1; arr += 4)
            {
                arr[0] = 0;
                arr[1] = 0;
                arr[2] = 0;
                arr[3] = 0;
            }
            for (; arr < arr2; arr++)
            {
                *arr = 0;
            }
        }

        public static unsafe void Copy(double* dest, double* source, int size)
        {
            const int c = ~3;
            int trunc = size & c;
            double* arr1 = dest + trunc;
            double* arr2 = dest + size;
            for (; dest < arr1; dest += 4, source += 4)
            {
                dest[0] = source[0];
                dest[1] = source[1];
                dest[2] = source[2];
                dest[3] = source[3];
            }
            for (; dest < arr2; dest++, source++)
            {
                *dest = *source;
            }
        }
#endif

        public LP Clone()
        {
            LP clone = (LP)MemberwiseClone();
            clone._B = (int[])_B.Clone();
            clone._V = (int[])_V.Clone();
            if (_S != null)
            {
                clone._S = (int[])_S.Clone();
            }
            clone._flags = (int[])_flags.Clone();
            clone._lb = (double[])_lb.Clone();
            clone._ub = (double[])_ub.Clone();
            if (_mb != null)
            {
                clone._mb = (double[])_mb.Clone();
            }
            if (numExtraConstraints > 0)
            {
                clone.extraConstraintsUsed = (int[])extraConstraintsUsed.Clone();
                // increase reference count
                for (int i = 0; i < numExtraConstraints; i++)
                {
                    arraySet.extraReferenceCount[extraConstraintsUsed[i]]++;
                }
            }
            return clone;
        }

        public double this[int row, int col]
        {
            get
            {
                if (row > baseRows) throw new ArgumentException();
                else if (row == baseRows) return arraySet._cost[col];
                else if (col == cols) return -_lb[cols + row];
                else return A[row, col];
            }
            set
            {
                A[row, col] = value;
            }
        }

        public void SetCost(int col, double value)
        {
            arraySet._cost[col] = value;
        }

        public void SetSpellDps(int col, double value)
        {
            arraySet._spellDps[col] = value;
        }

        private bool hardInfeasibility = false;

        public bool UpdateLowerBound(int col, double value)
        {
            if (value > _lb[col] + epsZero)
            {
                SetLowerBound(col, value);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateUpperBound(int col, double value)
        {
            if (value < _ub[col] - epsZero)
            {
                SetUpperBound(col, value);
                return true;
            }
            else
            {
                return false;
            }
        }

        public double GetLowerBound(int col)
        {
            return _lb[col];
        }

        public void SetLowerBound(int col, double value)
        {
            _lb[col] = value;
            if (double.IsNegativeInfinity(value))
            {
                _flags[col] &= ~flagLB;
            }
            else
            {
                _flags[col] |= flagLB;
            }
            if (_ub[col] - _lb[col] < epsZero)
            {
                _flags[col] |= flagFix;
            }
            else
            {
                _flags[col] &= ~flagFix;
            }
            if (_ub[col] < _lb[col] - epsZero) hardInfeasibility = true;
        }

        public void SetUpperBound(int col, double value)
        {
            _ub[col] = value;
            if (double.IsPositiveInfinity(value))
            {
                _flags[col] &= ~flagUB;
            }
            else
            {
                _flags[col] |= flagUB;
            }
            if (_ub[col] - _lb[col] < epsZero)
            {
                _flags[col] |= flagFix;
            }
            else
            {
                _flags[col] &= ~flagFix;
            }
            if (_ub[col] < _lb[col] - epsZero) hardInfeasibility = true;
        }

        public double GetRHS(int row)
        {
            //_b[row] = value;
            return -GetLowerBound(cols + row);
        }

        public void SetRHS(int row, double value)
        {
            //_b[row] = value;
            SetLowerBound(cols + row, -value);
        }

        public void SetLHS(int row, double value)
        {
            SetUpperBound(cols + row, -value);
        }

        public double GetConstraintElement(int index, int col)
        {
            return arraySet.extraConstraints[extraConstraintsUsed[index] * cols + col];
        }

        public void SetConstraintElement(int index, int col, double value)
        {
            arraySet.extraConstraints[extraConstraintsUsed[index] * cols + col] = value;
        }

        public double GetConstraintRHS(int index)
        {
            return -_lb[cols + baseRows + index];
        }

        public void SetConstraintRHS(int index, double value)
        {
            SetLowerBound(cols + baseRows + index, -value);
        }

        public double GetConstraintLHS(int index)
        {
            return -_ub[cols + baseRows + index];
        }

        public void SetConstraintLHS(int index, double value)
        {
            SetUpperBound(cols + baseRows + index, -value);
        }

        public int ExtraConstraintCount
        {
            get
            {
                return numExtraConstraints;
            }
        }

        public int AddColumn()
        {
            if (cols + rows + 100 >= maxSize)
            {
                maxSize = 2 * maxSize;
                ExtendInstanceArrays();
            }
            arraySet._cost[cols] = 0.0;
            _lb[cols] = 0.0; // we can reuse dirty arrays, initialize only when needed
            _ub[cols] = double.PositiveInfinity;
            arraySet._spellDps[cols] = 0.0;
            _flags[cols] = flagNLB | flagLB;
            cols++;
            return A.AddColumn();
        }

        private int maxSize;

        public void ReleaseConstraints()
        {
            for (int i = 0; i < numExtraConstraints; i++)
            {
                arraySet.extraReferenceCount[extraConstraintsUsed[i]]--;
            }
            numExtraConstraints = 0;
        }

        // should only add extra constraints that are tight when primal feasible
        public int AddConstraint(string name, out bool newConstraint)
        {
            if (cols + rows >= maxSize)
            {
                maxSize += 100;
                ExtendInstanceArrays();
            }
            numExtraConstraints++;
            rows++;
            if (rows > arraySet.maxRows)
            {
                arraySet.maxRows = rows + 10;
                arraySet.ExtendLPArrays();
            }
            if (numExtraConstraints > arraySet.maxExtra)
            {
                arraySet.maxExtra += 32;
#if SILVERLIGHT
                arraySet._pD = new ArrayOffset<double>[arraySet.maxExtra];
#else
                arraySet._pD = new double*[arraySet.maxExtra];
#endif
            }
            if (extraConstraintsUsed == null) extraConstraintsUsed = new int[32];
            if (numExtraConstraints > extraConstraintsUsed.Length)
            {
                int[] newArray = new int[extraConstraintsUsed.Length + 32];
                if (extraConstraintsUsed != null) Array.Copy(extraConstraintsUsed, newArray, extraConstraintsUsed.Length);
                extraConstraintsUsed = newArray;
            }
            int index = arraySet.GetConstraint(name, cols, out newConstraint);
            extraConstraintsUsed[numExtraConstraints - 1] = index;
            _lb[cols + rows - 1] = 0.0; // don't forget to clear
            _ub[cols + rows - 1] = double.PositiveInfinity;
            _flags[cols + rows - 1] = flagLB | flagB;
            arraySet._cost[cols + rows - 1] = 0.0;
            int[] newB = new int[rows];
            Array.Copy(_B, newB, rows - 1);
            _B = newB;
            //_B[rows - 1] = cols + rows - 1;
            // extra constraints should be at start so they are not eliminated when patching singular basis
            _B[rows - 1] = _B[numExtraConstraints - 1];
            _B[numExtraConstraints - 1] = cols + rows - 1;
            lu = new LU(rows, arraySet);
            return numExtraConstraints - 1;
        }

        private bool constructed;
        public void EndColumnConstruction()
        {
            if (constructed) return;
            if (cols + rows > maxSize + 100)
            {
                maxSize = cols + rows + 100;
                ExtendInstanceArrays();
            }
            A.EndConstruction();
            _B = new int[rows];
            _V = new int[cols];
            for (int i = 0; i < rows; i++)
            {
                _B[i] = cols + i;
                _lb[cols + i] = 0.0; // don't forget to clear
                _ub[cols + i] = double.PositiveInfinity;
                _flags[cols + i] = flagB | flagLB;
                arraySet._cost[cols + i] = 0.0;
            }
            for (int j = 0; j < cols; j++)
            {
                _V[j] = j;
            }
            if (arraySet.extraConstraints.Length < arraySet.maxExtraRows * cols)
            {
                arraySet.extraConstraints = new double[arraySet.maxExtraRows * cols];
            }
            constructed = true;
        }

        private void ExtendInstanceArrays()
        {
            if (_lb == null)
            {
                if (arraySet._lb != null && arraySet._lb.Length >= maxSize)
                {
                    _lb = arraySet._lb;
                    _ub = arraySet._ub;
                    _mb = arraySet._mb;
                    _flags = arraySet._flags;
                    // do not clear, initialize on each new column instead
                    //Array.Clear(_lb, 0, _lb.Length);
                    //Array.Clear(_ub, 0, _lb.Length);
                    //Array.Clear(_flags, 0, _lb.Length);
                    maxSize = _lb.Length;
                }
                else
                {
                    _lb = new double[maxSize];
                    _ub = new double[maxSize];
                    _mb = new double[maxSize];
                    _flags = new int[maxSize];
                    arraySet._lb = _lb;
                    arraySet._ub = _ub;
                    arraySet._mb = _mb;
                    arraySet._flags = _flags;
                }
            }
            else
            {
                double[] newlb = new double[maxSize];
                Array.Copy(_lb, newlb, _lb.Length);
                double[] newub = new double[maxSize];
                Array.Copy(_ub, newub, _ub.Length);
                double[] newmb = new double[maxSize];
                Array.Copy(_mb, newmb, _mb.Length);
                int[] newflags = new int[maxSize];
                Array.Copy(_flags, newflags, _flags.Length);
                if (_lb == arraySet._lb)
                {
                    arraySet._lb = newlb;
                    arraySet._ub = newub;
                    arraySet._mb = newmb;
                    arraySet._flags = newflags;
                }
                _lb = newlb;
                _ub = newub;
                _mb = newmb;
                _flags = newflags;
            }
        }

        public LP(int baseRows, int maxCols, ArraySet arraySet)
        {
            Initialize(baseRows, maxCols, arraySet);
        }

        public void Initialize(int baseRows, int maxCols, ArraySet arraySet)
        {
            this.baseRows = baseRows;
            this.arraySet = arraySet;
            if (baseRows + 10 > arraySet.maxRows || maxCols + 10 > arraySet.maxCols)
            {
                arraySet.maxRows = Math.Max(baseRows + 10, arraySet.maxRows);
                arraySet.maxCols = Math.Max(maxCols + 10, arraySet.maxCols);
                arraySet.RecreateLPArrays();
            }
            this.rows = baseRows;
            this.cols = 0;
            this.super = 0;
            maxSize = rows + 500;
            _lb = null;
            ExtendInstanceArrays();
            if (arraySet.extraReferenceCount != null) Array.Clear(arraySet.extraReferenceCount, 0, arraySet.extraReferenceCount.Length);
            arraySet.extraConstraintMap.Clear();

            if (A == null)
            {
                A = new SparseMatrix(baseRows, maxCols, arraySet);
            }
            else
            {
                A.Initialize(baseRows, maxCols, arraySet);
            }
            if (lu == null)
            {
                lu = new LU(rows, arraySet);
            }
            else
            {
                lu.Initialize(rows, arraySet);
            }
            //extraConstraints = new double[cols + rows + 1];
            //extraConstraints[cols + rows - 1] = 1;
            //numExtraConstraints = 1;
            //Array.Clear(_b, 0, baseRows);

            ColdStart = true;
            constructed = false;
            disabledDirty = false;
            numExtraConstraints = 0;
        }

        private bool ColdStart;

        private void SetupExtraConstraints()
        {
            for (int i = 0; i < numExtraConstraints; i++)
            {
#if SILVERLIGHT
                pD[i] = new ArrayOffset<double>(D, extraConstraintsUsed[i] * cols);
#else
                pD[i] = D + (extraConstraintsUsed[i] * cols);
#endif
            }
        }

#if SILVERLIGHT
        public double[] SolvePrimal(bool prepareForDual)
        {
            if (hardInfeasibility) return new double[cols + 1];
            double[] ret = null;

            this.a = arraySet.SparseMatrixData;
            this.U = arraySet.LU_U;
            this.d = arraySet._d;
            this.x = arraySet._x;
            this.w = arraySet._w;
            this.ww = arraySet._ww;
            this.wd = arraySet._wd;
            this.c = arraySet._c;
            this.u = arraySet._u;
            this.cost = arraySet._cost;
            this.sparseValue = arraySet.SparseMatrixValue;
            this.D = arraySet.extraConstraints;
            this.B = _B;
            this.V = _V;
            this.sparseRow = arraySet.SparseMatrixRow;
            this.sparseCol = arraySet.SparseMatrixCol;
            this.flags = _flags;
            this.lb = _lb;
            this.ub = _ub;
            this.pD = arraySet._pD;

            SetupExtraConstraints();

            lu.BeginSafe();
            if (prepareForDual)
            {
                this.cost = arraySet._costWorking;
                LoadCost();
                PerturbCost();
            }
            ret = SolvePrimalUnsafe(prepareForDual, prepareForDual, false, true);
            lu.EndUnsafe();

            this.a = null;
            this.U = null;
            this.d = null;
            this.x = null;
            this.w = null;
            this.ww = null;
            this.wd = null;
            this.c = null;
            this.u = null;
            this.cost = null;
            this.sparseValue = null;
            this.D = null;
            this.pD = null;
            this.B = null;
            this.V = null;
            this.sparseRow = null;
            this.sparseCol = null;
            this.flags = null;
            this.lb = null;
            this.ub = null;

            return ret;
        }
#else
        public unsafe double[] SolvePrimal(bool prepareForDual)
        {
            if (hardInfeasibility) return new double[cols + 1];
            double[] ret = null;

            fixed (double** pD = arraySet._pD)
            fixed (double* a = arraySet.SparseMatrixData, U = arraySet.LU_U, sL = arraySet.LUsparseL, column = arraySet.LUcolumn, column2 = arraySet.LUcolumn2, d = arraySet._d, x = arraySet._x, w = arraySet._w, ww = arraySet._ww, wd = arraySet._wd, c = arraySet._c, u = arraySet._u, cost = arraySet._cost, costw = arraySet._costWorking, sparseValue = arraySet.SparseMatrixValue, D = arraySet.extraConstraints, lb = _lb, ub = _ub)
            fixed (int* B = _B, V = _V, sparseRow = arraySet.SparseMatrixRow, sparseCol = arraySet.SparseMatrixCol, P = arraySet.LU_P, Q = arraySet.LU_Q, LJ = arraySet.LU_LJ, sLI = arraySet.LUsparseLI, sLstart = arraySet.LUsparseLstart, flags = _flags)
            {
                this.a = a;
                this.U = U;
                this.d = d;
                this.x = x;
                this.w = w;
                this.ww = ww;
                this.wd = wd;
                this.c = c;
                this.u = u;
                this.cost = cost;
                this.sparseValue = sparseValue;
                this.D = D;
                this.B = B;
                this.V = V;
                this.sparseRow = sparseRow;
                this.sparseCol = sparseCol;
                this.flags = flags;
                this.lb = lb;
                this.ub = ub;
                this.pD = pD;

                SetupExtraConstraints();

                lu.BeginUnsafe(U, sL, P, Q, LJ, sLI, sLstart, column, column2);
                if (prepareForDual)
                {
                    this.cost = costw;
                    LoadCost();
                    PerturbCost();
                }
                ret = SolvePrimalUnsafe(prepareForDual, prepareForDual, false, true);
                lu.EndUnsafe();

                this.a = null;
                this.U = null;
                this.d = null;
                this.x = null;
                this.w = null;
                this.ww = null;
                this.wd = null;
                this.c = null;
                this.u = null;
                this.cost = null;
                this.sparseValue = null;
                this.D = null;
                this.pD = null;
                this.B = null;
                this.V = null;
                this.sparseRow = null;
                this.sparseCol = null;
                this.flags = null;
                this.lb = null;
                this.ub = null;
            }

            return ret;
        }
#endif

#if SILVERLIGHT
        public void QuadraticQ(double[] x, double[] Qx)
#else
        public unsafe void QuadraticQ(double* x, double* Qx)
#endif
        {
            int sortj = 0;
            double xm = 0.0;
            for (int j = 0; j < cols; j++)
            {
                sortj = sort[j];
                xm += x[sortj] * a[sortj * baseRows + mpsRow];
                Qx[sortj] = spellDps[sortj] * xm;
            }
            double xe = x[sortj] * spellDps[sortj];
            for (int j = cols - 2; j >= 0; j--)
            {                
                sortj = sort[j];
                Qx[sortj] += a[sortj * baseRows + mpsRow] * xe;
                xe += x[sortj] * spellDps[sortj];
            }
        }

#if SILVERLIGHT
        public void QuadraticQOSB(double[] x, double[] Qx)
#else
        public unsafe void QuadraticQOSB(double* x, double* Qx)
#endif
        {
            int sortj = 0;
            double xm = 0.0;
            for (int j = 0; j < osbStructural; j++)
            {
                sortj = OSB[j];
                xm += x[sortj] * a[sortj * baseRows + mpsRow];
                Qx[sortj] = spellDps[sortj] * xm;
            }
            double xe = x[sortj] * spellDps[sortj];
            for (int j = osbStructural - 2; j >= 0; j--)
            {
                sortj = OSB[j];
                Qx[sortj] += a[sortj * baseRows + mpsRow] * xe;
                xe += x[sortj] * spellDps[sortj];
            }
        }

        private void UpdateMB()
        {
            // TODO this should be updated instead of reset each time
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                mb[col] = d[i];
            }
            for (int col = 0; col < cols + rows; col++)
            {
                if ((flags[col] & flagNLB) != 0)
                {
                    mb[col] = lb[col];
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    mb[col] = ub[col];
                }
            }
        }

        private void ComputeQx()
        {
            int sortj = 0;
            double xm = 0.0;
            for (int j = 0; j < cols; j++)
            {
                sortj = sort[j];
                xm += mb[sortj] * a[sortj * baseRows + mpsRow];
                qx[sortj] = spellDps[sortj] * xm;
            }
            double xe = mb[sortj] * spellDps[sortj];
            for (int j = cols - 2; j >= 0; j--)
            {
                sortj = sort[j];
                qx[sortj] += a[sortj * baseRows + mpsRow] * xe;
                xe += mb[sortj] * spellDps[sortj];
            }
        }

        public double ComputevQv(int incoming, double direction)
        {
            // v = 1 at incoming, 0 at all other nonbasic, -w at basic
            // expand v
            Zero(vd, cols);
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                vd[col] = - direction * w[i];
            }
            vd[V[incoming]] = direction;
            int sortj = 0;
            double xm = 0.0;
            for (int j = 0; j < cols; j++)
            {
                sortj = sort[j];
                xm += vd[sortj] * a[sortj * baseRows + mpsRow];
                qv[sortj] = spellDps[sortj] * xm;
            }
            double xe = vd[sortj] * spellDps[sortj];
            for (int j = cols - 2; j >= 0; j--)
            {
                sortj = sort[j];
                qv[sortj] += a[sortj * baseRows + mpsRow] * xe;
                xe += vd[sortj] * spellDps[sortj];
            }
            double v = 0.0;
            for (int j = 0; j < cols; j++)
            {
                v += qv[j] * vd[j];
            }
            return -v * Qk;
        }

        public double[] SolvePrimalQuadratic(int mpsRow, int[] sort, double k, bool skipLinear, int maximizeColumn, double targetValue, int globalRestarts)
        {
            if (hardInfeasibility) return new double[cols + 1];
            double[] ret = null;
            double[] bestret = null;

            if (arraySet.cg_p == null || arraySet.cg_p.Length < cols + rows)
            {
                arraySet.RecreateCGArrays();
            }

            this.mpsRow = mpsRow;
            this.sort = sort;
            this.Qk = k;
            if (_S == null || _S.Length < cols)
            {
                _S = new int[cols];
            }

#if SILVERLIGHT
            {
                this.a = arraySet.SparseMatrixData;
                this.U = arraySet.LU_U;
                this.d = arraySet._d;
                this.x = arraySet._x;
                this.w = arraySet._w;
                this.ww = arraySet._ww;
                this.wd = arraySet._wd;
                this.c = arraySet._c;
                this.u = arraySet._u;
                this.cost = arraySet._cost;
                this.sparseValue = arraySet.SparseMatrixValue;
                this.D = arraySet.extraConstraints;
                this.B = _B;
                this.V = _V;
                this.S = _S;
                this.OSB = arraySet._orderedSuperbasis;
                this.sparseRow = arraySet.SparseMatrixRow;
                this.sparseCol = arraySet.SparseMatrixCol;
                this.flags = _flags;
                this.lb = _lb;
                this.ub = _ub;
                this.mb = _mb;
                this.pD = arraySet._pD;
                this.qx = arraySet._qx;
                this.qv = arraySet._qv;
                this.vd = arraySet._vd;
                this.spellDps = arraySet._spellDps;
                this.cg_p = arraySet.cg_p;
                this.cg_x = arraySet.cg_x;
                this.cg_w = arraySet.cg_w;
                this.cg_r = arraySet.cg_r;
                this.cg_qp = arraySet.cg_qp;
                this.cg_rr = arraySet.cg_rr;
                this.cg_ww = arraySet.cg_ww;

                SetupExtraConstraints();

                lu.BeginSafe();
#else
            fixed (double** pD = arraySet._pD)
            fixed (double* a = arraySet.SparseMatrixData, U = arraySet.LU_U, sL = arraySet.LUsparseL, column = arraySet.LUcolumn, column2 = arraySet.LUcolumn2, d = arraySet._d, x = arraySet._x, w = arraySet._w, ww = arraySet._ww, wd = arraySet._wd, qx = arraySet._qx, qv = arraySet._qv, vd = arraySet._vd, c = arraySet._c, u = arraySet._u, cost = arraySet._cost, spellDps = arraySet._spellDps, costw = arraySet._costWorking, sparseValue = arraySet.SparseMatrixValue, D = arraySet.extraConstraints, lb = _lb, ub = _ub, mb = _mb, cg_p = arraySet.cg_p, cg_x = arraySet.cg_x, cg_w = arraySet.cg_w, cg_r = arraySet.cg_r, cg_qp = arraySet.cg_qp, cg_rr = arraySet.cg_rr, cg_ww = arraySet.cg_ww)
            fixed (int* B = _B, V = _V, S = _S, OSB = arraySet._orderedSuperbasis, sparseRow = arraySet.SparseMatrixRow, sparseCol = arraySet.SparseMatrixCol, P = arraySet.LU_P, Q = arraySet.LU_Q, LJ = arraySet.LU_LJ, sLI = arraySet.LUsparseLI, sLstart = arraySet.LUsparseLstart, flags = _flags)
            {
                this.a = a;
                this.U = U;
                this.d = d;
                this.x = x;
                this.w = w;
                this.ww = ww;
                this.wd = wd;
                this.c = c;
                this.u = u;
                this.cost = cost;
                this.sparseValue = sparseValue;
                this.D = D;
                this.B = B;
                this.V = V;
                this.S = S;
                this.OSB = OSB;
                this.sparseRow = sparseRow;
                this.sparseCol = sparseCol;
                this.flags = flags;
                this.lb = lb;
                this.ub = ub;
                this.mb = mb;
                this.pD = pD;
                this.qx = qx;
                this.qv = qv;
                this.vd = vd;
                this.spellDps = spellDps;
                this.cg_p = cg_p;
                this.cg_x = cg_x;
                this.cg_w = cg_w;
                this.cg_r = cg_r;
                this.cg_qp = cg_qp;
                this.cg_rr = cg_rr;
                this.cg_ww = cg_ww;

                SetupExtraConstraints();

                lu.BeginUnsafe(U, sL, P, Q, LJ, sLI, sLstart, column, column2);
#endif
                int step = 10;
                double lastScore = double.NegativeInfinity;
                List<int> perturbColumns = null;
            GLOBALRESTART:
                if (maximizeColumn > -1)
                {
                    ret = SolvePrimalQuadraticCGReciprocalUnsafe(maximizeColumn, targetValue);
                }
                else
                {
                    if (!skipLinear)
                    {
                        // we have to fix flags in case we're doing MIP and we're resolving with linear
                        for (int i = 0; i < super; i++)
                        {
                            int col = S[i];
                            if ((flags[col] & flagLB) != 0)
                            {
                                flags[col] = (flags[col] & ~flagN) | flagNLB;
                            }
                            else
                            {
                                flags[col] = (flags[col] & ~flagN) | flagNUB;
                            }
                        }
                        super = 0;
                        ret = SolvePrimalUnsafe(false, false, false, true);
                    }
                    ret = SolvePrimalQuadraticCGUnsafe();
                }
                double score = ret[ret.Length - 1];
                if (bestret == null || score > bestret[bestret.Length - 1])
                {
                    bestret = ret;
                }
                if (globalRestarts > 0 && score != 0.0)
                {
                    /*if (Math.Abs(score - lastScore) < epsPrimal)
                    {
                        step *= 2;
                    }*/
                    lastScore = score;
                    skipLinear = true;
                    GlobalRestart2(step, ref perturbColumns);
                    globalRestarts--;
                    goto GLOBALRESTART;
                }
                lu.EndUnsafe();

                this.a = null;
                this.U = null;
                this.d = null;
                this.x = null;
                this.w = null;
                this.ww = null;
                this.wd = null;
                this.c = null;
                this.u = null;
                this.cost = null;
                this.sparseValue = null;
                this.D = null;
                this.pD = null;
                this.B = null;
                this.V = null;
                this.S = null;
                this.OSB = null;
                this.sparseRow = null;
                this.sparseCol = null;
                this.flags = null;
                this.lb = null;
                this.ub = null;
                this.mb = null;
                this.qx = null;
                this.qv = null;
                this.vd = null;
                this.spellDps = null;
                this.cg_p = null;
                this.cg_x = null;
                this.cg_w = null;
                this.cg_r = null;
                this.cg_qp = null;
                this.cg_rr = null;
                this.cg_ww = null;
            }

            return bestret;
        }

        private void GlobalRestart2(int steps, ref List<int> perturbColumns)
        {
            Random rnd = new Random();

            if (perturbColumns == null)
            {
                perturbColumns = new List<int>();
                for (int col = 0; col < cols; col++)
                {
                    if (spellDps[col] == 0.0 && a[col * baseRows + mpsRow] <= 0.0)
                    {
                        perturbColumns.Add(col);
                    }
                }
            }

            while (steps > 0)
            {
                steps--;

                // pick a random nonbasic mana variable
                int col = perturbColumns[rnd.Next(perturbColumns.Count)];
                int maxj = Array.IndexOf(_V, col);
                if (maxj == -1)
                {
                    continue;
                }
                double direction;

                if ((flags[col] & flagNLB) != 0)
                {
                    S[super] = col;
                    super++;
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                    osbDirty = true;

                    direction = 1.0;
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    S[super] = col;
                    super++;
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                    osbDirty = true;

                    direction = -1.0;
                }
                else
                {
                    if (rnd.Next(2) == 0)
                    {
                        direction = 1.0;
                    }
                    else
                    {
                        direction = -1.0;
                    }
                }

                // get blocking basic
                ComputeBasisStep(maxj);

                double dist;
                if (direction > 0.0)
                {
                    dist = ub[col] - mb[col];
                }
                else
                {
                    dist = mb[col] - lb[col];
                }
                if (dist > 100.0)
                {
                    dist = 100.0;
                }
                double minr = dist * rnd.NextDouble();

                if (minr > 0.0)
                {
                    minr *= direction;
                    mb[col] += minr;
                    for (int i = 0; i < rows; i++)
                    {
                        mb[B[i]] -= minr * w[i];
                    }
                }
            }
        }


        private void GlobalRestart(int steps)
        {
            Random rnd = new Random();

            Decompose();

            if (lu.Singular)
            {
                PatchSingularBasis();
                Decompose();
            }

            ComputePrimalSolution(false);
            UpdateMB();

            while (steps > 0)
            {
                steps--;

                // pick a random nonbasic mana variable
                int maxj;
                int col;
                do
                {
                    maxj = rnd.Next(cols);
                    col = V[maxj];
                } while (a[col * baseRows + mpsRow] > 0.0);
                double direction;
                if ((flags[col] & flagNLB) != 0)
                {
                    direction = 1.0;
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    direction = -1.0;
                }
                else
                {
                    if (rnd.Next(2) == 0)
                    {
                        direction = 1.0;
                    }
                    else
                    {
                        direction = -1.0;
                    }
                }

                // get blocking basic
                ComputeBasisStep(maxj);

                int mini = -1;
                double minr = double.PositiveInfinity;
                int bound = flagNMid;
                bool changeBasis = false;

                if (!SelectReducedGradientOutgoing(direction, epsPrimal, ref mini, ref minr, ref bound))
                {
                    return;
                }

                double dist;
                if (direction > 0.0)
                {
                    dist = ub[col] - mb[col];
                }
                else
                {
                    dist = mb[col] - lb[col];
                }
                if (double.IsInfinity(dist))
                {
                    continue;
                }
                if (minr >= dist - epsZero)
                {
                    // just a bound swap
                    minr = dist;
                    if ((flags[col] & flagNMid) != 0)
                    {
                        int s = Array.IndexOf(_S, col);
                        S[s] = S[super - 1];
                        super--;
                        osbDirty = true;
                    }
                    if (direction > 0.0)
                    {
                        flags[col] = (flags[col] & ~flagN) | flagNUB;
                    }
                    else
                    {
                        flags[col] = (flags[col] & ~flagN) | flagNLB;
                    }
                }
                else if (mini == -1)
                {
                    return;
                }
                else
                {
                    // we hit basic bound, update basis
                    changeBasis = true;
                    osbDirty = true;

                    if ((flags[col] & flagNMid) != 0)
                    {
                        int s = Array.IndexOf(_S, col);
                        S[s] = S[super - 1];
                        super--;
                        osbDirty = true;
                    }
                }

                if (minr > 0.0)
                {
                    minr *= direction;
                    mb[col] += minr;
                    for (int i = 0; i < rows; i++)
                    {
                        mb[B[i]] -= minr * w[i];
                    }
                }

                // swap basis
                if (changeBasis)
                {
                    double pivot;
                    lu.Update(ww, mini, out pivot);
                    if (lu.Singular || Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                    {
                        // we aren't making the swap
                        // make sure to properly add superbasic back to S if we cancelled swap
                        col = V[maxj];
                        if ((flags[col] & flagNMid) != 0)
                        {
                            S[super] = col;
                            super++;
                        }

                        Decompose();

                        if (lu.Singular)
                        {
                            PatchSingularBasis();
                            Decompose();
                        }

                        ComputePrimalSolution(false);
                        UpdateMB();
                        continue;
                    }

                    int k = B[mini];
                    B[mini] = V[maxj];
                    V[maxj] = k;
                    flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                    flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;

                    if (steps > 0 && steps % 50 == 0)
                    {
                        Decompose();

                        if (lu.Singular)
                        {
                            PatchSingularBasis();
                            Decompose();
                        }

                        ComputePrimalSolution(false);
                        UpdateMB();
                    }
                }
            }
        }

        private void Decompose()
        {
#if SILVERLIGHT
            for (int j = 0; j < rows; j++)
            {
                int col = B[j];
                if (col < cols)
                {
                    int i = 0;
                    for (; i < baseRows; i++)
                    {
                        U[i * rows + j] = a[i + col * baseRows];
                    }
                    for (int k = 0; k < numExtraConstraints; k++, i++)
                    {
                        U[i * rows + j] = pD[k][col];
                    }
                }
                else
                {
                    for (int i = 0; i < rows; i++)
                    {
                        U[i * rows + j] = (i == col - cols) ? 1.0 : 0.0;
                    }
                }
            }
#else
            double* uj = U;
            double* ujend = U + rows;
            int* bj = B;
            for (; uj < ujend; uj++, bj++)
            {
                int col = *bj;
                if (col < cols)
                {
                    double* ai = a + (col * baseRows);
                    double* aend = ai + baseRows;
                    double* ui = uj;
                    for (; ai < aend; ai++, ui += rows)
                    {
                        *ui = *ai;
                    }
                    for (int k = 0; k < numExtraConstraints; k++, ui += rows)
                    {
                        *ui = pD[k][col];
                    }
                }
                else
                {
                    for (int i = 0; i < rows; i++)
                    {
                        uj[i * rows] = (i == col - cols) ? 1.0 : 0.0;
                    }
                }
            }
#endif
            lu.Decompose();
        }

        private void PatchSingularBasis()
        {
            for (int j = lu.Rank; j < rows; j++)
            {
                int singularColumn = arraySet.LU_Q[j];
                int singularRow = arraySet.LU_P[j];
                int slackColumn = cols + singularRow;
                int vindex = Array.IndexOf(_V, slackColumn);
                V[vindex] = B[singularColumn];
                B[singularColumn] = slackColumn;
                if ((flags[V[vindex]] & flagLB) != 0)
                {
                    flags[V[vindex]] = (flags[V[vindex]] | flagNLB) & ~flagB & ~flagDis;
                }
                else
                {
                    flags[V[vindex]] = (flags[V[vindex]] | flagNUB) & ~flagB & ~flagDis;
                }
                if ((flags[B[singularColumn]] & flagNMid) != 0)
                {
                    int s = Array.IndexOf(_S, B[singularColumn]);
                    S[s] = S[super - 1];
                    super--;
                }
                flags[B[singularColumn]] = (flags[B[singularColumn]] | flagB) & ~flagN & ~flagDis;
                //ValidateSuperBasis();
            }
            osbDirty = true;
        }

        private double DualPhaseILowerBound(int col)
        {
            //if (col < cols && (flags[col] & flagLB) == 0) return -1.0;
            if ((flags[col] & flagLB) == 0) return -1.0;
            return 0.0;
        }

        private double DualPhaseIUpperBound(int col)
        {
            //if (col < cols && (flags[col] & flagUB) == 0) return 1.0;
            if ((flags[col] & flagUB) == 0) return 1.0;
            return 0.0;
        }

        private void ComputePrimalSolution(bool dualPhaseI)
        {
            Zero(d, rows);
            //int j = 0;
            //for (; j < baseRows; j++)
            //{
            //    d[j] = b[j];
            //}
            //for (int k = 0; k < numExtraConstraints; k++, j++)
            //{
            //    d[j] = D[k * (cols + 1) + cols];
            //}
            for (int col = 0; col < cols + rows; col++)
            {
                double v = 0.0;
                if (dualPhaseI)
                {
                    if ((flags[col] & flagNLB) != 0)
                    {
                        v = DualPhaseILowerBound(col);
                    }
                    else if ((flags[col] & flagNUB) != 0)
                    {
                        v = DualPhaseIUpperBound(col);
                    }
                }
                else
                {
                    if ((flags[col] & flagNLB) != 0)
                    {
                        v = lb[col];
                    }
                    else if ((flags[col] & flagNUB) != 0)
                    {
                        v = ub[col];
                    }
                    else if ((flags[col] & flagNMid) != 0)
                    {
                        v = mb[col];
                    }
                }
                if (Math.Abs(v) >= epsZero)
                {
                    if (col < cols)
                    {
                        int sCol1 = sparseCol[col];
                        int sCol2 = sparseCol[col + 1];
#if SILVERLIGHT
                        for (int i = sCol1; i < sCol2; i++)
                        {
                            d[sparseRow[i]] -= sparseValue[i] * v;
                        }

#else
                        int* sRow = sparseRow + sCol1;
                        double* sValue = sparseValue + sCol1;
                        for (int i = sCol1; i < sCol2; i++, sRow++, sValue++)
                        {
                            d[*sRow] -= *sValue * v;
                        }
#endif
                        for (int k = 0; k < numExtraConstraints; k++)
                        {
                            d[baseRows + k] -= pD[k][col] * v;
                        }
                    }
                    else
                    {
                        d[col - cols] -= v;
                    }
                }
            }
            lu.FSolve(d);
        }

        private bool IsPrimalFeasible(double eps)
        {
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                if (d[i] < lb[col] - Math.Abs(lb[col]) * epsPrimalRel - eps || d[i] > ub[col] + Math.Abs(ub[col]) * epsPrimalRel + eps)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsPrimalQFeasible(double eps, out bool recalculate)
        {
            bool feasible = true;
            recalculate = false;
            for (int i = 0; i < rows + cols; i++)
            {
                if (mb[i] < lb[i] - Math.Abs(lb[i]) * epsPrimalRel - eps)
                {
                    if ((flags[i] & flagN) != 0)
                    {
                        mb[i] = lb[i];
                        recalculate = true;
                    }
                    feasible = false;
                }
                else if (mb[i] > ub[i] + Math.Abs(ub[i]) * epsPrimalRel + eps)
                {
                    if ((flags[i] & flagN) != 0)
                    {
                        mb[i] = ub[i];
                        recalculate = true;
                    }
                    feasible = false;
                }
            }
            return feasible;
        }

        private void ComputeReducedCosts()
        {
            for (int i = 0; i < rows; i++)
            {
                //if (B[i] < cols) u[i] = cost[B[i]];
                //else u[i] = 0.0;
                u[i] = cost[B[i]];
            }
            lu.BSolve(u);
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];

                if (col < cols)
                {
                    double costcol = cost[col];
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
#if SILVERLIGHT
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        costcol -= sparseValue[i] * u[sparseRow[i]];
                    }

#else
                    int* sRow = sparseRow + sCol1;
                    double* sValue = sparseValue + sCol1;
                    for (int i = sCol1; i < sCol2; i++, sRow++, sValue++)
                    {
                        costcol -= *sValue * u[*sRow];
                    }
#endif
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        costcol -= pD[k][col] * u[baseRows + k];
                    }
                    c[j] = costcol;
                }
                else
                {
                    c[j] = cost[col] - u[col - cols];
                }
            }
        }

        private void ComputeReducedCostGradient()
        {
            ComputeQx();
            for (int i = 0; i < rows; i++)
            {
                //if (B[i] < cols) u[i] = cost[B[i]];
                //else u[i] = 0.0;
                int col = B[i];
                if (col < cols)
                {
                    u[i] = cost[col] - Qk * qx[col];
                }
                else
                {
                    u[i] = cost[col];
                }
            }
            lu.BSolve(u);
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];

                if (col < cols)
                {
                    double costcol = cost[col] - Qk * qx[col];
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
#if SILVERLIGHT
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        costcol -= sparseValue[i] * u[sparseRow[i]];
                    }

#else
                    int* sRow = sparseRow + sCol1;
                    double* sValue = sparseValue + sCol1;
                    for (int i = sCol1; i < sCol2; i++, sRow++, sValue++)
                    {
                        costcol -= *sValue * u[*sRow];
                    }
#endif
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        costcol -= pD[k][col] * u[baseRows + k];
                    }
                    c[j] = costcol;
                }
                else
                {
                    c[j] = cost[col] - u[col - cols];
                }
            }
        }

        private void ComputePhaseIReducedCosts(out double infeasibility, double eps)
        {
            infeasibility = 0.0;
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                if (d[i] < lb[col] - Math.Abs(lb[col]) * epsPrimalRel - eps)
                {
                    x[i] = 1.0;
                    infeasibility += lb[col] - d[i];
                }
                else if (d[i] > ub[col] + Math.Abs(ub[col]) * epsPrimalRel + eps)
                {
                    x[i] = -1.0;
                    infeasibility += d[i] - ub[col];
                }
                else x[i] = 0.0;
            }
            lu.BSolve(x);
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];

                if (col < cols)
                {
                    double costcol = 0;
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
#if SILVERLIGHT
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        costcol -= sparseValue[i] * x[sparseRow[i]];
                    }

#else
                    int* sRow = sparseRow + sCol1;
                    double* sValue = sparseValue + sCol1;
                    for (int i = sCol1; i < sCol2; i++, sRow++, sValue++)
                    {
                        costcol -= *sValue * x[*sRow];
                    }
#endif
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        costcol -= pD[k][col] * x[baseRows + k];
                    }
                    c[j] = costcol;
                }
                else
                {
                    c[j] = -x[col - cols];
                }
            }
        }

        private void ComputePhaseIQReducedCosts(out double infeasibility, double eps)
        {
            infeasibility = 0.0;
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                if (mb[col] < lb[col] - Math.Abs(lb[col]) * epsPrimalRel - eps)
                {
                    x[i] = 1.0;
                    infeasibility += lb[col] - mb[col];
                }
                else if (mb[col] > ub[col] + Math.Abs(ub[col]) * epsPrimalRel + eps)
                {
                    x[i] = -1.0;
                    infeasibility += mb[col] - ub[col];
                }
                else x[i] = 0.0;
            }
            lu.BSolve(x);
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];

                if (col < cols)
                {
                    double costcol = 0;
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
#if SILVERLIGHT
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        costcol -= sparseValue[i] * x[sparseRow[i]];
                    }

#else
                    int* sRow = sparseRow + sCol1;
                    double* sValue = sparseValue + sCol1;
                    for (int i = sCol1; i < sCol2; i++, sRow++, sValue++)
                    {
                        costcol -= *sValue * x[*sRow];
                    }
#endif
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        costcol -= pD[k][col] * x[baseRows + k];
                    }
                    c[j] = costcol;
                }
                else
                {
                    c[j] = -x[col - cols];
                }
            }
        }

        private double[] ComputeReturnSolution()
        {
            double[] ret = new double[cols + 1];
            double value = 0.0;
            for (int i = 0; i < rows; i++)
            {
                if (B[i] < cols)
                {
                    double di = d[i];
                    if (Math.Abs(di - lb[B[i]]) < Math.Abs(lb[B[i]]) * epsPrimalRel + epsPrimalLow || di < lb[B[i]]) di = lb[B[i]];
                    else if (Math.Abs(di - ub[B[i]]) < Math.Abs(ub[B[i]]) * epsPrimalRel + epsPrimalLow || di > ub[B[i]]) di = ub[B[i]];
                    ret[B[i]] = di;
                    value += cost[B[i]] * di;
                }
            }
            for (int i = 0; i < cols; i++)
            {
                if ((flags[i] & flagNLB) != 0)
                {
                    ret[i] = lb[i];
                    value += cost[i] * lb[i];
                }
                else if ((flags[i] & flagNUB) != 0)
                {
                    ret[i] = ub[i];
                    value += cost[i] * ub[i];
                }
                else if ((flags[i] & flagNMid) != 0)
                {
                    ret[i] = mb[i];
                    value += cost[i] * mb[i];
                }
            }
            ret[cols] = value;
            return ret;
        }

        private double[] ComputeReturnSolutionQuadratic()
        {
            double[] ret = new double[cols + 1];
            double value = 0.0;
            for (int col = 0; col < cols; col++)
            {
                if ((flags[col] & flagNLB) != 0)
                {
                    ret[col] = lb[col];
                    value += cost[col] * lb[col];
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    ret[col] = ub[col];
                    value += cost[col] * ub[col];
                }
                else
                {
                    double di = mb[col];
                    if (Math.Abs(di - lb[col]) < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow || di < lb[col]) di = lb[col];
                    else if (Math.Abs(di - ub[col]) < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow || di > ub[col]) di = ub[col];
                    ret[col] = di;
                    value += cost[col] * di;
                }
            }
            ComputeQx();
            for (int i = 0; i < cols; i++)
            {
                value -= 0.5 * Qk * qx[i] * mb[i];
            }
            ret[cols] = value;
            return ret;
        }

        private double ComputeValue()
        {
            double value = 0.0;
            for (int i = 0; i < rows; i++)
            {
                if (B[i] < cols)
                {
                    value += cost[B[i]] * d[i];
                }
            }
            for (int i = 0; i < cols; i++)
            {
                if ((flags[i] & flagNLB) != 0)
                {
                    value += cost[i] * lb[i];
                }
                else if ((flags[i] & flagNUB) != 0)
                {
                    value += cost[i] * ub[i];
                }
                else if ((flags[i] & flagNMid) != 0)
                {
                    value += cost[i] * mb[i];
                }
            }
            return value;
        }

        private double ComputeValueQuadratic()
        {
            double value = 0.0;
            for (int i = 0; i < cols; i++)
            {
                value += cost[i] * mb[i];
            }
            ComputeQx();
            for (int i = 0; i < cols; i++)
            {
                value -= 0.5 * Qk * qx[i] * mb[i];
            }
            /*for (int i = 0; i < cols; i++)
            {
                value -= 0.5 * Qk * mb[i] * mb[i] * a[i * baseRows + mpsRow] * spellDps[i];
            }
            for (int i = 0; i < cols; i++)
            {
                for (int j = i + 1; j < cols; j++)
                {
                    if (sortinv[i] < sortinv[j])
                    {
                        value -= Qk * mb[i] * mb[j] * a[i * baseRows + mpsRow] * spellDps[j];
                    }
                    else
                    {
                        value -= Qk * mb[i] * mb[j] * a[j * baseRows + mpsRow] * spellDps[i];
                    }
                }
            }*/
            return value;
        }

        private double ComputeDualIValue()
        {
            double value = 0.0;
            for (int i = 0; i < rows; i++)
            {
                if (B[i] < cols)
                {
                    value += cost[B[i]] * d[i];
                }
            }
            for (int i = 0; i < cols; i++)
            {
                if ((flags[i] & flagNLB) != 0)
                {
                    value += cost[i] * DualPhaseILowerBound(i);
                }
                else if ((flags[i] & flagNUB) != 0)
                {
                    value += cost[i] * DualPhaseIUpperBound(i);
                }
            }
            return value;
        }

        private int SelectPrimalIncoming(out double direction, bool prepareForDual)
        {
            double maxc = (prepareForDual) ? epsDualI : epsDual;
            int maxj = -1;
            direction = 0.0;
            double dir = 0.0;
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                double costj = 0.0;
                if ((flags[col] & flagNLB) != 0)
                {
                    costj = c[j];
                    dir = 1.0;
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    costj = -c[j];
                    dir = -1.0;
                }
                else if ((flags[col] & flagNMid) != 0)
                {
                    if (c[j] > maxc)
                    {
                        costj = c[j];
                        dir = 1.0;
                    }
                    else
                    {
                        costj = -c[j];
                        dir = -1.0;
                    }
                }
                if (costj > maxc && (flags[col] & flagDis) == 0 && (flags[col] & flagFix) == 0)
                {
                    maxc = costj;
                    maxj = j;
                    direction = dir;
                }
            }
            return maxj;
        }

        private int SelectPrimalIncomingBoundary(out double direction, bool prepareForDual)
        {
            double maxc = (prepareForDual) ? epsDualI : epsDual;
            int maxj = -1;
            direction = 0.0;
            double dir = 0.0;
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                double costj = 0.0;
                if ((flags[col] & flagNLB) != 0)
                {
                    costj = c[j];
                    dir = 1.0;
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    costj = -c[j];
                    dir = -1.0;
                }
                else if ((flags[col] & flagNMid) != 0)
                {
                    continue;
                }
                if (costj > maxc && (flags[col] & flagDis) == 0 && (flags[col] & flagFix) == 0)
                {
                    maxc = costj;
                    maxj = j;
                    direction = dir;
                }
            }
            return maxj;
        }

        private int SelectPrimalAny()
        {
            double maxc = double.NegativeInfinity;
            int maxj = -1;
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                double costj = 0.0;
                if ((flags[col] & flagNLB) != 0)
                {
                    costj = c[j];
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    costj = -c[j];
                }
                else if ((flags[col] & flagNMid) != 0)
                {
                    if (c[j] > maxc)
                    {
                        costj = c[j];
                    }
                    else
                    {
                        costj = -c[j];
                    }
                }
                if (costj > maxc && (flags[col] & flagDis) == 0 && (flags[col] & flagFix) == 0)
                {
                    maxc = costj;
                    maxj = j;
                }
            }
            return maxj;
        }

        private int SelectPrimalReciprocal(int maximizeColumn, double eps)
        {
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                if (mb[col] < lb[col] + Math.Abs(lb[col]) * epsPrimalRel + eps)
                {
                    x[i] = 1.0;
                }
                else if (mb[col] > ub[col] - Math.Abs(ub[col]) * epsPrimalRel - eps)
                {
                    x[i] = -1.0;
                }
                else x[i] = 0.0;
            }
            lu.BSolve(x);

            double maxc = epsDual;
            int maxj = -1;

            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                if (col == maximizeColumn)
                {
                    continue;
                }

                double costcol;
                if (col < cols)
                {
                    costcol = 0;
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
#if SILVERLIGHT
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        costcol -= sparseValue[i] * x[sparseRow[i]];
                    }

#else
                    int* sRow = sparseRow + sCol1;
                    double* sValue = sparseValue + sCol1;
                    for (int i = sCol1; i < sCol2; i++, sRow++, sValue++)
                    {
                        costcol -= *sValue * x[*sRow];
                    }
#endif
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        costcol -= pD[k][col] * x[baseRows + k];
                    }
                }
                else
                {
                    costcol = -x[col - cols];
                }

                double costj;
                if ((flags[col] & flagNLB) != 0)
                {
                    costj = costcol;
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    costj = -costcol;
                }
                else/* if ((flags[col] & flagNMid) != 0)*/
                {
                    if (costcol > maxc)
                    {
                        costj = costcol;
                    }
                    else
                    {
                        costj = -costcol;
                    }
                }
                if (costj > maxc && (flags[col] & flagDis) == 0 && (flags[col] & flagFix) == 0)
                {
                    maxc = costj;
                    maxj = j;
                }
            }

            return maxj;
        }

        private int CheckTermination(out double direction)
        {
            double dir = 0.0;
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                double costj = 0.0;
                if ((flags[col] & flagNLB) != 0)
                {
                    costj = c[j];
                    dir = 1.0;
                }
                else if ((flags[col] & flagNUB) != 0)
                {
                    costj = -c[j];
                    dir = -1.0;
                }
                else if ((flags[col] & flagNMid) != 0)
                {
                    if (c[j] > epsDual)
                    {
                        costj = c[j];
                        dir = 1.0;
                    }
                    else
                    {
                        costj = -c[j];
                        dir = -1.0;
                    }
                }
                if (costj > epsDual && (flags[col] & flagDis) == 0 && (flags[col] & flagFix) == 0)
                {
                    ComputeBasisStep(j);
                    double t = ComputeQuadraticLimit(j, dir);

                    if (0.5 * c[j] * dir * t > epsPrimal)
                    {
                        direction = dir;
                        return j;
                    }
                }
            }
            direction = 0.0;
            return -1;
        }

        private bool SelectCGOutgoing(double eps, ref int refcol, ref double refalpha, ref int refbound)
        {
            for (int col = 0; col < cols + rows; col++)
            {
                double difflb = mb[col] - lb[col];
                double diffub = mb[col] - ub[col];
                bool ifeasiblelb = (difflb >= -Math.Abs(lb[col]) * epsPrimalRel - eps);
                bool ifeasibleub = (diffub <= +Math.Abs(ub[col]) * epsPrimalRel + eps);
                bool ifeasible = ifeasiblelb && ifeasibleub;
                if (!ifeasible)
                {
                    return false;
                }

                if (Math.Abs(cg_x[col]) > epsZero)
                {
                    // mb[col] + alpha * cg_x[col]
                    double limit;
                    if (cg_x[col] > 0)
                    {
                        if (mb[col] + refalpha * cg_x[col] - ub[col] > +Math.Abs(ub[col]) * epsPrimalRel + eps)
                        {
                            limit = (ub[col] - mb[col]) / cg_x[col];
                            if (limit < refalpha)
                            {
                                refalpha = limit;
                                refcol = col;
                                refbound = flagNUB;
                            }
                        }
                    }
                    else
                    {
                        if (mb[col] + refalpha * cg_x[col] - lb[col] < -Math.Abs(lb[col]) * epsPrimalRel - eps)
                        {
                            limit = (lb[col] - mb[col]) / cg_x[col];
                            if (limit < refalpha)
                            {
                                refalpha = limit;
                                refcol = col;
                                refbound = flagNLB;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool SelectPrimalOutgoing(double direction, bool feasible, double eps, ref int refmini, ref double refminr, ref int refbound)
        {
            // min over i of d[i]/w[i] where w[i]>0
            double minrr = refminr;
            double minr = refminr;
            int mini = refmini;
            int bound = refbound;
            double minv = 0.0;
            //double reducedcost = c[incoming];
            for (int i = 0; i < rows; i++)
            {
                double wi = w[i];
                double v = Math.Abs(wi);
                double widir = direction * wi;
                int col = B[i];
                double difflb = d[i] - lb[col];
                double diffub = d[i] - ub[col];
                bool ifeasiblelb = (difflb >= -Math.Abs(lb[col]) * epsPrimalRel - eps);
                bool ifeasibleub = (diffub <= +Math.Abs(ub[col]) * epsPrimalRel + eps);
                bool ifeasible = ifeasiblelb && ifeasibleub;
                if (feasible && !ifeasible)
                {
                    // we lost primal feasibility
                    // fall back to phase I
                    refmini = mini;
                    refminr = minr;
                    refbound = bound;
                    return false;
                }
                if (feasible || ifeasible)
                {
                    if (widir > epsPivot && (flags[col] & flagLB) != 0)
                    {
                        goto LBCHECK;
                    }
                    else if (widir < -epsPivot && (flags[col] & flagUB) != 0)
                    {
                        goto UBCHECK;
                    }
                }
                else
                {
                    if (!ifeasibleub && widir > epsPivot)
                    {
                        goto UBCHECK;
                    }
                    else if (!ifeasiblelb && widir < -epsPivot)
                    {
                        goto LBCHECK;
                    }
                }
                continue;
            LBCHECK:
                double r = difflb / widir;
                if (r < minrr + epsZero && (r < minrr || v > minv))
                {
                    minrr = r;
                    minr = difflb / wi;
                    mini = i;
                    minv = v;
                    bound = flagNLB;
                }
                continue;
            UBCHECK:
                r = diffub / widir;
                if (r < minrr + epsZero && (r < minrr || v > minv))
                {
                    minrr = r;
                    minr = diffub / wi;
                    mini = i;
                    minv = v;
                    bound = flagNUB;
                }
                continue;
            }
            refmini = mini;
            refminr = minr;
            refbound = bound;
            return true;
        }

        private void SelectPhaseIQOutgoing(double direction, double eps, ref int refmini, ref double refminr, ref int refbound)
        {
            // min over i of d[i]/w[i] where w[i]>0
            double minrr = refminr;
            double minr = refminr;
            int mini = refmini;
            int bound = refbound;
            double minv = 0.0;
            //double reducedcost = c[incoming];
            for (int i = 0; i < rows; i++)
            {
                double wi = w[i];
                double v = Math.Abs(wi);
                double widir = direction * wi;
                int col = B[i];
                double difflb = mb[col] - lb[col];
                double diffub = mb[col] - ub[col];
                bool ifeasiblelb = (difflb >= -Math.Abs(lb[col]) * epsPrimalRel - eps);
                bool ifeasibleub = (diffub <= +Math.Abs(ub[col]) * epsPrimalRel + eps);
                bool ifeasible = ifeasiblelb && ifeasibleub;
                if (ifeasible)
                {
                    if (widir > epsPivot && (flags[col] & flagLB) != 0)
                    {
                        goto LBCHECK;
                    }
                    else if (widir < -epsPivot && (flags[col] & flagUB) != 0)
                    {
                        goto UBCHECK;
                    }
                }
                else
                {
                    if (!ifeasibleub && widir > epsPivot)
                    {
                        goto UBCHECK;
                    }
                    else if (!ifeasiblelb && widir < -epsPivot)
                    {
                        goto LBCHECK;
                    }
                }
                continue;
            LBCHECK:
                double r = difflb / widir;
                if (r < minrr + epsZero && (r < minrr || v > minv))
                {
                    minrr = r;
                    minr = difflb / wi;
                    mini = i;
                    minv = v;
                    bound = flagNLB;
                }
                continue;
            UBCHECK:
                r = diffub / widir;
                if (r < minrr + epsZero && (r < minrr || v > minv))
                {
                    minrr = r;
                    minr = diffub / wi;
                    mini = i;
                    minv = v;
                    bound = flagNUB;
                }
                continue;
            }
            refmini = mini;
            refminr = minr;
            refbound = bound;
        }

        private bool SelectReducedGradientOutgoing(double direction, double eps, ref int refmini, ref double refminr, ref int refbound)
        {
            // min over i of d[i]/w[i] where w[i]>0
            double minrr = refminr;
            int mini = refmini;
            int bound = refbound;
            double minv = 0.0;
            //double reducedcost = c[incoming];
            for (int i = 0; i < rows; i++)
            {
                double wi = w[i];
                double v = Math.Abs(wi);
                double widir = direction * wi;
                int col = B[i];
                double difflb = mb[col] - lb[col];
                double diffub = mb[col] - ub[col];
                bool ifeasiblelb = (difflb >= -Math.Abs(lb[col]) * epsPrimalRel - eps);
                bool ifeasibleub = (diffub <= +Math.Abs(ub[col]) * epsPrimalRel + eps);
                bool ifeasible = ifeasiblelb && ifeasibleub;
                if (!ifeasible)
                {
                    // we lost feasibility
                    return false;
                }
                if (widir > epsPivot && (flags[col] & flagLB) != 0)
                {
                    goto LBCHECK;
                }
                else if (widir < -epsPivot && (flags[col] & flagUB) != 0)
                {
                    goto UBCHECK;
                }
                continue;
            LBCHECK:
                double r = difflb / widir;
                if (r < minrr + epsZero && (r < minrr || v > minv))
                {
                    minrr = r;
                    mini = i;
                    minv = v;
                    bound = flagNLB;
                }
                continue;
            UBCHECK:
                r = diffub / widir;
                if (r < minrr + epsZero && (r < minrr || v > minv))
                {
                    minrr = r;
                    mini = i;
                    minv = v;
                    bound = flagNUB;
                }
                continue;
            }
            refmini = mini;
            refminr = minrr;
            refbound = bound;
            return true;
        }

        private double ComputeQuadraticLimit(int incoming, double direction)
        {
            // feasible direction v, 1 in component incoming
            // [B N]*[vB vN] = 0
            // vB = -Binv * N*vN = -w
            // increase in value = t * vT * grad + t^2/2 * vT * Q * v
            // vT * grad = c[incoming]
            double vQv = ComputevQv(incoming, direction);
            // c[incoming] + t * vQv = 0
            if (vQv > -epsZero)
            {
                return double.PositiveInfinity;
            }
            return -c[incoming] * direction / vQv;
        }

        private void ComputeBasisStep(int incoming)
        {
            // w = U \ (L \ A(:,j));
            int maxcol = V[incoming];
            if (maxcol < cols)
            {
#if SILVERLIGHT
                int i = 0;
                for (; i < baseRows; i++)
                {
                    w[i] = a[i + maxcol * baseRows];
                }
                for (int k = 0; k < numExtraConstraints; k++, i++)
                {
                    w[i] = pD[k][maxcol];
                }
#else
                Copy(w, a + (maxcol * baseRows), baseRows);
                /*const int c = ~3;
                int trunc = baseRows & c;
                double* source = a + (maxcol * baseRows);
                double* wk = w;
                double* arr1 = wk + trunc;
                double* arr2 = wk + baseRows;
                for (; wk < arr1; wk += 4, source += 4)
                {
                    wk[0] = source[0];
                    wk[1] = source[1];
                    wk[2] = source[2];
                    wk[3] = source[3];
                }
                for (; wk < arr2; wk++, source++)
                {
                    *wk = *source;
                }*/
                if (numExtraConstraints > 0)
                {
                    double* wk = w + baseRows;
                    double* wkend = wk + numExtraConstraints;
                    double** pdk = pD;
                    for (; wk < wkend; wk++, pdk++)
                    {
                        *wk = (*pdk)[maxcol];
                    }
                }
#endif
            }
            else
            {
                Zero(w, rows);
                w[maxcol - cols] = 1.0;
                /*for (int i = 0; i < rows; i++)
                {
                    w[i] = (i == maxcol - cols) ? 1.0 : 0.0;
                }*/
            }
            //lu.FSolve(w);
            lu.FSolveL(w, ww, 0.00000001);
            lu.FSolveU(ww, w, 0.00000001);
        }

        private void ComputeBasisReplace(int incoming)
        {
            // w = U \ (L \ A(:,j));
            int maxcol = incoming;
            if (maxcol < cols)
            {
#if SILVERLIGHT
                int i = 0;
                for (; i < baseRows; i++)
                {
                    w[i] = a[i + maxcol * baseRows];
                }
                for (int k = 0; k < numExtraConstraints; k++, i++)
                {
                    w[i] = pD[k][maxcol];
                }
#else
                Copy(w, a + (maxcol * baseRows), baseRows);
                /*const int c = ~3;
                int trunc = baseRows & c;
                double* source = a + (maxcol * baseRows);
                double* wk = w;
                double* arr1 = wk + trunc;
                double* arr2 = wk + baseRows;
                for (; wk < arr1; wk += 4, source += 4)
                {
                    wk[0] = source[0];
                    wk[1] = source[1];
                    wk[2] = source[2];
                    wk[3] = source[3];
                }
                for (; wk < arr2; wk++, source++)
                {
                    *wk = *source;
                }*/
                if (numExtraConstraints > 0)
                {
                    double* wk = w + baseRows;
                    double* wkend = wk + numExtraConstraints;
                    double** pdk = pD;
                    for (; wk < wkend; wk++, pdk++)
                    {
                        *wk = (*pdk)[maxcol];
                    }
                }
#endif
            }
            else
            {
                Zero(w, rows);
                w[maxcol - cols] = 1.0;
                /*for (int i = 0; i < rows; i++)
                {
                    w[i] = (i == maxcol - cols) ? 1.0 : 0.0;
                }*/
            }
            lu.FSolveL(w, ww, 0.00000001);
        }

        private int SelectDualOutgoing(bool phaseI, out double delta, out int bound)
        {
            double mind = 0.0;
            int mini = -1;
            delta = 0.0;
            bound = 0;
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                double lbcol, ubcol;
                if (phaseI)
                {
                    lbcol = DualPhaseILowerBound(col);
                    ubcol = DualPhaseIUpperBound(col);
                }
                else
                {
                    lbcol = lb[col];
                    ubcol = ub[col];
                }
                if ((flags[col] & flagDis) == 0)
                {
                    if ((phaseI || (flags[col] & flagLB) != 0) && d[i] < lbcol - Math.Abs(lbcol) * epsPrimalRel - epsPrimal)
                    {
                        double dist = d[i] - lbcol;
                        if (dist < mind)
                        {
                            mind = dist;
                            mini = i;
                            delta = dist;
                            bound = flagNLB;
                        }
                    }
                    else if ((phaseI || (flags[col] & flagUB) != 0) && d[i] > ubcol + Math.Abs(ubcol) * epsPrimalRel + epsPrimal)
                    {
                        double dist = ubcol - d[i];
                        if (dist < mind)
                        {
                            mind = dist;
                            mini = i;
                            delta = -dist;
                            bound = flagNUB;
                        }
                    }
                }
            }
            return mini;
        }

        private bool ComputeDualPivotRow(bool phaseI, int outgoing)
        {
            // x = z <- e_mini*A_B^-1
            /*for (int i = 0; i < rows; i++)
            {
                x[i] = ((i == outgoing) ? 1 : 0);
            }
            lu.BSolve(x); // TODO exploit nature of x*/
            lu.BSolveUnit(x, outgoing);

#if SILVERLIGHT
            double eps = phaseI ? epsDualI : epsDual;
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                if (phaseI)
                {
                    if ((flags[col] & flagUB) != 0 && (flags[col] & flagLB) != 0) continue;
                }
                else
                {
                    if ((flags[col] & flagFix) != 0) continue;
                }
                if ((flags[col] & flagNLB) != 0 && c[j] > eps)
                {
                    // we lost dual feasibility
                    // creative use of shifting
                    double shift = eps - c[j];
                    if (Math.Abs(shift) < 1.0e-5)
                    {
                        c[j] += shift;
                        cost[col] += shift; // requires working copy for cost
                        costWorkingDirty = true;
                        shifted = true;
                    }
                    else
                    {
                        // fall back to dual phase I
                        return false;
                    }
                }
                else if ((flags[col] & flagNUB) != 0 && c[j] < -eps)
                {
                    // we lost dual feasibility
                    // creative use of shifting
                    double shift = -eps - c[j];
                    if (Math.Abs(shift) < 1.0e-5)
                    {
                        c[j] += shift;
                        cost[col] += shift; // requires working copy for cost
                        costWorkingDirty = true;
                        shifted = true;
                    }
                    else
                    {
                        // fall back to dual phase I
                        return false;
                    }
                }
                if (col < cols)
                {
                    wd[j] = 0;
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        wd[j] += sparseValue[i] * x[sparseRow[i]];
                    }
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        wd[j] += pD[k][col] * x[baseRows + k];
                    }
                }
                else
                {
                    wd[j] = x[col - cols];
                }
            }
#else
            double* cj = c;
            double* wdj = wd;
            int* Vj = V;
            double eps = phaseI ? epsDualI : epsDual;
            for (int j = 0; j < cols; j++, cj++, wdj++, Vj++)
            {
                int col = *Vj;
                if (phaseI)
                {
                    if ((flags[col] & flagUB) != 0 && (flags[col] & flagLB) != 0) continue;
                }
                else
                {
                    if ((flags[col] & flagFix) != 0) continue;
                }
                if ((flags[col] & flagNLB) != 0 && *cj > eps)
                {
                    // we lost dual feasibility
                    // creative use of shifting
                    double shift = eps - *cj;
                    if (Math.Abs(shift) < 1.0e-5)
                    {
                        c[j] += shift;
                        cost[col] += shift; // requires working copy for cost
                        costWorkingDirty = true;
                        shifted = true;
                    }
                    else
                    {
                        // fall back to dual phase I
                        return false;
                    }
                }
                else if ((flags[col] & flagNUB) != 0 && *cj < -eps)
                {
                    // we lost dual feasibility
                    // creative use of shifting
                    double shift = -eps - *cj;
                    if (Math.Abs(shift) < 1.0e-5)
                    {
                        c[j] += shift;
                        cost[col] += shift; // requires working copy for cost
                        costWorkingDirty = true;
                        shifted = true;
                    }
                    else
                    {
                        // fall back to dual phase I
                        return false;
                    }
                }
                if (col < cols)
                {
                    *wdj = 0;
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
                    int* sRow = sparseRow + sCol1;
                    int* sRow2 = sparseRow + sCol2;
                    double* sValue = sparseValue + sCol1;
                    for (; sRow < sRow2; sRow++, sValue++)
                    {
                        *wdj += *sValue * x[*sRow];
                    }
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        *wdj += pD[k][col] * x[baseRows + k];
                    }
                }
                else
                {
                    *wdj = x[col - cols];
                }
            }
#endif
            return true;
        }

        private void SelectDualIncoming(bool phaseI, int outgoing, out int minj, out double minr)
        {
            double minrr = double.PositiveInfinity;
            minr = double.PositiveInfinity;
            minj = -1;
            double minv = 0.0;
            double eps = phaseI ? epsDualI : epsDual;
            double direction = 1.0;
            if (phaseI)
            {
                if (d[outgoing] > DualPhaseIUpperBound(B[outgoing]))
                {
                    direction = -1.0;
                }
            }
            else
            {
                if (d[outgoing] > ub[B[outgoing]])
                {
                    direction = -1.0;
                }
            }
#if SILVERLIGHT
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                if (phaseI)
                {
                    if ((flags[col] & flagUB) != 0 && (flags[col] & flagLB) != 0) continue;
                }
                else
                {
                    if ((flags[col] & flagFix) != 0) continue;
                }
                double v = Math.Abs(wd[j]);
                double wdir = direction * wd[j];
                if (((flags[col] & flagNLB) != 0 && wdir < -epsPivot) || ((flags[col] & flagNUB) != 0 && wdir > epsPivot))
                {
                    double r = c[j] / wdir;
                    if (r < minrr + epsZero && (r < minrr || v > minv))
                    {
                        minr = c[j] / wd[j];
                        minrr = r;
                        minj = j;
                        minv = v;
                    }
                }
            }
#else
            double* cj = c;
            double* wdj = wd;
            int* Vj = V;
            for (int j = 0; j < cols; j++, cj++, wdj++, Vj++)
            {
                int col = *Vj;
                if (phaseI)
                {
                    if ((flags[col] & flagUB) != 0 && (flags[col] & flagLB) != 0) continue;
                }
                else
                {
                    if ((flags[col] & flagFix) != 0) continue;
                }
                double v = Math.Abs(*wdj);
                double wdir = direction * *wdj;
                if (((flags[col] & flagNLB) != 0 && wdir < -epsPivot) || ((flags[col] & flagNUB) != 0 && wdir > epsPivot))
                {
                    double r = *cj / wdir;
                    if (r < minrr + epsZero && (r < minrr || v > minv))
                    {
                        minr = *cj / *wdj;
                        minrr = r;
                        minj = j;
                        minv = v;
                    }
                }
            }
#endif
        }

#if SILVERLIGHT
        private void SelectDualIncomingWithBoundFlips(bool phaseI, double delta, out int minj, out double minr)
        {
            minj = -1;
            minr = double.PositiveInfinity;
            //double minv = 0.0;
            double eps = phaseI ? epsDualI : epsDual;
            double epsp = epsPivot;
            bool positive = (delta > 0);
            if (!positive) delta = -delta;
            int available = 0;
            int allavailable = 0;
            bool retried = false;

        RETRY:
            // mark eligible pivots and compute max step
            double maxstep = double.PositiveInfinity;
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                flags[col] &= ~flagPivot2;
                if (phaseI)
                {
                    if ((flags[col] & flagUB) != 0 && (flags[col] & flagLB) != 0)
                    {
                        flags[col] &= ~flagPivot;
                        continue;
                    }
                }
                else
                {
                    if ((flags[col] & flagFix) != 0)
                    {
                        flags[col] &= ~flagPivot;
                        continue;
                    }
                }
                double wdir;
                if (positive)
                    wdir = -wd[j];
                else
                    wdir = wd[j];
                if (((flags[col] & flagNLB) != 0 && wdir < -epsp) || ((flags[col] & flagNUB) != 0 && wdir > epsp))
                {
                    double r;
                    if (wdir > 0)
                    {
                        r = (c[j] + eps) / wdir;
                    }
                    else
                    {
                        r = (c[j] - eps) / wdir;
                    }
                    if (r < maxstep) maxstep = r;
                    flags[col] |= flagPivot;
                    available++;
                    allavailable++;
                }
                else
                {
                    flags[col] &= ~flagPivot;
                }
            }
            if (available == 0 && Math.Abs(delta) < epsPivot && !retried)
            {
                retried = true;
                epsp = 1.0e-7;
                goto RETRY;
            }

            double deltamin = 0.0;
            double step = Math.Max(maxstep * 10.0, eps);
            while (delta - deltamin > epsZero && allavailable > 0)
            {
                available = 0;
                delta -= deltamin;
                maxstep = double.PositiveInfinity;
                deltamin = 0;
                for (int j = 0; j < cols; j++)
                {
                    int col = V[j];
                    flags[col] &= ~flagPivot2;
                    if ((flags[col] & flagPivot) != 0)
                    {
                        double wdir;
                        if (positive)
                            wdir = -wd[j];
                        else
                            wdir = wd[j];
                        double r;
                        double v = Math.Abs(wd[j]);
                        if (wdir > 0) // upper bound
                        {
                            r = (c[j] + eps) / wdir;
                            if (c[j] - step * wdir < -eps)
                            {
                                flags[col] &= ~flagPivot;
                                flags[col] |= flagPivot2;
                                available++;
                                allavailable--;
                                if (phaseI)
                                {
                                    deltamin += (DualPhaseIUpperBound(col) - DualPhaseILowerBound(col)) * v;
                                }
                                else
                                {
                                    deltamin += (ub[col] - lb[col]) * v;
                                }
                            }
                            else if (r < maxstep) maxstep = r;
                        }
                        else // lower bound
                        {
                            r = (c[j] - eps) / wdir;
                            if (c[j] - step * wdir > eps)
                            {
                                flags[col] &= ~flagPivot;
                                flags[col] |= flagPivot2;
                                available++;
                                allavailable--;
                                if (phaseI)
                                {
                                    deltamin += (DualPhaseIUpperBound(col) - DualPhaseILowerBound(col)) * v;
                                }
                                else
                                {
                                    deltamin += (ub[col] - lb[col]) * v;
                                }
                            }
                            else if (r < maxstep) maxstep = r;
                        }
                    }
                }
                step = 2.0 * maxstep;
            }

            while (delta >= 0 && (available > 0 || delta > epsZero))
            {
                maxstep = double.PositiveInfinity;
                for (int j = 0; j < cols; j++)
                {
                    int col = V[j];
                    if ((flags[col] & flagPivot2) != 0)
                    {
                        double wdir;
                        if (positive)
                            wdir = -wd[j];
                        else
                            wdir = wd[j];
                        double r;
                        if (wdir > 0)
                        {
                            r = (c[j] + eps) / wdir;
                        }
                        else
                        {
                            r = (c[j] - eps) / wdir;
                        }
                        if (r < maxstep) maxstep = r;
                    }
                }

                minj = -1;
                double minv = 0.0;
                for (int j = 0; j < cols; j++)
                {
                    int col = V[j];
                    if ((flags[col] & flagPivot2) != 0)
                    {
                        double wdir;
                        if (positive)
                            wdir = -wd[j];
                        else
                            wdir = wd[j];
                        double r = c[j] / wdir;
                        double v = Math.Abs(wd[j]);
                        if (r <= maxstep)
                        {
                            if (phaseI)
                            {
                                delta -= (DualPhaseIUpperBound(col) - DualPhaseILowerBound(col)) * v;
                            }
                            else
                            {
                                delta -= (ub[col] - lb[col]) * v;
                            }
                            flags[col] &= ~flagPivot2;
                            available--;
                            if (v > minv)
                            {
                                minv = v;
                                minj = j;
                            }
                        }
                    }
                }
                if (minj == -1) return;
            }

            if (minj != -1)
            {
                minr = c[minj] / wd[minj];
            }
        }
#else
        private unsafe void SelectDualIncomingWithBoundFlips(bool phaseI, double delta, out int minj, out double minr)
        {
            minj = -1;
            minr = double.PositiveInfinity;
            //double minv = 0.0;
            double eps = phaseI ? epsDualI : epsDual;
            double epsp = epsPivot;
            bool positive = (delta > 0);
            if (!positive) delta = -delta;
            int available = 0;
            int allavailable = 0;
            bool retried = false;

        RETRY:
            // mark eligible pivots and compute max step
            double maxstep = double.PositiveInfinity;
            double* cj = c;
            double* wdj = wd;
            int* Vj = V;
            for (int j = 0; j < cols; j++, cj++, wdj++, Vj++)
            {
                int col = *Vj;
                flags[col] &= ~flagPivot2;
                if (phaseI)
                {
                    if ((flags[col] & flagUB) != 0 && (flags[col] & flagLB) != 0)
                    {
                        flags[col] &= ~flagPivot;
                        continue;
                    }
                }
                else
                {
                    if ((flags[col] & flagFix) != 0)
                    {
                        flags[col] &= ~flagPivot;
                        continue;
                    }
                }
                double wdir;
                if (positive)
                    wdir = -*wdj;
                else
                    wdir = *wdj;
                if (((flags[col] & flagNLB) != 0 && wdir < -epsp) || ((flags[col] & flagNUB) != 0 && wdir > epsp))
                {
                    double r;
                    if (wdir > 0)
                    {
                        r = (*cj + eps) / wdir;
                    }
                    else
                    {
                        r = (*cj - eps) / wdir;
                    }
                    if (r < maxstep) maxstep = r;
                    flags[col] |= flagPivot;
                    available++;
                    allavailable++;
                }
                else
                {
                    flags[col] &= ~flagPivot;
                }
            }
            if (available == 0 && Math.Abs(delta) < epsPivot && !retried)
            {
                retried = true;
                epsp = 1.0e-7;
                goto RETRY;
            }

            double deltamin = 0.0;
            double step = Math.Max(maxstep * 10.0, eps);
            while (delta - deltamin > epsZero && allavailable > 0)
            {
                available = 0;
                delta -= deltamin;
                maxstep = double.PositiveInfinity;
                cj = c;
                wdj = wd;
                Vj = V;
                deltamin = 0;
                for (int j = 0; j < cols; j++, cj++, wdj++, Vj++)
                {
                    int col = *Vj;
                    flags[col] &= ~flagPivot2;
                    if ((flags[col] & flagPivot) != 0)
                    {
                        double wdir;
                        if (positive)
                            wdir = -*wdj;
                        else
                            wdir = *wdj;
                        double r;
                        double v = Math.Abs(*wdj);
                        if (wdir > 0) // upper bound
                        {
                            r = (*cj + eps) / wdir;
                            if (*cj - step * wdir < -eps)
                            {
                                flags[col] &= ~flagPivot;
                                flags[col] |= flagPivot2;
                                available++;
                                allavailable--;
                                if (phaseI)
                                {
                                    deltamin += (DualPhaseIUpperBound(col) - DualPhaseILowerBound(col)) * v;
                                }
                                else
                                {
                                    deltamin += (ub[col] - lb[col]) * v;
                                }
                            }
                            else if (r < maxstep) maxstep = r;
                        }
                        else // lower bound
                        {
                            r = (*cj - eps) / wdir;
                            if (*cj - step * wdir > eps)
                            {
                                flags[col] &= ~flagPivot;
                                flags[col] |= flagPivot2;
                                available++;
                                allavailable--;
                                if (phaseI)
                                {
                                    deltamin += (DualPhaseIUpperBound(col) - DualPhaseILowerBound(col)) * v;
                                }
                                else
                                {
                                    deltamin += (ub[col] - lb[col]) * v;
                                }
                            }
                            else if (r < maxstep) maxstep = r;
                        }
                    }
                }
                step = 2.0 * maxstep;
            }

            while (delta >= 0 && (available > 0 || delta > epsZero))
            {
                maxstep = double.PositiveInfinity;
                cj = c;
                wdj = wd;
                Vj = V;
                for (int j = 0; j < cols; j++, cj++, wdj++, Vj++)
                {
                    int col = *Vj;
                    if ((flags[col] & flagPivot2) != 0)
                    {
                        double wdir;
                        if (positive)
                            wdir = -*wdj;
                        else
                            wdir = *wdj;
                        double r;
                        if (wdir > 0)
                        {
                            r = (*cj + eps) / wdir;
                        }
                        else
                        {
                            r = (*cj - eps) / wdir;
                        }
                        if (r < maxstep) maxstep = r;
                    }
                }

                minj = -1;
                double minv = 0.0;
                cj = c;
                wdj = wd;
                Vj = V;
                for (int j = 0; j < cols; j++, cj++, wdj++, Vj++)
                {
                    int col = *Vj;
                    if ((flags[col] & flagPivot2) != 0)
                    {
                        double wdir;
                        if (positive)
                            wdir = -*wdj;
                        else
                            wdir = *wdj;
                        double r = *cj / wdir;
                        double v = Math.Abs(*wdj);
                        if (r <= maxstep)
                        {
                            if (phaseI)
                            {
                                delta -= (DualPhaseIUpperBound(col) - DualPhaseILowerBound(col)) * v;
                            }
                            else
                            {
                                delta -= (ub[col] - lb[col]) * v;
                            }
                            flags[col] &= ~flagPivot2;
                            available--;
                            if (v > minv)
                            {
                                minv = v;
                                minj = j;
                            }
                        }
                    }
                }
                if (minj == -1) return;
            }

            if (minj != -1)
            {
                minr = c[minj] / wd[minj];
            }
        }
#endif

        private void UpdatePrimal(double minr, int mini, int maxj)
        {
            // minr = primal step
            // rd = dual step

            double rd = c[maxj] / w[mini];

            // x = z <- e_mini*A_B^-1
            /*for (int i = 0; i < rows; i++)
            {
                x[i] = ((i == mini) ? 1.0 : 0.0);
            }
            lu.BSolve(x); // TODO exploit nature of x*/
            lu.BSolveUnit(x, mini);

            // update primal and dual
            if (Math.Abs(minr) > epsZero)
            {
                for (int i = 0; i < rows; i++)
                {
                    d[i] -= minr * w[i];
                }
            }
            if ((flags[V[maxj]] & flagNLB) != 0)
            {
                d[mini] = lb[V[maxj]] + minr;
            }
            else if ((flags[V[maxj]] & flagNUB) != 0)
            {
                d[mini] = ub[V[maxj]] + minr;
            }
#if SILVERLIGHT
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                if (col < cols)
                {
                    //double v = 0.0;
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
                    // TODO reinvestigate moving rd out of the loop once dual is more stable
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        c[j] -= rd * sparseValue[i] * x[sparseRow[i]];
                    }
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        c[j] -= rd * pD[k][col] * x[baseRows + k];
                    }
                }
                else
                {
                    c[j] -= rd * x[col - cols];
                }
            }
#else
            double* cc = c;
            for (int j = 0; j < cols; j++, cc++)
            {
                int col = V[j];
                if (col < cols)
                {
                    //double v = 0.0;
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
                    int* sRow = sparseRow + sCol1;
                    int* sRow2 = sparseRow + sCol2;
                    double* sValue = sparseValue + sCol1;
                    // TODO reinvestigate moving rd out of the loop once dual is more stable
                    double sum = 0.0;
                    for (; sRow < sRow2; sRow++, sValue++)
                    {
                        sum += *sValue * x[*sRow];
                    }
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        sum += pD[k][col] * x[baseRows + k];
                    }
                    *cc -= rd * sum;
                }
                else
                {
                    *cc -= rd * x[col - cols];
                }
            }
#endif
            c[maxj] = -rd;
        }

        private void UpdateDual(bool phaseI, int minj, int mini, double minr, double delta, bool updatec)
        {
            // w = U \ (L \ A(:,j));
            int mincol = V[minj];
            if (mincol < cols)
            {
#if SILVERLIGHT
                int i = 0;
                for (; i < baseRows; i++)
                {
                    w[i] = a[i + mincol * baseRows];
                }
                for (int k = 0; k < numExtraConstraints; k++, i++)
                {
                    w[i] = pD[k][mincol];
                }
#else
                double* wi = w;
                double* ai = a + (mincol * baseRows);
                double* aend = ai + baseRows;
                for (; ai < aend; ai++, wi++)
                {
                    *wi = *ai;
                }
                for (int k = 0; k < numExtraConstraints; k++, wi++)
                {
                    *wi = pD[k][mincol];
                }
#endif
            }
            else
            {
                //Array.Clear(_w, 0, rows);
                //w[mincol - cols] = 1.0;
                for (int i = 0; i < rows; i++)
                {
                    w[i] = (i == mincol - cols) ? 1.0 : 0.0;
                }
            }
            //lu.FSolve(w);
            lu.FSolveL(w, ww, 0.00000001);
            lu.FSolveU(ww, w, 0.00000001);

            // -minr = dual step
            // rp = primal step

            //System.Diagnostics.Trace.WriteLine(w[mini] + " = " + wd[minj]);

            double l, u;
            if (phaseI)
            {
                u = DualPhaseIUpperBound(B[mini]);
                l = DualPhaseILowerBound(B[mini]);
            }
            else
            {
                u = ub[B[mini]];
                l = lb[B[mini]];
            }
            if (delta > 0)
            {
                delta = d[mini] - u;
            }
            else
            {
                delta = d[mini] - l;
            }
            double rp = delta / w[mini];

            // update primal and dual
            for (int i = 0; i < rows; i++)
            {
                d[i] -= rp * w[i];
            }
            if ((flags[mincol] & flagNLB) != 0)
            {
                d[mini] = (phaseI ? DualPhaseILowerBound(mincol) : lb[mincol]) + rp;
            }
            else if ((flags[mincol] & flagNUB) != 0)
            {
                d[mini] = (phaseI ? DualPhaseIUpperBound(mincol) : ub[mincol]) + rp;
            }
            if (updatec)
            {
#if SILVERLIGHT
                for (int j = 0; j < cols; j++)
                {
                    c[j] -= minr * wd[j];
                }
#else
                double* ccols = c + cols;
                for (double* cj = c, wdj = wd; cj < ccols; cj++, wdj++)
                {
                    *cj -= minr * *wdj;
                }
#endif
                c[minj] = -minr;
            }
        }

        private void PerturbCost()
        {
            Random rnd = new Random(0);
            for (int i = 0; i < cols; i++)
            {
                // usually fixed positions would not be perturbed, but in the context of MIP branching it's necessary to use the same perturbation
                // this wouldn't be an issue if the variable was nonbasic, but especially in MIP the fixed positions will
                // most often be on basic positions (for example when disabling a variable in a branch) and thus it's cost influences
                // reduced costs of all nonbasic variable which could lead to loss of feasibility
                //if ((flags[i] & flagFix) == 0)
                //{
                cost[i] -= 0.5 * (100.0 * epsDual + Math.Abs(cost[i]) * 1.0e-5) * (1 + rnd.NextDouble()); // perturb in negative direction since all variables/slacks have lower bound
                //}
            }
            costWorkingDirty = true;
        }

        private void LoadCost()
        {
            Array.Copy(arraySet._cost, 0, arraySet._costWorking, 0, cols + rows);
            costWorkingDirty = false;
        }

        private bool BoundFlip(bool phaseI, double dualStep, double eps, int incoming, bool flipBounds)
        {
            bool needDualI = false;
            bool flipsDone = false;
            Array.Clear(arraySet._x, 0, rows);
            bool modc = (Math.Abs(dualStep) > epsZero && incoming != -1);
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                if (!flipBounds) flags[col] &= ~flagFlip;
                if (j != incoming)
                {
                    if ((flags[col] & flagFix) != 0) continue;
                    if (modc) c[j] -= dualStep * wd[j];
                    double v = 0.0;
                    if ((flags[col] & flagNLB) != 0 && c[j] > eps)
                    {
                        if (phaseI)
                        {
                            // phase I flip
                            v = DualPhaseIUpperBound(col) - DualPhaseILowerBound(col);
                            if (flipBounds) flags[col] = (flags[col] | flagNUB) & ~flagNLB;
                            else flags[col] |= flagFlip;
                        }
                        else if ((flags[col] & flagUB) != 0)
                        {
                            // phase II flip
                            v = ub[col] - lb[col];
                            if (flipBounds) flags[col] = (flags[col] | flagNUB) & ~flagNLB;
                            else flags[col] |= flagFlip;
                        }
                        else
                        {
                            needDualI = true;
                        }
                    }
                    else if ((flags[col] & flagNUB) != 0 && c[j] < -eps)
                    {
                        if (phaseI)
                        {
                            // phase I flip
                            v = DualPhaseILowerBound(col) - DualPhaseIUpperBound(col);
                            if (flipBounds) flags[col] = (flags[col] | flagNLB) & ~flagNUB;
                            else flags[col] |= flagFlip;
                        }
                        else if ((flags[col] & flagLB) != 0)
                        {
                            // phase II flip
                            v = lb[col] - ub[col];
                            if (flipBounds) flags[col] = (flags[col] | flagNLB) & ~flagNUB;
                            else flags[col] |= flagFlip;
                        }
                        else
                        {
                            needDualI = true;
                        }
                    }
                    if (Math.Abs(v) > epsZero)
                    {
                        if (col < cols)
                        {
                            int sCol1 = sparseCol[col];
                            int sCol2 = sparseCol[col + 1];
#if SILVERLIGHT
                            for (int i = sCol1; i < sCol2; i++)
                            {
                                x[sparseRow[i]] += v * sparseValue[i];
                            }
#else
                            int* sRow = sparseRow + sCol1;
                            int* sRow2 = sparseRow + sCol2;
                            double* sValue = sparseValue + sCol1;
                            for (; sRow < sRow2; sRow++, sValue++)
                            {
                                x[*sRow] += v * *sValue;
                            }
#endif
                            for (int k = 0; k < numExtraConstraints; k++)
                            {
                                x[baseRows + k] += v * pD[k][col];
                            }
                        }
                        else
                        {
                            x[col - cols] += v;
                        }
                        flipsDone = true;
                    }
                }
            }
            if (incoming != -1) c[incoming] = -dualStep;
            if (flipsDone)
            {
                lu.FSolve(x);
                for (int i = 0; i < rows; i++)
                {
                    d[i] -= x[i];
                }
            }
            return needDualI;
        }

        private void UpdateBounds()
        {
            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                if ((flags[col] & flagFlip) != 0)
                {
                    //System.Diagnostics.Debug.WriteLine("Flipped " + col);
                    if ((flags[col] & flagNLB) != 0)
                    {
                        flags[col] = (flags[col] | flagNUB) & ~flagNLB;
                    }
                    else
                    {
                        flags[col] = (flags[col] | flagNLB) & ~flagNUB;
                    }
                }
            }
        }

        private bool PreprocessingSingletonConstraint()
        {
            bool updated = false;
            // singleton row            
            for (int i = 0; i < rows; i++)
            {
                //a[i + col * baseRows];
                double rhs = 0.0;
                int singlecol = -1;
                bool issingleton = true;
                for (int col = 0; col < cols; col++)
                {
                    double v;
                    if (i < baseRows)
                    {
                        v = a[i + col * baseRows];
                    }
                    else
                    {
                        v = pD[i - baseRows][col];
                    }
                    if (Math.Abs(v) > epsZero)
                    {
                        if ((flags[col] & flagFix) != 0)
                        {
                            rhs -= v * lb[col];
                        }
                        else
                        {
                            if (singlecol != -1)
                            {
                                issingleton = false;
                                break;
                            }
                            else
                            {
                                singlecol = col;
                            }
                        }
                    }
                }
                if (issingleton)
                {
                    if (singlecol == -1)
                    {
                        // all variables are fixed => row is fixed
                        updated |= UpdateLowerBound(cols + i, -rhs);
                        updated |= UpdateUpperBound(cols + i, -rhs);
                    }
                    else
                    {
                        // update col bounds
                        // v * x + s = rhs
                        // lb <= s <= ub
                        // -ub <= -s <= -lb
                        // x = (rhs - s) / v
                        double v;
                        if (i < baseRows)
                        {
                            v = a[i + singlecol * baseRows];
                        }
                        else
                        {
                            v = pD[i - baseRows][singlecol];
                        }
                        if (v > 0)
                        {
                            // (rhs - ub) / v <= x = (rhs - s) / v <= (rhs - lb) / v
                            updated |= UpdateLowerBound(singlecol, (rhs - ub[cols + i]) / v);
                            updated |= UpdateUpperBound(singlecol, (rhs - lb[cols + i]) / v);
                        }
                        else
                        {
                            // (rhs - ub) / v >= x = (rhs - s) / v >= (rhs - lb) / v
                            updated |= UpdateUpperBound(singlecol, (rhs - ub[cols + i]) / v);
                            updated |= UpdateLowerBound(singlecol, (rhs - lb[cols + i]) / v);
                        }
                    }
                }
            }
            return updated;
        }

        private bool PreprocessingTightenBounds()
        {
            bool updated = false;
            // singleton row            
            for (int i = 0; i < rows; i++)
            {
                //a[i + col * baseRows];
                double lbi = 0.0;
                double ubi = 0.0;
                for (int col = 0; col < cols; col++)
                {
                    double v;
                    if (i < baseRows)
                    {
                        v = a[i + col * baseRows];
                    }
                    else
                    {
                        v = pD[i - baseRows][col];
                    }
                    if (Math.Abs(v) > epsZero)
                    {
                        if (v > 0)
                        {
                            lbi += v * lb[col];
                            ubi += v * ub[col];
                        }
                        else
                        {
                            lbi += v * ub[col];
                            ubi += v * lb[col];
                        }
                    }
                }
                updated |= UpdateLowerBound(cols + i, -ubi);
                updated |= UpdateUpperBound(cols + i, -lbi);
                for (int col = 0; col < cols; col++)
                {
                    double v;
                    if (i < baseRows)
                    {
                        v = a[i + col * baseRows];
                    }
                    else
                    {
                        v = pD[i - baseRows][col];
                    }
                    if (Math.Abs(v) > epsZero)
                    {
                        if (v > 0)
                        {
                            updated |= UpdateUpperBound(col, lb[col] - (lbi + lb[cols + i]) / v);
                            updated |= UpdateLowerBound(col, ub[col] - (ub[cols + i] + ubi) / v);
                        }
                        else
                        {
                            updated |= UpdateLowerBound(col, ub[col] - (lbi + lb[cols + i]) / v);
                            updated |= UpdateUpperBound(col, lb[col] - (ub[cols + i] + ubi) / v);
                        }
                    }
                }
            }
            return updated;
        }

        private void Preprocessing()
        {
            if (hardInfeasibility) return;
            bool updated;
            do
            {
                updated = false;
                //updated |= PreprocessingSingletonConstraint();
                updated |= PreprocessingTightenBounds();
            } while (updated && !hardInfeasibility);
        }

        /*
        private void InitializeDSEWeights(bool identity)
        {
            if (identity)
            {
                for (int i = 0; i < rows; i++)
                {
                    beta[i] = 1.0;
                }
            }
            else
            {
                for (int j = 0; j < rows; j++)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        x[i] = ((i == j) ? 1 : 0);
                    }
                    lu.BSolve(x); // TODO exploit nature of x
                    beta[j] = 0.0;
                    for (int i = 0; i < rows; i++)
                    {
                        beta[j] += x[i] * x[i];
                    }
                }
            }
        }*/

        private double[] SolvePrimalUnsafe(bool prepareForDual, bool verifyRefactoredOptimality, bool shortLimit, bool highPrecision)
        {
            // LU = A_B
            // d = x_B <- A_B^-1*b ... primal solution
            // u = u <- c_B*A_B^-1 ... dual solution
            // w_N <- c_N - u*A_N  ... dual solution

            int limit = 5000;

            RESTART:
            int i, j, k;
            bool feasible = false;
            int round = 0;
            int redecompose = 0;
            const int maxRedecompose = 50;
            int verificationAttempts = 0;
            double eps = highPrecision ? epsPrimal : epsPrimalLow;
            double lowestInfeasibility = double.PositiveInfinity;
            int lastFeasible = 0;

            if (disabledDirty)
            {
                for (i = 0; i < cols + rows; i++)
                {
                    flags[i] &= ~flagDis;
                }
                disabledDirty = false;
            }

            do
            {
            DECOMPOSE:
                if (redecompose <= 0)
                {
                    Decompose();
                    redecompose = maxRedecompose; // decompose every 50 iterations to maintain stability
                    feasible = false; // when refactoring basis recompute the solution
                }
                if (lu.Singular)
                {
                    redecompose = 0;
                    PatchSingularBasis();
                    continue;
                }

                if (!feasible)
                {
                    ComputePrimalSolution(false);
                    feasible = IsPrimalFeasible(eps);

                    if (feasible)
                    {
                        lastFeasible = round;
                        // we have a feasible solution, initialize phase 2
                        ComputeReducedCosts();
                    }
                    else if (shortLimit)
                    {
                        // we're retuning dual optimality on unshifted/redecomposed basis
                        // if redecomposition caused loss of primal optimality then we probably
                        // won't be able to fix the optimality and doing so might just cause us problems
                        // in the dual
                        // abort and let dual handle the problems if we're also dual unfeasible
                        return null;
                    }
                }

                if (!feasible)
                {
                    double infeasibility;
                    ComputePhaseIReducedCosts(out infeasibility, eps);
                    if (infeasibility < lowestInfeasibility) lowestInfeasibility = infeasibility;
                    else if (infeasibility > lowestInfeasibility + eps)
                    {
                        // we're not using a shifting strategy for primal so the only way to combat
                        // numerical cycling is to lower the tolerances
                        if (eps < 1.0)
                        {
                            eps *= 10.0;
                            lowestInfeasibility = double.PositiveInfinity;
                        }
                        if (redecompose < maxRedecompose)
                        {
                            redecompose = 0;
                            goto DECOMPOSE;
                        }
                    }
                    if (infeasibility > 100.0 && !ColdStart)
                    {
                        // we're so far out of feasible region we're better off starting from scratch
                        for (i = 0; i < rows; i++)
                        {
                            B[i] = cols + i;
                            flags[cols + i] = (flags[cols + i] | flagB) & ~flagN & ~flagDis;
                        }
                        for (j = 0; j < cols; j++)
                        {
                            V[j] = j;
                            flags[j] = (flags[j] | flagNLB) & ~flagB & ~flagDis;
                        }
                        disabledDirty = false;
                        redecompose = 0;
                        ColdStart = true;
                        continue;
                    }
                }

            MINISTEP:
                double direction;
                int maxj = SelectPrimalIncoming(out direction, prepareForDual);

                if (maxj == -1)
                {
                    if (feasible && verifyRefactoredOptimality && redecompose < maxRedecompose && verificationAttempts < 5)
                    {
                        redecompose = 0;
                        feasible = false;
                        verificationAttempts++;
                        continue;
                    }
                    /*if (blacklistAllowed)
                    {
                        bool retry = false;
                        if (disabledDirty)
                        {
                            for (i = 0; i < cols + rows; i++)
                            {
                                if ((flags[i] & flagDis) != 0)
                                {
                                    flags[i] &= ~flagDis;
                                    retry = true;
                                }
                            }
                            disabledDirty = false;
                        }
                        if (retry)
                        {
                            blacklistAllowed = false;
                            redecompose = 0;
                            needsRecalc = true;
                            continue;
                        }
                    }*/

                    // rebuild solution so it's stable
                    //ComputePrimalSolution();

                    // optimum, return solution (or could be no feasible solution)
                    if (!feasible) return new double[cols + 1]; // if it's not feasible then return null solution
                    ColdStart = false;
                    //System.Diagnostics.Trace.WriteLine("Primal optimality in round " + round);
                    return ComputeReturnSolution();
                }

                int mini = -1;
                double minr = double.PositiveInfinity;
                int bound = 0;

                ComputeBasisStep(maxj);

                if (!SelectPrimalOutgoing(direction, feasible, eps, ref mini, ref minr, ref bound))
                {
                    feasible = false;
                    lowestInfeasibility = double.PositiveInfinity;
                    redecompose = 0;
                    goto DECOMPOSE;
                }

                if (mini == -1)
                {
                    // unbounded
                    if (!ColdStart)
                    {
                        // something went really horribly wrong, try from scratch
                        for (i = 0; i < rows; i++)
                        {
                            B[i] = cols + i;
                            flags[cols + i] = (flags[cols + i] | flagB) & ~flagN & ~flagDis;
                        }
                        for (j = 0; j < cols; j++)
                        {
                            V[j] = j;
                            flags[j] = (flags[j] | flagNLB) & ~flagB & ~flagDis;
                        }
                        redecompose = 0;
                        ColdStart = true;
                        continue;
                    }
                    // completely unstable, investigate
                    double[] ret = new double[cols + 1];
                    return ret;
                }

                // determine if we do a basis change or just a bound swap
                int col = V[maxj];
                if ((flags[col] & (flagLB | flagUB)) == (flagLB | flagUB) && Math.Abs(minr) >= ub[col] - lb[col] - epsZero)
                {
                    // do bound swap
                    if (direction > 0.0)
                    {
                        flags[col] = (flags[col] | flagNUB) & ~flagNLB;
                    }
                    else
                    {
                        flags[col] = (flags[col] | flagNLB) & ~flagNUB;
                    }

                    if (!feasible)
                    {
                        round++; // still increment to avoid infinite cycles
                        continue;
                    }

                    minr = direction * (ub[col] - lb[col]);
                    for (i = 0; i < rows; i++)
                    {
                        d[i] -= minr * w[i];
                    }

                    goto MINISTEP;
                }

                if (feasible)
                {
                    UpdatePrimal(minr, mini, maxj);
                }

                // swap base

                redecompose--;
                if (redecompose > 0)
                {
                    double pivot;
                    lu.Update(ww, mini, out pivot);
                    if (lu.Singular || Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                    {
                        if (redecompose == maxRedecompose - 1)
                        {
                            redecompose = 0;
                            feasible = false; // forces recalc
                            flags[V[maxj]] |= flagDis;
                            disabledDirty = true;
                            continue;
                        }
                        else
                        {
                            redecompose = 0;
                            feasible = false;
                            continue;
                        }
                    }
                }

                k = B[mini];
                B[mini] = V[maxj];
                V[maxj] = k;
                flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;

                round++;
            } while ((round < limit && !shortLimit) || round < 100); // limit computation so we don't dead loop, if everything works it shouldn't take more than this
            // when tuning dual feasibility limit to 100 rounds, we don't want to spend too much time on it

            // just in case
            // if feasible return the best we got
            if (feasible) return ComputeReturnSolution();
            if (lastFeasible > 0 && !shortLimit)
            {
                limit = lastFeasible;
                for (i = 0; i < rows; i++)
                {
                    B[i] = cols + i;
                    flags[cols + i] = (flags[cols + i] | flagB) & ~flagN & ~flagDis;
                }
                for (j = 0; j < cols; j++)
                {
                    V[j] = j;
                    flags[j] = (flags[j] | flagNLB) & ~flagB & ~flagDis;
                }
                goto RESTART;
            }
            return new double[cols + 1];
        }

        private double[] SolvePrimalQuadraticUnsafe()
        {
            // LU = A_B
            // d = x_B <- A_B^-1*b ... primal solution
            // u = u <- c_B*A_B^-1 ... dual solution
            // w_N <- c_N - u*A_N  ... dual solution

            int limit = 5000;

        RESTART:
            int i, j, k;
            bool feasible = false;
            int round = 0;
            int redecompose = 0;
            const int maxRedecompose = 50;
            double eps = epsPrimal;
            double lowestInfeasibility = double.PositiveInfinity;
            int lastFeasible = 0;

            if (disabledDirty)
            {
                for (i = 0; i < cols + rows; i++)
                {
                    flags[i] &= ~flagDis;
                }
                disabledDirty = false;
            }

            do
            {
            DECOMPOSE:
                if (redecompose <= 0)
                {
                    Decompose();
                    redecompose = maxRedecompose; // decompose every 50 iterations to maintain stability
                    feasible = false; // when refactoring basis recompute the solution
                }
                if (lu.Singular)
                {
                    redecompose = 0;
                    PatchSingularBasis();
                    continue;
                }

                if (!feasible)
                {
                    ComputePrimalSolution(false);
                    feasible = IsPrimalFeasible(eps);

                    if (feasible)
                    {
                        lastFeasible = round;
                    }
                }

                if (!feasible)
                {
                    double infeasibility;
                    ComputePhaseIReducedCosts(out infeasibility, eps);
                    if (infeasibility < lowestInfeasibility) lowestInfeasibility = infeasibility;
                    else if (infeasibility > lowestInfeasibility + eps)
                    {
                        // we're not using a shifting strategy for primal so the only way to combat
                        // numerical cycling is to lower the tolerances
                        if (eps < 1.0)
                        {
                            eps *= 10.0;
                            lowestInfeasibility = double.PositiveInfinity;
                        }
                        if (redecompose < maxRedecompose)
                        {
                            redecompose = 0;
                            goto DECOMPOSE;
                        }
                    }
                    if (infeasibility > 100.0 && !ColdStart)
                    {
                        // we're so far out of feasible region we're better off starting from scratch
                        for (i = 0; i < rows; i++)
                        {
                            B[i] = cols + i;
                            flags[cols + i] = (flags[cols + i] | flagB) & ~flagN & ~flagDis;
                        }
                        for (j = 0; j < cols; j++)
                        {
                            V[j] = j;
                            flags[j] = (flags[j] | flagNLB) & ~flagB & ~flagDis;
                        }
                        disabledDirty = false;
                        redecompose = 0;
                        ColdStart = true;
                        continue;
                    }
                }

            MINISTEP:
                if (feasible)
                {
                    UpdateMB();
                    ComputeReducedCostGradient();
                }

                double direction;
                int maxj = SelectPrimalIncoming(out direction, false);

                if (maxj == -1)
                {
                    // TODO verify optimality
                    // so far we only know it's a KKT point
                    /*if (feasible && verifyRefactoredOptimality && redecompose < maxRedecompose && verificationAttempts < 5)
                    {
                        redecompose = 0;
                        feasible = false;
                        verificationAttempts++;
                        continue;
                    }*/
                    /*if (blacklistAllowed)
                    {
                        bool retry = false;
                        if (disabledDirty)
                        {
                            for (i = 0; i < cols + rows; i++)
                            {
                                if ((flags[i] & flagDis) != 0)
                                {
                                    flags[i] &= ~flagDis;
                                    retry = true;
                                }
                            }
                            disabledDirty = false;
                        }
                        if (retry)
                        {
                            blacklistAllowed = false;
                            redecompose = 0;
                            needsRecalc = true;
                            continue;
                        }
                    }*/

                    // rebuild solution so it's stable
                    //ComputePrimalSolution();

                    // optimum, return solution (or could be no feasible solution)
                    if (!feasible) return new double[cols + 1]; // if it's not feasible then return null solution
                    ColdStart = false;
                    //System.Diagnostics.Trace.WriteLine("Primal optimality in round " + round);
                    return ComputeReturnSolutionQuadratic();
                }

                ComputeBasisStep(maxj);

                int mini = -1;
                double minr = ComputeQuadraticLimit(maxj, direction);
                int bound = flagNMid;

                if (!SelectPrimalOutgoing(direction, feasible, eps, ref mini, ref minr, ref bound))
                {
                    feasible = false;
                    lowestInfeasibility = double.PositiveInfinity;
                    redecompose = 0;
                    goto DECOMPOSE;
                }

                /*if (mini == -1)
                {
                    // unbounded
                    if (!ColdStart)
                    {
                        // something went really horribly wrong, try from scratch
                        for (i = 0; i < rows; i++)
                        {
                            B[i] = cols + i;
                            flags[cols + i] = (flags[cols + i] | flagB) & ~flagN & ~flagDis;
                        }
                        for (j = 0; j < cols; j++)
                        {
                            V[j] = j;
                            flags[j] = (flags[j] | flagNLB) & ~flagB & ~flagDis;
                        }
                        redecompose = 0;
                        ColdStart = true;
                        continue;
                    }
                    // completely unstable, investigate
                    double[] ret = new double[cols + 1];
                    return ret;
                }*/

                // determine if we do a basis change or just a bound swap
                int col = V[maxj];
                double dist;
                if (direction > 0.0)
                {
                    dist = ub[col] - mb[col];
                }
                else
                {
                    dist = mb[col] - lb[col];
                }
                if (Math.Abs(minr) >= dist - epsZero)
                {
                    if (direction > 0.0)
                    {
                        flags[col] = (flags[col] & ~flagN) | flagNUB;
                    }
                    else
                    {
                        flags[col] = (flags[col] & ~flagN) | flagNLB;
                    }
                }
                else if (mini == -1)
                {
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                    mb[col] = mb[col] + direction * minr;
                    dist = minr;
                }
                else
                {
                    goto UPDATE;
                }

                if (!feasible)
                {
                    round++; // still increment to avoid infinite cycles
                    continue;
                }

                minr = direction * dist;
                for (i = 0; i < rows; i++)
                {
                    d[i] -= minr * w[i];
                }

                goto MINISTEP;

            UPDATE:

                if (feasible)
                {
                    UpdatePrimal(minr, mini, maxj);
                }

                // swap base

                redecompose--;
                if (redecompose > 0)
                {
                    double pivot;
                    lu.Update(ww, mini, out pivot);
                    if (lu.Singular || Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]))
                    {
                        if (redecompose == maxRedecompose - 1)
                        {
                            redecompose = 0;
                            feasible = false; // forces recalc
                            flags[V[maxj]] |= flagDis;
                            disabledDirty = true;
                            continue;
                        }
                        else
                        {
                            redecompose = 0;
                            feasible = false;
                            continue;
                        }
                    }
                }

                k = B[mini];
                B[mini] = V[maxj];
                V[maxj] = k;
                flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;

                round++;
            } while (round < limit); // limit computation so we don't dead loop, if everything works it shouldn't take more than this
            // when tuning dual feasibility limit to 100 rounds, we don't want to spend too much time on it

            // just in case
            // if feasible return the best we got
            if (feasible) return ComputeReturnSolution();
            if (lastFeasible > 0)
            {
                limit = lastFeasible;
                for (i = 0; i < rows; i++)
                {
                    B[i] = cols + i;
                    flags[cols + i] = (flags[cols + i] | flagB) & ~flagN & ~flagDis;
                }
                for (j = 0; j < cols; j++)
                {
                    V[j] = j;
                    flags[j] = (flags[j] | flagNLB) & ~flagB & ~flagDis;
                }
                goto RESTART;
            }
            return new double[cols + 1];
        }

#if SILVERLIGHT
        private void NullSpaceReduce(double[] dest, double[] source)
#else
        private void NullSpaceReduce(double* dest, double* source)
#endif
        {
            //dest = Z*ZT*source

            // Z = [-Binv*S, I, 0]

            //Zero(dest, cols + rows);

            // ZT*source

            // (sourceS - (Binv*S)T*sourceB)
            // (sourceS - ST*BinvT*sourceB)
            // BinvT*sourceB = x
            // sourceBT*Binv = xT
            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                x[i] = source[col];
            }
            lu.BSolve(x, epsZero);

            for (int s = 0; s < super; s++)
            {
                int col = S[s];
                double v = source[col];

                if (col < cols)
                {
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        v -= sparseValue[i] * x[sparseRow[i]];
                    }
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        v -= pD[k][col] * x[baseRows + k];
                    }
                }
                else
                {
                    v -= x[col - cols];
                }

                dest[col] = v;
            }
        }

#if SILVERLIGHT
        private void NullSpaceExpand(double[] dest)
#else
        private void NullSpaceExpand(double* dest)
#endif
        {
            //dest = Z*ZT*source
            //destB = -Binv*S*destS

            // S*destS
            Zero(x, rows);
            for (int s = 0; s < super; s++)
            {
                int col = S[s];
                double v = dest[col];

                if (col < cols)
                {
                    int sCol1 = sparseCol[col];
                    int sCol2 = sparseCol[col + 1];
                    for (int i = sCol1; i < sCol2; i++)
                    {
                        x[sparseRow[i]] += sparseValue[i] * v;
                    }
                    for (int k = 0; k < numExtraConstraints; k++)
                    {
                        x[baseRows + k] += pD[k][col] * v;
                    }
                }
                else
                {
                    x[col - cols] += v;
                }
            }

            lu.FSolve(x, epsZero);

            for (int i = 0; i < rows; i++)
            {
                int col = B[i];
                dest[col] = -x[i];
            }
        }

        private void UpdateOSB()
        {
            int i = 0;
            for (int j = 0; j < cols; j++)
            {
                int sortj = sort[j];
                if ((flags[sortj] & flagSB) != 0)
                {
                    OSB[i] = sortj;
                    i++;
                }
            }
            osbStructural = i;
            for (int j = cols; j < cols + rows; j++)
            {
                if ((flags[j] & flagSB) != 0)
                {
                    OSB[i] = j;
                    i++;
                }
            }
            osbDirty = false;
        }

        private bool ComputeCGStep(double eps, out double maxAlpha, bool useReducedGradient, out bool needsReducedGradientStep)
        {
            // compute CG step for superbasic variables
            // see ftp://ftp.numerical.rl.ac.uk/pub/reports/ghnRAL98069.ps.gz

            // INITIALIZE
            int hardLimit = super * 5;
            needsReducedGradientStep = false;

        RESTART:

            //x = 0
            Zero(cg_x, cols + rows);

            Zero(cg_p, cols + rows);

            if (osbDirty)
            {
                UpdateOSB();
            }

            //r = g
            for (int i = 0; i < osbStructural; i++)
            {
                int col = OSB[i];
                cg_r[col] = cost[col] - Qk * qx[col];
            }
            for (int i = osbStructural; i < super + rows; i++)
            {
                int col = OSB[i];
                cg_r[col] = cost[col];
            }
            // we only use r up to cols, and we only need the basic/superbasic components

            //w = Z*ZT*r
            if (useReducedGradient)
            {
                for (int i = 0; i < cols; i++)
                {
                    cg_w[V[i]] = c[i];
                }
            }
            else
            {
                NullSpaceReduce(cg_w, cg_r);
            }

            // verify gradient on boundary, if some superbasic is on boundary and has gradient in
            // opposite direction we'll want to remove it from superbasic, so that the truncate tests
            // are meaningful
            for (int i = 0; i < super; i++)
            {
                int col = S[i];

                if (ub[col] - mb[col] < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow)
                {
                    if (ub[col] - (mb[col] + cg_w[col]) < -Math.Abs(ub[col]) * epsPrimalRel - epsPrimalLow)
                    {
                        cg_x[col] = 1.0;
                        NullSpaceExpand(cg_x);
                        maxAlpha = double.PositiveInfinity;
                        return true;
                    }
                }
                if (mb[col] - lb[col] < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow)
                {
                    if ((mb[col] + cg_w[col]) - lb[col] < -Math.Abs(lb[col]) * epsPrimalRel - epsPrimalLow)
                    {
                        cg_x[col] = -1.0;
                        NullSpaceExpand(cg_x);
                        maxAlpha = double.PositiveInfinity;
                        return true;
                    }
                }
            }

            //p = w
            //Copy(cg_p, cg_w, cols + rows);
            for (int i = 0; i < super; i++)
            {
                int col = S[i];
                cg_p[col] = cg_w[col];
            }
            //ValidateCGFeasibility2();

            maxAlpha = 1.0;

            // ITERATE

            double rtw = 0.0;
            /*for (int i = 0; i < cols; i++)
            {
                rtw += cg_r[i] * cg_w[i];
            }*/
            // r*w = rT*Z*ZT*r = (ZT*r)T * (ZT*r)
            // since Z=[-Binv*S, I, 0] we can just check the superbasic components of w
            // this'll be faster and also more accurate because for very small r
            // solving with basis might give bad results because of snapping to zero
            for (int i = 0; i < super; i++)
            {
                int col = S[i];
                rtw += cg_w[col] * cg_w[col];
            }

            double limit = eps * eps;

            if (rtw <= limit)
            {
                return false;
            }

            double minrtw = rtw;
            double startrtw = rtw;
            int minstep = -1;

            int step = 0;

            do
            {
                //alpha = -(rT * w) / (pT * Q * p)
                NullSpaceExpand(cg_p);
                QuadraticQOSB(cg_p, cg_qp);
                double pqp = 0.0;
                for (int i = 0; i < osbStructural; i++)
                {
                    int col = OSB[i];
                    pqp += cg_qp[col] * cg_p[col];
                }
                pqp *= Qk;
                if (pqp < limit)
                {
                    // we found a direction of negative curvature (positive in the maximization context)
                    // or possibly linear descent direction
                    // CG won't work in this case, i.e. it'll try to go opposite of gradient
                    // if we've done some steps before encountering negative curvature just use
                    // that as a good approximation
                    // otherwise return this feasible direction along which we'll have unbounded increase
                    if (pqp < -limit)
                    {
                        // definitely negative curvature
                        // use this as direction for line search until we hit boundary
                        // determine direction based on sign of gradient along this direction
                        // normalize size
                        NormalizeCGStep(cg_p);
                        double grad = ComputeGradient(cg_p);
                        if (grad < -limit)
                        {
                            // negative gradient, we should use the opposite direction
                            for (int i = 0; i < rows + super; i++)
                            {
                                int col = OSB[i];
                                cg_x[col] = -cg_p[col];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < rows + super; i++)
                            {
                                int col = OSB[i];
                                cg_x[col] = cg_p[col];
                            }
                        }
                        // verify that the selected direction has correct direction on boundary
                        // otherwise we're in danger of infinite loop
                        if (TrunkateBoundary(cg_x))
                        {
                            // we have to reverify that this is still direction of negative gradient
                            QuadraticQOSB(cg_x, cg_qp);
                            pqp = 0.0;
                            for (int i = 0; i < osbStructural; i++)
                            {
                                int col = OSB[i];
                                pqp += cg_qp[col] * cg_x[col];
                            }
                            pqp *= Qk;

                            grad = ComputeGradient(cg_x);

                            if (pqp > -limit || grad < -limit)
                            {
                                // we have a negative descent, but we can't make it feasible easily
                                // in this case we have to fall back to reduced gradient step
                                // so the we move into interior away from boundary
                                needsReducedGradientStep = true;
                                return false;
                            }
                        }
                        maxAlpha = double.PositiveInfinity;
                        return true;
                    }
                    else
                    {
                        // we have zero curvature
                        // we're either at optimum or we have unbounded linear descent
                        if (step == 0)
                        {
                            // things depend on the gradient
                            if (rtw > 0.001)
                            {
                                // gradient is significantly steep that we can treat this
                                // as unbounded linear descent
                                // this is in direction of gradient so we know it has to be a feasible direction
                                maxAlpha = double.PositiveInfinity;
                                for (int i = 0; i < rows + super; i++)
                                {
                                    int col = OSB[i];
                                    cg_x[col] = cg_p[col];
                                }
                                return true;
                            }
                            else
                            {
                                // assume we're close to optimal that it won't matter
                                // if there are other optimal directions we'll get better gradient next time
                                needsReducedGradientStep = true;
                                return false;
                            }
                        }
                        else
                        {
                            // we already computed some progress to be made, so use that
                            // if after making that we encounter linear descent we'll
                            // find this in subsequent step
                            return true;
                        }
                    }
                }
                double alpha = rtw / pqp;

                if (alpha > 100.0)
                {
                    // relative steepness is very small
                    // this could result in a very long step
                    // we try not to go too much out of feasible range, which is usually in the order of several hundred per variable
                    double infnorm = 0.0;
                    for (int i = 0; i < rows + super; i++)
                    {
                        int col = OSB[i];
                        double v = Math.Abs(cg_p[col]);
                        if (v > infnorm)
                        {
                            infnorm = v;
                        }
                    }

                    // if we're too large cancel based on our prediction if we made any progress
                    if (alpha * infnorm > 1000.0)
                    {
                        if (TrunkateBoundary(cg_p))
                        {
                            QuadraticQOSB(cg_p, cg_qp);
                            pqp = 0.0;
                            for (int i = 0; i < osbStructural; i++)
                            {
                                int col = OSB[i];
                                pqp += cg_qp[col] * cg_p[col];
                            }
                            pqp *= Qk;
                        }

                        // check if it's safe to treat this as linear descent
                        double grad = ComputeGradient(cg_p);
                        double t = 100 / infnorm;
                        if (Math.Abs(grad) * t - 0.5 * t * t * pqp > 0)
                        {
                            if (grad < 0)
                            {
                                // negative gradient, we should use the opposite direction
                                for (int i = 0; i < rows + super; i++)
                                {
                                    int col = OSB[i];
                                    cg_x[col] = -cg_p[col];
                                }
                            }
                            else
                            {
                                for (int i = 0; i < rows + super; i++)
                                {
                                    int col = OSB[i];
                                    cg_x[col] = cg_p[col];
                                }
                            }
                            maxAlpha = double.PositiveInfinity;
                            return true;
                        }

                        if (rtw > startrtw)
                        {
                            needsReducedGradientStep = true;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                //ValidateCGFeasibility();
                //ValidateCGFeasibility2();

                //x = x + alpha * p
                for (int i = 0; i < rows + super; i++)
                {
                    int col = OSB[i];
                    cg_x[col] += alpha * cg_p[col];
                }

                //ValidateCGFeasibility();

                //r~ = r + alpha * Q * p
                for (int i = 0; i < osbStructural; i++)
                {
                    int col = OSB[i];
                    cg_rr[col] = cg_r[col] - alpha * Qk * cg_qp[col];
                }

                //w~ = Z*ZT*r~
                NullSpaceReduce(cg_ww, cg_rr);

                //beta = (r~T * w~) / (rT * w)
                double rtwnew = 0.0;
                /*for (int i = 0; i < cols; i++)
                {
                    rtwnew += cg_rr[i] * cg_ww[i];
                }*/
                for (int i = 0; i < super; i++)
                {
                    int col = S[i];
                    rtwnew += cg_ww[col] * cg_ww[col];
                }
                step++;
                if (rtwnew <= limit)
                {
                    return true;
                }
                if (step > hardLimit || (step >= super && rtwnew < 2 * minrtw))
                {
                    if (rtwnew > startrtw)
                    {
                        if (minrtw < 0.1 * startrtw)
                        {
                            // we have a possibility of improving the value if we terminate early
                            hardLimit = minstep - 1;
                            goto RESTART;
                        }
                        needsReducedGradientStep = true;
                        return false;
                    }
                    if (step > super)
                    {
                        if (rtwnew < eps)
                        {
                            return true;
                        }
                        // often when we come here we're dealing with numerical instabilities
                        // it can happen that some of superbasics that start on boundary will have
                        // their value in opposite direction of boundary despite gradient being in
                        // the right direction
                        // if this happens it's better to project that direction out since
                        // otherwise we can get in an endless loop of adding/removing that variable
                        // from superbasis
                        TrunkateBoundary(cg_x);

                        // verify if generated direction has good gradient and curvature
                        QuadraticQOSB(cg_x, cg_qp);
                        pqp = 0.0;
                        for (int i = 0; i < osbStructural; i++)
                        {
                            int col = OSB[i];
                            pqp += cg_qp[col] * cg_x[col];
                        }
                        pqp *= Qk;
                        double grad = 0.0;
                        for (int i = 0; i < osbStructural; i++)
                        {
                            int col = OSB[i];
                            grad += (cost[col] - Qk * qx[col]) * cg_x[col];
                        }
                        if (pqp < -limit)
                        {
                            // negative curvature
                            if (grad < -limit)
                            {
                                // negative gradient, we should use the opposite direction
                                for (int i = 0; i < rows + super; i++)
                                {
                                    int col = OSB[i];
                                    cg_x[col] = -cg_x[col];
                                }
                            }
                            maxAlpha = double.PositiveInfinity;
                            return true;
                        }
                        else
                        {
                            // zero or positive curvature
                            // make sure gradient is ok
                            if (grad < -limit)
                            {
                                needsReducedGradientStep = true;
                                return false;
                            }
                            // make sure alpha = 1 is ok
                            // df = t * grad - 0.5 * t^2 * pqp
                            if (grad - 0.5 * pqp < -limit)
                            {
                                needsReducedGradientStep = true;
                                return false;
                            }
                            return true;
                        }
                    }
                    return true;
                }
                if (rtwnew < minrtw)
                {
                    minstep = step;
                    minrtw = rtwnew;
                }
                double beta = rtwnew / rtw;

                //p = w~ + beta*p
                /*for (int i = 0; i < cols + rows; i++)
                {
                    cg_p[i] = cg_ww[i] + beta * cg_p[i];
                }*/
                for (int i = 0; i < super; i++)
                {
                    int col = S[i];
                    cg_p[col] = cg_ww[col] + beta * cg_p[col];
                }
                //ValidateCGFeasibility2();

                //w = w~
                //r = r~
                rtw = rtwnew;
#if SILVERLIGHT
                double[] tmp = cg_w;
                cg_w = cg_ww;
                cg_ww = tmp;
                tmp = cg_r;
                cg_r = cg_rr;
                cg_rr = tmp;
#else
                double* tmp = cg_w;
                cg_w = cg_ww;
                cg_ww = tmp;
                tmp = cg_r;
                cg_r = cg_rr;
                cg_rr = tmp;
#endif
            } while (true);
        }

#if SILVERLIGHT
        private double ComputeGradient(double[] step)
#else
        private double ComputeGradient(double* step)
#endif
        {
            double grad = 0.0;
            for (int i = 0; i < osbStructural; i++)
            {
                int col = OSB[i];
                grad += (cost[col] - Qk * qx[col]) * step[col];
            }
            return grad;
        }

#if SILVERLIGHT
        private void NormalizeCGStep(double[] step)
#else
        private void NormalizeCGStep(double* step)
#endif
        {
            double infnorm = 0.0;
            for (int i = 0; i < rows + super; i++)
            {
                int col = OSB[i];
                double v = Math.Abs(step[col]);
                if (v > infnorm)
                {
                    infnorm = v;
                }
            }
            if (infnorm > 1000.0)
            {
                for (int i = 0; i < rows + super; i++)
                {
                    int col = OSB[i];
                    step[col] /= infnorm;
                }
            }
        }

#if SILVERLIGHT
        private bool TrunkateBoundary(double[] step)
#else
        private bool TrunkateBoundary(double* step)
#endif
        {
            bool dirty = false;
            for (int i = 0; i < super; i++)
            {
                int col = S[i];

                if (ub[col] - mb[col] < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow)
                {
                    if (ub[col] - (mb[col] + step[col]) < -Math.Abs(ub[col]) * epsPrimalRel - epsPrimalLow)
                    {
                        step[col] = 0;
                        dirty = true;
                    }
                }
                if (mb[col] - lb[col] < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow)
                {
                    if ((mb[col] + step[col]) - lb[col] < -Math.Abs(lb[col]) * epsPrimalRel - epsPrimalLow)
                    {
                        step[col] = 0;
                        dirty = true;
                    }
                }
            }
            if (dirty)
            {
                NullSpaceExpand(step);
                return true;
            }
            return false;
        }


        private bool CopositivityDetection(double eps)
        {
            // verify second order optimality conditions
            // based on http://iwi.eldoc.ub.rug.nl/FILES/root/2008/LinAlgApplBundfuss/2008LinAlgApplBundfuss.pdf

            Zero(cg_x, cols + rows);
            Zero(cg_p, cols + rows);

            // add all zero reduced gradient nonbasics into superbasis
            // they'll form the basis of simplex

            ComputeQx();

            for (int j = 0; j < cols; j++)
            {
                int col = V[j];
                if (Math.Abs(c[j]) < eps && (flags[col] & flagNMid) == 0)
                {
                    S[super] = col;
                    super++;
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                }
            }

            int maxv = super * 2;
            int maxs = 10;
            double[] vector = new double[maxv * super]; // contains superbasic components of simplex vectors vi
            double[] cross = new double[maxv * maxv]; // contains products viT * Q * vj
            int[] simplex = new int[maxs * super]; // contains simplexes with oriented vector indexes

            UpdateOSB();

            // initialize root vertexes
            int vCount = 1;
            for (int i = 0; i < super; i++)
            {
                vector[vCount * super + i] = 1.0;
                vCount++;
            }

            // flag products as not computed
            for (int i = 1; i <= super; i++)
            {
                for (int j = i; j <= super; j++)
                {
                    cross[(j - 1) * j / 2 + i] = double.NaN;
                }
            }

            // initialize root simplexes
            int simCount = 0;
            int combinations = (1 << super);
            for (int i = 0; i < combinations; i++)
            {
                bool valid = true;
                for (int j = 0; j < super; j++)
                {
                    int col = S[j];
                    if ((i & (1 << j)) != 0)
                    {
                        if (mb[col] - lb[col] < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow)
                        {
                            valid = false;
                            break;
                        }
                    }
                    else
                    {
                        if (ub[col] - mb[col] < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow)
                        {
                            valid = false;
                            break;
                        }
                    }
                }
                if (valid)
                {
                    if (simCount + 1 > maxs)
                    {
                        maxs += 10;
                        Array.Resize(ref simplex, maxs * super);
                    }
                    for (int j = 0; j < super; j++)
                    {
                        if ((i & (1 << j)) != 0)
                        {
                            simplex[simCount * super + j] = -(j + 1);
                        }
                        else
                        {
                            simplex[simCount * super + j] = (j + 1);
                        }
                    }
                    simCount++;
                }
            }

            while (simCount > 0)
            {
                int off = (simCount - 1) * super;

                const double limit = -1.0;
                double min = limit; // a rough detection, but we're not multiplying with Qk
                int mini = 0;
                int minj = 0;

                int vi;
                int vj;


                // find minimum product
                for (int i = 0; i < super; i++)
                {
                    for (int j = i; j < super; j++)
                    {
                        vi = simplex[off + i];
                        vj = simplex[off + j];

                        double sign = (vi * vj < 0) ? -1.0 : 1.0;

                        if (vi < 0) vi = -vi;
                        if (vj < 0) vj = -vj;
                        int index;
                        if (vi < vj)
                        {
                            index = (vj - 1) * vj / 2 + vi;
                        }
                        else
                        {
                            index = (vi - 1) * vi / 2 + vj;
                        }

                        if (double.IsNaN(cross[index]))
                        {
                            for (int s = 0; s < super; s++)
                            {
                                int col = S[s];
                                cg_p[col] = vector[vi * super + s];
                            }
                            NullSpaceExpand(cg_p);
                            QuadraticQOSB(cg_p, cg_qp);
                            for (int s = 0; s < super; s++)
                            {
                                int col = S[s];
                                cg_p[col] = vector[vj * super + s];
                            }
                            NullSpaceExpand(cg_p);
                            double pqp = 0.0;
                            for (int k = 0; k < osbStructural; k++)
                            {
                                int col = OSB[k];
                                pqp += cg_qp[col] * cg_p[col];
                            }
                            cross[index] = pqp;
                        }

                        double v = sign * cross[index];

                        if (i == j && v < -eps)
                        {
                            for (int s = 0; s < super; s++)
                            {
                                int col = S[s];
                                cg_x[col] = vector[vi * super + s];
                            }
                            NullSpaceExpand(cg_x);
                            return true;
                        }

                        if (v < min)
                        {
                            min = v;
                            mini = i;
                            minj = j;
                        }
                    }
                }

                if (min == limit)
                {
                    // remove simplex
                    simCount--;
                    continue;
                }

                // select splitting point
                vi = simplex[off + mini];
                vj = simplex[off + minj];
                double signi = (vi < 0) ? -1.0 : 1.0;
                double signj = (vj < 0) ? -1.0 : 1.0;
                if (vi < 0) vi = -vi;
                if (vj < 0) vj = -vj;
                double alpha = cross[(vi - 1) * vi / 2 + vi];
                double beta = cross[(vj - 1) * vj / 2 + vj];
                double gamma = min;

                double k1 = gamma / (gamma - alpha);
                double k2 = beta / (beta - gamma);

                if (k1 > k2 + epsZero)
                {
                    if (alpha < eps && beta < eps)
                    {
                        for (int s = 0; s < super; s++)
                        {
                            int col = S[s];
                            cg_x[col] = (signi * vector[vi * super + s] + signj * vector[vj * super + s]) / 2;
                        }
                        NullSpaceExpand(cg_x);
                        return true;
                    }
                    else if (alpha >= eps)
                    {
                        for (int s = 0; s < super; s++)
                        {
                            int col = S[s];
                            cg_x[col] = signj * vector[vj * super + s] - gamma / alpha * signi * vector[vi * super + s];
                        }
                        NullSpaceExpand(cg_x);
                        return true;
                    }
                    else
                    {
                        for (int s = 0; s < super; s++)
                        {
                            int col = S[s];
                            cg_x[col] = signi * vector[vi * super + s] - gamma / beta * signj * vector[vj * super + s];
                        }
                        NullSpaceExpand(cg_x);
                        return true;
                    }
                }

                double lambda;

                /*if (k2 - k1 < 0.01)
                {
                    // we're in dange of splitting a very small edge infinitely
                    // instead split the longest edge in half
                    int maxi = 0;
                    int maxj = 0;
                    double max = 0;
                    for (int i = 0; i < super; i++)
                    {
                        for (int j = i + 1; j < super; j++)
                        {
                            vi = simplex[off + i];
                            vj = simplex[off + j];
                            signi = (vi < 0) ? -1.0 : 1.0;
                            signj = (vj < 0) ? -1.0 : 1.0;
                            if (vi < 0) vi = -vi;
                            if (vj < 0) vj = -vj;
                            for (int s = 0; s < super; s++)
                            {
                                double diff = Math.Abs(signi * vector[vi * super + s] - signj * vector[vj * super + s]);
                                if (diff > max)
                                {
                                    max = diff;
                                    maxi = i;
                                    maxj = j;
                                }
                            }
                        }
                    }

                    mini = maxi;
                    minj = maxj;
                    vi = simplex[off + maxi];
                    vj = simplex[off + maxj];
                    signi = (vi < 0) ? -1.0 : 1.0;
                    signj = (vj < 0) ? -1.0 : 1.0;
                    if (vi < 0) vi = -vi;
                    if (vj < 0) vj = -vj;

                    lambda = 0.5;
                }
                else*/
                {
                    double k3 = (beta - gamma) / (alpha - 2 * gamma + beta);

                    lambda = Math.Max(k1, Math.Min(k3, k2));
                }

                if (vCount == maxv)
                {
                    maxv *= 2;
                    Array.Resize(ref vector, maxv * super);
                    Array.Resize(ref cross, maxv * maxv);
                }
                // add the splitting point
                for (int s = 0; s < super; s++)
                {
                    vector[vCount * super + s] = lambda * signi * vector[vi * super + s] + (1.0 - lambda) * signj * vector[vj * super + s];
                }
                // flag products as not computed
                for (int i = 1; i <= vCount; i++)
                {
                    cross[(vCount - 1) * vCount / 2 + i] = double.NaN;
                }
                vCount++;

                // remove old simplex
                simCount--;

                if (simCount + 2 > maxs)
                {
                    maxs += 10;
                    Array.Resize(ref simplex, maxs * super);
                }
                // add new simplex
                for (int s = 0; s < super; s++)
                {
                    simplex[(simCount + 1) * super + s] = simplex[simCount * super + s];
                }
                simplex[simCount * super + mini] = vCount - 1;
                simplex[(simCount + 1) * super + minj] = vCount - 1;
                simCount += 2;
            }

            return false;
        }

        private bool CopositivityDetectionLight(double eps)
        {
            // verify second order optimality conditions
            // based on http://iwi.eldoc.ub.rug.nl/FILES/root/2008/LinAlgApplBundfuss/2008LinAlgApplBundfuss.pdf

            Zero(cg_x, cols + rows);
            Zero(cg_p, cols + rows);

            // add all zero reduced gradient nonbasics into superbasis
            // they'll form the basis of simplex

            ComputeQx();

            int col;

            for (int j = 0; j < cols; j++)
            {
                col = V[j];
                if (Math.Abs(c[j]) < eps && (flags[col] & flagNMid) == 0)
                {
                    S[super] = col;
                    super++;
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                }
            }

            UpdateOSB();

            double[] vqv = new double[super];
            bool[] up = new bool[super];
            bool[] down = new bool[super];

            for (int i = 0; i < super; i++)
            {
                for (int s = 0; s < super; s++)
                {
                    col = S[s];
                    cg_x[col] = (i == s) ? 1.0 : 0.0;
                }
                NullSpaceExpand(cg_x);
                up[i] = true;
                down[i] = true;
                for (int j = 0; j < super + rows; j++)
                {                    
                    col = OSB[j];
                    if (Math.Abs(cg_x[col]) > epsZero)
                    {
                        if (ub[col] - mb[col] < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow)
                        {
                            if (cg_x[col] > 0)
                            {
                                up[i] = false;
                            }
                            else
                            {
                                down[i] = false;
                            }
                        }
                        if (mb[col] - lb[col] < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow)
                        {
                            if (cg_x[col] < 0)
                            {
                                up[i] = false;
                            }
                            else
                            {
                                down[i] = false;
                            }
                        }
                    }
                }
                QuadraticQOSB(cg_x, cg_qp);
                double pqp = 0.0;
                for (int k = 0; k < osbStructural; k++)
                {
                    col = OSB[k];
                    pqp += cg_qp[col] * cg_x[col];
                }
                vqv[i] = pqp;

                if (pqp < -eps)
                {
                    if (up[i])
                    {
                        return true;
                    }
                    else if (down[i])
                    {
                        for (int k = 0; i < rows + super; i++)
                        {
                            col = OSB[k];
                            cg_x[col] = -cg_x[col];
                        }
                    }
                }
            }

            // find minimum product
            for (int i = 0; i < super; i++)
            {
                for (int s = 0; s < super; s++)
                {
                    col = S[s];
                    cg_p[col] = (i == s) ? 1.0 : 0.0;
                }
                NullSpaceExpand(cg_p);
                QuadraticQOSB(cg_p, cg_qp);
                for (int j = i + 1; j < super; j++)
                {
                    for (int s = 0; s < super; s++)
                    {
                        col = S[s];
                        cg_p[col] = (j == s) ? 1.0 : 0.0;
                    }
                    NullSpaceExpand(cg_p);
                    double pqp = 0.0;
                    for (int k = 0; k < osbStructural; k++)
                    {
                        col = OSB[k];
                        pqp += cg_qp[col] * cg_p[col];
                    }

                    double signi = 1.0;
                    double signj = 1.0;

                    if (!up[i])
                    {
                        signi = -1.0;
                        if (!down[i])
                        {
                            continue;
                        }
                    }

                    if (!up[j])
                    {
                        signj = -1.0;
                        if (!down[j])
                        {
                            continue;
                        }
                    }

                    if (Math.Abs(pqp) > eps)
                    {
                        if (pqp > 0)
                        {
                            if (signi == 1.0 && down[i])
                            {
                                signi = -1.0;
                                pqp = -pqp;
                            }

                            if (pqp > 0)
                            {
                                if (signj == 1.0 && down[j])
                                {
                                    signj = -1.0;
                                    pqp = -pqp;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        double alpha = vqv[i];
                        double beta = vqv[j];
                        double gamma = pqp;

                        double k1 = gamma / (gamma - alpha);
                        double k2 = beta / (beta - gamma);

                        if (k1 > k2 + epsZero)
                        {
                            if (alpha < eps && beta < eps)
                            {
                                for (int s = 0; s < super; s++)
                                {
                                    col = S[s];
                                    cg_x[col] = (((i == s) ? signi : 0.0) + ((j == s) ? signj : 0.0)) / 2;
                                }
                                NullSpaceExpand(cg_x);
                                return true;
                            }
                            else if (alpha >= eps)
                            {
                                for (int s = 0; s < super; s++)
                                {
                                    col = S[s];
                                    cg_x[col] = ((j == s) ? signj : 0.0) - gamma / alpha * ((i == s) ? signi : 0.0);
                                }
                                NullSpaceExpand(cg_x);
                                return true;
                            }
                            else
                            {
                                for (int s = 0; s < super; s++)
                                {
                                    col = S[s];
                                    cg_x[col] = ((i == s) ? signi : 0.0) - gamma / beta * ((j == s) ? signj : 0.0);
                                }
                                NullSpaceExpand(cg_x);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private double[] SolvePrimalQuadraticCGUnsafe()
        {
            int limit = 5000;

            //RESTART:
            int i, k;
            int round = 0;
            int updateCount = 0;
            int redecompose = 0;
            int maxj, mini;
            const int maxRedecompose = 50;
            bool changeBasis;
            double direction;
            bool checkTermination;
            bool needsReducedGradientStep = false;
            osbDirty = true;

            //int verificationAttempts = 0;

            bool feasible = false;
            double lowestInfeasibility = double.PositiveInfinity;

            double eps = epsPrimal;

            if (disabledDirty)
            {
                for (i = 0; i < cols + rows; i++)
                {
                    flags[i] &= ~flagDis;
                }
                disabledDirty = false;
            }

            // in MIP a superbasic may become fixed, if this happens remove it from superbasis
            for (i = 0; i < super; i++)
            {
                int col = S[i];
                if ((flags[col] & flagFix) != 0)
                {
                    flags[col] = (flags[col] & ~flagN) | flagNLB;
                    S[i] = S[super - 1];
                    super--;
                    i--;
                }
            }

            do
            {
            DECOMPOSE:

                if (redecompose <= 0)
                {
                    Decompose();
                    redecompose = maxRedecompose; // decompose every 50 iterations to maintain stability
                    feasible = false; // when refactoring basis recompute the solution
                }
                if (lu.Singular)
                {
                    redecompose = 0;
                    PatchSingularBasis();
                    //ValidateSuperBasis();
                    continue;
                }

                if (!feasible)
                {
                    bool recalculate;
                    do
                    {
                        ComputePrimalSolution(false);
                        UpdateMB();
                        feasible = IsPrimalQFeasible(eps, out recalculate);
                    } while (recalculate);
                    updateCount = 0;
                }

                int bound = flagNMid;

                if (!feasible)
                {
                    round++;
                    if (round > limit)
                    {
                        break;
                    }

                    double infeasibility;
                    ComputePhaseIQReducedCosts(out infeasibility, eps);
                    if (infeasibility < lowestInfeasibility) lowestInfeasibility = infeasibility;
                    else if (infeasibility > lowestInfeasibility + eps)
                    {
                        // we're not using a shifting strategy for primal so the only way to combat
                        // numerical cycling is to lower the tolerances
                        if (eps < 1.0)
                        {
                            eps *= 10.0;
                            lowestInfeasibility = double.PositiveInfinity;
                        }
                        if (redecompose < maxRedecompose)
                        {
                            redecompose = 0;
                            goto DECOMPOSE;
                        }
                    }
                    // for establishing feasibility we don't need quadratic part, just do a simplex step
                    PhaseIQStep(out maxj, eps, out mini, out bound, out changeBasis);
                    updateCount++;

                    if (maxj == -1)
                    {
                        // if it's not feasible then return null solution
                        if (!feasible) return new double[cols + 1];
                    }

                    if (changeBasis)
                    {
                        if ((flags[V[maxj]] & flagNMid) != 0)
                        {
                            int s = Array.IndexOf(_S, V[maxj]);
                            S[s] = S[super - 1];
                            super--;
                        }

                        goto UPDATE;
                    }
                    else
                    {
                        continue;
                    }
                }

            MINISTEP:
                if (updateCount > 50)
                {
                    feasible = false;
                    continue;
                }

                round++;
                if (round > limit)
                {
                    break;
                }

                int col = -1;
                double alpha = 1.0;

                if (super > 0)
                {
                    ComputeQx();
                    if (ComputeCGStep(eps, out alpha, false, out needsReducedGradientStep))
                    {

                        if (!SelectCGOutgoing(eps, ref col, ref alpha, ref bound))
                        {
                            // we lost feasibility, go to phase I
                            feasible = false;
                            lowestInfeasibility = double.PositiveInfinity;
                            redecompose = 0;
                            goto DECOMPOSE;
                        }

                        if (col == -1)
                        {
                            for (i = 0; i < cols + rows; i++)
                            {
                                mb[i] += cg_x[i];
                            }
                            updateCount++;
                            //ValidateFeasibility();
                            //ValidateFlags();
                        }
                        /*else if ((flags[col] & flagNMid) == 0)
                        {
                            System.Diagnostics.Trace.Assert(Array.IndexOf(_B, col) != -1, "Bad outgoing selection!");
                        }*/
                    }
                }

                if (col == -1)
                {
                    // no bound hit, alpha = 1 is valid step
                    // determine if we can free some variable

                    ComputeReducedCostGradient();

                    maxj = SelectPrimalIncoming(out direction, false);

                    if (maxj == -1)
                    {
                        // we might have disabled some variables, unflag them and recheck
                        bool retry = false;
                        if (disabledDirty)
                        {
                            for (i = 0; i < cols + rows; i++)
                            {
                                if ((flags[i] & flagDis) != 0)
                                {
                                    flags[i] &= ~flagDis;
                                    retry = true;
                                }
                            }
                            disabledDirty = false;
                        }
                        if (retry)
                        {
                            maxj = SelectPrimalIncoming(out direction, false);
                        }

                        if (maxj == -1)
                        {
                            goto TERMINATION;
                        }
                    }

                    // freeing V[maxj] will result in improved solution

                    if (super == 0 || needsReducedGradientStep || (flags[V[maxj]] & flagNMid) != 0)
                    {
                        // special case the one free variable case
                        // in this situation the CG step is overkill
                        // we can use straight reduced gradient approach

                        // we also use this for superbasics when we're so close to optimal that
                        // CG decides it's not safe numerically to continue
                        // in that case the quadratic limit will most likely be very small
                        // but since we're working with unit step in reduced space the solving with
                        // basis shouldn't give too many problems due to zero snapping

                        updateCount++;
                        if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis, out checkTermination))
                        {
                            // we lost feasibility, go to phase I
                            feasible = false;
                            lowestInfeasibility = double.PositiveInfinity;
                            redecompose = 0;
                            goto DECOMPOSE;
                        }

                        if (checkTermination)
                        {
                            ComputeReducedCostGradient();
                            maxj = CheckTermination(out direction);
                            if (maxj == -1)
                            {
                                goto TERMINATION;
                            }
                            updateCount++;
                            if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis, out checkTermination))
                            {
                                // we lost feasibility, go to phase I
                                feasible = false;
                                lowestInfeasibility = double.PositiveInfinity;
                                redecompose = 0;
                                goto DECOMPOSE;
                            }
                        }

                        if (changeBasis)
                        {
                            if ((flags[V[maxj]] & flagNMid) != 0)
                            {
                                int s = Array.IndexOf(_S, V[maxj]);
                                S[s] = S[super - 1];
                                super--;
                            }

                            goto UPDATE;
                        }

                        if (super == 1)
                        {
                            // we already updated superbasics to optimal
                            // so we know CG will return zero step
                            // just skip ahead and add another free variable

                            ComputeReducedCostGradient();

                            maxj = SelectPrimalIncoming(out direction, false);

                            if (maxj == -1)
                            {
                                goto TERMINATION;
                            }

                            col = V[maxj];
                            if ((flags[col] & flagNMid) == 0)
                            {
                                S[super] = col;
                                super++;
                                flags[col] = (flags[col] & ~flagN) | flagNMid;
                                osbDirty = true;
                                //ValidateSuperBasis();
                            }
                        }

                        goto MINISTEP;
                    }

                    col = V[maxj];
                    S[super] = col;
                    super++;
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                    osbDirty = true;
                    //ValidateSuperBasis();

                    goto MINISTEP;
                }

            //CGSTEP:
                // we hit a bound
                // if it's superbasic just fix it
                // otherwise we have to do a basis update

                // move for alpha
                if (alpha > epsZero)
                {
                    for (i = 0; i < cols + rows; i++)
                    {
                        mb[i] += alpha * cg_x[i];
                    }
                    updateCount++;
                }
                //ValidateFeasibility();
                /*ValidateFlags();
                if ((bound & flagNUB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - ub[col]) < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
                if ((bound & flagNLB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - lb[col]) < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }*/


                if ((flags[col] & flagNMid) != 0)
                {
                    // fix superbasic
                    flags[col] = (flags[col] & ~flagN) | bound;
                    i = Array.IndexOf(_S, col);
                    S[i] = S[super - 1];
                    super--;
                    osbDirty = true;
                    //ValidateSuperBasis();

                    goto MINISTEP;
                }

                //System.Diagnostics.Trace.Assert(Array.IndexOf(_B, col) != -1, "Bad outgoing selection!");

                // select superbasic to enter basis
                // avoid selecting those flagged as disabled because they'll likely cause
                // basis singularity again

                // try to replace col with a combination of superbasics
                // perform this by repeated updates
                mini = Array.IndexOf(_B, col);
                int inc = -1;
                bool numericCheck = true;
                i = super - 1;
            //SUPERBASICUPDATE:
                for (; i >= 0; i--)
                {
                    inc = S[i];
                    // skip if we disabled for numerical stability
                    if (numericCheck && (flags[inc] & flagDis) != 0)
                    {
                        continue;
                    }
                    ComputeBasisReplace(inc);
                    // numerical stability verification only makes sense while we're still nonsinguler
                    // once we get singular further solver with U are undefined
                    if (numericCheck)
                    {
                        lu.FSolveU(ww, w, 0.00000001);
                    }

                    double pivot;
                    if (lu.Update(ww, mini, out pivot))
                    {
                        redecompose--;
                        if (numericCheck && Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                        {
                            // numerical errors accumulated are high
                            if (redecompose == maxRedecompose - 1)
                            {
                                redecompose = 0;
                                feasible = false; // forces recalc
                                flags[inc] |= flagDis;
                                disabledDirty = true;
                                goto DECOMPOSE;
                            }
                            else
                            {
                                redecompose = 0;
                                feasible = false;
                                goto DECOMPOSE;
                            }
                        }
                        if (lu.Singular)
                        {
                            // we need more than one superbasic to complete the replace
                            numericCheck = false;
                            //goto SUPERBASICUPDATE;
                            continue;
                        }
                        else
                        {
                            // replace successful, final column is inc
                            S[i] = S[super - 1];
                            super--;

                            maxj = Array.IndexOf(_V, inc);

                            k = B[mini];
                            B[mini] = V[maxj];
                            V[maxj] = k;
                            flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                            flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;
                            osbDirty = true;
                            //ValidateSuperBasis();
                            //ValidateFlags();

                            goto DECOMPOSE;
                        }
                    }
                }
                if (lu.Singular)
                {
                    Decompose();
                    redecompose = maxRedecompose;
                }
                if (lu.Singular)
                {
                    feasible = false;
                    redecompose = 0;
                    PatchSingularBasis();
                    //ValidateSuperBasis();
                    continue;
                }

                /*int inc = S[super - 1];
                if ((flags[inc] & flagDis) == 0)
                {
                    super--;
                }
                else
                {
                    bool found = false;
                    // check if there is some other superbasic that is not flagged disabled
                    for (i = super - 2; i >= 0; i--)
                    {
                        inc = S[i];
                        if ((flags[inc] & flagDis) == 0)
                        {
                            found = true;
                            S[i] = S[super - 1];
                            super--;
                            break;
                        }
                    }
                    if (!found)
                    {*/
                // all superbasics were flagged, so we can't actually do the swap
                // try to do a reduced gradient and see if it makes sense to free
                // another variable so that we can use it to enter basis

                // we're not actually at optimality point for superbasics
                // so whatever we get here will be good for reduced gradient only
                // CG step might want us to go in the opposite direction actually
                // but we can't allow that because we can't fix basis in that direction
                // so pretend superbasics are fixed and do a single direction change
                ComputeReducedCostGradient();

                maxj = SelectPrimalIncomingBoundary(out direction, false);

                if (maxj == -1)
                {
                    // we might have disabled some variables, unflag them and recheck
                    bool retry = false;
                    if (disabledDirty)
                    {
                        for (i = 0; i < cols + rows; i++)
                        {
                            if ((flags[i] & flagDis) != 0)
                            {
                                flags[i] &= ~flagDis;
                                retry = true;
                            }
                        }
                        disabledDirty = false;
                    }
                    else
                    {
                        retry = true;
                    }
                    if (retry)
                    {
                        maxj = SelectPrimalIncoming(out direction, false);
                    }

                    if (maxj == -1)
                    {
                        goto TERMINATION;
                    }
                }

                updateCount++;
                if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis, out checkTermination))
                {
                    // we lost feasibility, go to phase I
                    feasible = false;
                    lowestInfeasibility = double.PositiveInfinity;
                    redecompose = 0;
                    goto DECOMPOSE;
                }

                if (checkTermination)
                {
                    ComputeReducedCostGradient();
                    maxj = CheckTermination(out direction);
                    if (maxj == -1)
                    {
                        goto TERMINATION;
                    }
                    updateCount++;
                    if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis, out checkTermination))
                    {
                        // we lost feasibility, go to phase I
                        feasible = false;
                        lowestInfeasibility = double.PositiveInfinity;
                        redecompose = 0;
                        goto DECOMPOSE;
                    }
                }

                if (changeBasis)
                {
                    if ((flags[V[maxj]] & flagNMid) != 0)
                    {
                        int s = Array.IndexOf(_S, V[maxj]);
                        S[s] = S[super - 1];
                        super--;
                    }

                    goto UPDATE;
                }

                //ValidateSuperBasis();
                goto MINISTEP;
            /*}
        }
        ValidateSuperBasis();

        // swap base
        maxj = Array.IndexOf(_V, inc);

        ComputeBasisStep(maxj);*/

            UPDATE:

                redecompose--;
                if (redecompose > 0)
                {
                    double pivot;
                    lu.Update(ww, mini, out pivot);
                    if (lu.Singular || Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                    {
                        // we aren't making the swap
                        // make sure to properly add superbasic back to S if we cancelled swap
                        col = V[maxj];
                        if ((flags[col] & flagNMid) != 0)
                        {
                            S[super] = col;
                            super++;
                            //ValidateSuperBasis();
                        }
                        if (redecompose == maxRedecompose - 1)
                        {
                            redecompose = 0;
                            feasible = false; // forces recalc
                            flags[col] |= flagDis;
                            disabledDirty = true;
                            continue;
                        }
                        else
                        {
                            redecompose = 0;
                            feasible = false;
                            continue;
                        }
                    }
                }

                k = B[mini];
                B[mini] = V[maxj];
                V[maxj] = k;
                flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;
                //ValidateSuperBasis();
                //ValidateFlags();
                goto DECOMPOSE;

            TERMINATION:

                /*if (feasible && (redecompose < maxRedecompose || updateCount > 0) && verificationAttempts < 5)
                {
                    redecompose = 0;
                    feasible = false;
                    verificationAttempts++;
                    continue;
                }*/

                /*if (CopositivityDetectionLight(eps))
                {
                    alpha = double.PositiveInfinity;
                    if (!SelectCGOutgoing(eps, ref col, ref alpha, ref bound))
                    {
                        // we lost feasibility, go to phase I
                        feasible = false;
                        lowestInfeasibility = double.PositiveInfinity;
                        redecompose = 0;
                        goto DECOMPOSE;
                    }

                    if (col != -1)
                    {
                        // otherwise we're unbounded which should not happen
                        goto CGSTEP;
                    }
                }*/

                return ComputeReturnSolutionQuadratic();

            } while (round < limit); // limit computation so we don't dead loop, if everything works it shouldn't take more than this
            // when tuning dual feasibility limit to 100 rounds, we don't want to spend too much time on it

            // just in case
            // if feasible return the best we got
            if (feasible) return ComputeReturnSolutionQuadratic();
            return new double[cols + 1];
        }

        private double[] SolvePrimalQuadraticCGReciprocalUnsafe(int maximizeColumn, double targetValue)
        {
            int limit = 5000;

            //RESTART:
            int i, k;
            int round = 0;
            int updateCount = 0;
            int redecompose = 0;
            int maxj, mini;
            const int maxRedecompose = 50;
            bool changeBasis;
            double direction;
            osbDirty = true;
            bool needsReducedGradientStep = false;

            //int verificationAttempts = 0;

            bool feasible = false;
            double lowestInfeasibility = double.PositiveInfinity;

            double eps = epsPrimal;

            if (disabledDirty)
            {
                for (i = 0; i < cols + rows; i++)
                {
                    flags[i] &= ~flagDis;
                }
                disabledDirty = false;
            }

            do
            {
            DECOMPOSE:

                if (redecompose <= 0)
                {
                    Decompose();
                    redecompose = maxRedecompose; // decompose every 50 iterations to maintain stability
                    feasible = false; // when refactoring basis recompute the solution
                }
                if (lu.Singular)
                {
                    redecompose = 0;
                    PatchSingularBasis();
                    //ValidateSuperBasis();
                    continue;
                }

                if (!feasible)
                {
                    bool recalculate;
                    do
                    {
                        ComputePrimalSolution(false);
                        UpdateMB();
                        feasible = IsPrimalQFeasible(eps, out recalculate);
                    } while (recalculate);
                    updateCount = 0;
                }

                int bound = flagNMid;

                if (!feasible)
                {
                    round++;
                    if (round > limit)
                    {
                        break;
                    }

                    double infeasibility;
                    ComputePhaseIQReducedCosts(out infeasibility, eps);
                    if (infeasibility < lowestInfeasibility) lowestInfeasibility = infeasibility;
                    else if (infeasibility > lowestInfeasibility + eps)
                    {
                        // we're not using a shifting strategy for primal so the only way to combat
                        // numerical cycling is to lower the tolerances
                        if (eps < 1.0)
                        {
                            eps *= 10.0;
                            lowestInfeasibility = double.PositiveInfinity;
                        }
                        if (redecompose < maxRedecompose)
                        {
                            redecompose = 0;
                            goto DECOMPOSE;
                        }
                    }
                    // for establishing feasibility we don't need quadratic part, just do a simplex step
                    PhaseIQStep(out maxj, eps, out mini, out bound, out changeBasis);
                    updateCount++;

                    if (maxj == -1)
                    {
                        // if it's not feasible then return null solution
                        if (!feasible) return new double[cols + 1];
                    }

                    if (changeBasis)
                    {
                        if ((flags[V[maxj]] & flagNMid) != 0)
                        {
                            int s = Array.IndexOf(_S, V[maxj]);
                            S[s] = S[super - 1];
                            super--;
                        }

                        goto UPDATE;
                    }
                    else
                    {
                        continue;
                    }
                }

            MINISTEP:
                round++;
                if (round > limit)
                {
                    break;
                }

                double value = ComputeValueQuadratic();
                if (value > targetValue + epsZero)
                {
                    if ((flags[maximizeColumn] & flagB) != 0)
                    {
                        lb[maximizeColumn] = mb[maximizeColumn];
                        ComputeReducedCostGradient();
                        maxj = SelectPrimalAny();
                        if (maxj != -1)
                        {
                            mini = Array.IndexOf(_B, maximizeColumn);
                            bound = flagNLB;
                            ComputeBasisStep(maxj);
                            goto UPDATE;
                        }
                    }
                    else
                    {
                        if ((flags[maximizeColumn] & flagNMid) != 0)
                        {
                            int s = Array.IndexOf(_S, maximizeColumn);
                            S[s] = S[super - 1];
                            super--;
                            lb[maximizeColumn] = mb[maximizeColumn];
                            flags[maximizeColumn] = (flags[maximizeColumn] & ~flagN) | flagNLB;
                        }
                        // we're on lower bound
                        // increase lower bound while value remains above target
                        ComputeReducedCostGradient(); // don't need the whole gradient, change this
                        maxj = Array.IndexOf(_V, maximizeColumn);
                        ComputeBasisStep(maxj);
                        direction = 1.0;
                        double vQv = ComputevQv(maxj, direction);
                        // df = t * c[maximizeColumn] + 0.5 * t^2 * vQv = targetValue - value
                        // 0.5*A*t^2 + B*t + C = 0
                        // A = vQv, B = c[maximizeColumn], C = value - targetValue
                        double b = c[maxj];
                        double C = value - targetValue;
                        // t = [-B +- sqrt(B*B - 2*A*C)]/A
                        double t;
                        if (Math.Abs(vQv) < epsZero)
                        {
                            if (Math.Abs(b) < epsZero)
                            {
                                t = double.PositiveInfinity;
                            }
                            else
                            {
                                t = -C / b;
                            }
                        }
                        else
                        {
                            double sq = Math.Sqrt(b * b - 2 * vQv * C);
                            double t1 = (-b + sq) / vQv;
                            double t2 = (-b - sq) / vQv;
                            if (t1 >= 0)
                            {
                                if (t2 >= 0 && t2 < t1)
                                {
                                    t = t2;
                                }
                                else
                                {
                                    t = t1;
                                }
                            }
                            else
                            {
                                t = t2;
                            }
                        }

                        if (t > epsZero)
                        {
                            mini = -1;
                            double minr = t;
                            bound = flagNMid;

                            if (!SelectReducedGradientOutgoing(direction, eps, ref mini, ref minr, ref bound))
                            {
                                // we lost feasibility, go to phase I
                                feasible = false;
                                lowestInfeasibility = double.PositiveInfinity;
                                redecompose = 0;
                                goto DECOMPOSE;
                            }

                            mb[maximizeColumn] += minr;
                            lb[maximizeColumn] = mb[maximizeColumn];
                            for (i = 0; i < rows; i++)
                            {
                                mb[B[i]] -= minr * w[i];
                            }

                            if (mini != -1)
                            {
                                // we need to replace this with something that has negative gradient!!!
                                maxj = SelectPrimalReciprocal(maximizeColumn, eps);
                                if (maxj != -1)
                                {
                                    ComputeBasisStep(maxj);
                                    goto UPDATE;
                                }
                            }
                        }
                    }
                }

                int col = -1;
                double alpha = 1.0;

                if (super > 0)
                {
                    ComputeQx();
                    if (ComputeCGStep(eps, out alpha, false, out needsReducedGradientStep))
                    {

                        if (!SelectCGOutgoing(eps, ref col, ref alpha, ref bound))
                        {
                            // we lost feasibility, go to phase I
                            feasible = false;
                            lowestInfeasibility = double.PositiveInfinity;
                            redecompose = 0;
                            goto DECOMPOSE;
                        }

                        if (col == -1)
                        {
                            for (i = 0; i < cols + rows; i++)
                            {
                                mb[i] += cg_x[i];
                            }
                            updateCount++;
                            //ValidateFeasibility();
                            //ValidateFlags();
                        }
                        /*else if ((flags[col] & flagNMid) == 0)
                        {
                            System.Diagnostics.Trace.Assert(Array.IndexOf(_B, col) != -1, "Bad outgoing selection!");
                        }*/
                    }
                }

                if (col == -1)
                {
                    // no bound hit, alpha = 1 is valid step
                    // determine if we can free some variable

                    ComputeReducedCostGradient();

                    maxj = SelectPrimalIncoming(out direction, false);

                    if (maxj == -1)
                    {
                        // we might have disabled some variables, unflag them and recheck
                        bool retry = false;
                        if (disabledDirty)
                        {
                            for (i = 0; i < cols + rows; i++)
                            {
                                if ((flags[i] & flagDis) != 0)
                                {
                                    flags[i] &= ~flagDis;
                                    retry = true;
                                }
                            }
                            disabledDirty = false;
                        }
                        if (retry)
                        {
                            maxj = SelectPrimalIncoming(out direction, false);
                        }

                        if (maxj == -1)
                        {
                            goto TERMINATION;
                        }
                    }

                    // freeing V[maxj] will result in improved solution

                    if (super == 0 || (flags[V[maxj]] & flagNMid) != 0)
                    {
                        // special case the one free variable case
                        // in this situation the CG step is overkill
                        // we can use straight reduced gradient approach

                        // we also use this for superbasics when we're so close to optimal that
                        // CG decides it's not safe numerically to continue
                        // in that case the quadratic limit will most likely be very small
                        // but since we're working with unit step in reduced space the solving with
                        // basis shouldn't give too many problems due to zero snapping

                        updateCount++;
                        bool checkTermination;
                        if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis, out checkTermination))
                        {
                            // we lost feasibility, go to phase I
                            feasible = false;
                            lowestInfeasibility = double.PositiveInfinity;
                            redecompose = 0;
                            goto DECOMPOSE;
                        }

                        if (checkTermination)
                        {
                            ComputeReducedCostGradient();
                            maxj = CheckTermination(out direction);
                            if (maxj == -1)
                            {
                                goto TERMINATION;
                            }
                            updateCount++;
                            if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis, out checkTermination))
                            {
                                // we lost feasibility, go to phase I
                                feasible = false;
                                lowestInfeasibility = double.PositiveInfinity;
                                redecompose = 0;
                                goto DECOMPOSE;
                            }
                        }

                        if (changeBasis)
                        {
                            if ((flags[V[maxj]] & flagNMid) != 0)
                            {
                                int s = Array.IndexOf(_S, V[maxj]);
                                S[s] = S[super - 1];
                                super--;
                            }

                            goto UPDATE;
                        }

                        if (super == 1)
                        {
                            // we already updated superbasics to optimal
                            // so we know CG will return zero step
                            // just skip ahead and add another free variable

                            ComputeReducedCostGradient();

                            maxj = SelectPrimalIncoming(out direction, false);

                            if (maxj == -1)
                            {
                                goto TERMINATION;
                            }

                            col = V[maxj];
                            if ((flags[col] & flagNMid) == 0)
                            {
                                S[super] = col;
                                super++;
                                flags[col] = (flags[col] & ~flagN) | flagNMid;
                                osbDirty = true;
                                //ValidateSuperBasis();
                            }
                        }

                        goto MINISTEP;
                    }

                    col = V[maxj];
                    S[super] = col;
                    super++;
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                    osbDirty = true;
                    //ValidateSuperBasis();

                    goto MINISTEP;
                }

            //CGSTEP:
                // we hit a bound
                // if it's superbasic just fix it
                // otherwise we have to do a basis update

                // move for alpha
                if (alpha > epsZero)
                {
                    for (i = 0; i < cols + rows; i++)
                    {
                        mb[i] += alpha * cg_x[i];
                    }
                    updateCount++;
                }
                //ValidateFeasibility();
                /*ValidateFlags();
                if ((bound & flagNUB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - ub[col]) < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
                if ((bound & flagNLB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - lb[col]) < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }*/


                if ((flags[col] & flagNMid) != 0)
                {
                    // fix superbasic
                    flags[col] = (flags[col] & ~flagN) | bound;
                    i = Array.IndexOf(_S, col);
                    S[i] = S[super - 1];
                    super--;
                    osbDirty = true;
                    //ValidateSuperBasis();

                    goto MINISTEP;
                }

                //System.Diagnostics.Trace.Assert(Array.IndexOf(_B, col) != -1, "Bad outgoing selection!");

                // select superbasic to enter basis
                // avoid selecting those flagged as disabled because they'll likely cause
                // basis singularity again

                // try to replace col with a combination of superbasics
                // perform this by repeated updates
                mini = Array.IndexOf(_B, col);
                int inc = -1;
                bool numericCheck = true;
            SUPERBASICUPDATE:
                for (i = super - 1; i >= 0; i--)
                {
                    inc = S[i];
                    // skip if we disabled for numerical stability
                    if ((flags[col] & flagDis) != 0)
                    {
                        continue;
                    }
                    ComputeBasisReplace(inc);
                    // numerical stability verification only makes sense while we're still nonsinguler
                    // once we get singular further solver with U are undefined
                    if (numericCheck)
                    {
                        lu.FSolveU(ww, w, 0.00000001);
                    }

                    double pivot;
                    if (lu.Update(ww, mini, out pivot))
                    {
                        redecompose--;
                        if (numericCheck && Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                        {
                            // numerical errors accumulated are high
                            if (redecompose == maxRedecompose - 1)
                            {
                                redecompose = 0;
                                feasible = false; // forces recalc
                                flags[col] |= flagDis;
                                disabledDirty = true;
                                goto DECOMPOSE;
                            }
                            else
                            {
                                redecompose = 0;
                                feasible = false;
                                goto DECOMPOSE;
                            }
                        }
                        if (lu.Singular)
                        {
                            // we need more than one superbasic to complete the replace
                            numericCheck = false;
                            goto SUPERBASICUPDATE;
                        }
                        else
                        {
                            // replace successful, final column is inc
                            S[i] = S[super - 1];
                            super--;

                            maxj = Array.IndexOf(_V, inc);

                            k = B[mini];
                            B[mini] = V[maxj];
                            V[maxj] = k;
                            flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                            flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;
                            osbDirty = true;
                            //ValidateSuperBasis();
                            //ValidateFlags();

                            goto DECOMPOSE;
                        }
                    }
                }
                if (lu.Singular)
                {
                    Decompose();
                    redecompose = maxRedecompose;
                }

                /*int inc = S[super - 1];
                if ((flags[inc] & flagDis) == 0)
                {
                    super--;
                }
                else
                {
                    bool found = false;
                    // check if there is some other superbasic that is not flagged disabled
                    for (i = super - 2; i >= 0; i--)
                    {
                        inc = S[i];
                        if ((flags[inc] & flagDis) == 0)
                        {
                            found = true;
                            S[i] = S[super - 1];
                            super--;
                            break;
                        }
                    }
                    if (!found)
                    {*/
                // all superbasics were flagged, so we can't actually do the swap
                // try to do a reduced gradient and see if it makes sense to free
                // another variable so that we can use it to enter basis

                // we're not actually at optimality point for superbasics
                // so whatever we get here will be good for reduced gradient only
                // CG step might want us to go in the opposite direction actually
                // but we can't allow that because we can't fix basis in that direction
                // so pretend superbasics are fixed and do a single direction change
                ComputeReducedCostGradient();

                maxj = SelectPrimalIncoming(out direction, false);

                if (maxj == -1)
                {
                    // we might have disabled some variables, unflag them and recheck
                    bool retry = false;
                    if (disabledDirty)
                    {
                        for (i = 0; i < cols + rows; i++)
                        {
                            if ((flags[i] & flagDis) != 0)
                            {
                                flags[i] &= ~flagDis;
                                retry = true;
                            }
                        }
                        disabledDirty = false;
                    }
                    if (retry)
                    {
                        maxj = SelectPrimalIncoming(out direction, false);
                    }

                    if (maxj == -1)
                    {
                        goto TERMINATION;
                    }
                }

                updateCount++;
                if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis))
                {
                    // we lost feasibility, go to phase I
                    feasible = false;
                    lowestInfeasibility = double.PositiveInfinity;
                    redecompose = 0;
                    goto DECOMPOSE;
                }

                if (changeBasis)
                {
                    if ((flags[V[maxj]] & flagNMid) != 0)
                    {
                        int s = Array.IndexOf(_S, V[maxj]);
                        S[s] = S[super - 1];
                        super--;
                    }

                    goto UPDATE;
                }

                //ValidateSuperBasis();
                goto MINISTEP;
            /*}
        }
        //ValidateSuperBasis();

        // swap base
        maxj = Array.IndexOf(_V, inc);

        ComputeBasisStep(maxj);*/

            UPDATE:

                redecompose--;
                if (redecompose > 0)
                {
                    double pivot;
                    lu.Update(ww, mini, out pivot);
                    if (lu.Singular || Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                    {
                        // we aren't making the swap
                        // make sure to properly add superbasic back to S if we cancelled swap
                        col = V[maxj];
                        if ((flags[col] & flagNMid) != 0)
                        {
                            S[super] = col;
                            super++;
                            //ValidateSuperBasis();
                        }
                        if (redecompose == maxRedecompose - 1)
                        {
                            redecompose = 0;
                            feasible = false; // forces recalc
                            flags[col] |= flagDis;
                            disabledDirty = true;
                            continue;
                        }
                        else
                        {
                            redecompose = 0;
                            feasible = false;
                            continue;
                        }
                    }
                }

                k = B[mini];
                B[mini] = V[maxj];
                V[maxj] = k;
                flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;
                //ValidateSuperBasis();
                //ValidateFlags();
                goto DECOMPOSE;

            TERMINATION:

                /*if (feasible && (redecompose < maxRedecompose || updateCount > 0) && verificationAttempts < 5)
                {
                    redecompose = 0;
                    feasible = false;
                    verificationAttempts++;
                    continue;
                }*/

                /*if (CopositivityDetectionLight(eps))
                {
                    alpha = double.PositiveInfinity;
                    if (!SelectCGOutgoing(eps, ref col, ref alpha, ref bound))
                    {
                        // we lost feasibility, go to phase I
                        feasible = false;
                        lowestInfeasibility = double.PositiveInfinity;
                        redecompose = 0;
                        goto DECOMPOSE;
                    }

                    if (col != -1)
                    {
                        // otherwise we're unbounded which should not happen
                        goto CGSTEP;
                    }
                }*/

                double[] ret = ComputeReturnSolutionQuadratic();
                ret[ret.Length - 1] = lb[maximizeColumn];
                return ret;

            } while (round < limit); // limit computation so we don't dead loop, if everything works it shouldn't take more than this
            // when tuning dual feasibility limit to 100 rounds, we don't want to spend too much time on it

            // just in case
            // if feasible return the best we got
            if (feasible) return ComputeReturnSolutionQuadratic();
            return new double[cols + 1];
        }

        private double[] SolvePrimalQuadraticCGUnsafe2()
        {
            int limit = 5000;

            //RESTART:
            int i, k;
            int round = 0;
            int updateCount = 0;
            int redecompose = 0;
            int maxj, mini;
            const int maxRedecompose = 50;
            bool changeBasis;
            double direction;
            osbDirty = true;
            bool needsReducedGradientStep = false;

            //int verificationAttempts = 0;

            bool feasible = false;
            double lowestInfeasibility = double.PositiveInfinity;

            double eps = epsPrimal;

            if (disabledDirty)
            {
                for (i = 0; i < cols + rows; i++)
                {
                    flags[i] &= ~flagDis;
                }
                disabledDirty = false;
            }

            do
            {
            DECOMPOSE:

                if (redecompose <= 0)
                {
                    Decompose();
                    redecompose = maxRedecompose; // decompose every 50 iterations to maintain stability
                    feasible = false; // when refactoring basis recompute the solution
                }
                if (lu.Singular)
                {
                    redecompose = 0;
                    PatchSingularBasis();
                    //ValidateSuperBasis();
                    continue;
                }

                if (!feasible)
                {
                    bool recalculate;
                    do
                    {
                        ComputePrimalSolution(false);
                        UpdateMB();
                        feasible = IsPrimalQFeasible(eps, out recalculate);
                    } while (recalculate);
                    updateCount = 0;
                }

                int bound = flagNMid;

                if (!feasible)
                {
                    round++;
                    if (round > limit)
                    {
                        break;
                    }

                    double infeasibility;
                    ComputePhaseIQReducedCosts(out infeasibility, eps);
                    if (infeasibility < lowestInfeasibility) lowestInfeasibility = infeasibility;
                    else if (infeasibility > lowestInfeasibility + eps)
                    {
                        // we're not using a shifting strategy for primal so the only way to combat
                        // numerical cycling is to lower the tolerances
                        if (eps < 1.0)
                        {
                            eps *= 10.0;
                            lowestInfeasibility = double.PositiveInfinity;
                        }
                        if (redecompose < maxRedecompose)
                        {
                            redecompose = 0;
                            goto DECOMPOSE;
                        }
                    }
                    // for establishing feasibility we don't need quadratic part, just do a simplex step
                    PhaseIQStep(out maxj, eps, out mini, out bound, out changeBasis);
                    updateCount++;

                    if (maxj == -1)
                    {
                        // if it's not feasible then return null solution
                        if (!feasible) return new double[cols + 1];
                    }

                    if (changeBasis)
                    {
                        if ((flags[V[maxj]] & flagNMid) != 0)
                        {
                            int s = Array.IndexOf(_S, V[maxj]);
                            S[s] = S[super - 1];
                            super--;
                        }

                        goto UPDATE;
                    }
                    else
                    {
                        continue;
                    }
                }

            MINISTEP:
                round++;
                if (round > limit)
                {
                    break;
                }

                int col;
                double alpha;

                ComputeReducedCostGradient();

                maxj = SelectPrimalIncoming(out direction, false);

                if (maxj == -1)
                {
                    // we might have disabled some variables, unflag them and recheck
                    bool retry = false;
                    if (disabledDirty)
                    {
                        for (i = 0; i < cols + rows; i++)
                        {
                            if ((flags[i] & flagDis) != 0)
                            {
                                flags[i] &= ~flagDis;
                                retry = true;
                            }
                        }
                        disabledDirty = false;
                    }
                    if (retry)
                    {
                        maxj = SelectPrimalIncoming(out direction, false);
                    }

                    if (maxj == -1)
                    {
                        goto TERMINATION;
                    }
                }

                col = V[maxj];

                if ((flags[col] & flagNMid) != 0)
                {
                    col = -1;
                    alpha = 1.0;

                    if (ComputeCGStep(eps, out alpha, true, out needsReducedGradientStep))
                    {
                        if (!SelectCGOutgoing(eps, ref col, ref alpha, ref bound))
                        {
                            // we lost feasibility, go to phase I
                            feasible = false;
                            lowestInfeasibility = double.PositiveInfinity;
                            redecompose = 0;
                            goto DECOMPOSE;
                        }

                        if (col == -1)
                        {
                            for (i = 0; i < cols + rows; i++)
                            {
                                mb[i] += cg_x[i];
                            }
                            updateCount++;
                            //ValidateFeasibility();
                            //ValidateFlags();

                            goto MINISTEP;
                        }
                        else
                        {
                            // we hit a bound
                            // if it's superbasic just fix it
                            // otherwise we have to do a basis update

                            // move for alpha
                            if (alpha > epsZero)
                            {
                                for (i = 0; i < cols + rows; i++)
                                {
                                    mb[i] += alpha * cg_x[i];
                                }
                                updateCount++;
                            }
                            //ValidateFeasibility();
                            /*ValidateFlags();
                            if ((bound & flagNUB) != 0)
                            {
                                System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - ub[col]) < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                            }
                            if ((bound & flagNLB) != 0)
                            {
                                System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - lb[col]) < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                            }*/


                            if ((flags[col] & flagNMid) != 0)
                            {
                                // fix superbasic
                                flags[col] = (flags[col] & ~flagN) | bound;
                                i = Array.IndexOf(_S, col);
                                S[i] = S[super - 1];
                                super--;
                                osbDirty = true;
                                //ValidateSuperBasis();

                                goto MINISTEP;
                            }

                            //System.Diagnostics.Trace.Assert(Array.IndexOf(_B, col) != -1, "Bad outgoing selection!");

                            // select superbasic to enter basis
                            // avoid selecting those flagged as disabled because they'll likely cause
                            // basis singularity again

                            // try to replace col with a combination of superbasics
                            // perform this by repeated updates
                            mini = Array.IndexOf(_B, col);
                            int inc = -1;
                            bool numericCheck = true;
                        SUPERBASICUPDATE:
                            for (i = super - 1; i >= 0; i--)
                            {
                                inc = S[i];
                                // skip if we disabled for numerical stability
                                if ((flags[col] & flagDis) != 0)
                                {
                                    continue;
                                }
                                ComputeBasisReplace(inc);
                                // numerical stability verification only makes sense while we're still nonsinguler
                                // once we get singular further solver with U are undefined
                                if (numericCheck)
                                {
                                    lu.FSolveU(ww, w, 0.00000001);
                                }

                                double pivot;
                                if (lu.Update(ww, mini, out pivot))
                                {
                                    redecompose--;
                                    if (numericCheck && Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                                    {
                                        // numerical errors accumulated are high
                                        if (redecompose == maxRedecompose - 1)
                                        {
                                            redecompose = 0;
                                            feasible = false; // forces recalc
                                            flags[col] |= flagDis;
                                            disabledDirty = true;
                                            goto DECOMPOSE;
                                        }
                                        else
                                        {
                                            redecompose = 0;
                                            feasible = false;
                                            goto DECOMPOSE;
                                        }
                                    }
                                    if (lu.Singular)
                                    {
                                        // we need more than one superbasic to complete the replace
                                        numericCheck = false;
                                        goto SUPERBASICUPDATE;
                                    }
                                    else
                                    {
                                        // replace successful, final column is inc
                                        S[i] = S[super - 1];
                                        super--;

                                        maxj = Array.IndexOf(_V, inc);

                                        k = B[mini];
                                        B[mini] = V[maxj];
                                        V[maxj] = k;
                                        flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                                        flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;
                                        osbDirty = true;
                                        //ValidateSuperBasis();
                                        //ValidateFlags();

                                        goto DECOMPOSE;
                                    }
                                }
                            }
                            if (lu.Singular)
                            {
                                Decompose();
                                redecompose = maxRedecompose;
                            }

                            /*int inc = S[super - 1];
                            if ((flags[inc] & flagDis) == 0)
                            {
                                super--;
                            }
                            else
                            {
                                bool found = false;
                                // check if there is some other superbasic that is not flagged disabled
                                for (i = super - 2; i >= 0; i--)
                                {
                                    inc = S[i];
                                    if ((flags[inc] & flagDis) == 0)
                                    {
                                        found = true;
                                        S[i] = S[super - 1];
                                        super--;
                                        break;
                                    }
                                }
                                if (!found)
                                {*/
                            // all superbasics were flagged, so we can't actually do the swap
                            // try to do a reduced gradient and see if it makes sense to free
                            // another variable so that we can use it to enter basis

                            // we're not actually at optimality point for superbasics
                            // so whatever we get here will be good for reduced gradient only
                            // CG step might want us to go in the opposite direction actually
                            // but we can't allow that because we can't fix basis in that direction
                            // so pretend superbasics are fixed and do a single direction change
                            ComputeReducedCostGradient();

                            maxj = SelectPrimalIncoming(out direction, false);

                            if (maxj == -1)
                            {
                                // we might have disabled some variables, unflag them and recheck
                                bool retry = false;
                                if (disabledDirty)
                                {
                                    for (i = 0; i < cols + rows; i++)
                                    {
                                        if ((flags[i] & flagDis) != 0)
                                        {
                                            flags[i] &= ~flagDis;
                                            retry = true;
                                        }
                                    }
                                    disabledDirty = false;
                                }
                                if (retry)
                                {
                                    maxj = SelectPrimalIncoming(out direction, false);
                                }

                                if (maxj == -1)
                                {
                                    goto TERMINATION;
                                }
                            }

                            updateCount++;
                            if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis))
                            {
                                // we lost feasibility, go to phase I
                                feasible = false;
                                lowestInfeasibility = double.PositiveInfinity;
                                redecompose = 0;
                                goto DECOMPOSE;
                            }

                            if (changeBasis)
                            {
                                if ((flags[V[maxj]] & flagNMid) != 0)
                                {
                                    int s = Array.IndexOf(_S, V[maxj]);
                                    S[s] = S[super - 1];
                                    super--;
                                }

                                goto UPDATE;
                            }

                            //ValidateSuperBasis();
                            goto MINISTEP;
                        }
                    }
                }

                updateCount++;
                if (!ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis))
                {
                    // we lost feasibility, go to phase I
                    feasible = false;
                    lowestInfeasibility = double.PositiveInfinity;
                    redecompose = 0;
                    goto DECOMPOSE;
                }

                if (changeBasis)
                {
                    if ((flags[V[maxj]] & flagNMid) != 0)
                    {
                        int s = Array.IndexOf(_S, V[maxj]);
                        S[s] = S[super - 1];
                        super--;
                    }

                    goto UPDATE;
                }

                goto MINISTEP;

            UPDATE:

                redecompose--;
                if (redecompose > 0)
                {
                    double pivot;
                    lu.Update(ww, mini, out pivot);
                    if (lu.Singular || Math.Abs(w[mini] - pivot) > 1.0e-6 * Math.Abs(w[mini]) + epsZero)
                    {
                        // we aren't making the swap
                        // make sure to properly add superbasic back to S if we cancelled swap
                        col = V[maxj];
                        if ((flags[col] & flagNMid) != 0)
                        {
                            S[super] = col;
                            super++;
                            //ValidateSuperBasis();
                        }
                        if (redecompose == maxRedecompose - 1)
                        {
                            redecompose = 0;
                            feasible = false; // forces recalc
                            flags[col] |= flagDis;
                            disabledDirty = true;
                            continue;
                        }
                        else
                        {
                            redecompose = 0;
                            feasible = false;
                            continue;
                        }
                    }
                }

                k = B[mini];
                B[mini] = V[maxj];
                V[maxj] = k;
                flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                flags[V[maxj]] = (flags[V[maxj]] | bound) & ~flagB;
                //ValidateSuperBasis();
                //ValidateFlags();
                goto DECOMPOSE;

            TERMINATION:

                /*if (feasible && (redecompose < maxRedecompose || updateCount > 0) && verificationAttempts < 5)
                {
                    redecompose = 0;
                    feasible = false;
                    verificationAttempts++;
                    continue;
                }*/

                /*if (CopositivityDetectionLight(eps))
                {
                    alpha = double.PositiveInfinity;
                    if (!SelectCGOutgoing(eps, ref col, ref alpha, ref bound))
                    {
                        // we lost feasibility, go to phase I
                        feasible = false;
                        lowestInfeasibility = double.PositiveInfinity;
                        redecompose = 0;
                        goto DECOMPOSE;
                    }

                    if (col != -1)
                    {
                        // otherwise we're unbounded which should not happen
                        goto CGSTEP;
                    }
                }*/

                return ComputeReturnSolutionQuadratic();

            } while (round < limit); // limit computation so we don't dead loop, if everything works it shouldn't take more than this
            // when tuning dual feasibility limit to 100 rounds, we don't want to spend too much time on it

            // just in case
            // if feasible return the best we got
            if (feasible) return ComputeReturnSolutionQuadratic();
            return new double[cols + 1];
        }

        private void PhaseIQStep(out int maxj, double eps, out int mini, out int bound, out bool changeBasis)
        {
            double direction;
            maxj = SelectPrimalIncoming(out direction, false);


            mini = -1;
            double minr = double.PositiveInfinity;
            bound = flagNMid;
            changeBasis = false;

            if (maxj == -1)
            {
                // not feasible (can happen as part of MIP)
                return;
            }

            ComputeBasisStep(maxj);
            SelectPhaseIQOutgoing(direction, eps, ref mini, ref minr, ref bound);

            int col = V[maxj];
            double dist;
            if (direction > 0.0)
            {
                dist = ub[col] - mb[col];
            }
            else
            {
                dist = mb[col] - lb[col];
            }
            if (Math.Abs(minr) >= dist - epsZero)
            {
                // just a bound swap
                if ((flags[col] & flagNMid) != 0)
                {
                    int s = Array.IndexOf(_S, col);
                    S[s] = S[super - 1];
                    super--;
                }
                if (direction > 0.0)
                {
                    flags[col] = (flags[col] & ~flagN) | flagNUB;
                }
                else
                {
                    flags[col] = (flags[col] & ~flagN) | flagNLB;
                }
                //ValidateSuperBasis();                
            }
            else
            {
                // we hit basic bound, update basis
                changeBasis = true;
                dist = minr;
            }
            osbDirty = true;

            minr = direction * dist;
            mb[col] += minr;
            for (int i = 0; i < rows; i++)
            {
                mb[B[i]] -= minr * w[i];
            }
            /*if (!changeBasis)
            {
                ValidateFlags();
            }
            else
            {
                col = B[mini];
                if ((bound & flagNUB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - ub[col]) < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
                if ((bound & flagNLB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - lb[col]) < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
            }*/
        }

        private bool ReducedGradientStep(int maxj, double direction, double eps, out int mini, out int bound, out bool changeBasis)
        {
            bool checkTermination;
            return ReducedGradientStep(maxj, direction, eps, out mini, out bound, out changeBasis, out checkTermination);
        }

        private bool ReducedGradientStep(int maxj, double direction, double eps, out int mini, out int bound, out bool changeBasis, out bool checkTermination)
        {
            ComputeBasisStep(maxj);

            mini = -1;
            double minr = ComputeQuadraticLimit(maxj, direction);
            bound = flagNMid;
            changeBasis = false;
            checkTermination = false;

            if (!SelectReducedGradientOutgoing(direction, eps, ref mini, ref minr, ref bound))
            {
                return false;
            }

            int col = V[maxj];
            double dist;
            if (direction > 0.0)
            {
                dist = ub[col] - mb[col];
            }
            else
            {
                dist = mb[col] - lb[col];
            }
            if (minr >= dist - epsZero)
            {
                // just a bound swap
                minr = dist;
                if ((flags[col] & flagNMid) != 0)
                {
                    int s = Array.IndexOf(_S, col);
                    S[s] = S[super - 1];
                    super--;
                    osbDirty = true;
                }
                if (direction > 0.0)
                {
                    flags[col] = (flags[col] & ~flagN) | flagNUB;
                }
                else
                {
                    flags[col] = (flags[col] & ~flagN) | flagNLB;
                }
                //ValidateSuperBasis();
            }
            else if (mini == -1)
            {
                // no bound hit, add to superbasic
                if ((flags[col] & flagNMid) == 0)
                {
                    flags[col] = (flags[col] & ~flagN) | flagNMid;
                    S[super] = col;
                    super++;
                    osbDirty = true;
                }
                else
                {
                    // we were already superbasic
                    // check if we're making significant progress, if we're below threshold
                    // we should check if we're already as close to optimal as we'll be able to get
                    // due to numerical precision
                    if (0.5 * c[maxj] * direction * minr < eps)
                    {
                        checkTermination = true;
                    }
                }
                //ValidateSuperBasis();
            }
            else
            {
                // we hit basic bound, update basis
                changeBasis = true;
                osbDirty = true;
            }

            if (minr > 0.0)
            {
                minr *= direction;
                mb[col] += minr;
                for (int i = 0; i < rows; i++)
                {
                    mb[B[i]] -= minr * w[i];
                }
            }
            //ValidateFeasibility();
            /*if (!changeBasis)
            {
                ValidateFlags();
            }
            else
            {
                col = B[mini];
                if ((bound & flagNUB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - ub[col]) < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
                if ((bound & flagNLB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - lb[col]) < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
            }*/
            return true;
        }

        public void ValidateBaseIntegrity()
        {
#if !SILVERLIGHT
            for (int i = 0; i < cols + rows; i++)
            {
                if ((_flags[i] & flagN) != 0)
                {
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_V, i) != -1, "Basis corruption");
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_B, i) == -1, "Basis corruption");
                }
                if ((_flags[i] & flagB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_V, i) == -1, "Basis corruption");
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_B, i) != -1, "Basis corruption");
                }
            }
#endif
        }

        public void ValidateFeasibility()
        {
#if !SILVERLIGHT
            for (int row = 0; row < baseRows; row++)
            {
                double sum = 0.0;
                for (int i = 0; i < cols; i++)
                {
                    sum += mb[i] * a[row + i * baseRows];
                }
                sum += mb[cols + row];
                System.Diagnostics.Trace.Assert(Math.Abs(sum) < 0.00001, "Feasibility lost");
            }
            for (int i = 0; i < cols + rows; i++)
            {
                double difflb = mb[i] - lb[i];
                double diffub = mb[i] - ub[i];
                bool ifeasiblelb = (difflb >= -Math.Abs(lb[i]) * epsPrimalRel - epsPrimal);
                bool ifeasibleub = (diffub <= +Math.Abs(ub[i]) * epsPrimalRel + epsPrimal);
                System.Diagnostics.Trace.Assert(ifeasiblelb && ifeasibleub, "Feasibility lost");
            }
#endif
        }

        public void ValidateSuperBasis()
        {
#if !SILVERLIGHT
            for (int s = 0; s < super - 1; s++)
            {
                int col = S[s];
                int i = Array.IndexOf(_S, col, s + 1);
                System.Diagnostics.Trace.Assert(i == -1 || i >= super, "Superbasis corruption");
            }
            for (int s = 0; s < super; s++)
            {
                int col = S[s];
                int i = Array.IndexOf(_V, col);
                System.Diagnostics.Trace.Assert(i != -1, "Superbasis corruption");
                System.Diagnostics.Trace.Assert((flags[col] & flagNMid) != 0, "Superbasis corruption");
            }
#endif
        }

        public void ValidateFlags()
        {
#if !SILVERLIGHT
            for (int col = 0; col < cols + rows; col++)
            {
                if ((flags[col] & flagNUB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_V, col) != -1, "Flags corruption");
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - ub[col]) < Math.Abs(ub[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
                if ((flags[col] & flagNLB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_V, col) != -1, "Flags corruption");
                    System.Diagnostics.Trace.Assert(Math.Abs(mb[col] - lb[col]) < Math.Abs(lb[col]) * epsPrimalRel + epsPrimalLow, "Flags corruption");
                }
                if ((flags[col] & flagNMid) != 0)
                {
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_S, col) != -1, "Flags corruption");
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_V, col) != -1, "Flags corruption");
                }
                if ((flags[col] & flagB) != 0)
                {
                    System.Diagnostics.Trace.Assert(Array.IndexOf(_B, col) != -1, "Flags corruption");
                }
            }
#endif
        }

        public void ValidateCGFeasibility()
        {
#if !SILVERLIGHT
            for (int row = 0; row < baseRows; row++)
            {
                double sum = 0.0;
                for (int i = 0; i < cols; i++)
                {
                    sum += cg_x[i] * a[row + i * baseRows];
                }
                sum += cg_x[cols + row];
                System.Diagnostics.Trace.Assert(Math.Abs(sum) < epsPrimal, "CG Feasibility lost");
            }
#endif
        }

        public void ValidateCGFeasibility2()
        {
#if !SILVERLIGHT
            for (int row = 0; row < baseRows; row++)
            {
                double sum = 0.0;
                for (int i = 0; i < cols; i++)
                {
                    sum += cg_p[i] * a[row + i * baseRows];
                }
                sum += cg_p[cols + row];
                System.Diagnostics.Trace.Assert(Math.Abs(sum) < epsPrimal, "CG Feasibility lost");
            }
#endif
        }

#if SILVERLIGHT
        public double[] SolveDual(bool startWithPhaseI)
        {
            if (hardInfeasibility) return new double[cols + 1];
            double[] ret = null;


            this.a = arraySet.SparseMatrixData;
            this.U = arraySet.LU_U;
            this.d = arraySet._d;
            this.x = arraySet._x;
            this.w = arraySet._w;
            this.ww = arraySet._ww;
            this.wd = arraySet._wd;
            this.c = arraySet._c;
            this.u = arraySet._u;
            this.cost = arraySet._costWorking;
            this.sparseValue = arraySet.SparseMatrixValue;
            this.D = arraySet.extraConstraints;
            this.B = _B;
            this.V = _V;
            this.sparseRow = arraySet.SparseMatrixRow;
            this.sparseCol = arraySet.SparseMatrixCol;
            this.flags = _flags;
            this.lb = _lb;
            this.ub = _ub;
            this.pD = arraySet._pD;

            SetupExtraConstraints();

            lu.BeginSafe();
            LoadCost();
            PerturbCost();
            bool dualUnbounded;
            shifted = false;
            //Preprocessing();
            ret = SolveDualUnsafe(startWithPhaseI, out dualUnbounded);
            if (!dualUnbounded)
            {
                // unshifted might no longer be dual feasible for our tolerances
                // reoptimize it with primal, dual phase II tolerances are ok
                if (shifted)
                {
                    LoadCost();
                    PerturbCost();
                }
                ret = SolvePrimalUnsafe(false, true, true, true);
            }
            if ((costWorkingDirty && !dualUnbounded) || ret == null)
            {
                if (costWorkingDirty) LoadCost();
                int[] _Bcopy = null;
                int[] _Vcopy = null;
                int[] _flagscopy = null;
                if (!dualUnbounded)
                {
                    // we're just reoptimizing for changes in cost vector
                    // since we'll be reusing this basis for dual later
                    // we should store the basis, because the basis on unperturbed problem
                    // very likely is not dual feasible on perturbed system
                    // since we're using deterministic perturbation the likelihood of remaining
                    // dual feasible is much higher since changes due to shifting are very small
                    _Bcopy = (int[])_B.Clone();
                    _Vcopy = (int[])_V.Clone();
                    _flagscopy = (int[])_flags.Clone();
                }
                ret = SolvePrimalUnsafe(false, false, false, false);
                if (!dualUnbounded)
                {
                    _B = _Bcopy;
                    _V = _Vcopy;
                    _flags = _flagscopy;
                }
            }
            lu.EndUnsafe();

            this.a = null;
            this.U = null;
            this.d = null;
            this.x = null;
            this.w = null;
            this.ww = null;
            this.wd = null;
            this.c = null;
            this.u = null;
            this.cost = null;
            this.sparseValue = null;
            this.pD = null;
            this.D = null;
            this.B = null;
            this.V = null;
            this.sparseRow = null;
            this.sparseCol = null;
            this.flags = null;
            this.lb = null;
            this.ub = null;

            return ret;
        }
#else
        public unsafe double[] SolveDual(bool startWithPhaseI)
        {
            if (hardInfeasibility) return new double[cols + 1];
            double[] ret = null;

            fixed (double** pD = arraySet._pD)
            fixed (double* a = arraySet.SparseMatrixData, U = arraySet.LU_U, sL = arraySet.LUsparseL, column = arraySet.LUcolumn, column2 = arraySet.LUcolumn2, d = arraySet._d, x = arraySet._x, w = arraySet._w, ww = arraySet._ww, wd = arraySet._wd, c = arraySet._c, u = arraySet._u, cost = arraySet._costWorking, sparseValue = arraySet.SparseMatrixValue, D = arraySet.extraConstraints, lb = _lb, ub = _ub/*, beta = _beta, betaBackup = _betaBackup*/)
            fixed (int* B = _B, V = _V, sparseRow = arraySet.SparseMatrixRow, sparseCol = arraySet.SparseMatrixCol, P = arraySet.LU_P, Q = arraySet.LU_Q, LJ = arraySet.LU_LJ, sLI = arraySet.LUsparseLI, sLstart = arraySet.LUsparseLstart, flags = _flags)
            {
                this.a = a;
                this.U = U;
                this.d = d;
                this.x = x;
                this.w = w;
                this.ww = ww;
                this.wd = wd;
                this.c = c;
                this.u = u;
                this.cost = cost;
                this.sparseValue = sparseValue;
                this.D = D;
                this.B = B;
                this.V = V;
                this.sparseRow = sparseRow;
                this.sparseCol = sparseCol;
                this.flags = flags;
                this.lb = lb;
                this.ub = ub;
                this.pD = pD;
                //this.beta = beta;
                //this.betaBackup = betaBackup;

                SetupExtraConstraints();

                lu.BeginUnsafe(U, sL, P, Q, LJ, sLI, sLstart, column, column2);
                LoadCost();
                PerturbCost();
                bool dualUnbounded;
                shifted = false;
                //Preprocessing();
                ret = SolveDualUnsafe(startWithPhaseI, out dualUnbounded);
                if (!dualUnbounded)
                {
                    // unshifted might no longer be dual feasible for our tolerances
                    // reoptimize it with primal, dual phase II tolerances are ok
                    if (shifted)
                    {
                        LoadCost();
                        PerturbCost();
                    }
                    ret = SolvePrimalUnsafe(false, true, true, true);
                }
                if ((costWorkingDirty && !dualUnbounded) || ret == null)
                {
                    if (costWorkingDirty) LoadCost();
                    int[] _Bcopy = null;
                    int[] _Vcopy = null;
                    int[] _flagscopy = null;
                    if (!dualUnbounded)
                    {
                        // we're just reoptimizing for changes in cost vector
                        // since we'll be reusing this basis for dual later
                        // we should store the basis, because the basis on unperturbed problem
                        // very likely is not dual feasible on perturbed system
                        // since we're using deterministic perturbation the likelihood of remaining
                        // dual feasible is much higher since changes due to shifting are very small
                        _Bcopy = (int[])_B.Clone();
                        _Vcopy = (int[])_V.Clone();
                        _flagscopy = (int[])_flags.Clone();
                    }
                    ret = SolvePrimalUnsafe(false, false, false, false);
                    if (!dualUnbounded)
                    {
                        _B = _Bcopy;
                        _V = _Vcopy;
                        _flags = _flagscopy;
                    }
                }
                lu.EndUnsafe();

                this.a = null;
                this.U = null;
                this.d = null;
                this.x = null;
                this.w = null;
                this.ww = null;
                this.wd = null;
                this.c = null;
                this.u = null;
                this.cost = null;
                this.sparseValue = null;
                this.pD = null;
                this.D = null;
                this.B = null;
                this.V = null;
                this.sparseRow = null;
                this.sparseCol = null;
                this.flags = null;
                this.lb = null;
                this.ub = null;
            }

            return ret;
        }
#endif

        public double[] SolveDualUnsafe(bool startWithPhaseI, out bool dualUnbounded)
        {
            // LU = A_B
            // d = x_B <- A_B^-1*b ... primal solution
            // u = u <- c_B*A_B^-1 ... dual solution
            // w_N <- c_N - u*A_N  ... dual solution

            int i, k;
            int round = 0;
            bool needsRecalc = true;
            int redecompose = 0;
            const int maxRedecompose = 50;
            dualUnbounded = false;

            if (disabledDirty)
            {
                for (i = 0; i < cols + rows; i++)
                {
                    flags[i] &= ~flagDis;
                }
                disabledDirty = false;
            }

            bool blacklistAllowed = true;

            bool phaseI = false;
            if (startWithPhaseI)
            {
                Decompose();
                if (lu.Singular)
                {
                    PatchSingularBasis();
                    Decompose();
                }
                redecompose = maxRedecompose;
                ComputePrimalSolution(phaseI);
                ComputeReducedCosts();
                phaseI = BoundFlip(phaseI, 0.0, epsDualI, -1, true);
                if (phaseI)
                {
                    ComputePrimalSolution(phaseI);
                    BoundFlip(phaseI, 0.0, epsDualI, -1, true);
                }
                needsRecalc = false;
            }

            do
            {
            DECOMPOSE:
                if (redecompose <= 0)
                {
                    Decompose();
                    redecompose = maxRedecompose; // decompose each time to see where the instability comes from
                    needsRecalc = true; // for long dual phases refresh the reduced costs
                }

                if (lu.Singular)
                {
                    //System.Diagnostics.Trace.WriteLine("Basis singular in " + round);
                    // try to patch the basis by replacing singular columns with corresponding slacks of singular rows
                    redecompose = 0;
                    PatchSingularBasis();
                    goto DECOMPOSE;
                }

                if (needsRecalc)
                {
                    ComputePrimalSolution(phaseI);
                    ComputeReducedCosts();
                    needsRecalc = false;
                }

                double delta;
                int bound;

                int mini = SelectDualOutgoing(phaseI, out delta, out bound);

                if (mini == -1)
                {
                    // if we blacklisted some variables give it another try
                    if (disabledDirty)
                    {
                        for (i = 0; i < cols + rows; i++)
                        {
                            flags[i] &= ~flagDis;
                        }
                        disabledDirty = false;
                        blacklistAllowed = false;
                        redecompose = 0;
                        needsRecalc = true;
                        continue;
                    }
                    if (phaseI)
                    {
                        // if a variable is on a nonexisting bound then we must have an unrecoverable dual unfeasibility, shouldn't happen
                        for (int col = 0; col < cols + rows; col++)
                        {
                            if ((flags[col] & flagNLB) != 0 && (flags[col] & flagLB) == 0) return new double[cols + 1];
                            else if ((flags[col] & flagNUB) != 0 && (flags[col] & flagUB) == 0) return new double[cols + 1];
                        }
                        phaseI = false;
                        ComputePrimalSolution(phaseI);
                        goto DECOMPOSE;
                    }
                    // refine solution
                    //ComputePrimalSolution();
                    ColdStart = false;
                    //System.Diagnostics.Trace.WriteLine("Dual optimal in " + round);
                    return ComputeReturnSolution();
                }

                int minj;
                double minr;

                if (!ComputeDualPivotRow(phaseI, mini))
                {
                    Decompose();
                    //System.Diagnostics.Trace.WriteLine("Feasibility lost in " + round + ", conditioning " + U[rows * rows - 1]);
                    if (lu.Singular)
                    {
                        PatchSingularBasis();
                        Decompose();
                    }
                    redecompose = maxRedecompose;
                    ComputeReducedCosts();
                    phaseI = BoundFlip(phaseI, 0.0, epsDualI, -1, true);
                    if (phaseI)
                    {
                        ComputePrimalSolution(phaseI);
                        BoundFlip(phaseI, 0.0, epsDualI, -1, true);
                    }
                    goto DECOMPOSE;
                }

                //SelectDualIncoming(phaseI, mini, out minj, out minr);
                SelectDualIncomingWithBoundFlips(phaseI, delta, out minj, out minr);

                if (minj == -1)
                {
                    if (redecompose == maxRedecompose)
                    {
                        // unfeasible, return null solution, don't pursue this branch because no solution exists here
                        double[] ret = new double[cols + 1];
                        dualUnbounded = true;
                        // debug measure, verify primal infeasibility by solving the primal
                        //ret = SolvePrimalUnsafe(true, false, false);
                        //System.Diagnostics.Trace.WriteLine("Dual unfeasible after " + round);
                        return ret;
                    }
                    else
                    {
                        // make a clean factorization to make sure we're really unbounded
                        redecompose = 0;
                        needsRecalc = true;
                        continue;
                    }
                }

                // shifting to prevent numerical cycling
                if (minr * delta > 0)
                {
                    //System.Diagnostics.Debug.WriteLine("Value shifted in round " + round);
                    if (delta > 0)
                    {
                        minr = -1.0e-12;
                    }
                    else
                    {
                        minr = 1.0e-12;
                    }
                    double shift = minr * wd[minj] - c[minj];
                    c[minj] += shift;
                    cost[V[minj]] += shift; // requires working copy for cost
                    costWorkingDirty = true;
                    shifted = true;
                    // NLB: *cj < eps, NUB: *cj > -eps
                    // update goes *cj -= minr * *wdj
                    for (int j = 0; j < cols; j++)
                    {
                        if (j != minj)
                        {
                            int col = V[j];
                            if ((flags[col] & flagNLB) != 0)
                            {
                                if (c[j] - minr * wd[j] > epsDual)
                                {
                                    shift = epsDual - c[j] + minr * wd[j];
                                    c[j] += shift;
                                    cost[col] += shift; // requires working copy for cost
                                }
                            }
                            else if ((flags[col] & flagNUB) != 0)
                            {
                                if (c[j] - minr * wd[j] < -epsDual)
                                {
                                    shift = -epsDual - c[j] + minr * wd[j];
                                    c[j] += shift;
                                    cost[col] += shift; // requires working copy for cost
                                }
                            }
                        }
                    }
                }

                BoundFlip(phaseI, minr, phaseI ? epsDualI : epsDual, minj, false);
                UpdateDual(phaseI, minj, mini, minr, delta, false); // use true if using simple ratio test

                redecompose--;
                if (redecompose > 0)
                {
                    double pivot;
                    lu.Update(ww, mini, out pivot);
                    if (lu.Singular || Math.Abs(wd[minj] - pivot) > 1.0e-6 * Math.Abs(wd[minj]) + epsZero)
                    {
                        if (redecompose == maxRedecompose - 1)
                        {
                            // if we don't allow blacklisting we'll either go on if not singular or patch the basis
                            if (blacklistAllowed)
                            {
                                flags[B[mini]] |= flagDis;
                                disabledDirty = true;
                                redecompose = 0;
                                needsRecalc = true;
                                continue;
                            }
                        }
                        else
                        {
                            redecompose = 0;
                            needsRecalc = true;
                            continue;
                        }
                    }
                }

                //System.Diagnostics.Debug.WriteLine("Round " + round + ", swap " + B[mini] + " out, " + V[minj] + " in, value change estimate " + delta * minr);

                // swap base
                UpdateBounds();
                k = B[mini];
                B[mini] = V[minj];
                V[minj] = k;
                flags[B[mini]] = (flags[B[mini]] | flagB) & ~flagN;
                flags[V[minj]] = (flags[V[minj]] | bound) & ~flagB;

                round++;
            } while (round < 5000);
            // just in case
            return new double[cols + 1];
        }
    }
}
