using System.Text.Json;
using TinyFp.Extensions;

namespace WalkingDead;

public class WalkingDeadStepFour : IWalkingDeadSubject
{
    public FlowReducer Accept(FlowReducer reducer, StepEntity dead)
        => dead
            .ToOption(_ => _.Step != Steps.Step4)
            .Map(_ => reducer.Tee(__ => reducer.Action4 = JsonSerializer.Deserialize<ServiceFourResponse>(_.Output).ToOption()))
            .OrElse(reducer);
}