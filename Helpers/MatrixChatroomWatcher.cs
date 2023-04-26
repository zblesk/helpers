using Markdig;

namespace zblesk.Helpers;

/// <summary>
/// A simple watcher that calls an event for every message that arrives into a specific Matrix room.
/// </summary>
/// <remarks>Usage: https://zblesk.net/blog/the-simplest-matrix-chatbot-base-in-csharp/ </remarks>
public sealed class MatrixChatroomWatcher : IDisposable
{
    private readonly string _homeserverUrl;
    private readonly string _username;
    private readonly string _roomId;
    private readonly string _authToken;

    private string resumeToken = "";
    private bool started = false;
    private Timer? timer;

    public delegate void MessageCallback(dynamic message);

    /// <summary>
    /// This event is called for each received room event.
    /// </summary>
    public event MessageCallback? NewMessageReceived;

    public MatrixChatroomWatcher(string homeserverUrl, string roomId, string username, string token)
    {
        if (string.IsNullOrEmpty(homeserverUrl))
            throw new ArgumentException($"'{nameof(homeserverUrl)}' cannot be null or empty.", nameof(homeserverUrl));

        _homeserverUrl = homeserverUrl.TrimEnd('/');
        _username = username ?? throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
        _roomId = roomId ?? throw new ArgumentException($"'{nameof(roomId)}' cannot be null or empty.", nameof(roomId));
        _authToken = token ?? throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));
    }

    public async Task Start()
    {
        if (started)
            return;
        await InitializeSync();
        started = true;
        timer = new Timer(FetchMessages, null, 100, 1000);
    }

    public void Stop()
    {
        if (!started)
            return;
        timer?.Change(Timeout.Infinite, 0);
        started = false;
    }

    /// <summary>
    /// Send a message to the watched room
    /// </summary>
    /// <param name="message">Message to send</param>
    /// <param name="renderMarkdown">True if the message is in MD and should be rendered before sending.</param>
    public async Task SendMessage(string message, bool renderMarkdown = true)
    {
        object body = new
        {
            msgtype = "m.text",
            body = message,
        };
        if (renderMarkdown)
            body = new
            {
                msgtype = "m.text",
                body = message,
                format = "org.matrix.custom.html",
                formatted_body = Markdown.ToHtml(message),
            };
        await $"{_homeserverUrl}/_matrix/client/r0/rooms/{_roomId}/send/m.room.message?access_token={_authToken}"
                            .PostJsonAsync(body);
    }

    public async Task<dynamic> GetMessage(string eventId)
        => (await $"{_homeserverUrl}/_matrix/client/v3/rooms/{_roomId}/event/{eventId}?access_token={_authToken}"
               .GetAsync()
               .ReceiveJson())
               .content;

    public void Dispose()
        => timer?.Dispose();

    private async Task InitializeSync()
    {
        var res = await $"{_homeserverUrl}/_matrix/client/v3/sync?access_token={_authToken}"
                            .GetJsonAsync();
        resumeToken = res.next_batch;
    }

    private void FetchMessages(object? state)
    {
        var request = $"{_homeserverUrl}/_matrix/client/v3/sync?access_token={_authToken}"
                        .SetQueryParam("timeout", 10)
                        .SetQueryParam("since", resumeToken)
                        .GetJsonAsync();
        request.Wait();
        var response = request.Result;
        resumeToken = response.next_batch;
        if (((IDictionary<string, dynamic>)response).ContainsKey("rooms"))
        {
            var r = (IDictionary<string, dynamic>)response.rooms.join;
            if (r.ContainsKey(_roomId))
            {
                var events = r[_roomId]?.timeline?.events;
                if (events == null)
                    return;
                foreach (var roomEvent in events)
                {
                    if (roomEvent.sender != _username)
                        NewMessageReceived?.Invoke(roomEvent);
                }
            }
        }
    }

    public static class MessageTypes
    {
        public const string Message = "m.room.message";
        public const string Redaction = "m.room.redaction";
        public const string Reaction = "m.reaction";
    }
}
