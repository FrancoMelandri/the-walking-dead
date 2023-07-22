using TinyFp;

namespace WalkingDeath;

public class Pipeline
{
    public Either<string, Unit> Flow(FlowContext context)
        => Either<string, Unit>.Left(context.Id);
}