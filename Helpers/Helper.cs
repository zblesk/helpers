using Flurl.Http;

namespace zblesk.Helpers;

public static class Helper
{

    /// <summary>
    /// Send a mail via Mailgun.
    /// </summary>
    /// <param name="mailgunApiUrl">The API URL. Something like "https://api.mailgun.net/v3/your.domain/messages".</param>
    /// <param name="apiKey">Mailgun API key.</param>
    /// <param name="fromAddress">From address - Mailgun credential.</param>
    /// <param name="recipientAddress">Recipient's mail address.</param>
    /// <param name="subject">The mail's subject.</param>
    /// <param name="htmlMessage">The mail's message. Might be in HTML.</param>
    public static Task SendMailgunEmail(
        string mailgunApiUrl,
        string apiKey,
        string fromAddress,
        string recipientAddress,
        string subject,
        string htmlMessage)

        => mailgunApiUrl
            .WithBasicAuth("api", apiKey)
            .PostMultipartAsync(bc => bc
                .AddString("from", fromAddress)
                .AddString("to", recipientAddress)
                .AddString("subject", subject)
                .AddString("text", htmlMessage))
            .ReceiveString();

    public static int LevensteinDistance(string a, string b)
        => Levenstein(new Dictionary<(int, int), int>(), a, b, a.Length - 1, b.Length - 1);

    private static int Levenstein(Dictionary<(int, int), int> table, string source, string target, int i, int j)
    {
        if (table.ContainsKey((i, j)))
            return table[(i, j)];

        if (Math.Min(i, j) == 0)
            return Math.Max(i, j);

        var delete = Levenstein(table, source, target, i - 1, j) + 1;
        var add = Levenstein(table, source, target, i, j - 1) + 1;
        var match = Levenstein(table, source, target, i - 1, j - 1) + (source[i] == target[j] ? 0 : 1);

        var min = Math.Min(
            delete,
            Math.Min(
                add,
                match));

        table[(i, j)] = min;
        return min;
    }
}
