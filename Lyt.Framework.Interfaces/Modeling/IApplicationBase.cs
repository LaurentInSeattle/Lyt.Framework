namespace Lyt.Framework.Interfaces.Modeling;

public interface IApplicationBase
{
    List<IModel> GetModels();
    
    Task Shutdown ();
}

