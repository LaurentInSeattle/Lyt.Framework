namespace Lyt.Orchestrator;

public partial class WorkflowPage<TState, TTrigger> : ViewModel
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public virtual TState State { get; set; } = default;

    [ObservableProperty]
    private string? title; 

    public virtual Task OnInitialize() => Task.CompletedTask;

    public virtual Task OnShutdown() => this.OnDeactivateAsync(default);

    public virtual Task OnActivateAsync(TState fromState) => Task.CompletedTask;

    public virtual Task OnDeactivateAsync(TState toState) => Task.CompletedTask;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
    public WorkflowManager<TState, TTrigger> WorkflowManager { get; set; }
#pragma warning restore CS8618 
}
