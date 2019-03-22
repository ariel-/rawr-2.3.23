using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
#if SILVERLIGHT
    public class LU
#else
    public unsafe class LU
#endif
    {
        private int size;
        public int etaSize;
        private ArraySet arraySet;

        // Ln...L0 B = P U Q
        // B = (Ln...L0)inv P U Q

        // Bx = b
        // (Ln...L0)inv P U Q x = b
        // P U Q x = (Ln...L0)b
        // U Q x = [P' (Ln...L0)b] = U z
        // Q x = z => x = Q' z

        // B~ = B + aj ep'
        // Ln...L0 B~ = P U~ Q = P U Q + Ln...L0 aj ep'
        // U~ = U + [P' Ln...L0 aj] ep' Q'

        // y B = z
        // y (Ln...L0)inv P U Q = z
        // [y (Ln...L0)inv P] U = z Q' = yy U
        // y (Ln...L0)inv P = yy
        // y = yy P' (Ln...L0)
        // y' = L0'...Ln' P yy'
        // Li' v = (I + eta ep')' v = v + ep (eta' v)

        // L1inv B = P1 U1 Q1
        // L1inv B = P2 U2 Q2
        // P2' L1inv B Q2' = U2
        // U3 = E U2 = E P2' L1inv B Q2'
        // [E P2'] L1inv B = [E U2] Q2 = U3 Q2 = U3 Q3
        // [P2 E P2'] L1inv B = P2 U3 Q3 = P3 U3 Q3
        // E = I + eta ep'
        // P2 E P2' = I + P2 eta ep' P2'
        // ep' P2' = (P2 ep)'

        // E = I + ep eta'
        // P E P' = I + P ep eta' P'
        // eta' P' = (P eta)'

#if SILVERLIGHT
        private double[] U;
        private double[] sL;
        private int[] P;
        private int[] Q;
        private int[] LJ;
        private int[] sLI;
        private int[] sLstart;
        private double[] c;
        private double[] c2;
#else
        private double* U;
        private double* sL;
        private int* P;
        private int* Q;
        private int* LJ;
        private int* sLI;
        private int* sLstart;
        private double* c;
        private double* c2;
#endif

#if SILVERLIGHT
        public void BeginSafe()
        {
            this.U = arraySet.LU_U;
            this.sL = arraySet.LUsparseL;
            this.P = arraySet.LU_P;
            this.Q = arraySet.LU_Q;
            this.LJ = arraySet.LU_LJ;
            this.sLI = arraySet.LUsparseLI;
            this.sLstart = arraySet.LUsparseLstart;
            this.c = arraySet.LUcolumn;
            this.c2 = arraySet.LUcolumn2;
        }
#else
        public void BeginUnsafe(double* U, double* sL, int* P, int* Q, int* LJ, int* sLI, int* sLstart, double* c, double* c2)
        {
            this.U = U;
            this.sL = sL;
            this.P = P;
            this.Q = Q;
            this.LJ = LJ;
            this.sLI = sLI;
            this.sLstart = sLstart;
            this.c = c;
            this.c2 = c2;
        }
#endif

        public void EndUnsafe()
        {
            this.U = null;
            this.sL = null;
            this.P = null;
            this.Q = null;
            this.LJ = null;
            this.sLI = null;
            this.sLstart = null;
            this.c = null;
            this.c2 = null;
        }

        public bool Singular { get; set; }
        public int Rank { get; set; }

        // data will be modified, if you need to retain it clean pass a clone
        public LU(int size, ArraySet arraySet)
        {
            Initialize(size, arraySet);
        }

        public void Initialize(int size, ArraySet arraySet)
        {
            this.size = size;
            this.arraySet = arraySet;
            etaSize = 0;
            if (size > arraySet.LUsizeMax)
            {
                arraySet.LUsizeMax = size;
                arraySet.RecreateLUArrays();
            }
        }

#if SILVERLIGHT
        public void BSolve(double[] b)
#else
        public unsafe void BSolve(double* b)
#endif
        {
            BSolveU(b, c, 0.00000001);
            BSolveL(c, b, 0.00000001);
        }


#if SILVERLIGHT
        public void BSolve(double[] b, double epsZero)
#else
        public unsafe void BSolve(double* b, double epsZero)
#endif
        {
            BSolveU(b, c, epsZero);
            BSolveL(c, b, epsZero);
        }

#if SILVERLIGHT
        public void BSolveU(double[] b, double[] c, double epsZero)
#else
        public unsafe void BSolveU(double* b, double* c, double epsZero)
#endif
        {
            int i, k;
            for (i = 0; i < size; i++)
            {
                c[i] = b[Q[i]]; // shuffle Q
            }
            for (k = 0; k < size; k++)
            {
                if (U[k * size + k] != 0 && Math.Abs(c[k]) > epsZero)
                {
                    c[k] /= U[k * size + k];
                    for (i = k + 1; i < size; i++)
                    {
                        c[i] -= c[k] * U[k * size + i];
                    }
                }
                else c[k] = 0; // value underspecified
            }
        }

#if SILVERLIGHT
        public void BSolveUnit(double[] b, int col)
#else
        public unsafe void BSolveUnit(double* b, int col)
#endif
        {
            BSolveUUnit(col, c);
            BSolveL(c, b, 0.00000001);
        }

#if SILVERLIGHT
        public void BSolveUUnit(int col, double[] c)
#else
        public unsafe void BSolveUUnit(int col, double* c)
#endif
        {
            int i, k;
            LP.Zero(c, size);
            for (i = 0; ; i++)
            {
                if (Q[i] == col)
                {
                    col = i;
                    break;
                }
            }
            //c[col] = 1.0;
            /*for (i = 0; i < size; i++)
            {
                c[i] = b[Q[i]]; // shuffle Q
            }*/

            if (U[col * size + col] != 0)
            {
                c[col] = 1.0 / U[col * size + col];
                for (i = col + 1; i < size; i++)
                {
                    c[i] -= c[col] * U[col * size + i];
                }
            }
            else return;
            //else c[col] = 0; // value underspecified

            for (k = col + 1; k < size; k++)
            {
                if (U[k * size + k] != 0 && Math.Abs(c[k]) > 0.000001)
                {
                    c[k] /= U[k * size + k];
                    for (i = k + 1; i < size; i++)
                    {
                        c[i] -= c[k] * U[k * size + i];
                    }
                }
                else c[k] = 0; // value underspecified
            }
        }

#if SILVERLIGHT
        public void BSolveL(double[] b, double[] c, double epsZero)
#else
        public unsafe void BSolveL(double* b, double* c, double epsZero)
#endif
        {
            int i, j;
            for (i = 0; i < size; i++)
            {
                c[P[i]] = b[i];
            }
            for (j = etaSize - 1; j >= 0; j--)
            {
                if (j < size)
                {
                    // eta columns from initial decomposition
                    int row = LJ[j]; // we're updating row element
                    // c~ = c + erow (eta' c)
                    double f = 0.0;
                    /*for (i = 0; i < size; i++)
                    {
                        f += c[i] * L[j * size + i];
                    }*/
                    int maxi = sLstart[j + 1];
                    for (i = sLstart[j]; i < maxi; i++)
                    {
                        f += c[sLI[i]] * sL[i];
                    }
                    c[row] += f;
                }
                else
                {
                    // eta rows from update
                    int row = LJ[j]; // we're updating using row, if element is zero we can skip
                    // c~ = c + eta (erow' c)
                    double f = c[row];
                    if (Math.Abs(f) >= epsZero)
                    {
                        int maxi = sLstart[j + 1];
                        for (i = sLstart[j]; i < maxi; i++)
                        {
                            c[sLI[i]] += f * sL[i];
                        }
                    }
                }
            }
        }

#if SILVERLIGHT
        public void FSolve(double[] b)
#else
        public unsafe void FSolve(double* b)
#endif
        {
            FSolveL(b, c, 0.00000001);
            FSolveU(c, b, 0.00000001);
        }

#if SILVERLIGHT
        public void FSolve(double[] b, double epsZero)
#else
        public unsafe void FSolve(double* b, double epsZero)
#endif
        {
            FSolveL(b, c, epsZero);
            FSolveU(c, b, epsZero);
        }

#if SILVERLIGHT
        public void FSolveU(double[] b, double[] c, double epsZero)
        {
            int i, k;
            for (i = 0; i < size; i++)
            {
                c2[i] = b[i];
            }
            for (k = size - 1; k >= 0; k--)
            {
                if (U[k * size + k] != 0 && Math.Abs(c2[k]) > epsZero)
                {
                    c2[k] /= U[k * size + k];
                    for (i = 0; i < k; i++)
                    {
                        c2[i] -= c2[k] * U[i * size + k];
                    }
                }
                else c2[k] = 0; // value underspecified
            }
            // shuffle Q
            for (i = 0; i < size; i++)
            {
                c[Q[i]] = c2[i];
            }
        }
#else
        private static unsafe void Copy(double* dest, double* source, int size)
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

        // hand optimized version of the above silverlight code
        public unsafe void FSolveU(double* b, double* c, double epsZero)
        {
            int size = this.size;
            double* U = this.U;
            double* c2 = this.c2;
            Copy(c2, b, size);
            double* c2i;
            int k = size - 1;
            if (k >= 0)
            {
                double* c2k = c2 + k;
                double* Ukk = U + ((size + 1) * k);
                long Ukkstep = -8 - 8 * size;
                do
                {
                    double div = *Ukk;
                    double c2kv = *c2k;
                    if (div != 0 && Math.Abs(c2kv) > epsZero)
                    {
                        c2kv /= div;
                        *c2k = c2kv;
                        c2i = c2;
                        double* Uik = U + k;
                        /*double* arr1 = c2 + (k & ~3);
                        if (c2i < arr1)
                        {
                            long off1 = size * 8;
                            long off2 = size * 16;
                            long off3 = size * 24;
                            long off4 = size * 32;
                            do
                            {
                                c2i[0] -= c2kv * Uik[0];
                                c2i[1] -= c2kv * *((double*)((byte*)Uik + off1));
                                c2i[2] -= c2kv * *((double*)((byte*)Uik + off2));
                                c2i[3] -= c2kv * *((double*)((byte*)Uik + off3));
                                c2i += 4;
                                Uik = (double*)((byte*)Uik + off4);
                            } while (c2i < arr1);
                        }*/
                        for (; c2i < c2k; c2i++, Uik += size)
                        {
                            *c2i -= c2kv * *Uik;
                        }
                    }
                    else
                    {
                        *c2k = 0; // value underspecified
                    }
                    Ukk = (double*)((byte*)Ukk + Ukkstep);
                    c2k--;
                    k--;
                } while (k >= 0);
            }
            // shuffle Q
            int* Qi = Q;
            int* Qend = Qi + size;
            c2i = c2;
            for (; Qi < Qend; Qi++, c2i++)
            {
                c[*Qi] = *c2i;
            }
        }
#endif

#if SILVERLIGHT
        public void FSolveL(double[] b, double[] c, double epsZero)
        {
            // perform all eta operations and finally apply row permutation P
            int i, j;
            for (j = 0; j < etaSize; j++)
            {
                if (j < size)
                {
                    // eta columns from initial decomposition
                    int row = LJ[j]; // we're updating using row, if element is zero we can skip
                    // b~ = b + eta (erow' b)
                    double f = b[row];
                    if (Math.Abs(f) >= epsZero)
                    {
                        /*for (i = 0; i < size; i++)
                        {
                            b[i] += f * L[j * size + i];
                        }*/
                        int maxi = sLstart[j + 1];
                        for (i = sLstart[j]; i < maxi; i++)
                        {
                            b[sLI[i]] += f * sL[i];
                        }
                    }
                }
                else
                {
                    // eta rows from updates
                    // b~ = b + erow (eta' b)
                    int row = LJ[j]; // we're updating row element
                    double f = 0.0;
                    int maxi = sLstart[j + 1];
                    for (i = sLstart[j]; i < maxi; i++)
                    {
                        f += b[sLI[i]] * sL[i];
                    }
                    b[row] += f;
                }
            }
            for (i = 0; i < size; i++)
            {
                c[i] = b[P[i]];
            }
        }
#else
        public unsafe void FSolveL(double* b, double* c, double epsZero)
        {
            int size = this.size;
            int* sLstartj = this.sLstart;
            double* sL = this.sL;
            int* sLI = this.sLI;
            int* LJj = this.LJ;
            int* LJsize = LJj + size;
            int* LJend = LJj + etaSize;
            // perform all eta operations and finally apply row permutation P
            int i;
            for (; LJj < LJsize; LJj++)
            {
                int row = *LJj;
                // eta columns from initial decomposition
                // we're updating using row, if element is zero we can skip
                // b~ = b + eta (erow' b)
                double f = b[row];
                int* sLstartjinc = sLstartj + 1;
                if (Math.Abs(f) >= epsZero)
                {
                    /*for (i = 0; i < size; i++)
                    {
                        b[i] += f * L[j * size + i];
                    }*/
                    int maxi = *sLstartjinc;
                    for (i = *sLstartj; i < maxi; i++)
                    {
                        b[sLI[i]] += f * sL[i];
                    }
                }
                sLstartj = sLstartjinc;
            }
            for (; LJj < LJend; LJj++)
            {
                int row = *LJj;
                // eta rows from updates
                // b~ = b + erow (eta' b)
                // we're updating row element
                double f = 0.0;
                int* sLstartjinc = sLstartj + 1;
                int maxi = *sLstartjinc;
                for (i = *sLstartj; i < maxi; i++)
                {
                    f += b[sLI[i]] * sL[i];
                }
                b[row] += f;
                sLstartj = sLstartjinc;
            }
            for (i = 0; i < size; i++)
            {
                c[i] = b[P[i]];
            }
        }
#endif

        // Performance Log, start, load Kavan, repeat dps time = 1
        // Primal=6.08148534180949E-05, Decompose=3.38125119182951E-05 implementation LU
        // Primal=7.7155968256824E-05, Decompose=4.80555577214333E-05  initial implementation LU2 with column pivoting
        // Primal=4.44975972839637E-05, Decompose=1.34763462504299E-05 LU2 no column pivoting
        // Primal=4.10642857990147E-05, Decompose=1.28461907883656E-05 FSolveL => sparse
        // Primal=3.96870122800117E-05, Decompose=1.36131169065829E-05 BSolveL => sparse
        // Primal=2.73956138996725E-05, Decompose=4.29207679646327E-06 update rule

        // SMP
        // Primal=0.000248019081795013, Decompose=7.31092948026856E-05 LU2 no column pivoting
        // Primal=0.000217759250424469, Decompose=7.53546090376799E-05 sparse L
        // Primal=0.00013135285568704, Decompose=1.44506264553419E-05 update rule

        public void Decompose()
        {
            etaSize = 0; // reset eta file
            //Array.Clear(_L, 0, _L.Length);
            Singular = false;
            Rank = size;

            int i, j, k, pivi, pivj;
            // init P, Q
            for (i = 0; i < size; i++)
            {
                P[i] = i;
                Q[i] = i;
            }

            // eliminate by columns
            for (j = 0; j < size; j++)
            {
                // select pivot
                pivi = j;
                pivj = j;
                double max = Math.Abs(U[pivi * size + pivj]);
                double newmax;

                for (i = j; i < size; i++)
                {
                    if ((newmax = Math.Abs(U[i * size + j])) > max)
                    {
                        pivi = i;
                        pivj = j;
                        max = newmax;
                    }
                }

                if (max < 0.1)
                {
                    // don't allow a 0 if you can help it, even if it costs more to pivot columns
                    for (k = j + 1; k < size; k++)
                    {
                        for (i = j; i < size; i++)
                        {
                            if ((newmax = Math.Abs(U[i * size + k])) > max)
                            {
                                pivi = i;
                                pivj = k;
                                max = newmax;
                            }
                        }
                    }
                }

                if (pivi != j)
                {
                    for (k = j; k < size; k++) // columns before j have zeros in these rows
                    {
                        double t = U[pivi * size + k];
                        U[pivi * size + k] = U[j * size + k];
                        U[j * size + k] = t;
                    }

                    int tmp = P[pivi];
                    P[pivi] = P[j];
                    P[j] = tmp;
                }

                if (pivj != j)
                {
                    for (k = 0; k < size; k++)
                    {
                        double t = U[k * size + pivj];
                        U[k * size + pivj] = U[k * size + j];
                        U[k * size + j] = t;
                    }

                    int tmp = Q[pivj];
                    Q[pivj] = Q[j];
                    Q[j] = tmp;
                }

                // eliminate, construct eta vector

                if (etaSize >= arraySet.LUetaMax) throw new InvalidOperationException();

                if (!Singular && Math.Abs(max) < 0.000001)
                {
                    Singular = true;
                    Rank = j;
                }

                sLstart[etaSize + 1] = sLstart[etaSize];
                double a = 1 / U[j * size + j];
                for (i = j + 1; i < size; i++)
                {
                    int pi = P[i];
                    double f = -U[i * size + j] * a;
                    U[i * size + j] = 0;
                    if (Math.Abs(f) > 0.00000001)
                    {
                        //L[etaSize * size + pi] = f; // eta element that eliminates element in row i
                        // sparse L construction
                        sL[sLstart[etaSize + 1]] = f;
                        sLI[sLstart[etaSize + 1]] = pi;
                        sLstart[etaSize + 1]++;
                        // update U
                        for (k = j + 1; k < size; k++)
                        {
                            U[i * size + k] += f * U[j * size + k];
                        }
                    }
                }
                LJ[etaSize] = P[j]; // yes I had to guess, but I used unit test to make sure it works :)
                etaSize++;
            }
        }

        // B~ = B + aj ecol'
        // Ln...L0 B~ = P U~ Q = P U Q + Ln...L0 aj ecol'
        // U~ = U + [P' Ln...L0 aj] ecol' Q'
        // a = [P' Ln...L0 aj]

        // replace column col in basis B with aj
#if SILVERLIGHT
        public bool Update(double[] a, int col, out double pivot)
        {
            int i, j, k;
            for (j = 0; j < size; j++)
            {
                if (Q[j] == col) break;
                //c[i] = ecol[Q[i]]; // shuffle Q
            }
            col = j;

            double ujj = U[j * size + j];

            // place in column col = j
            // rotate columns to get upper hessenberg form (col...lastnz)
            int lastnz = -1;
            for (i = 0; i < size; i++)
            {
                if (Math.Abs(a[i]) > 0.000001) lastnz = i;
            }

            if (lastnz < col)
            {
                Singular = true;
                pivot = 0.0;
                return false;
            }

            // XXaXXXX    XXXXXaX
            //  XaXXXX     XXXXaX
            //   aXXXX      XXXaX   <--- col
            //   aXXXX  =>  XXXaX
            //   a XXX       XXaX
            //   a  XX        XaX   <--- lastnz
            //       X          X

            int Qcol = Q[col];
            for (j = col; j < lastnz; j++)
            {
                Q[j] = Q[j + 1];
                for (i = 0; i <= j + 1; i++)
                {
                    U[i * size + j] = U[i * size + j + 1];
                }                        
            }
            Q[lastnz] = Qcol;
            for (i = 0; i <= lastnz; i++)
            {
                U[i * size + lastnz] = a[i];
            }

            // eliminate row at index col up to lastnz using rows below it
            // we're eliminating on previous diagonals, so we know it is safe (we shouldn't update on a singular basis)

            // E = I + ep eta'
            // P E P' = I + P ep eta' P'
            // eta' P' = (P eta)'

            if (etaSize >= arraySet.LUetaMax) throw new InvalidOperationException();

            sLstart[etaSize + 1] = sLstart[etaSize];

            for (j = col; j < lastnz; j++)
            {
                int pj = P[j + 1];
                double f = -U[col * size + j] / U[(j + 1) * size + j];
                U[col * size + j] = 0;
                if (Math.Abs(f) > 0.00000001)
                {
                    // sparse L construction
                    sL[sLstart[etaSize + 1]] = f;
                    sLI[sLstart[etaSize + 1]] = pj;
                    sLstart[etaSize + 1]++;
                    // update U
                    for (k = j + 1; k < size; k++)
                    {
                        U[col * size + k] += f * U[(j + 1) * size + k];
                    }
                }
            }
            LJ[etaSize] = P[col];
            etaSize++;

            // rotate rows to get upper triangular form (col...lastnz)

            // XXXXXXX    XXXXXXX
            //  XXXXXX     XXXXXX
            //      XX      XXXXX   <--- col
            //   XXXXX  =>   XXXX
            //    XXXX        XXX
            //     XXX         XX   <--- lastnz
            //       X          X

            // store col in temp
            int Pcol = P[col];
            for (j = lastnz; j < size; j++)
            {
                c[j] = U[col * size + j];
            }
            for (i = col; i < lastnz; i++)
            {
                P[i] = P[i + 1];
                for (j = (i == 0 ? 0 : i - 1); j < size; j++)
                {
                    U[i * size + j] = U[(i + 1) * size + j];
                }
            }
            P[lastnz] = Pcol;
            if (lastnz > 0) U[lastnz * size + lastnz - 1] = 0.0;
            for (j = lastnz; j < size; j++)
            {
                U[lastnz * size + j] = c[j];
            }
            pivot = c[lastnz] / ujj;
            if (Math.Abs(U[lastnz * size + lastnz]) < 0.000001)
            {
                Singular = true;
            }
            return true;
        }
#else
        public unsafe bool Update(double* a, int col, out double pivot)
        {
            int i, j, k;
            int* Q = this.Q;
            double* U = this.U;
            for (j = 0; ; j++)
            {
                if (Q[j] == col) break;
            }
            col = j;

            double* Ujj = U + (j * size + j);
            double ujj = *Ujj;

            // place in column col = j
            // rotate columns to get upper hessenberg form (col...lastnz)
            int lastnz = size - 1;
            for (; lastnz >= 0; lastnz--)
            {
                if (Math.Abs(a[lastnz]) > 0.000001) break;
            }

            if (lastnz < col)
            {
                pivot = 0.0;
                return false;
            }
            
            // XXaXXXX    XXXXXaX
            //  XaXXXX     XXXXaX
            //   aXXXX      XXXaX   <--- col
            //   aXXXX  =>  XXXaX
            //   a XXX       XXaX
            //   a  XX        XaX   <--- lastnz
            //       X          X

            int* Qj = Q + col;
            int Qcol = *Qj;
            int* Qlastnz = Q + lastnz;
            for (; Qj < Qlastnz; Qj++)
            {
                *Qj = Qj[1];
            }
            *Qlastnz = Qcol;

            double* Uijend = U + lastnz;
            double* Uijendcol = Uijend + col * size;
            double* Uijendlast = Uijend + lastnz * size;
            int dif = col - lastnz;
            for (; Uijend <= Uijendcol; Uijend += size, a++)
            {
                //double* Uij = U + (i * size + col);
                //double* Uijend = U + (i * size + lastnz);
                for (double* Uij = Uijend + dif; Uij < Uijend; Uij++)
                {
                    *Uij = Uij[1];
                }
                *Uijend = *a;
            }
            for (; Uijend <= Uijendlast; Uijend += size, dif++, a++)
            {
                for (double* Uij = Uijend + dif; Uij < Uijend; Uij++)
                {
                    *Uij = Uij[1];
                }
                *Uijend = *a;
            }

            // eliminate row at index col up to lastnz using rows below it
            // we're eliminating on previous diagonals, so we know it is safe (we shouldn't update on a singular basis)

            // E = I + ep eta'
            // P E P' = I + P ep eta' P'
            // eta' P' = (P eta)'

            if (etaSize >= arraySet.LUetaMax) throw new InvalidOperationException();

            sLstart[etaSize + 1] = sLstart[etaSize];

            for (j = col; j < lastnz; j++)
            {
                int pj = P[j + 1];
                double f = -U[col * size + j] / U[(j + 1) * size + j];
                U[col * size + j] = 0;
                if (Math.Abs(f) > 0.00000001)
                {
                    // sparse L construction
                    sL[sLstart[etaSize + 1]] = f;
                    sLI[sLstart[etaSize + 1]] = pj;
                    sLstart[etaSize + 1]++;
                    // update U
                    for (k = j + 1; k < size; k++)
                    {
                        U[col * size + k] += f * U[(j + 1) * size + k];
                    }
                }
            }
            LJ[etaSize] = P[col];
            etaSize++;

            // rotate rows to get upper triangular form (col...lastnz)

            // XXXXXXX    XXXXXXX
            //  XXXXXX     XXXXXX
            //      XX      XXXXX   <--- col
            //   XXXXX  =>   XXXX
            //    XXXX        XXX
            //     XXX         XX   <--- lastnz
            //       X          X

            // store col in temp
            int Pcol = P[col];
            double* cj = c + lastnz;
            double* cend = c + size;
            long diff = (byte*)(U + (col * size)) - (byte*)c;
            for (; cj < cend; cj++)
            {
                *cj = *(double*)((byte*)cj + diff);
            }
            for (i = col; i < lastnz; i++)
            {
                P[i] = P[i + 1];
                Uijend = U + ((i + 1) * size);
                double* Uij = Uijend - size + i - 1;
                if (i == 0)
                {
                    Uij++;
                }
                for (; Uij < Uijend; Uij++)
                {
                    *Uij = Uij[size];
                }
            }
            P[lastnz] = Pcol;
            if (lastnz > 0) U[lastnz * size + lastnz - 1] = 0.0;
            for (j = lastnz; j < size; j++)
            {
                U[lastnz * size + j] = c[j];
            }
            pivot = c[lastnz] / ujj;
            if (Math.Abs(U[lastnz * size + lastnz]) < 0.000001)
            {
                Singular = true;
            }
            else
            {
                Singular = false;
            }
            return true;
        }
#endif
    }
}
