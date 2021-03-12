using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.IOC
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParamterAttribute:Attribute
    {
    }
}
