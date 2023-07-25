using TinyFp.Extensions;

namespace WalkingDead;

public interface IWalkingDeadVisitor
{
    FlowReducer Visit(string walkingDead, StepEntity[] deads);
}

public interface IWalkingDeadSubject
{
    FlowReducer Accept(FlowReducer reducer, StepEntity dead);
}

public class WalkingDeadVisitor : IWalkingDeadVisitor
{
    private readonly IWalkingDeadSubject[] _walkingDeadSubjects;

    public WalkingDeadVisitor(IWalkingDeadSubject[] walkingDeadSubjects)
    {
        _walkingDeadSubjects = walkingDeadSubjects;
    }

    public FlowReducer Visit(string walkingDead, StepEntity[] deads)
        => deads
            .Fold(new FlowReducer { FlowContext = new FlowContext { Id = walkingDead }.ToOption() },
                  (reducer, dead) => Walk(reducer, dead));

    private FlowReducer Walk(FlowReducer reducer, StepEntity dead)
        => _walkingDeadSubjects
            .Fold(reducer,
                  (reducer, subject) => subject.Accept(reducer, dead));
}
