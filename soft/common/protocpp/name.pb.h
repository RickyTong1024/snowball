// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: name.proto

#ifndef PROTOBUF_INCLUDED_name_2eproto
#define PROTOBUF_INCLUDED_name_2eproto

#include <string>

#include <google/protobuf/stubs/common.h>

#if GOOGLE_PROTOBUF_VERSION < 3006001
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please update
#error your headers.
#endif
#if 3006001 < GOOGLE_PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_table_driven.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/inlined_string_field.h>
#include <google/protobuf/metadata.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/unknown_field_set.h>
// @@protoc_insertion_point(includes)
#define PROTOBUF_INTERNAL_EXPORT_protobuf_name_2eproto 

namespace protobuf_name_2eproto {
// Internal implementation detail -- do not use these members.
struct TableStruct {
  static const ::google::protobuf::internal::ParseTableField entries[];
  static const ::google::protobuf::internal::AuxillaryParseTableField aux[];
  static const ::google::protobuf::internal::ParseTable schema[1];
  static const ::google::protobuf::internal::FieldMetadata field_metadata[];
  static const ::google::protobuf::internal::SerializationTable serialization_table[];
  static const ::google::protobuf::uint32 offsets[];
};
void AddDescriptors();
}  // namespace protobuf_name_2eproto
namespace dhc {
class name_t;
class name_tDefaultTypeInternal;
extern name_tDefaultTypeInternal _name_t_default_instance_;
}  // namespace dhc
namespace google {
namespace protobuf {
template<> ::dhc::name_t* Arena::CreateMaybeMessage<::dhc::name_t>(Arena*);
}  // namespace protobuf
}  // namespace google
namespace dhc {

// ===================================================================

class name_t : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:dhc.name_t) */ {
 public:
  name_t();
  virtual ~name_t();

  name_t(const name_t& from);

  inline name_t& operator=(const name_t& from) {
    CopyFrom(from);
    return *this;
  }
  #if LANG_CXX11
  name_t(name_t&& from) noexcept
    : name_t() {
    *this = ::std::move(from);
  }

  inline name_t& operator=(name_t&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }
  #endif
  inline const ::google::protobuf::UnknownFieldSet& unknown_fields() const {
    return _internal_metadata_.unknown_fields();
  }
  inline ::google::protobuf::UnknownFieldSet* mutable_unknown_fields() {
    return _internal_metadata_.mutable_unknown_fields();
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const name_t& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const name_t* internal_default_instance() {
    return reinterpret_cast<const name_t*>(
               &_name_t_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  void Swap(name_t* other);
  friend void swap(name_t& a, name_t& b) {
    a.Swap(&b);
  }

  // implements Message ----------------------------------------------

  inline name_t* New() const final {
    return CreateMaybeMessage<name_t>(NULL);
  }

  name_t* New(::google::protobuf::Arena* arena) const final {
    return CreateMaybeMessage<name_t>(arena);
  }
  void CopyFrom(const ::google::protobuf::Message& from) final;
  void MergeFrom(const ::google::protobuf::Message& from) final;
  void CopyFrom(const name_t& from);
  void MergeFrom(const name_t& from);
  void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input) final;
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const final;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* target) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(name_t* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return NULL;
  }
  inline void* MaybeArenaPtr() const {
    return NULL;
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const final;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // optional string name = 2;
  bool has_name() const;
  void clear_name();
  static const int kNameFieldNumber = 2;
  const ::std::string& name() const;
  void set_name(const ::std::string& value);
  #if LANG_CXX11
  void set_name(::std::string&& value);
  #endif
  void set_name(const char* value);
  void set_name(const char* value, size_t size);
  ::std::string* mutable_name();
  ::std::string* release_name();
  void set_allocated_name(::std::string* name);

  // optional uint64 guid = 1;
  bool has_guid() const;
  void clear_guid();
  static const int kGuidFieldNumber = 1;
  ::google::protobuf::uint64 guid() const;
  void set_guid(::google::protobuf::uint64 value);

  // @@protoc_insertion_point(class_scope:dhc.name_t)
 private:
  void set_has_guid();
  void clear_has_guid();
  void set_has_name();
  void clear_has_name();

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::internal::HasBits<1> _has_bits_;
  mutable ::google::protobuf::internal::CachedSize _cached_size_;
  ::google::protobuf::internal::ArenaStringPtr name_;
  ::google::protobuf::uint64 guid_;
  friend struct ::protobuf_name_2eproto::TableStruct;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// name_t

// optional uint64 guid = 1;
inline bool name_t::has_guid() const {
  return (_has_bits_[0] & 0x00000002u) != 0;
}
inline void name_t::set_has_guid() {
  _has_bits_[0] |= 0x00000002u;
}
inline void name_t::clear_has_guid() {
  _has_bits_[0] &= ~0x00000002u;
}
inline void name_t::clear_guid() {
  set_changed();
  guid_ = GOOGLE_ULONGLONG(0);
  clear_has_guid();
}
inline ::google::protobuf::uint64 name_t::guid() const {
  // @@protoc_insertion_point(field_get:dhc.name_t.guid)
  return guid_;
}
inline void name_t::set_guid(::google::protobuf::uint64 value) {
  set_changed();
  set_has_guid();
  guid_ = value;
  // @@protoc_insertion_point(field_set:dhc.name_t.guid)
}

// optional string name = 2;
inline bool name_t::has_name() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void name_t::set_has_name() {
  _has_bits_[0] |= 0x00000001u;
}
inline void name_t::clear_has_name() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void name_t::clear_name() {
  set_changed();
  name_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  clear_has_name();
}
inline const ::std::string& name_t::name() const {
  // @@protoc_insertion_point(field_get:dhc.name_t.name)
  return name_.GetNoArena();
}
inline void name_t::set_name(const ::std::string& value) {
  set_changed();
  set_has_name();
  name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:dhc.name_t.name)
}
#if LANG_CXX11
inline void name_t::set_name(::std::string&& value) {
  set_changed();
  set_has_name();
  name_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:dhc.name_t.name)
}
#endif
inline void name_t::set_name(const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  set_has_name();
  name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:dhc.name_t.name)
}
inline void name_t::set_name(const char* value, size_t size) {
  set_changed();
  set_has_name();
  name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:dhc.name_t.name)
}
inline ::std::string* name_t::mutable_name() {
  set_changed();
  set_has_name();
  // @@protoc_insertion_point(field_mutable:dhc.name_t.name)
  return name_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* name_t::release_name() {
  set_changed();
  // @@protoc_insertion_point(field_release:dhc.name_t.name)
  if (!has_name()) {
    return NULL;
  }
  clear_has_name();
  return name_.ReleaseNonDefaultNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void name_t::set_allocated_name(::std::string* name) {
  set_changed();
  if (name != NULL) {
    set_has_name();
  } else {
    clear_has_name();
  }
  name_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), name);
  // @@protoc_insertion_point(field_set_allocated:dhc.name_t.name)
}

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__

// @@protoc_insertion_point(namespace_scope)

}  // namespace dhc

// @@protoc_insertion_point(global_scope)

#endif  // PROTOBUF_INCLUDED_name_2eproto
