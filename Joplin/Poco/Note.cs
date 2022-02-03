using System.Diagnostics;
using Newtonsoft.Json;

namespace zblesk.Joplin;

[DebuggerDisplay("{title} ({id,nq})")]
public class Note : JoplinData
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    private bool _isHtml = false;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    private string? _body = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? parent_id { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? title { get; set; }

    /// <summary>
    /// The Note can either have a text body, or a HTML body. Setting one will clear the other.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? body
    {
        get => _isHtml ? null : _body;
        set
        {
            _isHtml = false;
            _body = value;
        }
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? body_html
    {
        get => _isHtml ? _body : null;
        set
        {
            _isHtml = true;
            _body = value;
        }
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? is_conflict { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? latitude { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? longitude { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? altitude { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? author { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? source_url { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? is_todo { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? todo_due { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ulong? todo_completed { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? source { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? source_application { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? application_data { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? order { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? encryption_applied { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? encryption_cipher_text { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? markup_language { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? is_shared { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? share_id { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? conflict_original_id { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? master_key_id { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? base_url { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? image_data_url { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? crop_rect { get; set; }

    public override string DefaultFetchFields => "id,parent_id,title,body,created_time,updated_time,is_conflict,latitude,longitude,altitude,author,source_url,is_todo,todo_due,todo_completed,source,source_application,application_data,order,user_created_time,user_updated_time,encryption_applied,encryption_cipher_text,markup_language,is_shared,share_id,conflict_original_id,master_key_id";

    public override string EntityApiPath => "notes";

}
