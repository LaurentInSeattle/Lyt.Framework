namespace Lyt.Framework.Interfaces.Modeling;

public interface IApplicationBase
{
    IEnumerable<IModel> GetModels();
    
    Task Shutdown ();
}

