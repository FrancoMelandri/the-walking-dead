using System.Text.Json;
using TinyFp;

namespace WalkingDead;

public class StepTwoMemento : IStepTwo
{
    private readonly IStepTwo _stepTwo;
    private readonly IStepRepository _stepRepository;

    public StepTwoMemento(IStepTwo stepTwo,
                          IStepRepository stepRepository)
    {
        _stepTwo = stepTwo;
        _stepRepository = stepRepository;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _stepTwo.Forward(flow)
            .Bind(_ => _stepRepository.Upsert(new StepEntity
            {
                Context = flow.FlowContext.Unwrap().Id,
                Step = Steps.Step2,
                Output = JsonSerializer.Serialize(_.Action2.Unwrap())
            }))
            .Map(_ => flow);
}

