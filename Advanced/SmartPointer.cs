namespace Workaround.Advanced;

//this structure is huge and honestly likely to be very slow... but it is a nice class
public unsafe class SmartPointer<T> where T: unmanaged
{
    //precautions
    private bool isfreed { get; set; } = false;

    //values
    public ManagedPointer<T>? pointer {
        get {
            if(isfreed == true)
            {
                throw new Exception("Attempted to access freed pointer!");
            }

            return pointer;
        }

        set {
            if(isfreed == true)
            {
                throw new Exception("Attempted to access freed pointer!");
            }

            pointer = value;
        }
    }
    public int size {
        get {
            if(pointer == null)
            {
                return 0;
            }

            return pointer.size;
        }
    }

    public T this[int index]
    {
        get {
            if(pointer == null)
            {
                throw new NullReferenceException();
            }

            if(isfreed == true)
            {
                throw new Exception("Attempted to access freed pointer!");
            }

            return pointer[index];
        }
        
        set {
            if(pointer == null)
            {
                throw new NullReferenceException();
            }

            if(isfreed == true)
            {
                throw new Exception("Attempted to access freed pointer!");
            }

            pointer[index] = value;
        }
    }

    public SmartPointer(ManagedPointer<T> ptr)
    {
        pointer = ptr;
    }

    public void Free()
    {
        if(pointer != null)
        {
            pointer.Free();
            pointer = null;
        }

        isfreed = true;
    }

    public unsafe static implicit operator T*(SmartPointer<T> ptr) => ptr.pointer!.unmanaged;
    public unsafe static implicit operator Span<T>(SmartPointer<T> ptr) => new Span<T>(ptr.pointer!.unmanaged, ptr.size / sizeof(T));
    
    public unsafe static implicit operator SmartPointer<T>(Span<T> span)
    {
        SmartPointer<T> ptr = new SmartPointer<T>(new ManagedPointer<T>(span.Length));
        
        for(int i = 0; i < span.Length; i++)
        {
            ptr[i] = span[i];
        }

        return ptr;
    }

    public static implicit operator ManagedPointer<T>(SmartPointer<T> ptr) => ptr.pointer!;

    public static implicit operator SmartPointer<T>(ManagedPointer<T> ptr)
    {
        return new SmartPointer<T>(ptr);
    }

    ~SmartPointer()
    {
        isfreed = true;
        this.Free();
    }
}