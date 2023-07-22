using FluentAssertions;
using NUnit.Framework;
using TinyFp;

namespace WalkingDeath;

public class PipelineTests
{
    private Pipeline _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new Pipeline();
    }

    [Test]
    public void ShouldReturnError()
    {
        var context = new FlowContext
        {
            Id = "hello;"
        };
        var result = _sut.Flow(context);
        result.IsLeft.Should().BeTrue();
    }
}