using TinyFp;

namespace WalkingDead;

public interface IStepRepository
{
    Either<string, StepEntity> Upsert(StepEntity entity);
}

public class StepRepository : IStepRepository
{
    public Either<string, StepEntity> Upsert(StepEntity entity)
        =>  Either<string, StepEntity>.Right(entity);
}
