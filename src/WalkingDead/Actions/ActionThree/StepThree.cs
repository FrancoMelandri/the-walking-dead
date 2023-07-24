using TinyFp;
using TinyFp.Extensions;

namespace WalkingDead;

public interface IStepThree : IStep
{}

public class StepThree : IStepThree
{
    private readonly IServiceThree _serviceThree;

    public StepThree(IServiceThree serviceThree)
    {
        _serviceThree = serviceThree;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _serviceThree
            .Action(new ServiceThreeRequest { Id = flow.Action2.Id })
            .ToOption()
            .Map(_ => flow.Tee(__ => __.Action3 = _))
            .Match(Either<string, FlowReducer>.Right,
                   () => Either<string, FlowReducer>.Left("error-3"));
}

