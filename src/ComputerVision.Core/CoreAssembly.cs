using System.Reflection;

namespace ComputerVision.Core
{
    public class CoreAssembly
    {
        public static Assembly Assembly => typeof (CoreAssembly).Assembly;
    }
}
