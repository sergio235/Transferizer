using System;
using System.Collections.Generic;
using System.Text;

namespace Transferizer.Model
{
    public struct Transfer
    {
        public string From { get; set; }
        public string To { get; set; }
        public bool IncludeSubFolders { get; set; }
        public string SearchPattern { get; set; }
    }
}
