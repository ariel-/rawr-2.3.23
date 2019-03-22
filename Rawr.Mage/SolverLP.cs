using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
#if SILVERLIGHT
    public sealed class SolverLP : IComparable<SolverLP>
#else
    public unsafe sealed class SolverLP : IComparable<SolverLP>
#endif
    {
        private int cRows;
        private int cCols;
        private Solver solver;
        private LP lp;
        private double[] compactSolution;
        private bool needsDual;
        private bool needsQuadratic;
        private int segments;
        public StringBuilder Log;
        private int mpsRow;
        private int[] sort;
        private double Qk;
        private int maximizeColumn;
        private double targetValue;
        //public int[] disabledHex;

#if SILVERLIGHT
        private double[] pRowScale;
        private double[] pColumnScale;
        private double[] pCost;
#else
        private double* pRowScale;
        private double* pColumnScale;
        private double* pCost;
#endif
        private SparseMatrix sparseMatrix;

#if SILVERLIGHT
        public void BeginSafe(double[] pRowScale, double[] pColumnScale, double[] pCost, double[] pData, double[] pValue, int[] pRow, int[] pCol)
        {
            this.pRowScale = pRowScale;
            this.pColumnScale = pColumnScale;
            this.pCost = pCost;
            sparseMatrix = lp.A;
            sparseMatrix.BeginSafe(pData, pValue, pRow, pCol);
        }
#else
        public void BeginUnsafe(double* pRowScale, double* pColumnScale, double* pCost, double* pData, double* pValue, int* pRow, int* pCol)
        {
            this.pRowScale = pRowScale;
            this.pColumnScale = pColumnScale;
            this.pCost = pCost;
            sparseMatrix = lp.A;
            sparseMatrix.BeginUnsafe(pData, pValue, pRow, pCol);
        }
#endif

        public void EndUnsafe()
        {
            sparseMatrix.EndUnsafe();
            sparseMatrix = null;
            pRowScale = null;
            pCost = null;
        }

        public void EndColumnConstruction()
        {
            lp.EndColumnConstruction();
        }

        public void ForceRecalculation(bool dual)
        {
            compactSolution = null;
            needsDual = dual;
        }

        public void ForceRecalculation()
        {
            compactSolution = null;
            needsDual = false;
        }

        public SolverLP Clone()
        {
            //if (compactSolution != null && !allowReuse) throw new InvalidOperationException();
            SolverLP clone = (SolverLP)this.MemberwiseClone();
            clone.compactSolution = null;
            clone.lp = lp.Clone();
            if (Log != null) clone.Log = new StringBuilder(Log.ToString());
            //if (disabledHex != null) clone.disabledHex = (int[])disabledHex.Clone();
            return clone;
        }

        public SolverLP()
        {
        }

        public SolverLP(ArraySet arraySet, int baseRows, int maximumColumns, Solver solver, int segments)
        {
            Initialize(arraySet, baseRows, maximumColumns, solver, segments);
        }

        public void Initialize(ArraySet arraySet, int baseRows, int maximumColumns, Solver solver, int segments)
        {
            this.arraySet = arraySet;
            if (baseRows > arraySet.maxSolverRows || maximumColumns > arraySet.maxSolverCols)
            {
                arraySet.maxSolverRows = Math.Max(baseRows, arraySet.maxSolverRows);
                arraySet.maxSolverCols = Math.Max(maximumColumns, arraySet.maxSolverCols);
                arraySet.RecreateSolverArrays();
            }

            this.solver = solver;
            this.segments = segments;
            cRows = baseRows;
            cCols = 0;

            for (int i = 0; i <= cRows; i++)
            {
                arraySet.rowScale[i] = 1.0;
            }

            if (lp == null)
            {
                lp = new LP(cRows, maximumColumns, arraySet);
            }
            else
            {
                lp.Initialize(cRows, maximumColumns, arraySet);
            }

            compactSolution = null;
            sort = null;
            needsDual = false;
            needsQuadratic = false;
            maximizeColumn = -1;
        }

        public void SetRowScale(int row, double value)
        {
            if (row == -1) return;
            arraySet.rowScale[row] = value;
        }

        public void SetRowScaleUnsafe(int row, double value)
        {
            if (row == -1) return;
            pRowScale[row] = value;
        }

        public void SetColumnScale(int col, double value)
        {
            arraySet.columnScale[col] = value;
        }

        public void SetColumnScaleUnsafe(int col, double value)
        {
            pColumnScale[col] = value;
        }

        public int Columns
        {
            get
            {
                return cCols;
            }
        }

        public int AddColumn()
        {
            arraySet.columnScale[cCols] = 1.0;
            cCols++;
            return lp.AddColumn();
        }

        public int AddColumnUnsafe()
        {
            pColumnScale[cCols] = 1.0;
            cCols++;
            return lp.AddColumn();
        }

        public double this[int row, int col]
        {
            get
            {
                if (row == -1 || col == -1) return 0;
                return lp[row, col] / arraySet.rowScale[row] / arraySet.columnScale[col];
            }
            set
            {
                if (row == -1 || col == -1) return;
                lp[row, col] = value * arraySet.rowScale[row] * arraySet.columnScale[col];
                compactSolution = null;
            }
        }

        public void SetElement(int row, int col, double value)
        {
            this[row, col] = value;
        }

        public void SetElementUnsafe(int row, int col, double value)
        {
            if (row == -1) return;
            sparseMatrix.SetElementUnsafe(row, col, value * pRowScale[row] * pColumnScale[col]);
        }

        public void SetRHS(int row, double value)
        {
            if (row == -1) return;
            lp.SetRHS(row, value * arraySet.rowScale[row]);
        }

        public double GetRHS(int row)
        {
            if (row == -1) return double.PositiveInfinity;
            return lp.GetRHS(row) / arraySet.rowScale[row];
        }

        public void SetLHS(int row, double value)
        {
            if (row == -1) return;
            lp.SetLHS(row, value * arraySet.rowScale[row]);
        }

        public void SetRHSUnsafe(int row, double value)
        {
            if (row == -1) return;
            lp.SetRHS(row, value * pRowScale[row]);
        }

        public void SetLHSUnsafe(int row, double value)
        {
            if (row == -1) return;
            lp.SetLHS(row, value * pRowScale[row]);
        }

        public void SetCost(int col, double value)
        {
            if (col == -1) return;
            lp.SetCost(col, value * arraySet.rowScale[cRows] * arraySet.columnScale[col]);
        }

        public void SetCostUnsafe(int col, double value)
        {
            pCost[col] = value * pRowScale[cRows] * pColumnScale[col];
        }

        public void SetSpellDpsUnsafe(int col, double value)
        {
            arraySet._spellDps[col] = value * pRowScale[cRows] * pColumnScale[col];
        }

        public void SetColumnLowerBound(int col, double value)
        {
            lp.SetLowerBound(col, value / arraySet.columnScale[col]);
        }

        public void SetColumnUpperBound(int col, double value)
        {
            lp.SetUpperBound(col, value / arraySet.columnScale[col]);
        }

        //private int rowMaximizeSegment = -1;
        //private int rowColdsnap = -1;

        public void EraseColumn(int col)
        {
            if (col == -1) return;
            lp.SetUpperBound(col, 0.0);
            compactSolution = null;
            needsDual = true;
        }

        /*public void UpdateMaximizeSegmentColumn(int col)
        {
            if (col == -1) return;
            if (rowMaximizeSegment == -1) rowMaximizeSegment = lp.AddConstraint();
            lp.SetConstraintElement(rowMaximizeSegment, col, lp.GetConstraintElement(rowMaximizeSegment, col) - 1.0 * columnScale[col]);
            compactSolution = null;
            needsDual = true;
        }

        public void UpdateMaximizeSegmentDuration(double value)
        {
            if (rowMaximizeSegment == -1) rowMaximizeSegment = lp.AddConstraint();
            lp.SetConstraintRHS(rowMaximizeSegment, lp.GetConstraintRHS(rowMaximizeSegment) - value);
            compactSolution = null;
            needsDual = true;
        }

        public bool HasColdsnapConstraints
        {
            get
            {
                return rowColdsnap != -1;
            }
        }*/

        public int AddConstraint()
        {
            bool newConstraint;
            return lp.AddConstraint(null, out newConstraint);
        }

        public int AddConstraint(string name, out bool newConstraint)
        {
            return lp.AddConstraint(name, out newConstraint);
        }

        public void ReleaseConstraints()
        {
            lp.ReleaseConstraints();
        }

        public void SetConstraintRHS(int index, double value)
        {
            lp.SetConstraintRHS(index, value);
        }

        public void SetConstraintLHS(int index, double value)
        {
            lp.SetConstraintLHS(index, value);
        }

        public void SetConstraintElement(int index, int col, double value)
        {
            lp.SetConstraintElement(index, col, arraySet.columnScale[col] * value);
        }

        /*public void AddColdsnapConstraints(int segments)
        {
            if (rowColdsnap == -1)
            {
                rowColdsnap = lp.AddConstraint();
                for (int i = 1; i < segments; i++) lp.AddConstraint();
            }
        }

        public void UpdateColdsnapColumn(int index, int col)
        {
            compactSolution = null;
            needsDual = true;
            if (col == -1) return;
            lp.SetConstraintElement(rowColdsnap + index, col, 1.0 * columnScale[col]);
        }

        public void UpdateColdsnapDuration(int index, double value)
        {
            compactSolution = null;
            needsDual = true;
            lp.SetConstraintRHS(rowColdsnap + index, value);
        }*/

        private ArraySet arraySet;
        public ArraySet ArraySet
        {
            get
            {
                return arraySet;
            }
        }

        private void SolveInternal()
        {
            if (compactSolution == null)
            {
                if (needsQuadratic)
                {
                    compactSolution = lp.SolvePrimalQuadratic(mpsRow, sort, Qk, needsDual, maximizeColumn, targetValue, solver.CalculationOptions.GlobalRestarts);
                }
                else if (needsDual)
                {
                    //System.Diagnostics.Trace.WriteLine("Solving " + Log.ToString());
                    //System.Diagnostics.Debug.WriteLine("Solving H=" + HeroismHash.ToString("X") + ", AP=" + APHash.ToString("X") + ", IV=" + IVHash.ToString("X"));
                    compactSolution = lp.SolveDual(false);
                }
                else
                {
                    compactSolution = lp.SolvePrimal(false);
                    //compactSolution = lp.SolveDual(true);
                }
                UnscaleSolution();
            }
        }

        private void UnscaleSolution()
        {
            if (maximizeColumn == -1)
            {
                compactSolution[compactSolution.Length - 1] /= arraySet.rowScale[cRows];
            }
            for (int i = 0; i < compactSolution.Length - 1; i++)
            {
                compactSolution[i] *= arraySet.columnScale[i];
            }
        }

        public double[] Solve()
        {
            SolveInternal();
            return compactSolution;
        }

        public void SolvePrimalDual()
        {
            lp.SolvePrimal(true);
            //lp.SolveDual(true);
            compactSolution = lp.SolveDual(true);
            UnscaleSolution();
        }

        public void SolvePrimalQuadratic(int mpsRow, int[] sort, double k, bool skipLinear, int maximizeColumn, double targetValue)
        {
            this.mpsRow = mpsRow;
            this.sort = sort;
            this.Qk = k;
            this.maximizeColumn = maximizeColumn;
            this.targetValue = targetValue * arraySet.rowScale[cRows];
            compactSolution = lp.SolvePrimalQuadratic(mpsRow, sort, k, skipLinear, maximizeColumn, this.targetValue, solver.CalculationOptions.GlobalRestarts);
            UnscaleSolution();
            needsQuadratic = true;
        }

        public double Value
        {
            get
            {
                SolveInternal();
                return compactSolution[compactSolution.Length - 1];
            }
        }

        int IComparable<SolverLP>.CompareTo(SolverLP other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            if (compactSolution == null) return base.ToString();
            return "LP: " + compactSolution[compactSolution.Length - 1].ToString();
        }

        public string Listing
        {
            get
            {
                if (compactSolution == null) return "";
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < solver.SolutionVariable.Count; i++)
                {
                    if (compactSolution[i] > 0.01)
                    {
                        if (solver.SolutionVariable[i].Type == VariableType.Spell)
                        {
                            sb.AppendLine(String.Format("{2} {0}: {1:F}", solver.SolutionVariable[i].State.BuffLabel + "+" + solver.SolutionVariable[i].Cycle.Name, compactSolution[i], solver.SolutionVariable[i].Segment));
                        }
                        else
                        {
                            sb.AppendLine(String.Format("{2} {0}: {1:F}", solver.SolutionVariable[i].Type, compactSolution[i], solver.SolutionVariable[i].Segment));
                        }
                    }
                }
                return sb.ToString();
            }
        }
    }
}
