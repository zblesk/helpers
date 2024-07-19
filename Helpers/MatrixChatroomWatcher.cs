using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
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

    public delegate void MessageCallback(JsonNode message);

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

    /// <summary>
    /// Starts the Watcher
    /// </summary>
    /// <returns></returns>
    public async Task Start()
    {
        if (started)
            return;
        await InitializeSync();
        started = true;
        timer = new Timer(FetchMessages, null, 100, 1000);
    }

    /// <summary>
    /// Stops the Watcher
    /// </summary>
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

    /// <summary>
    /// Gets a specific message from the watched room
    /// </summary>
    /// <param name="eventId">Event ID of the message to get</param>
    /// <returns>The message object</returns>
    public async Task<dynamic> GetMessage(string eventId)
        => (await $"{_homeserverUrl}/_matrix/client/v3/rooms/{_roomId}/event/{eventId}?access_token={_authToken}"
               .GetAsync()
               .ReceiveJson<dynamic>())
               .content;

    /// <summary>
    /// Sends a Reacton ('annotation') to a specific message in the watched room
    /// </summary>
    /// <param name="eventId">Event ID of the message to react to</param>
    /// <param name="emoji">The reaction emoji</param>
    public async Task SendReaction(string eventId, string emoji)
    {
        using var stream = new MemoryStream();
        using var json = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true, SkipValidation = false });
        json.WriteStartObject();
        json.WriteStartObject("m.relates_to");
        json.WritePropertyName("rel_type");
        json.WriteStringValue("m.annotation");
        json.WritePropertyName("event_id");
        json.WriteStringValue(eventId);
        json.WritePropertyName("key");
        json.WriteStringValue(emoji);
        json.WriteEndObject();
        json.WriteEndObject();
        json.Flush();

        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        var body = reader.ReadToEnd();

        await $"{_homeserverUrl}/_matrix/client/v3/rooms/{_roomId}/send/m.reaction?access_token={_authToken}"
                            .PostStringAsync(body);
    }

    public void Dispose()
        => timer?.Dispose();

    private async Task InitializeSync()
    {
        var res = await $"{_homeserverUrl}/_matrix/client/v3/sync?access_token={_authToken}"
                            .GetJsonAsync<JsonObject>();
        resumeToken = (string?)res?["next_batch"] ?? "";
    }

    private void FetchMessages(object? state)
    {
        var request = $"{_homeserverUrl}/_matrix/client/v3/sync?access_token={_authToken}"
                        .SetQueryParam("timeout", 10)
                        .SetQueryParam("since", resumeToken)
                        .GetJsonAsync<JsonObject>();
        request.Wait();
        var response = request.Result;
        resumeToken = (string?)response["next_batch"] ?? "";
        if (response.ContainsKey("rooms"))
        {
            var r = response?["rooms"]?["join"];
            if (r != null
                && r.ToString().Contains(_roomId))
            {
                var events = (JsonArray?)r[_roomId]?["timeline"]?["events"];
                if (events == null)
                    return;
                foreach (var roomEvent in events)
                {
                    if (roomEvent == null) continue;
                    if (roomEvent["sender"]?.ToString() != _username)
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
