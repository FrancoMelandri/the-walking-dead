using TinyFp;
using TinyFp.Extensions;

namespace WalkingDead;

public interface IStepTwo : IStep
{}

public class StepTwo : IStepTwo
{
    private readonly IServiceTwo _serviceTwo;

    public StepTwo(IServiceTwo serviceTwo)
    {
        _serviceTwo = serviceTwo;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _serviceTwo
            .Action(new ServiceTwoRequest { Id = flow.Action1.Unwrap().Id })
            .ToOption()
            .Map(_ => flow.Tee(__ => __.Action2 = _.ToOption()))
            .Match(Either<string, FlowReducer>.Right,
                   () => Either<string, FlowReducer>.Left("error-2"));
}

