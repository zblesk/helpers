using System.Diagnostics;

namespace zblesk.Joplin;

[DebuggerDisplay("Notebook {title} ({id,nq})")]
public class Notebook : JoplinData
{
    public string? id { get; set; }
    public string? title { get; set; }
    public string? encryption_cipher_text { get; set; }
    public ulong? encryption_applied { get; set; }
    public string? parent_id { get; set; }
    public bool? is_shared { get; set; }
    public string? share_id { get; set; }
    public string? master_key_id { get; set; }
    public string? icon { get; set; }

    public override string DefaultFetchFields => "id,title,created_time,updated_time,user_created_time,user_updated_time,encryption_cipher_text,encryption_applied,parent_id,is_shared,share_id,master_key_id,icon";

    public override string EntityApiPath => "folders";

    public override string SearchType => "folder";
}
