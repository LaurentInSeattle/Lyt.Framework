namespace Lyt.Utilities.Randomizing;

public static class Numbers
{
    public static List<int> PrimeFactors(this int n)
    {
        List<int> factors = [];

        // Add all the 2s that divide n
        while (0 == n % 2)
        {

            factors.Add(2);
            n /= 2;
        }

        // n must be odd at this point. So we can skip one element (Note i = i +2)
        for (int i = 3; i * i < n; i += 2)
        {
            // While i divides n, save i and divide n
            while (n % i == 0)
            {

                factors.Add(i);
                n /= i;
            }

            if (n == 1)
            {
                break;
            }
        }

        if (n > 2)
        {
            // n is a prime number greater than 2
            factors.Add(n);
        }

        return factors;
    }
}
