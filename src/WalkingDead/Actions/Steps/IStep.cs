using TinyFp;

namespace WalkingDead;

public interface IStep
{
     Either<string, FlowReducer> Forward(FlowReducer flow);
}
