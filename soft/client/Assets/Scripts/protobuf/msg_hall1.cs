//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: msg_hall1.proto
namespace protocol.game
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"cmsg_recharge_google")]
  public partial class cmsg_recharge_google : global::ProtoBuf.IExtensible
  {
    public cmsg_recharge_google() {}
    
    private int _id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int id
    {
      get { return _id; }
      set { _id = value; }
    }
    private string _package_name;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"package_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string package_name
    {
      get { return _package_name; }
      set { _package_name = value; }
    }
    private string _product_id;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"product_id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string product_id
    {
      get { return _product_id; }
      set { _product_id = value; }
    }
    private string _purchase_token;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"purchase_token", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string purchase_token
    {
      get { return _purchase_token; }
      set { _purchase_token = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}