using System.Text.Json;
using TinyFp;

namespace WalkingDead;

public class StepOneMemento : IStepOne
{
    private readonly IStepOne _stepOne;
    private readonly IStepRepository _stepRepository;

    public StepOneMemento(IStepOne stepOne,
                          IStepRepository stepRepository)
    {
        _stepOne = stepOne;
        _stepRepository = stepRepository;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _stepOne.Forward(flow)
            .Bind(_ => _stepRepository.Upsert(new StepEntity
            {
                Context = flow.FlowContext.Unwrap().Id,
                Step = Steps.Step1,
                Output = JsonSerializer.Serialize(_.Action1.Unwrap())
            }))
            .Map(_ => flow);
}

