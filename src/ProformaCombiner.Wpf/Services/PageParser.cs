namespace ProformaCombiner.Wpf.Services;

public static class PageParser
{
    private const int MAX_COPIES = 100;

    public static List<int> ParsePages(string? text)
    {
        var result = new List<int>();
        if (string.IsNullOrWhiteSpace(text))
        {
            result.Add(1);
            return result;
        }

        var parts = text.Trim().Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var raw in parts)
        {
            var token = raw.Trim();

            int copies = 1;
            int xPos = token.LastIndexOf('x');
            if (xPos < 0) xPos = token.LastIndexOf('X');
            int starPos = token.LastIndexOf('*');
            int sepPos = Math.Max(xPos, starPos);

            if (sepPos > 0 && sepPos < token.Length - 1)
            {
                if (int.TryParse(token[(sepPos + 1)..].Trim(), out var n))
                {
                    copies = n > 0 ? n : 1;
                    if (copies > MAX_COPIES) copies = MAX_COPIES;
                    token = token[..sepPos].Trim();
                }
            }

            int dash = token.IndexOf('-');
            if (dash > 0)
            {
                if (int.TryParse(token[..dash].Trim(), out var a) && int.TryParse(token[(dash + 1)..].Trim(), out var b))
                {
                    if (a <= b)
                    {
                        for (int i = a; i <= b; i++)
                            for (int c = 0; c < copies; c++)
                                result.Add(i);
                    }
                    else
                    {
                        for (int i = a; i >= b; i--)
                            for (int c = 0; c < copies; c++)
                                result.Add(i);
                    }
                }
            }
            else
            {
                if (int.TryParse(token, out var n) && n > 0)
                {
                    for (int c = 0; c < copies; c++)
                        result.Add(n);
                }
            }
        }

        if (result.Count == 0)
            result.Add(1);

        return result;
    }
}
