using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Base.Algorithms
{
    public class State<TAbility>
    {
        public List<StateTransition<TAbility>> Transitions { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class StateTransition<TAbility>
    {
        public State<TAbility> TargetState { get; set; }
        public TAbility Ability { get; set; }
        public virtual double TransitionProbability { get; set; }
        public virtual double TransitionDuration { get; set; }
    }

    public abstract class StateSpaceGenerator<TAbility>
    {
        private class ObjectComparer : IEqualityComparer<State<TAbility>>
        {
            bool IEqualityComparer<State<TAbility>>.Equals(State<TAbility> x, State<TAbility> y)
            {
                return (object)x == (object)y;
            }

            int IEqualityComparer<State<TAbility>>.GetHashCode(State<TAbility> obj)
            {
                return obj.GetHashCode();
            }
        }

        public List<State<TAbility>> GenerateStateSpace()
        {
            List<State<TAbility>> remainingStates = new List<State<TAbility>>();
            List<State<TAbility>> processedStates = new List<State<TAbility>>();
            // we want to do object comparisons, we rely on getting unique state instances and we want to use fast comparisons
            // in case derived class implements IEquatable interface or similar
            Dictionary<State<TAbility>, bool> seenStates = new Dictionary<State<TAbility>, bool>(new ObjectComparer());
            State<TAbility> initState = GetInitialState();
            remainingStates.Add(initState);
            seenStates[initState] = true;

            while (remainingStates.Count > 0)
            {
                State<TAbility> state = remainingStates[remainingStates.Count - 1];
                remainingStates.RemoveAt(remainingStates.Count - 1);

                List<StateTransition<TAbility>> transitions = GetStateTransitions(state);
                state.Transitions = transitions;
                foreach (StateTransition<TAbility> transition in transitions)
                {
                    if (transition.TargetState != state && !seenStates.ContainsKey(transition.TargetState))
                    {
                        remainingStates.Add(transition.TargetState);
                        seenStates[transition.TargetState] = true;
                    }
                }

                processedStates.Add(state);
            }

           return processedStates;
        }

        protected abstract State<TAbility> GetInitialState();
        protected abstract List<StateTransition<TAbility>> GetStateTransitions(State<TAbility> state);
    }
}
