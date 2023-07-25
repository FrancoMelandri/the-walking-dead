using System.Text.Json;
using TinyFp;

namespace WalkingDead;

public class StepFourMemento : IStepFour
{
    private readonly IStepFour _stepFour;
    private readonly IStepRepository _stepRepository;

    public StepFourMemento(IStepFour stepFour,
                           IStepRepository stepRepository)
    {
        _stepFour = stepFour;
        _stepRepository = stepRepository;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _stepFour.Forward(flow)
            .Bind(_ => _stepRepository.Upsert(new StepEntity
            {
                Context = flow.FlowContext.Unwrap().Id,
                Step = Steps.Step4,
                Output = JsonSerializer.Serialize(_.Action4.Unwrap())
            }))
            .Map(_ => flow);
}

