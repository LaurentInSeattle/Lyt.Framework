namespace Lyt.Utilities.Randomizing;

using Lyt.Framework.Interfaces.Randomizing;

public class Chooser<T> where T : class
{
    private readonly IRandomizer randomizer;

    private readonly IList<T> source;

    private readonly Queue<T> recent;

    public Chooser(IRandomizer randomizer, IList<T> source)
    {
        if (source.Count < 4)
        {
            throw new ArgumentException("Source too small");
        }

        this.randomizer = randomizer;
        this.source = source;
        this.recent = new Queue<T>(source.Count / 2);
    }

    public int Count => this.source.Count;

    public T Next()
    {
        T? next;
        bool found;
        do
        {
            int index = this.randomizer.Next(this.source.Count);
            next = this.source[index];
            found = !this.recent.Contains(next);
        } while (!found);

        this.recent.Enqueue(next);
        if (this.recent.Count > 3 * this.source.Count / 4)
        {
            _ = this.recent.Dequeue();
        }

        return next;
    }
}
