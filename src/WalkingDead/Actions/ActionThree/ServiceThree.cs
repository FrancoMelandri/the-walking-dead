namespace WalkingDead;

public interface IServiceThree
{
    ServiceThreeResponse Action(ServiceThreeRequest request);
}

public class ServiceThree : IServiceThree
{
    public ServiceThreeResponse Action(ServiceThreeRequest request)
        => throw new System.NotImplementedException();
}

public class ServiceThreeResponse
{
    public string Id { get; set; }
}

public class ServiceThreeRequest
{
    public string Id { get; set; }
}

