namespace Lyt.Framework.Interfaces.Modeling;

public sealed record class ModelUpdateMessage( 
    IModel Model, string? PropertyName = "", string? MethodName = "");
