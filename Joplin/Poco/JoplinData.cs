using Newtonsoft.Json;

namespace zblesk.Joplin;

public abstract class JoplinData
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? id { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? created_time { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? updated_time { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? user_created_time { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? user_updated_time { get; set; }

    [JsonIgnore]
    public abstract string EntityApiPath { get; }

    [JsonIgnore]
    public abstract string DefaultFetchFields { get; }

    [JsonIgnore]
    public abstract string SearchType { get; }


    [JsonIgnore]
    public DateTime? CreatedTime
    {
        get => Helpers.DateExtensions.FromUnixTimestampMiliseconds(created_time);
        set => created_time = Helpers.DateExtensions.ToUnixTimestampMiliseconds(value);
    }

    [JsonIgnore]
    public DateTime? UpdatedTime
    {
        get => Helpers.DateExtensions.FromUnixTimestampMiliseconds(updated_time);
        set => updated_time = Helpers.DateExtensions.ToUnixTimestampMiliseconds(value);
    }

    [JsonIgnore]
    public DateTime? UserCreatedTime
    {
        get => Helpers.DateExtensions.FromUnixTimestampMiliseconds(user_created_time);
        set => user_created_time = Helpers.DateExtensions.ToUnixTimestampMiliseconds(value);
    }

    [JsonIgnore]
    public DateTime? UserUpdatedTime
    {
        get => Helpers.DateExtensions.FromUnixTimestampMiliseconds(user_updated_time);
        set => user_updated_time = Helpers.DateExtensions.ToUnixTimestampMiliseconds(value);
    }
}
