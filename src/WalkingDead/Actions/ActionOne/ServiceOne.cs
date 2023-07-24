namespace WalkingDead;

public interface IServiceOne
{
    ServiceOneResponse Action(ServiceOneRequest request);
}

public class ServiceOne : IServiceOne
{
    public ServiceOneResponse Action(ServiceOneRequest request)
        => throw new System.NotImplementedException();
}

public class ServiceOneResponse
{
    public string Id { get; set; }
}

public class ServiceOneRequest
{
    public string Id { get; set; }
}

