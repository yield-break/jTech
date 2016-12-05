using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jTech.Common.Core;
using jTech.Service.Boot;

namespace jTech.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = CompositionContainerFactory.Instance;
            IServiceBoot serviceBoot = container.GetExportedValue<IServiceBoot>();
        }
    }
}
