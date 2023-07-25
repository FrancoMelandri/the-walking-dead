using System.Text.Json;
using TinyFp.Extensions;

namespace WalkingDead;

public class WalkingDeadStepTwo : IWalkingDeadSubject
{
    public FlowReducer Accept(FlowReducer reducer, StepEntity dead)
        => dead
            .ToOption(_ => _.Step != Steps.Step2)
            .Map(_ => reducer.Tee(__ => reducer.Action2 = JsonSerializer.Deserialize<ServiceTwoResponse>(_.Output).ToOption()))
            .OrElse(reducer);
}
