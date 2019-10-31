//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.10
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace RakNet {

public class RakNetListUnsignedInt : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RakNetListUnsignedInt(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(RakNetListUnsignedInt obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~RakNetListUnsignedInt() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          RakNetPINVOKE.CSharp_RakNet_delete_RakNetListUnsignedInt(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public uint Get(uint position) {
    SWIGTYPE_p_unsigned_int ret = GetHelper(position);
    return UnsignedIntPointer.frompointer(ret).value();
  }

  public uint Pop() {
    SWIGTYPE_p_unsigned_int ret = PopHelper();
    return UnsignedIntPointer.frompointer(ret).value();
  }
    public uint this[int index]  
    {  
        get   
        {
            return Get((uint)index); // use indexto retrieve and return another value.    
        }  
        set   
        {
            Replace(value, value, (uint)index, "Not used", 0);// use index and value to set the value somewhere.   
        }  
    } 

  public RakNetListUnsignedInt() : this(RakNetPINVOKE.CSharp_RakNet_new_RakNetListUnsignedInt__SWIG_0(), true) {
  }

  public RakNetListUnsignedInt(RakNetListUnsignedInt original_copy) : this(RakNetPINVOKE.CSharp_RakNet_new_RakNetListUnsignedInt__SWIG_1(RakNetListUnsignedInt.getCPtr(original_copy)), true) {
    if (RakNetPINVOKE.SWIGPendingException.Pending) throw RakNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public RakNetListUnsignedInt CopyData(RakNetListUnsignedInt original_copy) {
    RakNetListUnsignedInt ret = new RakNetListUnsignedInt(RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_CopyData(swigCPtr, RakNetListUnsignedInt.getCPtr(original_copy)), false);
    if (RakNetPINVOKE.SWIGPendingException.Pending) throw RakNetPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private SWIGTYPE_p_unsigned_int GetHelper(uint position) {
    SWIGTYPE_p_unsigned_int ret = new SWIGTYPE_p_unsigned_int(RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_GetHelper(swigCPtr, position), false);
    return ret;
  }

  public void Push(uint input, string file, uint line) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Push(swigCPtr, input, file, line);
  }

  private SWIGTYPE_p_unsigned_int PopHelper() {
    SWIGTYPE_p_unsigned_int ret = new SWIGTYPE_p_unsigned_int(RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_PopHelper(swigCPtr), false);
    return ret;
  }

  public void Insert(uint input, uint position, string file, uint line) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Insert__SWIG_0(swigCPtr, input, position, file, line);
  }

  public void Insert(uint input, string file, uint line) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Insert__SWIG_1(swigCPtr, input, file, line);
  }

  public void Replace(uint input, uint filler, uint position, string file, uint line) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Replace__SWIG_0(swigCPtr, input, filler, position, file, line);
  }

  public void Replace(uint input) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Replace__SWIG_1(swigCPtr, input);
  }

  public void RemoveAtIndex(uint position) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_RemoveAtIndex(swigCPtr, position);
  }

  public void RemoveAtIndexFast(uint position) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_RemoveAtIndexFast(swigCPtr, position);
  }

  public void RemoveFromEnd(uint num) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_RemoveFromEnd__SWIG_0(swigCPtr, num);
  }

  public void RemoveFromEnd() {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_RemoveFromEnd__SWIG_1(swigCPtr);
  }

  public uint GetIndexOf(uint input) {
    uint ret = RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_GetIndexOf(swigCPtr, input);
    return ret;
  }

  public uint Size() {
    uint ret = RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Size(swigCPtr);
    return ret;
  }

  public void Clear(bool doNotDeallocateSmallBlocks, string file, uint line) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Clear(swigCPtr, doNotDeallocateSmallBlocks, file, line);
  }

  public void Preallocate(uint countNeeded, string file, uint line) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Preallocate(swigCPtr, countNeeded, file, line);
  }

  public void Compress(string file, uint line) {
    RakNetPINVOKE.CSharp_RakNet_RakNetListUnsignedInt_Compress(swigCPtr, file, line);
  }

}

}
