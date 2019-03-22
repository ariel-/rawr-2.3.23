using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace Rawr.Optimizer
{
    /// <summary>
    /// Abstract class that is used by the optimizer to validate a certain range of slots in an
    /// individual. When optimizer is constructing an individual it will call the validator after
    /// it constructs the individual up to EndSlot, if validation fails optimizer restarts construction
    /// at StartSlot.
    /// </summary>
    /// <typeparam name="TItem">Type of items.</typeparam>
    public abstract class OptimizerRangeValidatorBase<TItem>
    {
        /// <summary>
        /// Gets or sets the first slot of validation range.
        /// </summary>
        public int StartSlot { get; set; }

        /// <summary>
        /// Gets or sets the last slot of validation range.
        /// </summary>
        public int EndSlot { get; set; }

        /// <summary>
        /// Checks whether the range of items is valid.
        /// </summary>
        /// <param name="items">Array of items that define an individual.</param>
        /// <returns>True if the range of items is valid, False otherwise.</returns>
        /// <remarks>Validation should only depend on items between StartSlot and EndSlot. Items after EndSlot
        /// may not be initialized.</remarks>
        public abstract bool IsValid(TItem[] items);
    }

    /// <summary>
    /// Abstract optimizer class that supports optimization of individuals that are defined by a list of items
    /// that fill slots where each slot has a list of valid items that can fill it and there can be restrictions
    /// on which item combinations are valid.
    /// </summary>
    /// <typeparam name="TItem">Type of items.</typeparam>
    /// <typeparam name="TIndividual">Type of the individuals.</typeparam>
    /// <typeparam name="TValuation">Type associated with valuation result of an individual.</typeparam>
    public abstract class OptimizerBase<TItem, TIndividual, TValuation>
        where TIndividual : class
    {
        protected int _thoroughness;

        public OptimizationMethod OptimizationMethod { get; set; }

        /// <summary>
        /// Number of item slots that define an individual.
        /// </summary>
        protected int slotCount;
        /// <summary>
        /// Array of lists for each slot of potential items to fill the slot. This should be populated
        /// unless a custom implementation of GetRandomItem and LookForDirectItemUpgrades is provided.
        /// </summary>
        protected List<TItem>[] slotItems;
        /// <summary>
        /// List of validators sorted in the order they should be executed.
        /// </summary>
        protected List<OptimizerRangeValidatorBase<TItem>> validators;

        public bool ThreadPoolValuation { get; set; }

        /// <summary>
        /// When set to true it will maintain two arrays of individuals and reuse them, but when an individual
        /// is copied between generations it needs to make an explicit copy over. If you use recycling that would
        /// create an error since the same individual could be reused in the same generation for different genes.
        /// </summary>
        protected bool SupportsRecycling { get; set; }

        public virtual void CancelAsync()
        {
            cancellationPending = true;
        }

        protected bool cancellationPending;

        protected abstract void ReportProgress(int progressPercentage, float bestValue);

        private static Random _randSeed = new Random();

        [ThreadStatic]
        private static Random _rand;

        protected Random Rnd
        {
            get
            {
                Random r = _rand;
                if (r == null)
                {
                    int seed;
                    lock (_randSeed) seed = _randSeed.Next();
                    _rand = r = new Random(seed);
                }
                return r;
            }
        }

        protected TIndividual Optimize(out float bestValue)
        {
            TValuation bestValuation;
            bool injected;
            return Optimize(null, 0, out bestValue, out bestValuation, out injected);
        }

        protected TIndividual Optimize(TIndividual injectIndividual, out float bestValue, out bool injected)
        {
            TValuation bestValuation;
            return Optimize(injectIndividual, injectIndividual != null ? GetOptimizationValue(injectIndividual) : 0.0f, out bestValue, out bestValuation, out injected);
        }

        protected TIndividual Optimize(TIndividual injectIndividual, float injectValue, out float bestValue, out TValuation bestValuation, out bool injected)
        {
            bool oldVolatility = Item.OptimizerManagedVolatiliy;
            try
            {
                Item.OptimizerManagedVolatiliy = true;
                switch (OptimizationMethod)
                {
                    case OptimizationMethod.GeneticAlgorithm:
                        return OptimizeGA(injectIndividual, injectValue, out bestValue, out bestValuation, out injected);
                    case OptimizationMethod.SimulatedAnnealing:
                        return OptimizeSA(injectIndividual, injectValue, out bestValue, out bestValuation, out injected);
                }
                bestValue = 0.0f;
                bestValuation = default(TValuation);
                injected = false;
                return null;
            }
            finally
            {
                Item.OptimizerManagedVolatiliy = oldVolatility;
            }
        }

        /// <summary>
        /// Optimization function based on simulated annealing
        /// http://en.wikipedia.org/wiki/Simulated_annealing
        /// Author: ebo
        /// </summary>
        private TIndividual OptimizeSA(TIndividual injectIndividual, float injectValue, out float best, out TValuation bestValuation, out bool injected)
        {
            best = -10000000;
            bestValuation = default(TValuation);
            injected = false;

            Random rand = Rnd;

            TIndividual currentIndividual = BuildRandomIndividual(null);
            double currentValue = GetOptimizationValue(currentIndividual);

            TIndividual bestIndividual = currentIndividual;
            double bestValue = currentValue;

            int maxCycles = _thoroughness * _thoroughness;

            //http://research.microsoft.com/constraint-reasoning/workshops/autonomous-cp07/papers/2.pdf

            double temp = 10;
            double acceptRate = 0.5;
            double lamRate = 0.5;

            for (int cycle = 0; cycle < maxCycles; cycle++)
            {
                if (cancellationPending) return null;
                ReportProgress((int)Math.Round((float)cycle / ((float)(maxCycles / 100f))), (float)bestValue);

                // Generate new character
                TIndividual nextIndividual = GeneratorBuildSACharacter(currentIndividual);

                double nextValue = GetOptimizationValue(nextIndividual);


                // Save best character
                if (nextValue > bestValue)
                {
                    bestIndividual = nextIndividual;
                    bestValue = nextValue;
                }

                if (nextValue > currentValue)
                {
                    //Better solution. Accept move
                    currentIndividual = nextIndividual;
                    currentValue = nextValue;
                    acceptRate = 1.0 / 500.0 * (499.0 * acceptRate + 1);
                }
                else
                {
                    if (SAAcceptance(currentValue, nextValue, temp, rand))
                    {
                        //accept move
                        currentIndividual = nextIndividual;
                        currentValue = nextValue;
                        acceptRate = 1.0 / 500.0 * (499.0 * acceptRate + 1);
                    }
                    else
                    {
                        //reject move
                        acceptRate = 1.0 / 500.0 * (499.0 * acceptRate);
                    }

                }


                // tune acceptRate
                double part = (double)cycle / maxCycles;
                if (part < 0.15)
                {
                    lamRate = 0.44 + 0.56 * Math.Pow(560, -cycle / (maxCycles * 0.15));
                }
                else if (part < 0.65)
                {
                    lamRate = 0.44;
                }
                else
                {
                    lamRate = 0.44 * Math.Pow(440, -((double)cycle / (double)maxCycles - 0.65) / 0.35);
                }

                if (acceptRate > lamRate)
                {
                    temp *= 0.999;
                }
                else
                {
                    temp /= 0.999;
                }


            }

            best = (float)bestValue;
            bestValuation = GetValuation(bestIndividual);

            return PostProcess(bestIndividual);
        }

        protected int noImprove;

        protected float bestValue;
        protected TValuation bestValuation;
        protected TIndividual bestIndividual;

        protected TIndividual[] population;
        private float[] values;
        int islandSize;
        int popSize;
        TIndividual[] individualIsland;
        int[] islandNoImprove;
        int islandStagnationLimit;
        int islandCount;
        TIndividual[] popCopy;
        float[] share;

        private object bestValueLock = new object();
        private AutoResetEvent valuationsComplete = new AutoResetEvent(false);
        private int threadPoolStarted;
        private int threadPoolComplete;
        private int threadPoolSize;

        private void GetThreadPoolValuation(object state)
        {
            int i = Interlocked.Increment(ref threadPoolStarted) - 1;
            if (i >= threadPoolSize)
            {
                // everything is queued up already, drop out
                if (Interlocked.Decrement(ref startedThreads) == 0)
                {
                    valuationsComplete.Set();
                }
                return;
            }
            remainingThreadsToSpawn--; // these are called in sequence, no need for synchronization
            if (remainingThreadsToSpawn > 0)
            {
                Interlocked.Increment(ref startedThreads);
                ThreadPool.QueueUserWorkItem(GetThreadPoolValuation);
            }            
            do
            {
                TValuation valuation = GetValuation(population[i]);
                values[i] = GetOptimizationValue(population[i], valuation);
                lock (bestValueLock)
                {
                    if (values[i] > this.bestValue)
                    {
                        this.bestValue = values[i];
                        this.bestValuation = valuation;
                        bestIndividual = BuildCopyIndividual(population[i], bestIndividual);
                        noImprove = -1;
                        //if (population[i].Geneology != null) System.Diagnostics.Trace.WriteLine(best + " " + population[i].Geneology);
                    }
                }
                if (Interlocked.Increment(ref threadPoolComplete) == threadPoolSize)
                {
                    // all work is done, check if we're the last worker
                    if (Interlocked.Decrement(ref startedThreads) == 0)
                    {
                        valuationsComplete.Set();
                    }
                    return;
                }
                // try to get more work
                i = Interlocked.Increment(ref threadPoolStarted) - 1;
                if (i >= threadPoolSize)
                {
                    // all work was already grabbed, go out
                    if (Interlocked.Decrement(ref startedThreads) == 0)
                    {
                        // we're certain everyone is done, if someone was still processing
                        // they wouldn't have decremented their startedThreads yet
                        valuationsComplete.Set();
                    }
                    return;
                }
            } while (true);
        }

        private void ThreadPoolGeneratePopulation(object state)
        {
            Random rand = Rnd;
            int i = Interlocked.Increment(ref threadPoolStarted) - 1;
            if (i >= threadPoolSize)
            {
                // everything is queued up already, drop out
                if (Interlocked.Decrement(ref startedThreads) == 0)
                {
                    valuationsComplete.Set();
                }
                return;
            }
            remainingThreadsToSpawn--; // these are called in sequence, no need for synchronization
            if (remainingThreadsToSpawn > 0)
            {
                Interlocked.Increment(ref startedThreads);
                ThreadPool.QueueUserWorkItem(ThreadPoolGeneratePopulation);
            }
            do
            {
                // do work with index i
                int island = i / islandSize;
                if (i % islandSize == 0)
                {
                    if (individualIsland[island] == null)
                    {
                        population[i] = BuildRandomIndividual(population[i]);
                        //population[i].Geneology = "Random";
                    }
                    else
                    {
                        population[i] = BuildCopyIndividual(individualIsland[island], population[i]);
                    }
                }
                else if (rand.NextDouble() < 0.05d)
                {
                    //completely random
                    population[i] = BuildRandomIndividual(population[i]);
                    //population[i].Geneology = "Random";
                }
                else if (rand.NextDouble() < 0.4d)
                {
                    int transplant = island;
                    if (islandNoImprove[island] > islandStagnationLimit) transplant = rand.Next(islandCount);
                    //crossover
                    float s = (float)rand.NextDouble();
                    float sum = 0;
                    int i1, i2;
                    for (i1 = transplant * islandSize; i1 < Math.Min(popSize, (transplant + 1) * islandSize) - 1; i1++)
                    {
                        sum += share[i1];
                        if (sum >= s) break;
                    }
                    s = (float)rand.NextDouble();
                    sum = 0;
                    for (i2 = island * islandSize; i2 < Math.Min(popSize, (island + 1) * islandSize) - 1; i2++)
                    {
                        sum += share[i2];
                        if (sum >= s) break;
                    }
                    population[i] = BuildChildIndividual(popCopy[i1], popCopy[i2], population[i]);
                    //population[i].Geneology = "Crossover(" + values[i1] + ", " + values[i2] + ")";
                }
                else
                {
                    int transplant = island;
                    if (islandNoImprove[island] > islandStagnationLimit && rand.NextDouble() < 1.0 / islandSize) transplant = rand.Next(islandCount);
                    //mutate
                    float s = (float)rand.NextDouble();
                    float sum = 0;
                    int i1;
                    for (i1 = transplant * islandSize; i1 < Math.Min(popSize, (transplant + 1) * islandSize) - 1; i1++)
                    {
                        sum += share[i1];
                        if (sum >= s) break;
                    }
                    population[i] = BuildMutantIndividual(popCopy[i1], population[i]);
                    //population[i].Geneology = "Mutation(" + values[i1] + ")";
                }
                // end work with index i
                if (Interlocked.Increment(ref threadPoolComplete) == threadPoolSize)
                {
                    // all work is done, check if we're the last worker
                    if (Interlocked.Decrement(ref startedThreads) == 0)
                    {
                        valuationsComplete.Set();
                    }
                    return;
                }
                // try to get more work
                i = Interlocked.Increment(ref threadPoolStarted) - 1;
                if (i >= threadPoolSize)
                {
                    // all work was already grabbed, go out
                    if (Interlocked.Decrement(ref startedThreads) == 0)
                    {
                        // we're certain everyone is done, if someone was still processing
                        // they wouldn't have decremented their startedThreads yet
                        valuationsComplete.Set();
                    }
                    return;
                }
            } while (true);
        }

        private TIndividual OptimizeGA(TIndividual injectIndividual, float injectValue, out float bestValue, out TValuation bestValuation, out bool injected)
        {
            //Begin Genetic
            int i1, i2;
            this.bestValue = bestValue = -10000000;
            this.bestValuation = bestValuation = default(TValuation);
            injected = false;
            // verify inject individual
            if (injectIndividual != null && !IsIndividualValid(injectIndividual))
            {
                injectIndividual = null;
            }

            popSize = _thoroughness;
            islandSize = 20;
            islandStagnationLimit = 50;
            islandCount = (popSize - 1) / islandSize + 1;
            int cycleLimit = _thoroughness;
            population = new TIndividual[popSize];
            popCopy = new TIndividual[popSize];
            values = new float[popSize];
            float[] minIsland = new float[islandCount];
            float[] maxIsland = new float[islandCount];
            float[] bestIsland = new float[islandCount];
            individualIsland = new TIndividual[islandCount];
            islandNoImprove = new int[islandCount];
            for (int i = 0; i < islandCount; i++)
            {
                bestIsland[i] = -10000000;
            }
            share = new float[popSize];
            float s, sum;
            bestIndividual = null;
            Random rand = Rnd;

            if (_thoroughness > 1)
            {
                for (int i = 0; i < popSize; i++)
                {
                    population[i] = BuildRandomIndividual(null);
                }
            }
            else
            {
                bestIndividual = injectIndividual;
                if (bestIndividual == null)
                {
                    bestIndividual = BuildRandomIndividual(null);
                }
                this.bestValue = GetOptimizationValue(bestIndividual);
            }

            noImprove = 0;
            while (noImprove < cycleLimit)
            {
                if (_thoroughness > 1)
                {
                    if (cancellationPending)
                    {
                        population = null;
                        values = null;
                        return null;
                    }
                    ReportProgress((int)Math.Round((float)noImprove / ((float)cycleLimit / 100f)), this.bestValue);

                    for (int i = 0; i < islandCount; i++)
                    {
                        minIsland[i] = 10000000;
                        maxIsland[i] = -10000000;
                        islandNoImprove[i]++;
                    }
                    //minv = 10000000;
                    //maxv = -10000000;
                    if (ThreadPoolValuation)
                    {
                        threadPoolStarted = 0;
                        threadPoolComplete = 0;
                        threadPoolSize = popSize;
                        remainingThreadsToSpawn = EffectiveMaxConcurrencyLevel;

                        startedThreads = 1;
                        GetThreadPoolValuation(null);

                        valuationsComplete.WaitOne();
                    }
                    else
                    {
                        for (int i = 0; i < popSize; i++)
                        {
                            TValuation valuation = GetValuation(population[i]);
                            values[i] = GetOptimizationValue(population[i], valuation);
                            if (values[i] > this.bestValue)
                            {
                                this.bestValue = values[i];
                                this.bestValuation = valuation;
                                bestIndividual = BuildCopyIndividual(population[i], bestIndividual);
                                noImprove = -1;
                                //if (population[i].Geneology != null) System.Diagnostics.Trace.WriteLine(best + " " + population[i].Geneology);
                            }
                        }
                    }
                    for (int i = 0; i < popSize; i++)
                    {
                        int island = i / islandSize;                        
                        if (values[i] < minIsland[island]) minIsland[island] = values[i];
                        if (values[i] > maxIsland[island])
                        {
                            maxIsland[island] = values[i];
                            individualIsland[island] = population[i];
                        }
                        if (values[i] > bestIsland[island])
                        {
                            bestIsland[island] = values[i];
                            islandNoImprove[island] = 0;
                        }
                    }
                    for (int island = 0; island < islandCount; island++)
                    {
                        sum = 0;
                        /*for (int i = island * islandSize; i < Math.Min(popSize, (island + 1) * islandSize); i++)
                            sum += values[i] - minIsland[island] + (maxIsland[island] - minIsland[island]) / 2;
                        for (int i = island * islandSize; i < Math.Min(popSize, (island + 1) * islandSize); i++)
                            share[i] = sum == 0 ? 1f / (Math.Min(popSize, (island + 1) * islandSize) - island * islandSize) : (values[i] - minIsland[island] + (maxIsland[island] - minIsland[island]) / 2) / sum;*/
                        // it seems a lot of values are huge negative due to missing constraints which can distort the values and make everything look the same
                        float delta = maxIsland[island] - minIsland[island];
                        for (int i = island * islandSize; i < Math.Min(popSize, (island + 1) * islandSize); i++)
                        {
                            float value = (float)(Math.Pow((values[i] - minIsland[island]) / delta, 10.0) + 0.1);
                            sum += value;
                            share[i] = value;
                        }
                        if (sum == 0)
                        {
                            for (int i = island * islandSize; i < Math.Min(popSize, (island + 1) * islandSize); i++)
                            {
                                share[i] = 1f / (Math.Min(popSize, (island + 1) * islandSize) - island * islandSize);
                            }
                        }
                        else
                        {
                            for (int i = island * islandSize; i < Math.Min(popSize, (island + 1) * islandSize); i++)
                            {
                                share[i] /= sum;
                            }
                        }
                    }
                }

                noImprove++;

                if (_thoroughness > 1 && noImprove < cycleLimit)
                {
                    TIndividual[] swap = population;
                    population = popCopy;
                    popCopy = swap;
                    if (ThreadPoolValuation)
                    {
                        threadPoolStarted = 0;
                        threadPoolComplete = 0;
                        threadPoolSize = popSize;
                        remainingThreadsToSpawn = EffectiveMaxConcurrencyLevel;

                        startedThreads = 1;
                        ThreadPoolGeneratePopulation(null);

                        valuationsComplete.WaitOne();
                    }
                    else
                    {
                        for (int i = 0; i < popSize; i++)
                        {
                            int island = i / islandSize;
                            if (i % islandSize == 0)
                            {
                                if (individualIsland[island] == null)
                                {
                                    population[i] = BuildRandomIndividual(population[i]);
                                    //population[i].Geneology = "Random";
                                }
                                else
                                {
                                    population[i] = BuildCopyIndividual(individualIsland[island], population[i]);
                                }
                            }
                            else if (rand.NextDouble() < 0.05d)
                            {
                                //completely random
                                population[i] = BuildRandomIndividual(population[i]);
                                //population[i].Geneology = "Random";
                            }
                            else if (rand.NextDouble() < 0.4d)
                            {
                                int transplant = island;
                                if (islandNoImprove[island] > islandStagnationLimit) transplant = rand.Next(islandCount);
                                //crossover
                                s = (float)rand.NextDouble();
                                sum = 0;
                                for (i1 = transplant * islandSize; i1 < Math.Min(popSize, (transplant + 1) * islandSize) - 1; i1++)
                                {
                                    sum += share[i1];
                                    if (sum >= s) break;
                                }
                                s = (float)rand.NextDouble();
                                sum = 0;
                                for (i2 = island * islandSize; i2 < Math.Min(popSize, (island + 1) * islandSize) - 1; i2++)
                                {
                                    sum += share[i2];
                                    if (sum >= s) break;
                                }
                                population[i] = BuildChildIndividual(popCopy[i1], popCopy[i2], population[i]);
                                //population[i].Geneology = "Crossover(" + values[i1] + ", " + values[i2] + ")";
                            }
                            else
                            {
                                int transplant = island;
                                if (islandNoImprove[island] > islandStagnationLimit && rand.NextDouble() < 1.0 / islandSize) transplant = rand.Next(islandCount);
                                //mutate
                                s = (float)rand.NextDouble();
                                sum = 0;
                                for (i1 = transplant * islandSize; i1 < Math.Min(popSize, (transplant + 1) * islandSize) - 1; i1++)
                                {
                                    sum += share[i1];
                                    if (sum >= s) break;
                                }
                                population[i] = BuildMutantIndividual(popCopy[i1], population[i]);
                                //population[i].Geneology = "Mutation(" + values[i1] + ")";
                            }
                        }
                    }
                }
                else if (_thoroughness > 1 && injectIndividual != null && !injected && injectValue > this.bestValue)
                {
                    population[popSize - 1] = BuildCopyIndividual(injectIndividual, population[popSize - 1]);
                    noImprove = 0;
                    injected = true;
                }
                else
                {
                    //last try, look for single direct upgrades
                    LookForDirectItemUpgrades();
                }
            }

            //if (this.bestValue == 0)
            //{
            //    bestIndividual = null;
            //    this.bestValuation = default(TValuation);
            //}

            bestValuation = this.bestValuation;
            bestValue = this.bestValue;
            TIndividual ret = bestIndividual;

            population = null;
            values = null;
            bestIndividual = null;
            this.bestValuation = default(TValuation);
            individualIsland = null;
            islandNoImprove = null;
            popCopy = null;
            share = null;

            return PostProcess(ret);
        }

        protected virtual TIndividual PostProcess(TIndividual bestIndividual)
        {
            return bestIndividual;
        }

        protected virtual void LookForDirectItemUpgrades()
        {
            KeyValuePair<float, TIndividual> results;
            TValuation directValuation;
            for (int slot = 0; slot < slotCount; slot++)
            {
                // bestIndividual could potentially be null here... need to validate.
                if (null == bestIndividual)
                {
                    //error Do we want to do some special handling here?
                }
                else
                {
                    results = LookForDirectItemUpgrades(slotItems[slot], slot, this.bestValue, bestIndividual, null, out directValuation);
                    if (results.Key > this.bestValue)
                    {
                        this.bestValue = results.Key;
                        this.bestValuation = directValuation;
                        bestIndividual = results.Value;
                        noImprove = 0;
                        population[0] = bestIndividual;
                        //population[0].Geneology = "DirectUpgrade";
                    }
                }
            }
        }

        protected virtual int EffectiveMaxConcurrencyLevel
        {
            get
            {
#if SILVERLIGHT
                return 4;
#else
                return Environment.ProcessorCount;
#endif
            }
        }

        protected object directValuationLock = new object();
        protected TIndividual bestDirectIndividual;
        protected TValuation bestDirectValuation;
        protected float bestDirectValue;
        protected int directValuationsComplete;
        protected int directValuationsIndex;
        protected int directValuationsSlot;
        protected bool directValuationFoundUpgrade;
        protected List<TItem> directValuationsList;
        protected TItem[] directValuationsTemplate;
        protected int remainingThreadsToSpawn;
        protected int startedThreads;

        protected virtual void ThreadPoolDirectUpgradeValuation(object ignore)
        {
            TIndividual swappedIndividual = null;
            float value = 0;
            TValuation valuation = default(TValuation);
            // get initial work item
            lock (directValuationLock)
            {
                if (directValuationsIndex >= directValuationsList.Count)
                {
                    // everything is queued up already, drop out
                    startedThreads--;
                    if (startedThreads == 0)
                    {
                        Monitor.Pulse(directValuationLock);
                    }
                    return;
                }
                remainingThreadsToSpawn--;
                if (remainingThreadsToSpawn > 0)
                {
                    startedThreads++;
                    ThreadPool.QueueUserWorkItem(ThreadPoolDirectUpgradeValuation);
                }
                directValuationsTemplate[directValuationsSlot] = directValuationsList[directValuationsIndex++];
                if (IsIndividualValid(directValuationsTemplate))
                {
                    swappedIndividual = GenerateIndividual(directValuationsTemplate, false, swappedIndividual);
                }
            }

            do
            {
                if (swappedIndividual != null)
                {
                    value = GetOptimizationValue(swappedIndividual, valuation = GetValuation(swappedIndividual));
                }

                lock (directValuationLock)
                {
                    directValuationsComplete++;
                    if (swappedIndividual != null && value > bestDirectValue)
                    {
                        bestDirectValue = value;
                        bestDirectValuation = valuation;
                        bestDirectIndividual = BuildCopyIndividual(swappedIndividual, bestDirectIndividual);
                        directValuationFoundUpgrade = true;
                    }
                    if (directValuationsComplete >= directValuationsList.Count)
                    {
                        startedThreads--;
                        if (startedThreads == 0)
                        {
                            Monitor.Pulse(directValuationLock);
                        }
                        return;
                    }
                    // get more work
                    if (directValuationsIndex < directValuationsList.Count)
                    {
                        directValuationsTemplate[directValuationsSlot] = directValuationsList[directValuationsIndex++];
                        if (IsIndividualValid(directValuationsTemplate))
                        {
                            swappedIndividual = GenerateIndividual(directValuationsTemplate, false, swappedIndividual);
                        }
                        else
                        {
                            swappedIndividual = null;
                        }
                    }
                    else
                    {
                        // everything is queued up already
                        startedThreads--;
                        return;
                    }
                }
            } while (true);
        }

        protected virtual KeyValuePair<float, TIndividual> LookForDirectItemUpgrades(List<TItem> items, int slot, float best, TIndividual bestIndividual, TItem[] itemList, out TValuation bestValuation)
        {
            bestValuation = default(TValuation);
            float value;
            bool foundUpgrade = false;
            if (itemList == null)
            {
                itemList = (TItem[])GetItems(bestIndividual).Clone();
            }
            if (ThreadPoolValuation)
            {
                bestDirectValue = best;
                directValuationFoundUpgrade = false;
                directValuationsIndex = 0;
                directValuationsComplete = 0;
                directValuationsList = items;
                directValuationsTemplate = itemList;
                directValuationsSlot = slot;
                remainingThreadsToSpawn = EffectiveMaxConcurrencyLevel;

                startedThreads = 1;
                ThreadPoolDirectUpgradeValuation(null);

                lock (directValuationLock)
                {
                    while (directValuationsComplete < directValuationsList.Count || startedThreads > 0) Monitor.Wait(directValuationLock);
                    if (directValuationFoundUpgrade)
                    {
                        best = bestDirectValue;
                        bestValuation = bestDirectValuation;
                        bestIndividual = bestDirectIndividual;
                        foundUpgrade = true;
                    }
                    bestDirectIndividual = null;
                    bestDirectValuation = default(TValuation);
                    directValuationsList = null;
                    directValuationsTemplate = null;
                }
            }
            else
            {
                foreach (TItem item in items)
                {
                    itemList[slot] = item;
                    if (IsIndividualValid(itemList))
                    {
                        TIndividual swappedIndividual = GenerateIndividual(itemList, false, null);
                        TValuation valuation;
                        value = GetOptimizationValue(swappedIndividual, valuation = GetValuation(swappedIndividual));
                        if (value > best)
                        {
                            best = value;
                            bestValuation = valuation;
                            bestIndividual = swappedIndividual;
                            foundUpgrade = true;
                        }
                    }
                }
            }
            if (foundUpgrade)
                return new KeyValuePair<float, TIndividual>(best, bestIndividual);
            return new KeyValuePair<float, TIndividual>(float.NegativeInfinity, null);
        }

        protected virtual float GetOptimizationValue(TIndividual individual)
        {
            return GetOptimizationValue(individual, GetValuation(individual));
        }

        // items can be used as a context to limit the choices if not all in the list are valid
        protected virtual TItem GetRandomItem(int slot, TItem[] items)
        {
            List<TItem> list = slotItems[slot];
            int count = list.Count;
            if (count == 0) return default(TItem);
            return list[Rnd.Next(count)];
        }

        protected virtual bool IsIndividualValid(TItem[] items)
        {
            foreach (OptimizerRangeValidatorBase<TItem> validator in validators)
            {
                if (!validator.IsValid(items)) return false;
            }
            return true;
        }

        protected virtual bool IsIndividualValid(TIndividual individual)
        {
            return IsIndividualValid(GetItems(individual));
        }

        protected abstract TValuation GetValuation(TIndividual individual);
        protected abstract float GetOptimizationValue(TIndividual individual, TValuation valuation);
        protected abstract TItem GetItem(TIndividual individual, int slot);
        /// <remarks>The returned array will be treated as readonly.</remarks>
        protected abstract TItem[] GetItems(TIndividual individual);
        /// <remarks>If you need to store the array make a copy unless canUseArray is true, because it might be reused and modified otherwise.</remarks>
        protected abstract TIndividual GenerateIndividual(TItem[] items, bool canUseArray, TIndividual recycledIndividual);

        /// <remarks>Similar to GetItems, but only return the array if it is readily available and can be reused</remarks>
        protected virtual TItem[] GetRecycledItems(TIndividual recycledIndividual)
        {
            return null;
        }

        protected delegate TItem GeneratorItemSelector(int slot, TItem[] items);

        protected virtual TIndividual GeneratorBuildIndividual(GeneratorItemSelector itemSelector, TIndividual recycledIndividual)
        {
            TItem[] item = GetRecycledItems(recycledIndividual) ?? new TItem[slotCount];
            int validatorIndex = 0;
            for (int slot = 0; slot < slotCount; slot++)
            {
                item[slot] = itemSelector(slot, item);
                // check if we have to validate
                while (validatorIndex < validators.Count && validators[validatorIndex].EndSlot == slot)
                {
                    // validate
                    if (!validators[validatorIndex].IsValid(item))
                    {
                        // rewind
                        slot = validators[validatorIndex].StartSlot - 1; // for will increment it
                        validatorIndex = validators.FindIndex(validator => validator.EndSlot > slot);
                        if (validatorIndex == -1) validatorIndex = validators.Count;
                        break;
                    }
                    else
                    {
                        validatorIndex++; // move to next validator
                    }
                }
            }

            return GenerateIndividual(item, true, recycledIndividual);
        }

        protected virtual TIndividual BuildCopyIndividual(TIndividual individual, TIndividual recycledIndividual)
        {
            if (SupportsRecycling)
            {
                TItem[] item = GetRecycledItems(recycledIndividual) ?? new TItem[slotCount];
                Array.Copy(GetItems(individual), 0, item, 0, slotCount);
                return GenerateIndividual(item, true, recycledIndividual);            
            }
            else
            {
                return individual;
            }
        }

        protected virtual TIndividual BuildRandomIndividual(TIndividual recycledIndividual)
        {
            return GeneratorBuildIndividual(GetRandomItem, recycledIndividual);
        }

        protected virtual TIndividual BuildChildIndividual(TIndividual father, TIndividual mother, TIndividual recycledIndividual)
        {
            return GeneratorBuildIndividual(
                delegate(int slot, TItem[] items)
                {
                    return Rnd.NextDouble() < 0.5d ? GetItem(father, slot) : GetItem(mother, slot);
                },
                recycledIndividual);
        }

        /// <summary>
        /// This is funtions decides wether we take a new character or drop it
        /// Author: ebo
        /// </summary>
        private bool SAAcceptance(double e, double enew, double T, Random R)
        {
            // Always accept character if its better
            if (enew > e)
            {
                return true;
            }
            else
            {
                // Accept based on difference and temperature
                // higher temperature means bigger differences possible (or likely)
                // see http://en.wikipedia.org/wiki/Simulated_annealing
                double chance = Math.Exp((enew - e) / T);
                return chance > R.NextDouble();
            }

        }

        /// <summary>
        /// This is funtions clones a character and changes one item and based on a probability one enchant
        /// Author: ebo
        /// </summary>
        protected virtual TIndividual GeneratorBuildSACharacter(TIndividual parent)
        {
            TItem[] item = new TItem[slotCount];

            for (int slot = 0; slot < slotCount; slot++)
            {
                item[slot] = GetItem(parent, slot);
            }
            Random rand = Rnd;
            double r = rand.NextDouble();
            bool successfull = false;

            r = rand.NextDouble();

            while (!successfull)
            {
                // Make sure to change one item
                // There are better methods to make sure to change one item (shuffled list of all slots) but this works
                int slot = rand.Next(slotCount);
                TItem newitem = GetRandomItem(slot, item);
                TItem oldItem = item[slot];
                item[slot] = newitem;
                if (IsIndividualValid(item))
                {
                    successfull = true;
                }
                else
                {
                    item[slot] = oldItem;
                }
            }

            return GenerateIndividual(item, true, null);
        }

        protected virtual TIndividual BuildMutantIndividual(TIndividual parent, TIndividual recycledIndividual)
        {
            Random rand = Rnd;
            int targetMutations = 2;
            while (targetMutations < 32 && rand.NextDouble() < 0.75d) targetMutations++;
            double mutationChance = (double)targetMutations / 32d;

            return GeneratorBuildIndividual(
                delegate(int slot, TItem[] items)
                {
                    if (Rnd.NextDouble() < mutationChance)
                    {
                        return GetRandomItem(slot, items);
                    }
                    else
                    {
                        return GetItem(parent, slot);
                    }
                },
                recycledIndividual);
        }

        protected virtual TIndividual BuildSingleItemSwapIndividual(TIndividual baseIndividual, int replaceSlot, TItem replaceItem)
        {
            TItem[] item = new TItem[slotCount];
            Array.Copy(GetItems(baseIndividual), item, slotCount);
            item[replaceSlot] = replaceItem;
            return GenerateIndividual(item, true, null);
        }
    }
}
