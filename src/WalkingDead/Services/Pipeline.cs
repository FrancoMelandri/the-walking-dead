namespace WalkingDead;

public class Pipeline
{
    private readonly IServiceOne _serviceOne;
    private readonly IServiceTwo _serviceTwo;
    private readonly IServiceThree _serviceThree;
    private readonly IServiceFour _serviceFour;

    public Pipeline(IServiceOne serviceOne,
        IServiceTwo serviceTwo,
        IServiceThree serviceThree,
        IServiceFour serviceFour)
    {
        _serviceOne = serviceOne;
        _serviceTwo = serviceTwo;
        _serviceThree = serviceThree;
        _serviceFour = serviceFour;
    }

    public string Flow(FlowContext context)
    {
        var first = _serviceOne.Action(new ServiceOneRequest { Id = context.Id });
        if (first != null)
        {
            var second = _serviceTwo.Action(new ServiceTwoRequest { Id = first.Id });
            if (second != null)
            {
                var third = _serviceThree.Action(new ServiceThreeRequest { Id = second.Id });
                if (third != null)
                {
                    var fourth = _serviceFour.Action(new ServiceFourRequest { Id = third.Id });
                    if (fourth != null)
                        return fourth.Id;
                    return "error-4";
                }
                return "error-3";
            }
            return "error-2";
        }
        return "error-1";
    }
}