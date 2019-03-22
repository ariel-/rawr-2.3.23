using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
    public partial class SpecialEffect
    {
        public abstract class Interpolator
        {
            protected float[,] grid;
            protected float procChanceMin;
            protected float procChanceMax;
            protected const int procChanceN = 100;
            protected float intervalMin;
            protected float intervalMax;
            protected const int intervalN = 100;
            protected float fightDuration;
            protected bool discretizationCorrection;
            protected float procFactor;
            protected float intervalFactor;

            public Interpolator(float fightDuration, bool discretizationCorrection)
            {
                this.fightDuration = fightDuration;
                this.discretizationCorrection = discretizationCorrection;
            }

            public float this[float procChance, float interval]
            {
                get
                {
                    bool updateGrid = false;
                    if (grid == null)
                    {
                        procChanceMin = 0.5f * procChance;
                        procChanceMax = Math.Min(1.0f, 1.5f * procChance);
                        intervalMin = 0.5f * interval;
                        intervalMax = 1.5f * interval;
                        grid = new float[procChanceN + 1, intervalN + 1];
                        updateGrid = true;
                    }
                    if (procChance < procChanceMin)
                    {
                        procChanceMin = 0.5f * procChance;
                        updateGrid = true;
                    }
                    if (procChance > procChanceMax)
                    {
                        procChanceMax = Math.Min(1.0f, 1.5f * procChance);
                        updateGrid = true;
                    }
                    if (interval < intervalMin)
                    {
                        intervalMin = 0.5f * interval;
                        updateGrid = true;
                    }
                    if (interval >= intervalMax)
                    {
                        intervalMax = 1.5f * interval;
                        updateGrid = true;
                    }
                    if (updateGrid)
                    {
                        procFactor = procChanceN / (procChanceMax - procChanceMin);
                        intervalFactor = intervalN / (intervalMax - intervalMin);
                        UpdateGrid();
                    }
                    //float p = (procChance - procChanceMin) / (procChanceMax - procChanceMin);
                    //float x = p * procChanceN;
                    float x = (procChance - procChanceMin) * procFactor;
                    int i = (int)x;
                    x -= i;
                    //float ivl = (interval - intervalMin) / (intervalMax - intervalMin);
                    //float y = ivl * intervalN;
                    float y = (interval - intervalMin) * intervalFactor;
                    int j = (int)y;
                    y -= j;
                    if (i >= procChanceN)
                    {
                        // treat 100% proc chance or higher as 100%, might want to consider throwing exception instead
                        return grid[procChanceN, j] + y * (grid[procChanceN, j + 1] - grid[procChanceN, j]);
                    }
                    else
                    {
                        float v0 = grid[i, j] + x * (grid[i + 1, j] - grid[i, j]);
                        float v1 = grid[i, j + 1] + x * (grid[i + 1, j + 1] - grid[i, j + 1]);
                        return v0 + y * (v1 - v0);
                    }
                }
            }

            private void UpdateGrid()
            {
                for (int i = 0; i <= procChanceN; i++)
                {
                    for (int j = 0; j <= intervalN; j++)
                    {
                        grid[i, j] = Evaluate(procChanceMin + (float)i / procChanceN * (procChanceMax - procChanceMin), intervalMin + (float)j / intervalN * (intervalMax - intervalMin));
                    }
                }
            }

            protected abstract float Evaluate(float procChance, float interval);
        }
    }
}
