
using System;
using TinyFp;

namespace WalkingDead;

public interface IStepLoaderRepository
{
    Either<string, string[]> StillWalking();
    Either<string, StepEntity[]> StillWalking(string id);
}

public class StepLoaderRepository : IStepLoaderRepository
{
    public Either<string, string[]> StillWalking()
        =>  Either<string, string[]>.Right(Array.Empty<string>());

    public Either<string, StepEntity[]> StillWalking(string id)
        =>  Either<string, StepEntity[]>.Right(Array.Empty<StepEntity>());
}