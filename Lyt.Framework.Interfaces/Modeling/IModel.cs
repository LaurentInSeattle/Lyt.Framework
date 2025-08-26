namespace Lyt.Framework.Interfaces.Modeling;

public interface IModel
{
    /// <summary>  Initializes the model. </summary>
    Task Initialize();

    /// <summary> Shutdowns the model </summary>
    Task Shutdown();
}

public interface IApplicationModel
{
    /// <summary>  Initializes all models. </summary>
    Task Initialize();

    /// <summary> Shutdowns all models </summary>
    Task Shutdown();
}
