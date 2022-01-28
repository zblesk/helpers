using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zblesk.Joplin.Poco;

public abstract class JoplinData
{
    public ulong? created_time { get; set; }
    public ulong? updated_time { get; set; }
    public ulong? user_created_time { get; set; }
    public ulong? user_updated_time { get; set; }

    public abstract string DefaultFetchFields { get; }
    public abstract string EntityApiPath { get; }
}
