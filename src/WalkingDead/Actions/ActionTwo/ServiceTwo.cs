namespace WalkingDead;

public interface IServiceTwo
{
    ServiceTwoResponse Action(ServiceTwoRequest request);
}

public class ServiceTwo : IServiceTwo
{
    public ServiceTwoResponse Action(ServiceTwoRequest request)
        => throw new System.NotImplementedException();
}

public class ServiceTwoResponse
{
    public string Id { get; set; }
}

public class ServiceTwoRequest
{
    public string Id { get; set; }
}

