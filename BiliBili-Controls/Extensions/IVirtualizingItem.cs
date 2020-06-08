using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Controls.Extensions
{
    public interface IVirtualizingItem
    {
        bool IsVirtualized { get; }
        bool CanVirtualize { get; }
        bool CanRealize { get; }
        void Virtualize();
        void Realize();
    }
}
