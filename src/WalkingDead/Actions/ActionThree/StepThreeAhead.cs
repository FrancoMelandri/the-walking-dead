using TinyFp;

namespace WalkingDead;

public class StepThreeAhead : IStepThree
{
    private readonly IStepThree _stepThree;

    public StepThreeAhead(IStepThree stepThree)
    {
        _stepThree = stepThree;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => flow
            .Action3
            .Match(_ => Either<string, FlowReducer>.Right(flow),
                   () => _stepThree.Forward(flow));
}

