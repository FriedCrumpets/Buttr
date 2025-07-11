using System.Threading;
using UnityEngine;

namespace Buttr.Core {
    internal interface IApplicationRunner {
        Awaitable Run(CancellationToken cancellationToken);
        void Quit();
    }

}