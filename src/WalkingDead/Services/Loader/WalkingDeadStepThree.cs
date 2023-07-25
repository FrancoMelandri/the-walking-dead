using System.Text.Json;
using TinyFp.Extensions;

namespace WalkingDead;

public class WalkingDeadStepThree : IWalkingDeadSubject
{
    public FlowReducer Accept(FlowReducer reducer, StepEntity dead)
        => dead
            .ToOption(_ => _.Step != Steps.Step3)
            .Map(_ => reducer.Tee(__ => reducer.Action3 = JsonSerializer.Deserialize<ServiceThreeResponse>(_.Output).ToOption()))
            .OrElse(reducer);
}
