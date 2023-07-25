using TinyFp;

namespace WalkingDead;

public class StepTwoAhead : IStepTwo
{
    private readonly IStepTwo _stepTwo;

    public StepTwoAhead(IStepTwo stepTwo)
    {
        _stepTwo = stepTwo;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => flow
            .Action2
            .Match(_ => Either<string, FlowReducer>.Right(flow),
                   () => _stepTwo.Forward(flow));
}

