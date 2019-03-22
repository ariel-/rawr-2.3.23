using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rawr.Base.Algorithms
{

    /// <summary>
    /// A thread-safe calculation cache.
    /// Ensures that calculation is done only once for each parameters
    /// even if the same calculation is requested from different threads
    /// and it's not cached yet.
    /// </summary>
    /// <typeparam name="TParameters">Type of calculation parameters. 
    /// Parameters must be represented as a single equatable object.</typeparam>
    /// <typeparam name="TResult">Type of the result</typeparam>
    public class CalculationCache<TParameters, TResult> : IDisposable
        where TParameters : IEquatable<TParameters>
    {

        /// <summary>
        /// Describes a calculation
        /// </summary>
        /// <param name="parameters">Calculation parameters</param>
        /// <returns>Calculation result</returns>
        public delegate TResult Calculation(TParameters parameters);


        private readonly Calculation calculation;
        private readonly object syncRoot = new object();
        private readonly Dictionary<TParameters, TResult> cachedResults =
            new Dictionary<TParameters, TResult>();
        private readonly Dictionary<TParameters, CalculationProcessInfo> processingCalculations =
            new Dictionary<TParameters, CalculationProcessInfo>();


        /// <summary>
        /// Creates a new calculation cache.
        /// </summary>
        /// <param name="calculation">The delegate that does the actual calculation</param>
        public CalculationCache(Calculation calculation)
        {
            if (calculation == null)
                throw new ArgumentNullException("calculation");

            this.calculation = calculation;
        }


        /// <summary>
        /// Returns cached result, or calculates a new one, or waits until the calculation is complete.
        /// Thread-safe.
        /// </summary>
        /// <param name="parameters">Calculation parameters</param>
        /// <returns>Calculation result</returns>
        public TResult GetResult(TParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            TResult result;
            while (true)
            {
                CalculationProcessInfo calculationProcessInfo = null;
                bool shouldWait = false;

                try
                {
                    lock (syncRoot)
                    {
                        if (cachedResults.TryGetValue(parameters, out result))
                            return result;

                        shouldWait = TryAcquireCalculationWait(parameters, out calculationProcessInfo);

                        if (!shouldWait)
                            calculationProcessInfo = CreateCalculationProcessInfo(parameters);
                    }

                    if (shouldWait)
                    {
                        calculationProcessInfo.Event.WaitOne();
                    }
                    else
                    {
                        result = calculation(parameters);

                        lock (syncRoot)
                            cachedResults.Add(parameters, result);

                        return result;
                    }
                }
                finally
                {
                    lock (syncRoot)
                    {
                        if (shouldWait)
                            ReleaseCalculationWait(parameters, calculationProcessInfo);
                        else if (calculationProcessInfo != null)
                            CompleteCalculationProcessInfo(parameters, calculationProcessInfo);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var processingCalculation in processingCalculations)
                processingCalculation.Value.Dispose();
        }


        private bool TryAcquireCalculationWait(
            TParameters parameters,
            out CalculationProcessInfo calculationProcessInfo)
        {
            bool result;

            try
            {
            }
            // Finally block will not be interrupted by thread aborting
            finally
            {
                result = processingCalculations.TryGetValue(parameters, out calculationProcessInfo);

                if (result)
                {
                    checked
                    {
                        calculationProcessInfo.WaitCount++;
                    }
                }
            }

            return result;
        }

        private void ReleaseCalculationWait(
            TParameters parameters,
            CalculationProcessInfo calculationProcessInfo)
        {
            calculationProcessInfo.WaitCount--;

            if (calculationProcessInfo.Done && (calculationProcessInfo.WaitCount == 0))
            {
                processingCalculations.Remove(parameters);
                calculationProcessInfo.Dispose();
            }
        }

        private CalculationProcessInfo CreateCalculationProcessInfo(TParameters parameters)
        {
            CalculationProcessInfo calculationProcessInfo = new CalculationProcessInfo();
            processingCalculations.Add(parameters, calculationProcessInfo);
            return calculationProcessInfo;
        }

        private void CompleteCalculationProcessInfo(
            TParameters parameters,
            CalculationProcessInfo calculationProcessInfo)
        {
            calculationProcessInfo.Done = true;

            if (calculationProcessInfo.WaitCount > 0)
            {
                calculationProcessInfo.Event.Set();
            }
            else
            {
                processingCalculations.Remove(parameters);
                calculationProcessInfo.Dispose();
            }
        }


        private class CalculationProcessInfo : IDisposable
        {

            public CalculationProcessInfo()
            {
                Event = new ManualResetEvent(false);
            }


            public bool Done { get;  set; }
            public ManualResetEvent Event { get; set; }
            public int WaitCount { get; set; }


            public void Dispose()
            {
                Event.Close();
            }

        }

    }

}
