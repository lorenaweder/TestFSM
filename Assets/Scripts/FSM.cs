using UnityEngine;
using System.Collections.Generic;

namespace FiniteStateMachine 
{
	public delegate bool TransitionCondition();

	// TODO IState en Stacked que contemple OnSleep / OnAwake de estados en cola
	public interface IState
	{
		void Update(float deltaTime);
		void OnEnterState();
		void OnExitState();
	}

	public class EmptyState : IState 
	{
		public void Update(float deltaTime) {}
		public void OnEnterState() {}
		public void OnExitState() {}
	}

	public class FSM 
	{
		private IState activeState;
		private Dictionary<string, IState> states;
		private Dictionary<IState, List<StateTransition>> transitions;

		public FSM()
		{
			activeState = new EmptyState();
			states = new Dictionary<string, IState>();
			transitions = new Dictionary<IState, List<StateTransition>>();
		}

		public void RegisterState(string name, IState state)
		{
			states.Add(name, state);
			if(states.Count == 1)
				TransitionTo(name);
		}

		public void RegisterTransition(string fromStateName, string toStateName, TransitionCondition condition)
		{
			IState fromState;
			
			if( !(states.TryGetValue(fromStateName, out fromState) && states.ContainsKey(toStateName) ) )
				return;

			if(!transitions.ContainsKey(fromState))
				transitions.Add(fromState, new List<StateTransition>());

			transitions[fromState].Add(new StateTransition(toStateName, condition));
		}

		public void Update(float deltaTime)
		{
			activeState.Update(deltaTime);

			foreach(StateTransition t in transitions[activeState])
			{
				if(t.EvaluateCondition())
				{
					TransitionTo(t.desiredState);
				}
			}
		}

		public void TransitionTo(string stateName)
		{
			activeState.OnExitState();
			activeState = states[stateName];
			activeState.OnEnterState();
		}

	}

	public class StateTransition
	{
		public StateTransition(string desiredState, TransitionCondition condition)
		{
			this.desiredState = desiredState;
			this.condition = condition;
		}

		public bool EvaluateCondition()
		{
			return condition();
		}
		
		public string desiredState;
		TransitionCondition condition;
	}

	namespace Stacked 
	{		
		public enum TransitionType
		{
			ReplacePreviousState,
			StorePreviousState, 
			RemoveCurrentState
		}

		public class FSM 
		{
			private Stack<IState> activeStates;
			private Dictionary<string, IState> states;
			private Dictionary<IState, List<StateTransition>> transitions;

			public FSM()
			{
				activeStates = new Stack<IState>();
				activeStates.Push(new EmptyState());

				states = new Dictionary<string, IState>();
				transitions = new Dictionary<IState, List<StateTransition>>();
			}

			public void RegisterState(string name, IState state)
			{
				states.Add(name, state);
				if(states.Count == 1)
					TransitionTo(name, TransitionType.ReplacePreviousState);
			}

			public void RegisterTransitionToPrevious(string stateName, TransitionCondition condition)
			{
				RegisterTransition(stateName, "previous", condition, TransitionType.RemoveCurrentState);
			}

			public void RegisterTransition(string fromStateName, string toStateName, TransitionCondition condition, TransitionType transition = TransitionType.ReplacePreviousState)
			{
				IState fromState;

				if( !(states.TryGetValue(fromStateName, out fromState) && (transition == TransitionType.RemoveCurrentState || states.ContainsKey(toStateName)) ) )
					return;

				if(!transitions.ContainsKey(fromState))
					transitions.Add(fromState, new List<StateTransition>());

				transitions[fromState].Add(new StateTransition(toStateName, condition, transition));
			}

			public void Update(float deltaTime)
			{
				activeStates.Peek().Update(deltaTime);

				foreach(StateTransition t in transitions[activeStates.Peek()])
				{
					if(t.EvaluateCondition())
					{
						TransitionTo(t.desiredState, t.transition);
					}
				}
			}

			public void TransitionTo(string stateName, TransitionType transition)
			{
				switch(transition)
				{
				case TransitionType.StorePreviousState:
					activeStates.Peek().OnExitState();
					activeStates.Push(states[stateName]);
					activeStates.Peek().OnEnterState();
					break;
				case TransitionType.RemoveCurrentState:
					activeStates.Pop().OnExitState();
					activeStates.Peek().OnEnterState();
					break;
				case TransitionType.ReplacePreviousState:
					activeStates.Pop().OnExitState();
					activeStates.Push(states[stateName]);
					activeStates.Peek().OnEnterState();
					break;				
				}
			}

		}

		public class StateTransition
		{
			public string desiredState;
			TransitionCondition condition;
			public TransitionType transition;

			public StateTransition(string desiredState, TransitionCondition condition, TransitionType transition)
			{
				this.desiredState = desiredState;
				this.condition = condition;
				this.transition = transition;
			}

			public bool EvaluateCondition()
			{
				return condition();
			}
		}
	}
}