using TinyFp;
using TinyFp.Extensions;

namespace WalkingDead;

public class PipelineRetry
{
    private readonly IStepLoaderRepository _stepLoaderRepository;
    private readonly IPipeline _pipeline;
    private readonly IWalkingDeadVisitor _walkingDeadVisitor;

    public PipelineRetry(IStepLoaderRepository stepLoaderRepository,
                         IPipeline pipeline,
                         IWalkingDeadVisitor walkingDeadVisitor)
    {
        _stepLoaderRepository = stepLoaderRepository;
        _pipeline = pipeline;
        _walkingDeadVisitor = walkingDeadVisitor;
    }

    public Either<string, Unit> Retry()
        =>_stepLoaderRepository
            .StillWalking()
            .Bind(OrderToWalk);

    private Either<string, Unit> OrderToWalk(string[] walkingDead)
        => walkingDead
            .Fold(Either<string, Unit>.Right(Unit.Default),
                  (a, i) => OrderToWalk(i));

    private Either<string, Unit> OrderToWalk(string walkingDead)
        => _stepLoaderRepository
            .StillWalking(walkingDead)
            .Bind(_ => OrderToWalk(walkingDead, _));

    private Either<string, Unit> OrderToWalk(string walkingDead, StepEntity[] steps)
        => _walkingDeadVisitor
            .Visit(walkingDead, steps)
            .Map(_pipeline.Execute);
}