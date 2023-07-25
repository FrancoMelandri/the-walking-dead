using TinyFp;

namespace WalkingDead;

public interface IPipeline
{
    Either<string, Unit> Execute(FlowReducer reducer);
}

public class Pipeline : IPipeline
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

    public Either<string, Unit> Execute(FlowReducer reducer)
        =>_stepOne
            .Forward(reducer)
            .Bind(_stepTwo.Forward)
            .Bind(_stepThree.Forward)
            .Bind(_stepFour.Forward)
            .Match(_ => Either<string, Unit>.Right(Unit.Default), _ => _);
}
