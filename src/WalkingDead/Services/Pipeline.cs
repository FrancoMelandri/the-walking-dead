using TinyFp.Extensions;

namespace WalkingDead;

public class Pipeline
{
    private readonly IStepOne _stepOne;
    private readonly IStepTwo _stepTwo;
    private readonly IStepThree _stepThree;
    private readonly IStepFour _stepFour;

    public Pipeline(IStepOne stepOne,
        IStepTwo stepTwo,
        IStepThree stepThree,
        IStepFour stepFour)
    {
        _stepOne = stepOne;
        _stepTwo = stepTwo;
        _stepThree = stepThree;
        _stepFour = stepFour;
    }

    public string Flow(FlowContext context)
        =>_stepOne
                .Forward(new FlowReducer { FlowContext = context.ToOption() })
                .Bind(_stepTwo.Forward)
                .Bind(_stepThree.Forward)
                .Bind(_stepFour.Forward)
                .Match(_ => _.Action4.Unwrap().Id, _ => _);
}