using System;
using System.Text.Json;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace WalkingDead;

public class PipelineRetryTests
{
    private PipelineRetry _sut;
    private Mock<IStepLoaderRepository> _stepLoaderRepository;
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
        _stepRepository = new();
        _stepLoaderRepository = new();

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

        var subjects = new IWalkingDeadSubject[]
        {
            new WalkingDeadStepOne(),
            new WalkingDeadStepTwo(),
            new WalkingDeadStepThree(),
            new WalkingDeadStepFour()
        };
        var walkingDeadVisitor = new WalkingDeadVisitor(subjects);
        _sut = new PipelineRetry(_stepLoaderRepository.Object,
                                  pipeline,
                                  walkingDeadVisitor);

        _stepLoaderRepository
            .Setup(m => m.StillWalking())
            .Returns(new [] { "dead1" } );

        _stepLoaderRepository
            .Setup(m => m.StillWalking("dead1"))
            .Returns(Array.Empty<StepEntity>() );
    }

    [Test]
    public void WhenNoServiceOne_ShouldReturnError()
    {
        _serviceOne
            .Setup(m => m.Action(It.IsAny<ServiceOneRequest>()))
            .Returns((ServiceOneResponse)null);

        var result = _sut.Retry();

        result.OnLeft(_ => _.Should().Be("error-1"));
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

        var result = _sut.Retry();
        result.OnLeft(_ => _.Should().Be("error-2"));

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


        var result = _sut.Retry();
        result.OnLeft(_ => _.Should().Be("error-3"));

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

        var result = _sut.Retry();
        result.OnLeft(_ => _.Should().Be("error-4"));

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

        var result = _sut.Retry();
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

    [Test]
    public void WhenAllServices_ButOne_AlreadyDone_ShouldOk()
    {
        _stepLoaderRepository
            .Setup(m => m.StillWalking("dead1"))
            .Returns(new []
            {
                new StepEntity
                {
                    Step = "Step1",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceOneResponse { Id = "hello1" })
                }
            } );
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

        var result = _sut.Retry();
        result.IsRight.Should().BeTrue();

        _serviceOne
            .Verify(m => m.Action(It.IsAny<ServiceOneRequest>()),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step2")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step3")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step4")));
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(3));
    }

    [Test]
    public void WhenAllServices_ButOne_AndTwo_AlreadyDone_ShouldOk()
    {
        _stepLoaderRepository
            .Setup(m => m.StillWalking("dead1"))
            .Returns(new []
            {
                new StepEntity
                {
                    Step = "Step1",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceOneResponse { Id = "hello1" })
                },
                new StepEntity
                {
                    Step = "Step2",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceTwoResponse { Id = "hello2" })
                }
            } );
        _serviceThree
            .Setup(m => m.Action(It.IsAny<ServiceThreeRequest>()))
            .Returns(new ServiceThreeResponse{ Id = "Hello3" });
        _serviceFour
            .Setup(m => m.Action(It.IsAny<ServiceFourRequest>()))
            .Returns(new ServiceFourResponse { Id = "HelloWorld" });
        _stepRepository
            .Setup(m => m.Upsert(It.IsAny<StepEntity>()))
            .Returns(new StepEntity());

        var result = _sut.Retry();
        result.IsRight.Should().BeTrue();

        _serviceOne
            .Verify(m => m.Action(It.IsAny<ServiceOneRequest>()),
                                  Times.Never);
        _serviceTwo
            .Verify(m => m.Action(It.IsAny<ServiceTwoRequest>()),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step12")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step3")));
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step4")));
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(2));
    }

    [Test]
    public void WhenAllServices_ButOne_AndTwo_AndThree_AlreadyDone_ShouldOk()
    {
        _stepLoaderRepository
            .Setup(m => m.StillWalking("dead1"))
            .Returns(new []
            {
                new StepEntity
                {
                    Step = "Step1",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceOneResponse { Id = "hello1" })
                },
                new StepEntity
                {
                    Step = "Step2",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceTwoResponse { Id = "hello2" })
                },
                new StepEntity
                {
                    Step = "Step3",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceThreeResponse { Id = "hello3" })
                }
            } );
        _serviceFour
            .Setup(m => m.Action(It.IsAny<ServiceFourRequest>()))
            .Returns(new ServiceFourResponse { Id = "HelloWorld" });
        _stepRepository
            .Setup(m => m.Upsert(It.IsAny<StepEntity>()))
            .Returns(new StepEntity());

        var result = _sut.Retry();
        result.IsRight.Should().BeTrue();

        _serviceOne
            .Verify(m => m.Action(It.IsAny<ServiceOneRequest>()),
                                  Times.Never);
        _serviceTwo
            .Verify(m => m.Action(It.IsAny<ServiceTwoRequest>()),
                                  Times.Never);
        _serviceThree
            .Verify(m => m.Action(It.IsAny<ServiceThreeRequest>()),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step2")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step3")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step4")));
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(1));
    }

    [Test]
    public void WhenAllServices_ButOne_AndTwo_AndThree_AndFour_AlreadyDone_ShouldOk()
    {
        _stepLoaderRepository
            .Setup(m => m.StillWalking("dead1"))
            .Returns(new []
            {
                new StepEntity
                {
                    Step = "Step1",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceOneResponse { Id = "hello1" })
                },
                new StepEntity
                {
                    Step = "Step2",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceTwoResponse { Id = "hello2" })
                },
                new StepEntity
                {
                    Step = "Step3",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceThreeResponse { Id = "hello3" })
                },
                new StepEntity
                {
                    Step = "Step4",
                    Context = "dead1",
                    Output = JsonSerializer.Serialize(new ServiceFourResponse { Id = "hello34" })
                }
            } );
        _stepRepository
            .Setup(m => m.Upsert(It.IsAny<StepEntity>()))
            .Returns(new StepEntity());

        var result = _sut.Retry();
        result.IsRight.Should().BeTrue();

        _serviceOne
            .Verify(m => m.Action(It.IsAny<ServiceOneRequest>()),
                                  Times.Never);
        _serviceTwo
            .Verify(m => m.Action(It.IsAny<ServiceTwoRequest>()),
                                  Times.Never);
        _serviceThree
            .Verify(m => m.Action(It.IsAny<ServiceThreeRequest>()),
                                  Times.Never);
        _serviceFour
            .Verify(m => m.Action(It.IsAny<ServiceFourRequest>()),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step1")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step2")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step3")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.Is<StepEntity>(p => p.Step == "Step4")),
                                  Times.Never);
        _stepRepository
            .Verify(m => m.Upsert(It.IsAny<StepEntity>()),
                                  Times.Exactly(0));
    }
}