namespace Lyt.Orchestrator;

public sealed class WorkflowManager<TState, TTrigger> : IDisposable
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public const int DefaultAnimationDuration = 0; // milliseconds
    public const int MinimumAnimationDuration = 100; // milliseconds

    private readonly ILogger logger;
    private readonly FiniteStateMachine<TState, TTrigger, ViewModel> stateMachine;
    private readonly Dictionary<TState, WorkflowPage<TState, TTrigger>> pageIndex;
    private readonly IOrchestratorHostControl orchestratorHostControl;

    private bool disposedValue;

    public WorkflowManager(
        ILogger logger,
        IOrchestratorHostControl orchestratorHostControl,
        StateMachineDefinition<TState, TTrigger, ViewModel> stateMachineDefinition)
    {
        this.logger = logger;
        this.orchestratorHostControl = orchestratorHostControl;
        this.stateMachine = new(this.logger);
        this.pageIndex = [];
        this.CreateWorkflowStateMachine(stateMachineDefinition);
    }

    public WorkflowPage<TState, TTrigger>? GetPage(TState state)
        => this.pageIndex.TryGetValue(state, out var workflowPage) ? workflowPage : null;

    public WorkflowPage<TState, TTrigger>? ActivePage { get; private set; }

    public bool IsTransitioning { get; private set; }

    private void CreateWorkflowStateMachine(StateMachineDefinition<TState, TTrigger, ViewModel> stateMachineDefinition)
    {
        try
        {
            List<IView> views = [];  
            this.stateMachine.Initialize(stateMachineDefinition);
            foreach (StateDefinition<TState, TTrigger, ViewModel> stateDefinition in stateMachineDefinition.StateDefinitions)
            {
                TState state = stateDefinition.State;
                var page = stateDefinition.Tag as WorkflowPage<TState, TTrigger>;
                if ((page is not null)&& (page.ViewBase is not null))
                {
                    page.WorkflowManager = this; 
                    this.pageIndex.Add(state, page);
                    views.Add(page.ViewBase); 
                }
                else
                {
                    this.logger.Error("Null page or not Workflow Page: Improperly defined state machine: " + state.ToString());
                }
            }

            this.orchestratorHostControl.Initialize(views);
        }
        catch (Exception e)
        {
            this.logger.Error("Improperly defined state machine:\n" + e.ToString());
            throw;
        }
    }

    public async Task Initialize()
    {
        foreach (var page in this.pageIndex.Values)
        {
            await page.OnInitialize();
        }
    }

    public async Task Shutdown()
    {
        foreach (var page in this.pageIndex.Values)
        {
            await page.OnShutdown();
        }
    }

    public async Task Start()
    {
        var newState = this.stateMachine.State;
        var activated = await this.ActivatePage(newState);

        // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
        new NavigationMessage(activated, null).Publish();
    }

    public void ClearBackNavigation() => this.stateMachine.ClearBackNavigation();

    public async Task<bool> Next()
    {
        if (this.CanGoNext(out var _, out var _))
        {
            return await this.TryGoNext();
        }

        return false;
    }

    public async Task<bool> Back()
    {
        if (this.CanGoBack(out TState _))
        {
            return await this.TryGoBack();
        }

        return false;
    }

    public async Task<bool> Fire(TTrigger trigger)
    {
        if (this.CanFire(trigger))
        {
            return await this.TryFire(trigger);
        }

        return false;
    }

    public bool CanGoBack(out TState state)
    {
        // We can go back if we are not transitioning and the navigation stack is not empty
        state = this.stateMachine.State;
        return !this.IsTransitioning && this.stateMachine.CanGoBack(out state);
    }

    public bool CanGoNext(out TState state, out TTrigger trigger)
    {
        state = default;
        trigger = default;
        if (this.IsTransitioning)
        {
            return false;
        }

        return this.stateMachine.CanGoNext(out state, out trigger);
    }

    public bool CanFire(TTrigger trigger) => this.stateMachine.CanFire(trigger);

    private async Task<bool> TryGoBack()
    {
        var oldState = this.stateMachine.State;
        if (!this.CanGoBack(out TState newState))
        {
            this.logger.Debug(string.Format("Cannot go back from: {0} ", oldState.ToString()));
            return false;
        }

        this.IsTransitioning = true;
        this.stateMachine.GoBack();
        var deactivated = await this.DeactivatePage(oldState);
        var activated = await this.ActivatePage(newState);
        this.IsTransitioning = false;

        // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
        new NavigationMessage(activated, deactivated).Publish();

        string message =
            string.Format("Backwards workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
        this.logger.Info(message);
        return true;
    }

    private async Task<bool> TryGoNext()
    {
        var oldState = this.stateMachine.State;
        bool canGoNext = this.stateMachine.CanGoNext(out TState newState, out TTrigger trigger);
        if (canGoNext)
        {
            this.IsTransitioning = true;
            this.stateMachine.GoNext(trigger);
            var deactivated = await this.DeactivatePage(oldState);
            var activated = await this.ActivatePage(newState);
            this.IsTransitioning = false;

            // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
            new NavigationMessage(activated, deactivated).Publish();

            string message =
                string.Format("Forward workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
            this.logger.Info(message);
        }

        return canGoNext;
    }

    private async Task<bool> TryFire(TTrigger trigger)
    {
        var oldState = this.stateMachine.State;
        bool canFire = this.stateMachine.CanFire(trigger);
        if (canFire)
        {
            this.IsTransitioning = true;
            this.stateMachine.Fire(trigger);
            TState newState = this.stateMachine.State;
            var deactivated = await this.DeactivatePage(oldState);
            var activated = await this.ActivatePage(newState);
            this.IsTransitioning = false;

            // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
            new NavigationMessage(activated, deactivated).Publish();

            string message =
                string.Format("Forward workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
            this.logger.Info(message);
        }

        return canFire;
    }

    private async Task<WorkflowPage<TState, TTrigger>?> DeactivatePage(TState oldState)
    {
        this.logger.Info("Orchestrator: Deactivating " + oldState.ToString());
        var deactivated = this.ActivePage;
        if ((deactivated is not null) && (deactivated.ViewBase is not null))
        {
            await deactivated.OnDeactivateAsync(oldState);
            this.orchestratorHostControl.Deactivate(deactivated.ViewBase);
            return deactivated;
        }

        string message = "Active Page is null or unbound."; 
        this.logger.Error(message);
        throw new Exception(message);
    }

    private async Task<WorkflowPage<TState, TTrigger>> ActivatePage(TState newState)
    {
        // Get the new page, activates it
        this.logger.Info("Orchestrator: Activating " + newState.ToString());
        this.ActivePage = this.pageIndex[newState];
        if ((this.ActivePage is not null) && (this.ActivePage.ViewBase is not null))
        {
            await this.ActivePage.OnActivateAsync(newState);
            this.orchestratorHostControl.Activate(this.ActivePage.ViewBase);
            return this.ActivePage;
        }

        string message = "Active Page is null or unbound.";
        this.logger.Error(message);
        throw new Exception(message);
    }

    private void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                this.stateMachine.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            this.pageIndex.Clear();

            this.disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~WorkflowManager()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}