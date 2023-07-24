using TinyFp;
using TinyFp.Extensions;

namespace WalkingDead;

public interface IStepFour : IStep
{}

public class StepFour : IStepFour
{
    private readonly IServiceFour _serviceFour;

    public StepFour(IServiceFour serviceFour)
    {
        _serviceFour = serviceFour;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _serviceFour
            .Action(new ServiceFourRequest { Id = flow.Action3.Unwrap().Id })
            .ToOption()
            .Map(_ => flow.Tee(__ => __.Action4 = _.ToOption()))
            .Match(Either<string, FlowReducer>.Right,
                   () => Either<string, FlowReducer>.Left("error-4"));
}

