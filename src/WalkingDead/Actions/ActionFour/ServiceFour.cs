namespace WalkingDead;

public interface IServiceFour
{
    ServiceFourResponse Action(ServiceFourRequest request);
}

public class ServiceFour : IServiceFour
{
    public ServiceFourResponse Action(ServiceFourRequest request)
        => throw new System.NotImplementedException();
}

public class ServiceFourResponse
{
    public string Id { get; set; }
}

public class ServiceFourRequest
{
    public string Id { get; set; }
}

