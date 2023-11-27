using Godot;
using static Godot.GD;


[GlobalClass]
public partial class StateMachine : Node
{
	private State _currentState = State.None;

	public double stateTime;
	IStateMachine owner;
	public State currentState
	{
		set
		{
			// Owner.trasition_state(value);
			// Print($"from: {_currentState} to: {value}");
			owner.TransitionState(_currentState, value);
			stateTime = 0;
			_currentState = value;
		}
		get { return _currentState; }
	}



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Owner._Ready();
		// await ToSignal(Owner, "ready");
		Print($"{Owner} is ready");
		owner = (IStateMachine)Owner;
		currentState = State.Idle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// while (true)
		// {
		// 	var next = owner.GetNextState(currentState);
		// 	if (next == currentState) break;
		// 	currentState = next;
		// }
		var next = owner.GetNextState(currentState);
		currentState = next;
		owner.TickPhysics(currentState, delta);
		stateTime += delta;
	}
}
