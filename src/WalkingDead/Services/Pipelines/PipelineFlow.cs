using TinyFp;
using TinyFp.Extensions;

namespace WalkingDead;

public class PipelineFlow
{
    private readonly IPipeline _pipeline;

    public PipelineFlow(IPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public Either<string, Unit> Flow(FlowContext context)
        => _pipeline
            .Execute(new FlowReducer { FlowContext = context.ToOption() });
}
