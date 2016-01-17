using UnityEngine;
using System.Collections;
using FiniteStateMachine;

public class Enemy : MonoBehaviour 
{
	const int MAX_HEALTH = 200;
	const int LOOK_FOR_MEDIKIT = 50;

	[Range(0, MAX_HEALTH)] 
	public int health = MAX_HEALTH;
	public bool playerIsInSight   = false;
	public bool playerIsAttacking = false;

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

		brain.RegisterTransition("wander", "attack", WanderToAttack);
		brain.RegisterTransition("attack", "wander", AttackToWander);
		brain.RegisterTransition("attack", "evade",  AttackToEvade);
		brain.RegisterTransition("evade",  "attack", EvadeToAttack);
		brain.RegisterTransition("evade",  "heal",   EvadeToHeal);
		brain.RegisterTransition("heal",   "wander", HealToWander);
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

public class Wander : IState 
{
	public void Update(float deltaTime) {}
	public void OnEnterState() 
	{
		Debug.Log("I'm happily wandering");
	}
	public void OnExitState() 
	{
		Debug.Log("Not so happy anymore");
	}
}

public class Attack : IState 
{
	public void Update(float deltaTime) {}
	public void OnEnterState() 
	{
		Debug.Log("Drawing my sword!");
	}
	public void OnExitState() 
	{
		Debug.Log("Sword sheated again");
	}
}

public class Evade : IState 
{
	public void Update(float deltaTime) {}
	public void OnEnterState() 
	{
		Debug.Log("Running like a coward");
	}
	public void OnExitState() 
	{
		Debug.Log("Fine, no more running like a coward");
	}
}

public class Heal : IState 
{
	public void Update(float deltaTime) {}
	public void OnEnterState() 
	{
		Debug.Log("Oh crap, I need a doctor");
	}
	public void OnExitState() 
	{
		Debug.Log("Leaving the doctor");
	}
}