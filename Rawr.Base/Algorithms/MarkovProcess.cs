using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Base.Algorithms
{
    public class MarkovProcess<TAbility>
    {
        public List<State<TAbility>> StateSpace { get; private set; }
        public double[] StateWeight { get; private set; }
        public Dictionary<TAbility, double> AbilityWeight { get; private set; }
        public double AverageTransitionDuration { get; private set; }

#if SILVERLIGHT
        public MarkovProcess(List<State<TAbility>> stateSpace)
#else
        public unsafe MarkovProcess(List<State<TAbility>> stateSpace)
#endif
        {
            StateSpace = stateSpace;
            for (int i = 0; i < StateSpace.Count; i++)
            {
                StateSpace[i].Index = i;
            }

            int size = StateSpace.Count + 1;

            LU.ArraySet arraySet = ArrayPool<LU.ArraySet>.RequestArraySet();
            LU M = new LU(size, arraySet);

            StateWeight = new double[size];

#if SILVERLIGHT
            M.BeginSafe();

            Array.Clear(arraySet.LU_U, 0, size * size);

            //U[i * rows + j]

            foreach (State<TAbility> state in StateSpace)
            {
                foreach (StateTransition<TAbility> transition in state.Transitions)
                {
                    arraySet.LU_U[transition.TargetState.Index * size + state.Index] += transition.TransitionProbability;
                }
                arraySet.LU_U[state.Index * size + state.Index] -= 1.0;
            }

            for (int i = 0; i < size - 1; i++)
            {
                arraySet.LU_U[(size - 1) * size + i] = 1;
            }

            StateWeight[size - 1] = 1;

            M.Decompose();
            M.FSolve(StateWeight);

            M.EndUnsafe();            
#else
            fixed (double* U = arraySet.LU_U, x = StateWeight)
            fixed (double* sL = arraySet.LUsparseL, column = arraySet.LUcolumn, column2 = arraySet.LUcolumn2)
            fixed (int* P = arraySet.LU_P, Q = arraySet.LU_Q, LJ = arraySet.LU_LJ, sLI = arraySet.LUsparseLI, sLstart = arraySet.LUsparseLstart)
            {
                M.BeginUnsafe(U, sL, P, Q, LJ, sLI, sLstart, column, column2);

                Array.Clear(arraySet.LU_U, 0, size * size);

                //U[i * rows + j]

                foreach (State<TAbility> state in StateSpace)
                {
                    foreach (StateTransition<TAbility> transition in state.Transitions)
                    {
                        U[transition.TargetState.Index * size + state.Index] += transition.TransitionProbability;
                    }
                    U[state.Index * size + state.Index] -= 1.0;
                }

                for (int i = 0; i < size - 1; i++)
                {
                    U[(size - 1) * size + i] = 1;
                }

                x[size - 1] = 1;

                M.Decompose();
                M.FSolve(x);

                M.EndUnsafe();
            }
#endif

            AbilityWeight = new Dictionary<TAbility, double>();

            foreach (State<TAbility> state in StateSpace)
            {
                double stateWeight = StateWeight[state.Index];
                if (stateWeight > 0)
                {
                    foreach (StateTransition<TAbility> transition in state.Transitions)
                    {
                        double transitionProbability = transition.TransitionProbability;
                        if (transitionProbability > 0)
                        {
                            double p = stateWeight * transitionProbability;
                            if (transition.Ability != null)
                            {
                                double weight;
                                AbilityWeight.TryGetValue(transition.Ability, out weight);
                                AbilityWeight[transition.Ability] = weight + p;
                            }
                            AverageTransitionDuration += p * transition.TransitionDuration;
                        }
                    }
                }
            }
           
            ArrayPool<LU.ArraySet>.ReleaseArraySet(arraySet);
        }
    }
}
