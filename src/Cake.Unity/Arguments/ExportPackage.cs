using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.Unity.Arguments
{
    public class ExportPackage
    {
        public required string[] AssetPaths { get; set; }
        public required string PackageName { get; set; }
    }
}
