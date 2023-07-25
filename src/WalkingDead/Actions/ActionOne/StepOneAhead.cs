using TinyFp;

namespace WalkingDead;

public class StepOneAhead : IStepOne
{
    private readonly IStepOne _stepOne;

    public StepOneAhead(IStepOne stepOne)
    {
        _stepOne = stepOne;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => flow
            .Action1
            .Match(_ => Either<string, FlowReducer>.Right(flow),
                   () => _stepOne.Forward(flow));
}

