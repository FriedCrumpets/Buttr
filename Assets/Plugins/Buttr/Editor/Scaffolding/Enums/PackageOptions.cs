using System;

namespace Buttr.Editor.Scaffolding {
    [Flags]
    internal enum PackageOptions {
        None           = 0,
        Handlers       = 1 << 0,
        Behaviours     = 1 << 1,
        Identifiers    = 1 << 2,
        Configurations = 1 << 3,
        Common         = 1 << 4,
        Exceptions     = 1 << 5,
        Definitions    = 1 << 6,
        Controller    = 1 << 7,
    }
}