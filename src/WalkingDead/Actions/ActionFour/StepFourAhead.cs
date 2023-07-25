using TinyFp;

namespace WalkingDead;

public class StepFourAhead : IStepFour
{
    private readonly IStepFour _stepFour;

    public StepFourAhead(IStepFour stepFour)
    {
        _stepFour = stepFour;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => flow
            .Action4
            .Match(_ => Either<string, FlowReducer>.Right(flow),
                   () => _stepFour.Forward(flow));
}

