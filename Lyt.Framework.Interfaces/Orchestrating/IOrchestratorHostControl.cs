using Lyt.Framework.Interfaces.Binding;

namespace Lyt.Framework.Interfaces.Orchestrating;

public interface IOrchestratorHostControl
{
    void Initialize(IEnumerable<IView> views);

    void Activate(IView view);
    
    void Deactivate(IView view);
}
