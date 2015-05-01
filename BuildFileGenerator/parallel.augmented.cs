using System.Collections.Generic;

namespace BuildFileGenerator
{
    public partial class parallel
    {
        public parallel()
        {
            Namespaces = new HashSet<string>();
        }

        public HashSet<string> Namespaces { get; set; }
        public string TestOutputDirectory { get; set; }
    }
}
