using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace WalkingDead;

public class PipelineFlowTests
{
    private PipelineFlow _sut;
    private Mock<IServiceOne> _serviceOne;
    private Mock<IServiceTwo> _serviceTwo;
    private Mock<IServiceThree> _serviceThree;
    private Mock<IServiceFour> _serviceFour;
    private Mock<IStepRepository> _stepRepository;

    [SetUp]
    public void SetUp()
    {
        _serviceOne = new();
        _serviceTwo = new();
        _serviceThree = new();
        _serviceFour = new();
        _stepRepository = new ();

        var stepOne = new StepOneAhead(
                        new StepOneMemento(
                            new StepOne(_serviceOne.Object),
                            _stepRepository.Object));
        var stepTwo = new StepTwoAhead(
                        new StepTwoMemento(
                            new StepTwo(_serviceTwo.Object),
                            _stepRepository.Object));
        var stepThree = new StepThreeAhead(
                        new StepThreeMemento(
                            new StepThree(_serviceThree.Object),
                            _stepRepository.Object));
        var stepFour = new StepFourAhead(
                        new StepFourMemento(
                            new StepFour(_serviceFour.Object),
                            _stepRepository.Object));

        var pipeline = new Pipeline(stepOne,
            stepTwo,
            stepThree,
            stepFour);
        _sut = new PipelineFlow(pipeline);
    }

    [Test]
    public void WhenNoServiceOne_ShouldReturnError()
    {
        _serviceOne
            .Setup(m => m.Action(It.IsAny<ServiceOneRequest>()))
            .Returns((ServiceOneResponse)null);

        var context = new FlowContext
        {
            Id = "hello"
        };
        var result = _sut.Flow(context);

        result.UnwrapLeft().Should().Be("error-1");
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Never);
    }

    [Test]
    public void WhenNoServiceTwo_ShouldReturnError()
    {
        _serviceOne
            .Setup(m => m.Action(It.IsAny<ServiceOneRequest>()))
            .Returns(new ServiceOneResponse { Id = "Hello1" });
        _serviceTwo
            .Setup(m => m.Action(It.IsAny<ServiceTwoRequest>()))
            .Returns((ServiceTwoResponse)null);
        _stepRepository
            .Setup(m => m.Upsert(It.IsAny<StepEntity>()))
            .Returns(new StepEntity());

        var context = new FlowContext
        {
            Id = "hello"
        };
        var result = _sut.Flow(context);
        result.UnwrapLeft().Should().Be("error-2");

        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")));
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(1));
    }

    [Test]
    public void WhenNoServiceThree_ShouldReturnError()
    {
        _serviceOne
            .Setup(m => m.Action(It.IsAny<ServiceOneRequest>()))
            .Returns(new ServiceOneResponse { Id = "Hello1" });
        _serviceTwo
            .Setup(m => m.Action(It.IsAny<ServiceTwoRequest>()))
            .Returns(new ServiceTwoResponse { Id = "Hello2"});
        _serviceThree
            .Setup(m => m.Action(It.IsAny<ServiceThreeRequest>()))
            .Returns((ServiceThreeResponse)null);
        _stepRepository
            .Setup(m => m.Upsert(It.IsAny<StepEntity>()))
            .Returns(new StepEntity());

        var context = new FlowContext
        {
            Id = "hello"
        };
        var result = _sut.Flow(context);
        result.UnwrapLeft().Should().Be("error-3");
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step2")));
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(2));

    }

    [Test]
    public void WhenNoServiceFour_ShouldReturnError()
    {
        _serviceOne
            .Setup(m => m.Action(It.IsAny<ServiceOneRequest>()))
            .Returns(new ServiceOneResponse { Id = "Hello1" });
        _serviceTwo
            .Setup(m => m.Action(It.IsAny<ServiceTwoRequest>()))
            .Returns(new ServiceTwoResponse { Id = "Hello2"});
        _serviceThree
            .Setup(m => m.Action(It.IsAny<ServiceThreeRequest>()))
            .Returns(new ServiceThreeResponse{ Id = "Hello3" });
        _serviceFour
            .Setup(m => m.Action(It.IsAny<ServiceFourRequest>()))
            .Returns((ServiceFourResponse)null);
        _stepRepository
            .Setup(m => m.Upsert(It.IsAny<StepEntity>()))
            .Returns(new StepEntity());

        var context = new FlowContext
        {
            Id = "hello"
        };
        var result = _sut.Flow(context);
        result.UnwrapLeft().Should().Be("error-4");
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step2")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step3")));
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(3));
    }

    [Test]
    public void WhenAllServices_ShouldOk()
    {
        _serviceOne
            .Setup(m => m.Action(It.IsAny<ServiceOneRequest>()))
            .Returns(new ServiceOneResponse { Id = "Hello1" });
        _serviceTwo
            .Setup(m => m.Action(It.IsAny<ServiceTwoRequest>()))
            .Returns(new ServiceTwoResponse { Id = "Hello2"});
        _serviceThree
            .Setup(m => m.Action(It.IsAny<ServiceThreeRequest>()))
            .Returns(new ServiceThreeResponse{ Id = "Hello3" });
        _serviceFour
            .Setup(m => m.Action(It.IsAny<ServiceFourRequest>()))
            .Returns(new ServiceFourResponse { Id = "HelloWorld" });
        _stepRepository
            .Setup(m => m.Upsert(It.IsAny<StepEntity>()))
            .Returns(new StepEntity());

        var context = new FlowContext
        {
            Id = "hello"
        };
        var result = _sut.Flow(context);
        result.IsRight.Should().BeTrue();

        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step2")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step3")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step4")));
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(4));
    }
}
