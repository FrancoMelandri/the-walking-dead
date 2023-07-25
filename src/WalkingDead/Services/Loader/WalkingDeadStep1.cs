using System.Text.Json;
using TinyFp.Extensions;

namespace WalkingDead;

public class WalkingDeadStepOne : IWalkingDeadSubject
{
    public FlowReducer Accept(FlowReducer reducer, StepEntity dead)
        => dead
            .ToOption(_ => _.Step != Steps.Step1)
            .Map(_ => reducer.Tee(__ => reducer.Action1 = JsonSerializer.Deserialize<ServiceOneResponse>(_.Output).ToOption()))
            .OrElse(reducer);
}
