using System.Diagnostics;

namespace zblesk.Joplin.Poco;

[DebuggerDisplay("{title} ({id,nq})")]
public class Note
{
    public string? id { get; set; }
    public string? parent_id { get; set; }
    public string? title { get; set; }
    public string? body { get; set; }
    public ulong? created_time { get; set; }
    public ulong? updated_time { get; set; }
    public bool? is_conflict { get; set; }
    public string? latitude { get; set; }
    public string? longitude { get; set; }
    public string? altitude { get; set; }
    public string? author { get; set; }
    public string? source_url { get; set; }
    public int? is_todo { get; set; }
    public int? todo_due { get; set; }
    public ulong? todo_completed { get; set; }
    public string? source { get; set; }
    public string? source_application { get; set; }
    public string? application_data { get; set; }
    public string? order { get; set; }
    public ulong? user_created_time { get; set; }
    public ulong? user_updated_time { get; set; }
    public int? encryption_applied { get; set; }
    public int? markup_language { get; set; }
    public bool? is_shared { get; set; }
    public string? share_id { get; set; }
    public string? conflict_original_id { get; set; }
    public string? master_key_id { get; set; }
    public string? body_html { get; set; }
    public string? base_url { get; set; }
    public string? image_data_url { get; set; }
    public string? crop_rect { get; set; }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
