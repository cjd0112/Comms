using System;
using System.Collections.Generic;
using System.Text;

namespace AMLClient
{
    public class ClientActors
    {
        public int NumNodes{ get; set; }
        public String Service { get; set; }
    }

    public class ClientActorConfig
    {
        public ClientActors ClientActors { get; set; }
    }
}
