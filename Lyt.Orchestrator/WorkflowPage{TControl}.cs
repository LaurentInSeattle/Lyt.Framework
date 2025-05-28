namespace Lyt.Orchestrator;

public class WorkflowPage<TState, TTrigger, TControl> : WorkflowPage<TState, TTrigger>
    where TState : struct, Enum
    where TTrigger : struct, Enum
    where TControl : class, IView, new()
{
    public TControl View
        => this.ViewBase as TControl ?? throw new InvalidOperationException("View is null");
}
