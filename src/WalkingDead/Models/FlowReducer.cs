using TinyFp;

namespace WalkingDead;

public class FlowReducer
{
    public Option<FlowContext> FlowContext { get; set; }
    public Option<ServiceOneResponse> Action1 { get; set; }
    public Option<ServiceTwoResponse> Action2 { get; set; }
    public Option<ServiceThreeResponse> Action3 { get; set; }
    public Option<ServiceFourResponse> Action4 { get; set; }
}