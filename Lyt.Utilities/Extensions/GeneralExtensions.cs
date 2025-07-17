namespace Lyt.Utilities.Extensions;

public static class GeneralExtensions
{
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
    {
        if (sequence == null)
        {
            return;
        }

        foreach (var item in sequence)
        {
            if (item != null)
            {
                action(item);
            }
        }
    }

    public static bool IsOutOfBounds<T>(this int index, ICollection<T> collection)
        => (index < 0) || (index >= collection.Count);

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        => null == collection || 0 == collection.Count;

    public static bool ToBool(this bool? ternary) => ternary ?? false;

    public static bool IsNullOrEmpty(this Guid? id) => null == id || Guid.Empty == id;

    public static void With ( ref bool flag , Action action)
    {
        flag = true;
        action();
        flag = false;
    }
}
