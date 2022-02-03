using Newtonsoft.Json;

namespace zblesk.Joplin;

public abstract class JoplinData
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? id { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ulong? created_time { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ulong? updated_time { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ulong? user_created_time { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ulong? user_updated_time { get; set; }

    public abstract string EntityApiPath { get; }
    public abstract string DefaultFetchFields { get; }
}
