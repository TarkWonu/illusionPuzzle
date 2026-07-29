using System;

[Serializable]
public abstract class Condition
{
    public abstract bool IsCorrect();
    
}

[Serializable]
public class RotateCondition : Condition
{
    public RotatePath rotate;
    public PathRotateState rotateState;

    public override bool IsCorrect()
    {
        return rotate.pathRotate== rotateState;
    }
}

[Serializable]
public class MoveCondition : Condition
{
    public ElevatorPath elevate;
    public PathMoveState elevateState;

    public override bool IsCorrect()
    {
        return elevate.moveState== elevateState;
    }
}

[Serializable]
public class SeesawCondition : Condition
{
    public SeesawController seesaw;
    public SeesawState seesawState;

    public override bool IsCorrect()
    {
        return seesaw.state == seesawState;
    }
} 