using System.Runtime.InteropServices;

namespace Workaround.Memory;

public static class Manager
{
    public unsafe static void WritePinned<T>(T* ptr, T obj) where T: unmanaged
    {
        *ptr = obj;
    }

    public unsafe static void WritePinned<T>(T* ptr, T[] obj) where T: unmanaged
    {
        for(int i = 0; i < obj.Length; i++)
        {
            ptr[i] = obj[i];
        }
    }

    public unsafe static void AllocatePinnedBytes(ref byte* buff, int length = 1)
    {
        IntPtr bts = Marshal.AllocHGlobal(sizeof(byte) * length);
        buff = new ManagedPointer<byte>((byte *)bts, sizeof(byte) * length);
    }

    public unsafe static void AllocatePinnedGeneric<T>(ref T* buff, int length = 1) where T: unmanaged
    {
        IntPtr ptr = Marshal.AllocHGlobal(sizeof(T) * length);
        buff = new ManagedPointer<T>((T *)ptr, sizeof(T) * length);
    }

    public unsafe static void FreeGenericPtr<T>(ref T* ptr) where T: unmanaged
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }

    public unsafe static void FreeBytePtr(ref byte* bts)
    {
        Marshal.FreeHGlobal((IntPtr)bts);
    }
}