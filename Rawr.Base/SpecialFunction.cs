using System;
using System.IO;
/*
	**************************************************************************
	**
	**    Class  SpecialFunction (C#)
	**
	**************************************************************************
	**    Copyright (C) 1984 Stephen L. Moshier (original C version - Cephes Math Library)
	**    Copyright (C) 1996 Leigh Brookshaw	(Java version)
	**    Copyright (C) 2005 Miroslav Stampar	(C# version [->this<-])
	**
	**    This program is free software; you can redistribute it and/or modify
	**    it under the terms of the GNU General Public License as published by
	**    the Free Software Foundation; either version 2 of the License, or
	**    (at your option) any later version.
	**
	**    This program is distributed in the hope that it will be useful,
	**    but WITHOUT ANY WARRANTY; without even the implied warranty of
	**    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	**    GNU General Public License for more details.
	**
	**    You should have received a copy of the GNU General Public License
	**    along with this program; if not, write to the Free Software
	**    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
	**************************************************************************
	**
	**    This class is an extension of System.Math. It includes a number
	**    of special functions not found in the Math class.
	**
	*************************************************************************/


/**
 * This class contains physical constants and special functions not found
 * in the System.Math class.
 * Like the System.Math class this class is final and cannot be
 * subclassed.
 * All physical constants are in cgs units.
 * NOTE: These special functions do not necessarily use the fastest
 * or most accurate algorithms.
 *
 * @version $Revision: 1.8 $, $Date: 2005/09/12 09:52:34 $
 */

// Adopted from the version available at http://www.codeproject.com/KB/cs/SpecialFunction.aspx
// using more of the .NET provided versions where available and changed to .NET naming guidelines
// most functions optimized for performance characteristics of .NET platform
// added interpolation methods for Ibeta function
// Kavan@Rawr

namespace Rawr
{
    public static class SpecialFunction
    {
        // Machine constants

        public static double MACHEP = 5.9604645E-8; // 1.11022302462515654042E-16
        private const double MAXLOG = 7.09782712893383996732E2;
        private const double MINLOG = -7.451332191019412076235E2;
        private const double MAXGAM = 171.624376956302725;
        private const double SQTPI = 2.50662827463100050242E0;
        private const double SQRTH = 7.07106781186547524401E-1;
        private const double LOGPI = 1.14472988584940017414;


        /// <summary>
        /// Returns the hyperbolic arc cosine of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Acosh(double x)
        {
            if (x < 1.0) throw new ArithmeticException("range exception");
            return Math.Log(x + Math.Sqrt(x * x - 1));
        }


        /// <summary>
        /// Returns the hyperbolic arc sine of the specified number.
        /// </summary>
        /// <param name="xx"></param>
        /// <returns></returns>
        public static double Asinh(double xx)
        {
            double x;
            int sign;
            if (xx == 0.0) return xx;
            if (xx < 0.0)
            {
                sign = -1;
                x = -xx;
            }
            else
            {
                sign = 1;
                x = xx;
            }
            return sign * Math.Log(x + Math.Sqrt(x * x + 1));
        }


        /// <summary>
        /// Returns the hyperbolic arc tangent of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Atanh(double x)
        {
            if (x > 1.0 || x < -1.0) throw
                                         new ArithmeticException("range exception");
            return 0.5 * Math.Log((1.0 + x) / (1.0 - x));
        }


        /// <summary>
        /// Returns the Bessel function of order 0 of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double J0(double x)
        {
            double ax;

            if ((ax = Math.Abs(x)) < 8.0)
            {
                double y = x * x;
                double ans1 = 57568490574.0 + y * (-13362590354.0 + y * (651619640.7
                    + y * (-11214424.18 + y * (77392.33017 + y * (-184.9052456)))));
                double ans2 = 57568490411.0 + y * (1029532985.0 + y * (9494680.718
                    + y * (59272.64853 + y * (267.8532712 + y * 1.0))));

                return ans1 / ans2;

            }
            else
            {
                double z = 8.0 / ax;
                double y = z * z;
                double xx = ax - 0.785398164;
                double ans1 = 1.0 + y * (-0.1098628627e-2 + y * (0.2734510407e-4
                    + y * (-0.2073370639e-5 + y * 0.2093887211e-6)));
                double ans2 = -0.1562499995e-1 + y * (0.1430488765e-3
                    + y * (-0.6911147651e-5 + y * (0.7621095161e-6
                    - y * 0.934935152e-7)));

                return Math.Sqrt(0.636619772 / ax) *
                    (Math.Cos(xx) * ans1 - z * Math.Sin(xx) * ans2);
            }
        }


        /// <summary>
        /// Returns the Bessel function of order 1 of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double J1(double x)
        {
            double ax;
            double y;
            double ans1, ans2;

            if ((ax = Math.Abs(x)) < 8.0)
            {
                y = x * x;
                ans1 = x * (72362614232.0 + y * (-7895059235.0 + y * (242396853.1
                    + y * (-2972611.439 + y * (15704.48260 + y * (-30.16036606))))));
                ans2 = 144725228442.0 + y * (2300535178.0 + y * (18583304.74
                    + y * (99447.43394 + y * (376.9991397 + y * 1.0))));
                return ans1 / ans2;
            }
            else
            {
                double z = 8.0 / ax;
                double xx = ax - 2.356194491;
                y = z * z;

                ans1 = 1.0 + y * (0.183105e-2 + y * (-0.3516396496e-4
                    + y * (0.2457520174e-5 + y * (-0.240337019e-6))));
                ans2 = 0.04687499995 + y * (-0.2002690873e-3
                    + y * (0.8449199096e-5 + y * (-0.88228987e-6
                    + y * 0.105787412e-6)));
                double ans = Math.Sqrt(0.636619772 / ax) *
                    (Math.Cos(xx) * ans1 - z * Math.Sin(xx) * ans2);
                if (x < 0.0) ans = -ans;
                return ans;
            }
        }


        /// <summary>
        /// Returns the Bessel function of order n of the specified number.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Jn(int n, double x)
        {
            int j, m;
            double ax, bj, bjm, bjp, sum, tox, ans;
            bool jsum;

            double ACC = 40.0;
            double BIGNO = 1.0e+10;
            double BIGNI = 1.0e-10;

            if (n == 0) return J0(x);
            if (n == 1) return J1(x);

            ax = Math.Abs(x);
            if (ax == 0.0) return 0.0;
            else if (ax > (double)n)
            {
                tox = 2.0 / ax;
                bjm = J0(ax);
                bj = J1(ax);
                for (j = 1; j < n; j++)
                {
                    bjp = j * tox * bj - bjm;
                    bjm = bj;
                    bj = bjp;
                }
                ans = bj;
            }
            else
            {
                tox = 2.0 / ax;
                m = 2 * ((n + (int)Math.Sqrt(ACC * n)) / 2);
                jsum = false;
                bjp = ans = sum = 0.0;
                bj = 1.0;
                for (j = m; j > 0; j--)
                {
                    bjm = j * tox * bj - bjp;
                    bjp = bj;
                    bj = bjm;
                    if (Math.Abs(bj) > BIGNO)
                    {
                        bj *= BIGNI;
                        bjp *= BIGNI;
                        ans *= BIGNI;
                        sum *= BIGNI;
                    }
                    if (jsum) sum += bj;
                    jsum = !jsum;
                    if (j == n) ans = bjp;
                }
                sum = 2.0 * sum - bj;
                ans /= sum;
            }
            return x < 0.0 && n % 2 == 1 ? -ans : ans;
        }


        /// <summary>
        /// Returns the Bessel function of the second kind, of order 0 of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Y0(double x)
        {
            if (x < 8.0)
            {
                double y = x * x;

                double ans1 = -2957821389.0 + y * (7062834065.0 + y * (-512359803.6
                    + y * (10879881.29 + y * (-86327.92757 + y * 228.4622733))));
                double ans2 = 40076544269.0 + y * (745249964.8 + y * (7189466.438
                    + y * (47447.26470 + y * (226.1030244 + y * 1.0))));

                return (ans1 / ans2) + 0.636619772 * J0(x) * Math.Log(x);
            }
            else
            {
                double z = 8.0 / x;
                double y = z * z;
                double xx = x - 0.785398164;

                double ans1 = 1.0 + y * (-0.1098628627e-2 + y * (0.2734510407e-4
                    + y * (-0.2073370639e-5 + y * 0.2093887211e-6)));
                double ans2 = -0.1562499995e-1 + y * (0.1430488765e-3
                    + y * (-0.6911147651e-5 + y * (0.7621095161e-6
                    + y * (-0.934945152e-7))));
                return Math.Sqrt(0.636619772 / x) *
                    (Math.Sin(xx) * ans1 + z * Math.Cos(xx) * ans2);
            }
        }


        /// <summary>
        /// Returns the Bessel function of the second kind, of order 1 of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Y1(double x)
        {
            if (x < 8.0)
            {
                double y = x * x;
                double ans1 = x * (-0.4900604943e13 + y * (0.1275274390e13
                    + y * (-0.5153438139e11 + y * (0.7349264551e9
                    + y * (-0.4237922726e7 + y * 0.8511937935e4)))));
                double ans2 = 0.2499580570e14 + y * (0.4244419664e12
                    + y * (0.3733650367e10 + y * (0.2245904002e8
                    + y * (0.1020426050e6 + y * (0.3549632885e3 + y)))));
                return (ans1 / ans2) + 0.636619772 * (J1(x) * Math.Log(x) - 1.0 / x);
            }
            else
            {
                double z = 8.0 / x;
                double y = z * z;
                double xx = x - 2.356194491;
                double ans1 = 1.0 + y * (0.183105e-2 + y * (-0.3516396496e-4
                    + y * (0.2457520174e-5 + y * (-0.240337019e-6))));
                double ans2 = 0.04687499995 + y * (-0.2002690873e-3
                    + y * (0.8449199096e-5 + y * (-0.88228987e-6
                    + y * 0.105787412e-6)));
                return Math.Sqrt(0.636619772 / x) *
                    (Math.Sin(xx) * ans1 + z * Math.Cos(xx) * ans2);
            }
        }


        /// <summary>
        /// Returns the Bessel function of the second kind, of order n of the specified number.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Yn(int n, double x)
        {
            double by, bym, byp, tox;

            if (n == 0) return Y0(x);
            if (n == 1) return Y1(x);

            tox = 2.0 / x;
            by = Y1(x);
            bym = Y0(x);
            for (int j = 1; j < n; j++)
            {
                byp = j * tox * by - bym;
                bym = by;
                by = byp;
            }
            return by;
        }


        /// <summary>
        /// Returns the factorial of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Fac(double x)
        {
            double d = Math.Abs(x);

            // TODO: Using integer version for large parameter values (like 20) may result in overflow
            // however double could store many such values with just some loss of precision.

            if (Math.Floor(d) == d) return (double)Fac((int)x);
            else return Gamma(x + 1.0);
        }


        /// <summary>
        /// Returns the factorial of the specified number.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public static int Fac(int j)
        {
            // TODO: Unchecked arithmetical operations can easily overflow here.

            int i = j;
            int d = 1;
            if (j < 0) i = Math.Abs(j);
            while (i > 1)
            {
                d *= i--;
            }
            if (j < 0) return -d;
            else return d;
        }


        /// <summary>
        /// Returns the gamma function of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Gamma(double x)
        {
            double p, z;

            double q = Math.Abs(x);

            if (q > 33.0)
            {
                if (x < 0.0)
                {
                    p = Math.Floor(q);
                    if (p == q) throw new ArithmeticException("gamma: overflow");
                    //int i = (int)p;
                    z = q - p;
                    if (z > 0.5)
                    {
                        p += 1.0;
                        z = q - p;
                    }
                    z = q * Math.Sin(Math.PI * z);
                    if (z == 0.0) throw new ArithmeticException("gamma: overflow");
                    z = Math.Abs(z);
                    z = Math.PI / (z * Stirf(q));

                    return -z;
                }
                else
                {
                    return Stirf(x);
                }
            }

            z = 1.0;
            while (x >= 3.0)
            {
                x -= 1.0;
                z *= x;
            }

            while (x < 0.0)
            {
                if (x == 0.0)
                {
                    throw new ArithmeticException("gamma: singular");
                }
                else if (x > -1.0E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }
                z /= x;
                x += 1.0;
            }

            while (x < 2.0)
            {
                if (x == 0.0)
                {
                    throw new ArithmeticException("gamma: singular");
                }
                else if (x < 1.0E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }
                z /= x;
                x += 1.0;
            }

            if ((x == 2.0) || (x == 3.0)) return z;

            x -= 2.0;
            //p = Polevl(x, P, 6);
            //double[] P = {
            //             1.60119522476751861407E-4,
            //             1.19135147006586384913E-3,
            //             1.04213797561761569935E-2,
            //             4.76367800457137231464E-2,
            //             2.07448227648435975150E-1,
            //             4.94214826801497100753E-1,
            //             9.99999999999999996796E-1
            //         };
            p = 1.60119522476751861407E-4;
            p = p * x + 1.19135147006586384913E-3;
            p = p * x + 1.04213797561761569935E-2;
            p = p * x + 4.76367800457137231464E-2;
            p = p * x + 2.07448227648435975150E-1;
            p = p * x + 4.94214826801497100753E-1;
            p = p * x + 9.99999999999999996796E-1;
            //q = Polevl(x, Q, 7);
            //double[] Q = {
            //             -2.31581873324120129819E-5,
            //             5.39605580493303397842E-4,
            //             -4.45641913851797240494E-3,
            //             1.18139785222060435552E-2,
            //             3.58236398605498653373E-2,
            //             -2.34591795718243348568E-1,
            //             7.14304917030273074085E-2,
            //             1.00000000000000000320E0
            //         };
            q = -2.31581873324120129819E-5;
            q = q * x + 5.39605580493303397842E-4;
            q = q * x + -4.45641913851797240494E-3;
            q = q * x + 1.18139785222060435552E-2;
            q = q * x + 3.58236398605498653373E-2;
            q = q * x + -2.34591795718243348568E-1;
            q = q * x + 7.14304917030273074085E-2;
            q = q * x + 1.00000000000000000320E0;
            return z * p / q;

        }


        /// <summary>
        /// Return the gamma function computed by Stirling's formula.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double Stirf(double x)
        {
            //double[] STIR = {
            //                7.87311395793093628397E-4,
            //                -2.29549961613378126380E-4,
            //                -2.68132617805781232825E-3,
            //                3.47222221605458667310E-3,
            //                8.33333333333482257126E-2,
            //};
            const double MAXSTIR = 143.01608;

            double w = 1.0 / x;
            double y = Math.Exp(x);

            //w = 1.0 + w * Polevl(w, STIR, 4);

            double ans = 7.87311395793093628397E-4;
            ans = ans * w + -2.29549961613378126380E-4;
            ans = ans * w + -2.68132617805781232825E-3;
            ans = ans * w + 3.47222221605458667310E-3;
            ans = ans * w + 8.33333333333482257126E-2;

            w = 1.0 + w * ans;

            if (x > MAXSTIR)
            {
                /* Avoid overflow in Math.Pow() */
                double v = Math.Pow(x, 0.5 * x - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = Math.Pow(x, x - 0.5) / y;
            }
            y = SQTPI * y * w;
            return y;
        }


        /// <summary>
        /// Returns the complemented incomplete gamma function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Igamc(double a, double x)
        {
            double big = 4.503599627370496e15;
            double biginv = 2.22044604925031308085e-16;
            double ans, ax, c, yc, r, t, y, z;
            double pk, pkm1, pkm2, qk, qkm1, qkm2;

            if (x <= 0 || a <= 0) return 1.0;

            if (x < 1.0 || x < a) return 1.0 - Igam(a, x);

            ax = a * Math.Log(x) - x - LogGamma(a);
            if (ax < -MAXLOG) return 0.0;

            ax = Math.Exp(ax);

            /* continued fraction */
            y = 1.0 - a;
            z = x + y + 1.0;
            c = 0.0;
            pkm2 = 1.0;
            qkm2 = x;
            pkm1 = x + 1.0;
            qkm1 = z * x;
            ans = pkm1 / qkm1;

            do
            {
                c += 1.0;
                y += 1.0;
                z += 2.0;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if (qk != 0)
                {
                    r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (Math.Abs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
            } while (t > MACHEP);

            return ans * ax;
        }


        /// <summary>
        /// Returns the incomplete gamma function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Igam(double a, double x)
        {
            double ans, ax, c, r;

            if (x <= 0 || a <= 0) return 0.0;

            if (x > 1.0 && x > a) return 1.0 - Igamc(a, x);

            /* Compute  x**a * exp(-x) / gamma(a)  */
            ax = a * Math.Log(x) - x - LogGamma(a);
            if (ax < -MAXLOG) return (0.0);

            ax = Math.Exp(ax);

            /* power series */
            r = a;
            c = 1.0;
            ans = 1.0;

            do
            {
                r += 1.0;
                c *= x / r;
                ans += c;
            } while (c / ans > MACHEP);

            return (ans * ax / a);

        }


        /**
             * Returns the area under the left hand tail (from 0 to x)
             * of the Chi square probability density function with
             * v degrees of freedom.
             **/

        /// <summary>
        /// Returns the chi-square function (left hand tail).
        /// </summary>
        /// <param name="df">degrees of freedom</param>
        /// <param name="x">double value</param>
        /// <returns></returns>
        public static double Chisq(double df, double x)
        {
            if (x < 0.0 || df < 1.0) return 0.0;

            return Igam(df / 2.0, x / 2.0);

        }


        /**
             * Returns the area under the right hand tail (from x to
             * infinity) of the Chi square probability density function
             * with v degrees of freedom:
             **/

        /// <summary>
        /// Returns the chi-square function (right hand tail).
        /// </summary>
        /// <param name="df">degrees of freedom</param>
        /// <param name="x">double value</param>
        /// <returns></returns>
        public static double Chisqc(double df, double x)
        {
            if (x < 0.0 || df < 1.0) return 0.0;

            return Igamc(df / 2.0, x / 2.0);

        }


        /// <summary>
        /// Returns the sum of the first k terms of the Poisson distribution.
        /// </summary>
        /// <param name="k">number of terms</param>
        /// <param name="x">double value</param>
        /// <returns></returns>
        public static double Poisson(int k, double x)
        {
            if (k < 0 || x < 0) return 0.0;

            return Igamc((double)(k + 1), x);
        }


        /// <summary>
        /// Returns the sum of the terms k+1 to infinity of the Poisson distribution.
        /// </summary>
        /// <param name="k">start</param>
        /// <param name="x">double value</param>
        /// <returns></returns>
        public static double Poissonc(int k, double x)
        {
            if (k < 0 || x < 0) return 0.0;

            return Igam((double)(k + 1), x);
        }


        /// <summary>
        /// Returns the area under the Gaussian probability density function, integrated from minus infinity to a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Normal(double a)
        {
            double x, y, z;

            x = a * SQRTH;
            z = Math.Abs(x);

            if (z < SQRTH) y = 0.5 + 0.5 * Erf(x);
            else
            {
                y = 0.5 * Erfc(z);
                if (x > 0) y = 1.0 - y;
            }

            return y;
        }


        /// <summary>
        /// Returns the complementary error function of the specified number.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Erfc(double a)
        {
            double x, y, z, p, q;

            //double[] P = {
            //             2.46196981473530512524E-10,
            //             5.64189564831068821977E-1,
            //             7.46321056442269912687E0,
            //             4.86371970985681366614E1,
            //             1.96520832956077098242E2,
            //             5.26445194995477358631E2,
            //             9.34528527171957607540E2,
            //             1.02755188689515710272E3,
            //             5.57535335369399327526E2
            //         };
            //double[] Q = {
            //             //1.0
            //             1.32281951154744992508E1,
            //             8.67072140885989742329E1,
            //             3.54937778887819891062E2,
            //             9.75708501743205489753E2,
            //             1.82390916687909736289E3,
            //             2.24633760818710981792E3,
            //             1.65666309194161350182E3,
            //             5.57535340817727675546E2
            //         };

            //double[] R = {
            //             5.64189583547755073984E-1,
            //             1.27536670759978104416E0,
            //             5.01905042251180477414E0,
            //             6.16021097993053585195E0,
            //             7.40974269950448939160E0,
            //             2.97886665372100240670E0
            //         };
            //double[] S = {
            //             //1.00000000000000000000E0, 
            //             2.26052863220117276590E0,
            //             9.39603524938001434673E0,
            //             1.20489539808096656605E1,
            //             1.70814450747565897222E1,
            //             9.60896809063285878198E0,
            //             3.36907645100081516050E0
            //         };

            if (a < 0.0) x = -a;
            else x = a;

            if (x < 1.0) return 1.0 - Erf(a);

            z = -a * a;

            if (z < -MAXLOG)
            {
                if (a < 0) return (2.0);
                else return (0.0);
            }

            z = Math.Exp(z);

            if (x < 8.0)
            {
                //p = Polevl(x, P, 8);
                p = 2.46196981473530512524E-10;
                p = p * x + 5.64189564831068821977E-1;
                p = p * x + 7.46321056442269912687E0;
                p = p * x + 4.86371970985681366614E1;
                p = p * x + 1.96520832956077098242E2;
                p = p * x + 5.26445194995477358631E2;
                p = p * x + 9.34528527171957607540E2;
                p = p * x + 1.02755188689515710272E3;
                p = p * x + 5.57535335369399327526E2;
                //q = P1evl(x, Q, 8);
                q = x + 1.32281951154744992508E1;
                q = q * x + 8.67072140885989742329E1;
                q = q * x + 3.54937778887819891062E2;
                q = q * x + 9.75708501743205489753E2;
                q = q * x + 1.82390916687909736289E3;
                q = q * x + 2.24633760818710981792E3;
                q = q * x + 1.65666309194161350182E3;
                q = q * x + 5.57535340817727675546E2;
            }
            else
            {
                //p = Polevl(x, R, 5);
                p = 5.64189583547755073984E-1;
                p = p * x + 1.27536670759978104416E0;
                p = p * x + 5.01905042251180477414E0;
                p = p * x + 6.16021097993053585195E0;
                p = p * x + 7.40974269950448939160E0;
                p = p * x + 2.97886665372100240670E0;
                //q = P1evl(x, S, 6);
                q = x + 2.26052863220117276590E0;
                q = q * x + 9.39603524938001434673E0;
                q = q * x + 1.20489539808096656605E1;
                q = q * x + 1.70814450747565897222E1;
                q = q * x + 9.60896809063285878198E0;
                q = q * x + 3.36907645100081516050E0;
            }

            y = (z * p) / q;

            if (a < 0) y = 2.0 - y;

            if (y == 0.0)
            {
                if (a < 0) return 2.0;
                else return (0.0);
            }


            return y;
        }


        /// <summary>
        /// Returns the error function of the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Erf(double x)
        {
            double y, z;
            //double[] T = {
            //             9.60497373987051638749E0,
            //             9.00260197203842689217E1,
            //             2.23200534594684319226E3,
            //             7.00332514112805075473E3,
            //             5.55923013010394962768E4
            //         };
            //double[] U = {
            //             //1.00000000000000000000E0,
            //             3.35617141647503099647E1,
            //             5.21357949780152679795E2,
            //             4.59432382970980127987E3,
            //             2.26290000613890934246E4,
            //             4.92673942608635921086E4
            //         };

            if (Math.Abs(x) > 1.0) return (1.0 - Erfc(x));
            z = x * x;
            //y = x * Polevl(z, T, 4) / P1evl(z, U, 5);
            double zT = 9.60497373987051638749E0;
            zT = zT * z + 9.00260197203842689217E1;
            zT = zT * z + 2.23200534594684319226E3;
            zT = zT * z + 7.00332514112805075473E3;
            zT = zT * z + 5.55923013010394962768E4;
            double zU = z + 3.35617141647503099647E1;
            zU = zU * z + 5.21357949780152679795E2;
            zU = zU * z + 4.59432382970980127987E3;
            zU = zU * z + 2.26290000613890934246E4;
            zU = zU * z + 4.92673942608635921086E4;
            y = x * zT / zU;
            return y;
        }


        /// <summary>
        /// Evaluates polynomial of degree N
        /// </summary>
        /// <param name="x"></param>
        /// <param name="coef"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        private static double Polevl(double x, double[] coef, int n)
        {
            double ans;

            ans = coef[0];

            for (int i = 1; i <= n; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }


        /// <summary>
        /// Evaluates polynomial of degree N with assumtion that coef[N] = 1.0
        /// </summary>
        /// <param name="x"></param>
        /// <param name="coef"></param>
        /// <param name="N"></param>
        /// <returns></returns>		
        private static double P1evl(double x, double[] coef, int n)
        {
            double ans;

            ans = x + coef[0];

            for (int i = 1; i < n; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }


        /// <summary>
        /// Returns the natural logarithm of gamma function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double LogGamma(double x)
        {
            double p, q, w, z;

            //double[] A = {
            //             8.11614167470508450300E-4,
            //             -5.95061904284301438324E-4,
            //             7.93650340457716943945E-4,
            //             -2.77777777730099687205E-3,
            //             8.33333333333331927722E-2
            //         };
            //double[] B = {
            //             -1.37825152569120859100E3,
            //             -3.88016315134637840924E4,
            //             -3.31612992738871184744E5,
            //             -1.16237097492762307383E6,
            //             -1.72173700820839662146E6,
            //             -8.53555664245765465627E5
            //         };
            //double[] C = {
            //             /* 1.00000000000000000000E0, */
            //             -3.51815701436523470549E2,
            //             -1.70642106651881159223E4,
            //             -2.20528590553854454839E5,
            //             -1.13933444367982507207E6,
            //             -2.53252307177582951285E6,
            //             -2.01889141433532773231E6
            //         };

            if (x < -34.0)
            {
                q = -x;
                w = LogGamma(q);
                p = Math.Floor(q);
                if (p == q) throw new ArithmeticException("lgam: Overflow");
                z = q - p;
                if (z > 0.5)
                {
                    p += 1.0;
                    z = p - q;
                }
                z = q * Math.Sin(Math.PI * z);
                if (z == 0.0) throw new
                                  ArithmeticException("lgamma: Overflow");
                z = LOGPI - Math.Log(z) - w;
                return z;
            }

            if (x < 13.0)
            {
                z = 1.0;
                while (x >= 3.0)
                {
                    x -= 1.0;
                    z *= x;
                }
                while (x < 2.0)
                {
                    if (x == 0.0) throw new
                                      ArithmeticException("lgamma: Overflow");
                    z /= x;
                    x += 1.0;
                }
                if (z < 0.0) z = -z;
                if (x == 2.0) return Math.Log(z);
                x -= 2.0;
                //p = x * Polevl(x, B, 5) / P1evl(x, C, 6);
                double xB = -1.37825152569120859100E3;
                xB = xB * x + -3.88016315134637840924E4;
                xB = xB * x + -3.31612992738871184744E5;
                xB = xB * x + -1.16237097492762307383E6;
                xB = xB * x + -1.72173700820839662146E6;
                xB = xB * x + -8.53555664245765465627E5;
                double xC = x + -3.51815701436523470549E2;
                xC = xC * x + -1.70642106651881159223E4;
                xC = xC * x + -2.20528590553854454839E5;
                xC = xC * x + -1.13933444367982507207E6;
                xC = xC * x + -2.53252307177582951285E6;
                xC = xC * x + -2.01889141433532773231E6;
                p = x * xB / xC;
                return (Math.Log(z) + p);
            }

            if (x > 2.556348e305) throw new
                                      ArithmeticException("lgamma: Overflow");

            q = (x - 0.5) * Math.Log(x) - x + 0.91893853320467274178;
            if (x > 1.0e8) return (q);

            p = 1.0 / (x * x);
            if (x >= 1000.0)
                q += ((7.9365079365079365079365e-4 * p
                    - 2.7777777777777777777778e-3) * p
                    + 0.0833333333333333333333) / x;
            else
            {
                //q += Polevl(p, A, 4) / x;
                double pA = 8.11614167470508450300E-4;
                pA = pA * p + -5.95061904284301438324E-4;
                pA = pA * p + 7.93650340457716943945E-4;
                pA = pA * p + -2.77777777730099687205E-3;
                pA = pA * p + 8.33333333333331927722E-2;
                q += pA / x;
            }
            return q;
        }

#if !SILVERLIGHT
        [Serializable]
#endif
        private struct InterpolationData
        {
            public float[/*b*/] Y;
            public float Bmin;
            public float Bmax;
        }

        private static readonly InterpolationData[/*a*/][/*x*/] ibetaCache;
        private const int ibetaCacheSize = 100;
        private const int ibetaBResolution = 100;
        private const int ibetaXResolution = 100;            

        static SpecialFunction()
        {
#if !SILVERLIGHT
            string cacheName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SpecialCache.bin");

            // TODO reevaluate the situation in Silverlight
            var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
 
            try
            {
                if (File.Exists(cacheName))
                {
                    using (FileStream stream = new FileStream(cacheName, FileMode.Open, FileAccess.Read))
                    {
                        ibetaCache = (InterpolationData[][])serializer.Deserialize(stream);
                    }
                }
            }
            catch
            {
            }
#endif
            if (ibetaCache == null)
            {
                ibetaCache = CalculateIbetaCache();
#if !SILVERLIGHT
                try
                {
                    using (FileStream stream = new FileStream(cacheName, FileMode.Create, FileAccess.Write))
                    {
                        serializer.Serialize(stream, ibetaCache);
                    }
                }
                catch
                {
                }
#endif
            }
        }

        private static InterpolationData[][] CalculateIbetaCache()
        {
            InterpolationData[][] ibetaCache = new InterpolationData[ibetaCacheSize][];

            for (int a = 0; a < ibetaCacheSize; a++)
            {
                ibetaCache[a] = new InterpolationData[ibetaXResolution + 1];
                for (int i = 1; i < ibetaXResolution; i++)
                {
                    ibetaCache[a][i].Y = new float[ibetaBResolution + 1];

                    double x = i / (double)ibetaXResolution;
                    double bmin = BisectIbeta(a + 1, x, 0.0001, 0.00001);
                    double bmax = BisectIbeta(a + 1, x, 0.9999, 0.00001);

                    ibetaCache[a][i].Bmin = (float)bmin;
                    ibetaCache[a][i].Bmax = (float)bmax;
                    for (int j = 0; j <= ibetaBResolution; j++)
                    {
                        ibetaCache[a][i].Y[j] = (float)Ibeta(a + 1, bmin + (bmax - bmin) * j / (double)ibetaBResolution, x);
                    }
                }
            }
            return ibetaCache;
        }

        private static double BisectIbeta(double a, double x, double y, double epsilon)
        {
            // find b such that Ibeta(a,b,x)=y
            double low = 0;
            double lowy = 0;
            double high = 100;
            double highy = Ibeta(a, high, x);

            while (highy < y)
            {
                high += 100;
                highy = Ibeta(a, high, x);
            }

            while (highy - lowy > epsilon)
            {
                double mid = (low + high) / 2.0;
                double midy = Ibeta(a, mid, x);

                if (midy > y)
                {
                    high = mid;
                    highy = midy;
                }
                else
                {
                    low = mid;
                    lowy = midy;
                }
            }

            return (low + high) / 2.0;
        }

        public static float IbetaInterpolated(int a, float b, float x)
        {
            if (a > ibetaCacheSize)
            {
                // not in cache, regen cache for values up to here?
                return (float)Ibeta(a, b, x);
            }
            a -= 1;

            float xx = x * ibetaXResolution;
            int i = (int)xx;
            if (i >= ibetaXResolution) return 1.0f;

            // interpolate at 4 control x values on grid along b

            // i - 1
            float y0;
            float y1;
            if (i >= 2)
            {
                y0 = InterpolateAux(ibetaCache[a][i - 1], b);
                y1 = InterpolateAux(ibetaCache[a][i], b);
            }
            else if (i == 1)
            {
                y0 = 0.0f;
                y1 = InterpolateAux(ibetaCache[a][i], b);
            }
            else // i == 0
            {
                y0 = 0.0f;
                y1 = 0.0f;
            }
            float y2;
            float y3;
            if (i < ibetaXResolution - 2)
            {
                y2 = InterpolateAux(ibetaCache[a][i + 1], b);
                y3 = InterpolateAux(ibetaCache[a][i + 2], b);
            }
            else if (i == ibetaXResolution - 2)
            {
                y2 = InterpolateAux(ibetaCache[a][i + 1], b);
                y3 = 1.0f;
            }
            else // i >= ibetaXResolution - 1
            {
                y2 = 1.0f;
                y3 = 1.0f;
            }
            xx -= i;
            // Catmull–Rom spline
            float y = 0.5f * (y0 * (xx * ((2 - xx) * xx - 1)) + y1 * (xx * xx * (3 * xx - 5) + 2) + y2 * (xx * ((4 - 3 * xx) * xx + 1)) + y3 * ((xx - 1) * xx * xx));
            //System.Diagnostics.Trace.WriteLine("Interp: " + y + "   Exact: " + Ibeta(a + 1, b, x));
            return Math.Min(1.0f, y);
        }

        private static float InterpolateAux(InterpolationData data, float b)
        {
            if (b < data.Bmin)
            {
                return data.Y[0] * b / data.Bmin;
            }
            else if (b > data.Bmax)
            {
                float d = b - data.Bmax;
                return data.Y[ibetaBResolution] + (1.0f - data.Y[ibetaBResolution]) * d / (1 + d);
            }
            else
            {
                float d = (b - data.Bmin) * ibetaBResolution / (data.Bmax - data.Bmin);
                int j = (int)d;
                // hermite interpolation
                float y0;
                if (j == 0)
                {
                    y0 = 0f;
                }
                else
                {
                    y0 = data.Y[j - 1];
                }
                float y1 = data.Y[j];
                float y2;
                float y3;
                if (j <= ibetaBResolution - 2)
                {
                    y2 = data.Y[j + 1];
                    y3 = data.Y[j + 2];
                }
                else if (j == ibetaBResolution - 1)
                {
                    y2 = data.Y[j + 1];
                    y3 = 1f;
                }
                else // j > ibetaBResolution - 1
                {
                    y2 = 1f;
                    y3 = 1f;
                }
                d -= j;
                // Catmull–Rom spline
                return 0.5f * (y0 * (d * ((2 - d) * d - 1)) + y1 * (d * d * (3 * d - 5) + 2) + y2 * (d * ((4 - 3 * d) * d + 1)) + y3 * ((d - 1) * d * d));
            }
        }

        public static float IbetaInterpolatedLinear(int a, float b, float x)
        {
            if (a > ibetaCacheSize)
            {
                // not in cache, regen cache for values up to here?
                return (float)Ibeta(a, b, x);
            }
            a -= 1;

            float xx = x * ibetaXResolution;
            int i = (int)xx;
            if (i >= ibetaXResolution) return 1.0f;

            // interpolate at 2 control x values on grid along b

            // i - 1
            float y1;
            if (i >= 1)
            {
                y1 = InterpolateAuxLinear(ibetaCache[a][i], b);
            }
            else // i == 0
            {
                y1 = 0.0f;
            }
            float y2;
            if (i <= ibetaXResolution - 2)
            {
                y2 = InterpolateAuxLinear(ibetaCache[a][i + 1], b);
            }
            else // i >= ibetaXResolution - 1
            {
                y2 = 1.0f;
            }
            xx -= i;
            // linear spline
            return y1 + xx * (y2 - y1);
        }

        private static float InterpolateAuxLinear(InterpolationData data, float b)
        {
            if (b < data.Bmin)
            {
                return data.Y[0] * b / data.Bmin;
            }
            else if (b > data.Bmax)
            {
                float d = b - data.Bmax;
                return data.Y[ibetaBResolution] + (1.0f - data.Y[ibetaBResolution]) * d / (1 + d);
            }
            else
            {
                float d = (b - data.Bmin) * ibetaBResolution / (data.Bmax - data.Bmin);
                int j = (int)d;
                // linear interpolation
                float y1 = data.Y[j];
                float y2;
                if (j <= ibetaBResolution - 1)
                {
                    y2 = data.Y[j + 1];
                }
                else // j > ibetaBResolution - 1
                {
                    y2 = 1f;
                }
                d -= j;
                // linear spline
                return y1 + d * (y2 - y1);
            }
        }

        public static float Ibeta(int a, float b, float x)
        {
            return (float)Ibeta((double)a, (double)b, (double)x);
        }

        /// <summary>
        /// Returns the incomplete beta function evaluated from zero to xx.
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="bb"></param>
        /// <param name="xx"></param>
        /// <returns></returns>
        public static double Ibeta(double aa, double bb, double xx)
        {
            double a, b, t, x, xc, w, y;
            bool flag;

            if (aa <= 0.0 || bb <= 0.0) throw new
                                            ArithmeticException("ibeta: Domain error!");

            if ((xx <= 0.0) || (xx >= 1.0))
            {
                if (xx == 0.0) return 0.0;
                if (xx == 1.0) return 1.0;
                throw new ArithmeticException("ibeta: Domain error!");
            }

            flag = false;
            if ((bb * xx) <= 1.0 && xx <= 0.95)
            {
                t = Pseries(aa, bb, xx);
                return t;
            }

            w = 1.0 - xx;

            /* Reverse a and b if x is greater than the mean. */
            if (xx > (aa / (aa + bb)))
            {
                flag = true;
                a = bb;
                b = aa;
                xc = xx;
                x = w;
            }
            else
            {
                a = aa;
                b = bb;
                xc = w;
                x = xx;
            }

            if (flag && (b * x) <= 1.0 && x <= 0.95)
            {
                t = Pseries(a, b, x);
                if (t <= MACHEP) t = 1.0 - MACHEP;
                else t = 1.0 - t;
                return t;
            }

            /* Choose expansion for better convergence. */
            y = x * (a + b - 2.0) - (a - 1.0);
            if (y < 0.0)
                w = Incbcf(a, b, x);
            else
                w = Incbd(a, b, x) / xc;

            /* Multiply w by the factor
                   a      b   _             _     _
                  x  (1-x)   | (a+b) / ( a | (a) | (b) ) .   */

            y = a * Math.Log(x);
            t = b * Math.Log(xc);
            if ((a + b) < MAXGAM && Math.Abs(y) < MAXLOG && Math.Abs(t) < MAXLOG)
            {
                t = Math.Pow(xc, b);
                t *= Math.Pow(x, a);
                t /= a;
                t *= w;
                t *= Gamma(a + b) / (Gamma(a) * Gamma(b));
                if (flag)
                {
                    if (t <= MACHEP) t = 1.0 - MACHEP;
                    else t = 1.0 - t;
                }
                return t;
            }
            /* Resort to logarithms.  */
            y += t + LogGamma(a + b) - LogGamma(a) - LogGamma(b);
            y += Math.Log(w / a);
            if (y < MINLOG)
                t = 0.0;
            else
                t = Math.Exp(y);

            if (flag)
            {
                if (t <= MACHEP) t = 1.0 - MACHEP;
                else t = 1.0 - t;
            }
            return t;
        }


        /// <summary>
        /// Returns the continued fraction expansion #1 for incomplete beta integral.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double Incbcf(double a, double b, double x)
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, thresh;
            int n;
            double big = 4.503599627370496e15;
            double biginv = 2.22044604925031308085e-16;

            k1 = a;
            k2 = a + b;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = b - 1.0;
            k7 = k4;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {
                xk = -(x * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (x * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0) r = pk / qk;
                if (r != 0)
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh) return ans;

                k1 += 1.0;
                k2 += 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 -= 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((Math.Abs(qk) + Math.Abs(pk)) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if ((Math.Abs(qk) < biginv) || (Math.Abs(pk) < biginv))
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            } while (++n < 300);

            return ans;
        }


        /// <summary>
        /// Returns the continued fraction expansion #2 for incomplete beta integral.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double Incbd(double a, double b, double x)
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, z, thresh;
            int n;
            double big = 4.503599627370496e15;
            double biginv = 2.22044604925031308085e-16;

            k1 = a;
            k2 = b - 1.0;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = a + b;
            k7 = a + 1.0;
            ;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x / (1.0 - x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {
                xk = -(z * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (z * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0) r = pk / qk;
                if (r != 0)
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh) return ans;

                k1 += 1.0;
                k2 -= 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 += 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((Math.Abs(qk) + Math.Abs(pk)) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if ((Math.Abs(qk) < biginv) || (Math.Abs(pk) < biginv))
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            } while (++n < 300);

            return ans;
        }


        /// <summary>
        /// Returns the power series for incomplete beta integral. Use when b*x is small and x not too close to 1.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double Pseries(double a, double b, double x)
        {
            double s, t, u, v, n, t1, z, ai;

            ai = 1.0 / a;
            u = (1.0 - b) * x;
            v = u / (a + 1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = MACHEP * ai;
            while (Math.Abs(v) > z)
            {
                u = (n - b) * x / n;
                t *= u;
                v = t / (a + n);
                s += v;
                n += 1.0;
            }
            s += t1;
            s += ai;

            u = a * Math.Log(x);
            if ((a + b) < MAXGAM && Math.Abs(u) < MAXLOG)
            {
                t = Gamma(a + b) / (Gamma(a) * Gamma(b));
                s = s * t * Math.Pow(x, a);
            }
            else
            {
                t = LogGamma(a + b) - LogGamma(a) - LogGamma(b) + u + Math.Log(s);
                if (t < MINLOG) s = 0.0;
                else s = Math.Exp(t);
            }
            return s;
        }

    }
}