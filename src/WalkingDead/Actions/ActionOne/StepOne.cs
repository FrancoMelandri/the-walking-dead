using TinyFp;
using TinyFp.Extensions;

namespace WalkingDead;

public interface IStepOne : IStep
{}

public class StepOne : IStepOne
{
    private readonly IServiceOne _serviceOne;

    public StepOne(IServiceOne serviceOne)
    {
        _serviceOne = serviceOne;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _serviceOne
            .Action(new ServiceOneRequest { Id = flow.FlowContext.Id })
            .ToOption()
            .Map(_ => flow.Tee(__ => __.Action1 = _))
            .Match(Either<string, FlowReducer>.Right,
                   () => Either<string, FlowReducer>.Left("error-1"));
}

