using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Exceptions;

/// <summary>
/// Veyrin 專用的測試斷言異常 (解決 CS0246)
/// </summary>
public class AssertException : Exception
{
    public AssertException(string message) : base(message) { }
}
