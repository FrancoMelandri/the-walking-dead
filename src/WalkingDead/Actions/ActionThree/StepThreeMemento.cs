using System.Text.Json;
using TinyFp;

namespace WalkingDead;

public class StepThreeMemento : IStepThree
{
    private readonly IStepThree _stepThree;
    private readonly IStepRepository _stepRepository;

    public StepThreeMemento(IStepThree stepThree,
                          IStepRepository stepRepository)
    {
        _stepThree = stepThree;
        _stepRepository = stepRepository;
    }

    public Either<string, FlowReducer> Forward(FlowReducer flow)
        => _stepThree.Forward(flow)
            .Bind(_ => _stepRepository.Upsert(new StepEntity
            {
                Context = flow.FlowContext.Unwrap().Id,
                Step = Steps.Step3,
                Output = JsonSerializer.Serialize(_.Action3.Unwrap())
            }))
            .Map(_ => flow);
}

