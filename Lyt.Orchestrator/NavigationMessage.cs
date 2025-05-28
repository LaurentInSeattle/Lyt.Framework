namespace Lyt.Orchestrator;

public sealed record class NavigationMessage(ViewModel Activated, ViewModel? Deactivated = null); 