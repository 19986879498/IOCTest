using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.IOC
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContainerProptry:Attribute
    {
    }
}
