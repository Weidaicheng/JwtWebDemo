using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtWebDemo
{
    /// <summary>
    /// Jwt检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class JwtCheckAttribute : Attribute
    {
    }
}
