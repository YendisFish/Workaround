using System.Runtime.InteropServices;

namespace Workaround;

//this structure is subject to UAF errors! Please use with caution! I recommend the new structure Workaround.Advanced.SmartPointer<T>
public class ManagedPointer<T> where T: unmanaged
{
    public unsafe T* unmanaged { get; set; }
    public int size { get; init; }

    public unsafe T this[int index]
    {
        get {
            if(index > size / sizeof(T))
            {
                throw new IndexOutOfRangeException();
            } else {
                return unmanaged[index];
            }
        }

        set {
            if(index > size / sizeof(T))
            {
                throw new IndexOutOfRangeException();
            } else {
                unmanaged[index] = value;
            }
        }
    }

    public unsafe ManagedPointer(ref T obj, int size = 1)
    {
        IntPtr ptr = Marshal.AllocHGlobal(sizeof(T) * size);
        unmanaged = (T *)ptr;
        this.size = sizeof(T) * size;
        *unmanaged = obj;
    }

    public unsafe ManagedPointer(T* obj, int size = 1)
    {
        IntPtr ptr = Marshal.AllocHGlobal(sizeof(T) * size);
        unmanaged = (T *)ptr;
        this.size = sizeof(T) * size;
        *unmanaged = *obj;
    }

    public unsafe ManagedPointer(int size)
    {
        IntPtr ptr = Marshal.AllocHGlobal(sizeof(T) * size);
        unmanaged = (T *)ptr;
        this.size = sizeof(T) * size;
    }

    public unsafe T Dereference()
    {
        return *unmanaged;
    }

    public unsafe void Free()
    {
        Marshal.FreeHGlobal((IntPtr)unmanaged);
    }

    public unsafe static ManagedPointer<char> FromString(string str)
    {
        ManagedPointer<char> ret;
        fixed(char *ptr = str)
        {
            ret = new ManagedPointer<char>(ptr, str.Length);
        }

        for(int i = 0; i < str.Length; i++)
        {
            ret[i] = str[i];
        }

        return ret;
    }

    public unsafe static implicit operator ManagedPointer<T>(T* ptr)
    {
        ManagedPointer<T> ret = new(ref *ptr, Marshal.SizeOf(*ptr));
        return ret;
    }

    public unsafe static implicit operator ManagedPointer<T>(T val)
    {
        return &val;
    }

    public unsafe static implicit operator T*(ManagedPointer<T> ptr) => ptr.unmanaged;
    public unsafe static implicit operator Span<T>(ManagedPointer<T> ptr) => new Span<T>(ptr.unmanaged, ptr.size / sizeof(T));
    
    public unsafe static implicit operator ManagedPointer<T>(Span<T> span)
    {
        ManagedPointer<T> ptr = new ManagedPointer<T>(span.Length);
        
        for(int i = 0; i < span.Length; i++)
        {
            ptr[i] = span[i];
        }

        return ptr;
    }

    ~ManagedPointer()
    {
        this.Free();
    }
}

public static class ManagedPointerExtensions
{
    public static unsafe string AsString(this ManagedPointer<char> ptr)
    {
        string ret = "";

        for(int i = 0; i < ptr.size / sizeof(char); i++)
        {
            ret = ret + ptr[i];
        }

        return ret;
    }
}