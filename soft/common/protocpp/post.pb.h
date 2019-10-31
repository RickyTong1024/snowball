// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: post.proto

#ifndef PROTOBUF_INCLUDED_post_2eproto
#define PROTOBUF_INCLUDED_post_2eproto

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
#define PROTOBUF_INTERNAL_EXPORT_protobuf_post_2eproto 

namespace protobuf_post_2eproto {
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
}  // namespace protobuf_post_2eproto
namespace dhc {
class post_t;
class post_tDefaultTypeInternal;
extern post_tDefaultTypeInternal _post_t_default_instance_;
}  // namespace dhc
namespace google {
namespace protobuf {
template<> ::dhc::post_t* Arena::CreateMaybeMessage<::dhc::post_t>(Arena*);
}  // namespace protobuf
}  // namespace google
namespace dhc {

// ===================================================================

class post_t : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:dhc.post_t) */ {
 public:
  post_t();
  virtual ~post_t();

  post_t(const post_t& from);

  inline post_t& operator=(const post_t& from) {
    CopyFrom(from);
    return *this;
  }
  #if LANG_CXX11
  post_t(post_t&& from) noexcept
    : post_t() {
    *this = ::std::move(from);
  }

  inline post_t& operator=(post_t&& from) noexcept {
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
  static const post_t& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const post_t* internal_default_instance() {
    return reinterpret_cast<const post_t*>(
               &_post_t_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  void Swap(post_t* other);
  friend void swap(post_t& a, post_t& b) {
    a.Swap(&b);
  }

  // implements Message ----------------------------------------------

  inline post_t* New() const final {
    return CreateMaybeMessage<post_t>(NULL);
  }

  post_t* New(::google::protobuf::Arena* arena) const final {
    return CreateMaybeMessage<post_t>(arena);
  }
  void CopyFrom(const ::google::protobuf::Message& from) final;
  void MergeFrom(const ::google::protobuf::Message& from) final;
  void CopyFrom(const post_t& from);
  void MergeFrom(const post_t& from);
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
  void InternalSwap(post_t* other);
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

  // repeated int32 type = 7;
  int type_size() const;
  void clear_type();
  static const int kTypeFieldNumber = 7;
  ::google::protobuf::int32 type(int index) const;
  void set_type(int index, ::google::protobuf::int32 value);
  void add_type(::google::protobuf::int32 value);
  const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
      type() const;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
      mutable_type();

  // repeated int32 value1 = 8;
  int value1_size() const;
  void clear_value1();
  static const int kValue1FieldNumber = 8;
  ::google::protobuf::int32 value1(int index) const;
  void set_value1(int index, ::google::protobuf::int32 value);
  void add_value1(::google::protobuf::int32 value);
  const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
      value1() const;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
      mutable_value1();

  // repeated int32 value2 = 9;
  int value2_size() const;
  void clear_value2();
  static const int kValue2FieldNumber = 9;
  ::google::protobuf::int32 value2(int index) const;
  void set_value2(int index, ::google::protobuf::int32 value);
  void add_value2(::google::protobuf::int32 value);
  const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
      value2() const;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
      mutable_value2();

  // repeated int32 value3 = 10;
  int value3_size() const;
  void clear_value3();
  static const int kValue3FieldNumber = 10;
  ::google::protobuf::int32 value3(int index) const;
  void set_value3(int index, ::google::protobuf::int32 value);
  void add_value3(::google::protobuf::int32 value);
  const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
      value3() const;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
      mutable_value3();

  // optional string title = 4;
  bool has_title() const;
  void clear_title();
  static const int kTitleFieldNumber = 4;
  const ::std::string& title() const;
  void set_title(const ::std::string& value);
  #if LANG_CXX11
  void set_title(::std::string&& value);
  #endif
  void set_title(const char* value);
  void set_title(const char* value, size_t size);
  ::std::string* mutable_title();
  ::std::string* release_title();
  void set_allocated_title(::std::string* title);

  // optional string text = 5;
  bool has_text() const;
  void clear_text();
  static const int kTextFieldNumber = 5;
  const ::std::string& text() const;
  void set_text(const ::std::string& value);
  #if LANG_CXX11
  void set_text(::std::string&& value);
  #endif
  void set_text(const char* value);
  void set_text(const char* value, size_t size);
  ::std::string* mutable_text();
  ::std::string* release_text();
  void set_allocated_text(::std::string* text);

  // optional string sender_name = 6;
  bool has_sender_name() const;
  void clear_sender_name();
  static const int kSenderNameFieldNumber = 6;
  const ::std::string& sender_name() const;
  void set_sender_name(const ::std::string& value);
  #if LANG_CXX11
  void set_sender_name(::std::string&& value);
  #endif
  void set_sender_name(const char* value);
  void set_sender_name(const char* value, size_t size);
  ::std::string* mutable_sender_name();
  ::std::string* release_sender_name();
  void set_allocated_sender_name(::std::string* sender_name);

  // optional uint64 guid = 1;
  bool has_guid() const;
  void clear_guid();
  static const int kGuidFieldNumber = 1;
  ::google::protobuf::uint64 guid() const;
  void set_guid(::google::protobuf::uint64 value);

  // optional uint64 player_guid = 2;
  bool has_player_guid() const;
  void clear_player_guid();
  static const int kPlayerGuidFieldNumber = 2;
  ::google::protobuf::uint64 player_guid() const;
  void set_player_guid(::google::protobuf::uint64 value);

  // optional uint64 send_date = 3;
  bool has_send_date() const;
  void clear_send_date();
  static const int kSendDateFieldNumber = 3;
  ::google::protobuf::uint64 send_date() const;
  void set_send_date(::google::protobuf::uint64 value);

  // optional int32 is_read = 11;
  bool has_is_read() const;
  void clear_is_read();
  static const int kIsReadFieldNumber = 11;
  ::google::protobuf::int32 is_read() const;
  void set_is_read(::google::protobuf::int32 value);

  // optional int32 is_pick = 12;
  bool has_is_pick() const;
  void clear_is_pick();
  static const int kIsPickFieldNumber = 12;
  ::google::protobuf::int32 is_pick() const;
  void set_is_pick(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:dhc.post_t)
 private:
  void set_has_guid();
  void clear_has_guid();
  void set_has_player_guid();
  void clear_has_player_guid();
  void set_has_send_date();
  void clear_has_send_date();
  void set_has_title();
  void clear_has_title();
  void set_has_text();
  void clear_has_text();
  void set_has_sender_name();
  void clear_has_sender_name();
  void set_has_is_read();
  void clear_has_is_read();
  void set_has_is_pick();
  void clear_has_is_pick();

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::internal::HasBits<1> _has_bits_;
  mutable ::google::protobuf::internal::CachedSize _cached_size_;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 > type_;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 > value1_;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 > value2_;
  ::google::protobuf::RepeatedField< ::google::protobuf::int32 > value3_;
  ::google::protobuf::internal::ArenaStringPtr title_;
  ::google::protobuf::internal::ArenaStringPtr text_;
  ::google::protobuf::internal::ArenaStringPtr sender_name_;
  ::google::protobuf::uint64 guid_;
  ::google::protobuf::uint64 player_guid_;
  ::google::protobuf::uint64 send_date_;
  ::google::protobuf::int32 is_read_;
  ::google::protobuf::int32 is_pick_;
  friend struct ::protobuf_post_2eproto::TableStruct;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// post_t

// optional uint64 guid = 1;
inline bool post_t::has_guid() const {
  return (_has_bits_[0] & 0x00000008u) != 0;
}
inline void post_t::set_has_guid() {
  _has_bits_[0] |= 0x00000008u;
}
inline void post_t::clear_has_guid() {
  _has_bits_[0] &= ~0x00000008u;
}
inline void post_t::clear_guid() {
  set_changed();
  guid_ = GOOGLE_ULONGLONG(0);
  clear_has_guid();
}
inline ::google::protobuf::uint64 post_t::guid() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.guid)
  return guid_;
}
inline void post_t::set_guid(::google::protobuf::uint64 value) {
  set_changed();
  set_has_guid();
  guid_ = value;
  // @@protoc_insertion_point(field_set:dhc.post_t.guid)
}

// optional uint64 player_guid = 2;
inline bool post_t::has_player_guid() const {
  return (_has_bits_[0] & 0x00000010u) != 0;
}
inline void post_t::set_has_player_guid() {
  _has_bits_[0] |= 0x00000010u;
}
inline void post_t::clear_has_player_guid() {
  _has_bits_[0] &= ~0x00000010u;
}
inline void post_t::clear_player_guid() {
  set_changed();
  player_guid_ = GOOGLE_ULONGLONG(0);
  clear_has_player_guid();
}
inline ::google::protobuf::uint64 post_t::player_guid() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.player_guid)
  return player_guid_;
}
inline void post_t::set_player_guid(::google::protobuf::uint64 value) {
  set_changed();
  set_has_player_guid();
  player_guid_ = value;
  // @@protoc_insertion_point(field_set:dhc.post_t.player_guid)
}

// optional uint64 send_date = 3;
inline bool post_t::has_send_date() const {
  return (_has_bits_[0] & 0x00000020u) != 0;
}
inline void post_t::set_has_send_date() {
  _has_bits_[0] |= 0x00000020u;
}
inline void post_t::clear_has_send_date() {
  _has_bits_[0] &= ~0x00000020u;
}
inline void post_t::clear_send_date() {
  set_changed();
  send_date_ = GOOGLE_ULONGLONG(0);
  clear_has_send_date();
}
inline ::google::protobuf::uint64 post_t::send_date() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.send_date)
  return send_date_;
}
inline void post_t::set_send_date(::google::protobuf::uint64 value) {
  set_changed();
  set_has_send_date();
  send_date_ = value;
  // @@protoc_insertion_point(field_set:dhc.post_t.send_date)
}

// optional string title = 4;
inline bool post_t::has_title() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void post_t::set_has_title() {
  _has_bits_[0] |= 0x00000001u;
}
inline void post_t::clear_has_title() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void post_t::clear_title() {
  set_changed();
  title_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  clear_has_title();
}
inline const ::std::string& post_t::title() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.title)
  return title_.GetNoArena();
}
inline void post_t::set_title(const ::std::string& value) {
  set_changed();
  set_has_title();
  title_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:dhc.post_t.title)
}
#if LANG_CXX11
inline void post_t::set_title(::std::string&& value) {
  set_changed();
  set_has_title();
  title_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:dhc.post_t.title)
}
#endif
inline void post_t::set_title(const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  set_has_title();
  title_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:dhc.post_t.title)
}
inline void post_t::set_title(const char* value, size_t size) {
  set_changed();
  set_has_title();
  title_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:dhc.post_t.title)
}
inline ::std::string* post_t::mutable_title() {
  set_changed();
  set_has_title();
  // @@protoc_insertion_point(field_mutable:dhc.post_t.title)
  return title_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* post_t::release_title() {
  set_changed();
  // @@protoc_insertion_point(field_release:dhc.post_t.title)
  if (!has_title()) {
    return NULL;
  }
  clear_has_title();
  return title_.ReleaseNonDefaultNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void post_t::set_allocated_title(::std::string* title) {
  set_changed();
  if (title != NULL) {
    set_has_title();
  } else {
    clear_has_title();
  }
  title_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), title);
  // @@protoc_insertion_point(field_set_allocated:dhc.post_t.title)
}

// optional string text = 5;
inline bool post_t::has_text() const {
  return (_has_bits_[0] & 0x00000002u) != 0;
}
inline void post_t::set_has_text() {
  _has_bits_[0] |= 0x00000002u;
}
inline void post_t::clear_has_text() {
  _has_bits_[0] &= ~0x00000002u;
}
inline void post_t::clear_text() {
  set_changed();
  text_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  clear_has_text();
}
inline const ::std::string& post_t::text() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.text)
  return text_.GetNoArena();
}
inline void post_t::set_text(const ::std::string& value) {
  set_changed();
  set_has_text();
  text_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:dhc.post_t.text)
}
#if LANG_CXX11
inline void post_t::set_text(::std::string&& value) {
  set_changed();
  set_has_text();
  text_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:dhc.post_t.text)
}
#endif
inline void post_t::set_text(const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  set_has_text();
  text_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:dhc.post_t.text)
}
inline void post_t::set_text(const char* value, size_t size) {
  set_changed();
  set_has_text();
  text_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:dhc.post_t.text)
}
inline ::std::string* post_t::mutable_text() {
  set_changed();
  set_has_text();
  // @@protoc_insertion_point(field_mutable:dhc.post_t.text)
  return text_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* post_t::release_text() {
  set_changed();
  // @@protoc_insertion_point(field_release:dhc.post_t.text)
  if (!has_text()) {
    return NULL;
  }
  clear_has_text();
  return text_.ReleaseNonDefaultNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void post_t::set_allocated_text(::std::string* text) {
  set_changed();
  if (text != NULL) {
    set_has_text();
  } else {
    clear_has_text();
  }
  text_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), text);
  // @@protoc_insertion_point(field_set_allocated:dhc.post_t.text)
}

// optional string sender_name = 6;
inline bool post_t::has_sender_name() const {
  return (_has_bits_[0] & 0x00000004u) != 0;
}
inline void post_t::set_has_sender_name() {
  _has_bits_[0] |= 0x00000004u;
}
inline void post_t::clear_has_sender_name() {
  _has_bits_[0] &= ~0x00000004u;
}
inline void post_t::clear_sender_name() {
  set_changed();
  sender_name_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  clear_has_sender_name();
}
inline const ::std::string& post_t::sender_name() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.sender_name)
  return sender_name_.GetNoArena();
}
inline void post_t::set_sender_name(const ::std::string& value) {
  set_changed();
  set_has_sender_name();
  sender_name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:dhc.post_t.sender_name)
}
#if LANG_CXX11
inline void post_t::set_sender_name(::std::string&& value) {
  set_changed();
  set_has_sender_name();
  sender_name_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:dhc.post_t.sender_name)
}
#endif
inline void post_t::set_sender_name(const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  set_has_sender_name();
  sender_name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:dhc.post_t.sender_name)
}
inline void post_t::set_sender_name(const char* value, size_t size) {
  set_changed();
  set_has_sender_name();
  sender_name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:dhc.post_t.sender_name)
}
inline ::std::string* post_t::mutable_sender_name() {
  set_changed();
  set_has_sender_name();
  // @@protoc_insertion_point(field_mutable:dhc.post_t.sender_name)
  return sender_name_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* post_t::release_sender_name() {
  set_changed();
  // @@protoc_insertion_point(field_release:dhc.post_t.sender_name)
  if (!has_sender_name()) {
    return NULL;
  }
  clear_has_sender_name();
  return sender_name_.ReleaseNonDefaultNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void post_t::set_allocated_sender_name(::std::string* sender_name) {
  set_changed();
  if (sender_name != NULL) {
    set_has_sender_name();
  } else {
    clear_has_sender_name();
  }
  sender_name_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), sender_name);
  // @@protoc_insertion_point(field_set_allocated:dhc.post_t.sender_name)
}

// repeated int32 type = 7;
inline int post_t::type_size() const {
  return type_.size();
}
inline void post_t::clear_type() {
  set_changed();
  type_.Clear();
}
inline ::google::protobuf::int32 post_t::type(int index) const {
  // @@protoc_insertion_point(field_get:dhc.post_t.type)
  return type_.Get(index);
}
inline void post_t::set_type(int index, ::google::protobuf::int32 value) {
  set_changed();
  type_.Set(index, value);
  // @@protoc_insertion_point(field_set:dhc.post_t.type)
}
inline void post_t::add_type(::google::protobuf::int32 value) {
  set_changed();
  type_.Add(value);
  // @@protoc_insertion_point(field_add:dhc.post_t.type)
}
inline const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
post_t::type() const {
  // @@protoc_insertion_point(field_list:dhc.post_t.type)
  return type_;
}
inline ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
post_t::mutable_type() {
  set_changed();
  // @@protoc_insertion_point(field_mutable_list:dhc.post_t.type)
  return &type_;
}

// repeated int32 value1 = 8;
inline int post_t::value1_size() const {
  return value1_.size();
}
inline void post_t::clear_value1() {
  set_changed();
  value1_.Clear();
}
inline ::google::protobuf::int32 post_t::value1(int index) const {
  // @@protoc_insertion_point(field_get:dhc.post_t.value1)
  return value1_.Get(index);
}
inline void post_t::set_value1(int index, ::google::protobuf::int32 value) {
  set_changed();
  value1_.Set(index, value);
  // @@protoc_insertion_point(field_set:dhc.post_t.value1)
}
inline void post_t::add_value1(::google::protobuf::int32 value) {
  set_changed();
  value1_.Add(value);
  // @@protoc_insertion_point(field_add:dhc.post_t.value1)
}
inline const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
post_t::value1() const {
  // @@protoc_insertion_point(field_list:dhc.post_t.value1)
  return value1_;
}
inline ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
post_t::mutable_value1() {
  set_changed();
  // @@protoc_insertion_point(field_mutable_list:dhc.post_t.value1)
  return &value1_;
}

// repeated int32 value2 = 9;
inline int post_t::value2_size() const {
  return value2_.size();
}
inline void post_t::clear_value2() {
  set_changed();
  value2_.Clear();
}
inline ::google::protobuf::int32 post_t::value2(int index) const {
  // @@protoc_insertion_point(field_get:dhc.post_t.value2)
  return value2_.Get(index);
}
inline void post_t::set_value2(int index, ::google::protobuf::int32 value) {
  set_changed();
  value2_.Set(index, value);
  // @@protoc_insertion_point(field_set:dhc.post_t.value2)
}
inline void post_t::add_value2(::google::protobuf::int32 value) {
  set_changed();
  value2_.Add(value);
  // @@protoc_insertion_point(field_add:dhc.post_t.value2)
}
inline const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
post_t::value2() const {
  // @@protoc_insertion_point(field_list:dhc.post_t.value2)
  return value2_;
}
inline ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
post_t::mutable_value2() {
  set_changed();
  // @@protoc_insertion_point(field_mutable_list:dhc.post_t.value2)
  return &value2_;
}

// repeated int32 value3 = 10;
inline int post_t::value3_size() const {
  return value3_.size();
}
inline void post_t::clear_value3() {
  set_changed();
  value3_.Clear();
}
inline ::google::protobuf::int32 post_t::value3(int index) const {
  // @@protoc_insertion_point(field_get:dhc.post_t.value3)
  return value3_.Get(index);
}
inline void post_t::set_value3(int index, ::google::protobuf::int32 value) {
  set_changed();
  value3_.Set(index, value);
  // @@protoc_insertion_point(field_set:dhc.post_t.value3)
}
inline void post_t::add_value3(::google::protobuf::int32 value) {
  set_changed();
  value3_.Add(value);
  // @@protoc_insertion_point(field_add:dhc.post_t.value3)
}
inline const ::google::protobuf::RepeatedField< ::google::protobuf::int32 >&
post_t::value3() const {
  // @@protoc_insertion_point(field_list:dhc.post_t.value3)
  return value3_;
}
inline ::google::protobuf::RepeatedField< ::google::protobuf::int32 >*
post_t::mutable_value3() {
  set_changed();
  // @@protoc_insertion_point(field_mutable_list:dhc.post_t.value3)
  return &value3_;
}

// optional int32 is_read = 11;
inline bool post_t::has_is_read() const {
  return (_has_bits_[0] & 0x00000040u) != 0;
}
inline void post_t::set_has_is_read() {
  _has_bits_[0] |= 0x00000040u;
}
inline void post_t::clear_has_is_read() {
  _has_bits_[0] &= ~0x00000040u;
}
inline void post_t::clear_is_read() {
  set_changed();
  is_read_ = 0;
  clear_has_is_read();
}
inline ::google::protobuf::int32 post_t::is_read() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.is_read)
  return is_read_;
}
inline void post_t::set_is_read(::google::protobuf::int32 value) {
  set_changed();
  set_has_is_read();
  is_read_ = value;
  // @@protoc_insertion_point(field_set:dhc.post_t.is_read)
}

// optional int32 is_pick = 12;
inline bool post_t::has_is_pick() const {
  return (_has_bits_[0] & 0x00000080u) != 0;
}
inline void post_t::set_has_is_pick() {
  _has_bits_[0] |= 0x00000080u;
}
inline void post_t::clear_has_is_pick() {
  _has_bits_[0] &= ~0x00000080u;
}
inline void post_t::clear_is_pick() {
  set_changed();
  is_pick_ = 0;
  clear_has_is_pick();
}
inline ::google::protobuf::int32 post_t::is_pick() const {
  // @@protoc_insertion_point(field_get:dhc.post_t.is_pick)
  return is_pick_;
}
inline void post_t::set_is_pick(::google::protobuf::int32 value) {
  set_changed();
  set_has_is_pick();
  is_pick_ = value;
  // @@protoc_insertion_point(field_set:dhc.post_t.is_pick)
}

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__

// @@protoc_insertion_point(namespace_scope)

}  // namespace dhc

// @@protoc_insertion_point(global_scope)

#endif  // PROTOBUF_INCLUDED_post_2eproto
