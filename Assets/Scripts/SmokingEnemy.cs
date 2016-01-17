using UnityEngine;
using System.Collections;
using FiniteStateMachine.Stacked;

public class SmokingEnemy : MonoBehaviour 
{
	const int MAX_HEALTH = 200;
	const int LOOK_FOR_MEDIKIT = 50;

	[Range(0, MAX_HEALTH)] 
	public int health = MAX_HEALTH;
	public bool playerIsInSight   = false;
	public bool playerIsAttacking = false;
	public bool smoke = false;

	private FSM brain = new FSM();


	void Update()
	{
		brain.Update(Time.deltaTime);
	}

	void Start()
	{
		SetupBrain();
	}

	private void SetupBrain()
	{
		brain.RegisterState("wander", new Wander());
		brain.RegisterState("attack", new Attack());
		brain.RegisterState("evade",  new Evade());
		brain.RegisterState("heal",	  new Heal());
		brain.RegisterState("smoke",  new Smoke());

		brain.RegisterTransition("wander", "attack", WanderToAttack);
		brain.RegisterTransition("attack", "wander", AttackToWander);
		brain.RegisterTransition("attack", "evade",  AttackToEvade);
		brain.RegisterTransition("evade",  "attack", EvadeToAttack);
		brain.RegisterTransition("evade",  "heal",   EvadeToHeal);
		brain.RegisterTransition("heal",   "wander", HealToWander);

		brain.RegisterTransition("wander", "smoke",  StartSmoking, TransitionType.StorePreviousState);
		brain.RegisterTransition("attack", "smoke",  StartSmoking, TransitionType.StorePreviousState);
		brain.RegisterTransition("evade",  "smoke",  StartSmoking, TransitionType.StorePreviousState);
		brain.RegisterTransition("heal",   "smoke",  StartSmoking, TransitionType.StorePreviousState);
		brain.RegisterTransitionToPrevious("smoke",  StopSmoking);
	}

	bool StartSmoking()
	{
		return smoke;
	}

	bool StopSmoking()
	{
		return !smoke;
	}

	bool WanderToAttack()
	{
		return playerIsInSight;
	}

	bool AttackToWander()
	{
		return !playerIsInSight;
	}

	bool AttackToEvade()
	{
		return playerIsAttacking;
	}

	bool EvadeToAttack()
	{
		return !playerIsAttacking;
	}

	bool EvadeToHeal()
	{
		return health <= LOOK_FOR_MEDIKIT;
	}

	bool HealToWander()
	{
		return health == MAX_HEALTH;
	}

}
	

public class Smoke : FiniteStateMachine.IState 
{
	public void Update(float deltaTime) {}
	public void OnEnterState() 
	{
		Debug.Log("<color='red'>I smoke, I'm so cool...</color>");
	}
	public void OnExitState() 
	{
		Debug.Log("<color='green'>Ok, I'm quitting smoking</color>");
	}
}