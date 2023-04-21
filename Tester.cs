using System.Runtime.InteropServices;
using Workaround.Memory;

namespace Workaround;

class Program
{
    public unsafe static void Main()
    {
        int *ptr = null;
        Manager.AllocatePinnedGeneric<int>(ref ptr);
        
        Console.WriteLine(*ptr);

        Manager.FreeGenericPtr(ref ptr);
    }
}