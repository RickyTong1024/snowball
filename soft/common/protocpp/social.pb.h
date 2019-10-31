// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: social.proto

#ifndef PROTOBUF_INCLUDED_social_2eproto
#define PROTOBUF_INCLUDED_social_2eproto

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
#define PROTOBUF_INTERNAL_EXPORT_protobuf_social_2eproto 

namespace protobuf_social_2eproto {
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
}  // namespace protobuf_social_2eproto
namespace dhc {
class social_t;
class social_tDefaultTypeInternal;
extern social_tDefaultTypeInternal _social_t_default_instance_;
}  // namespace dhc
namespace google {
namespace protobuf {
template<> ::dhc::social_t* Arena::CreateMaybeMessage<::dhc::social_t>(Arena*);
}  // namespace protobuf
}  // namespace google
namespace dhc {

// ===================================================================

class social_t : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:dhc.social_t) */ {
 public:
  social_t();
  virtual ~social_t();

  social_t(const social_t& from);

  inline social_t& operator=(const social_t& from) {
    CopyFrom(from);
    return *this;
  }
  #if LANG_CXX11
  social_t(social_t&& from) noexcept
    : social_t() {
    *this = ::std::move(from);
  }

  inline social_t& operator=(social_t&& from) noexcept {
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
  static const social_t& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const social_t* internal_default_instance() {
    return reinterpret_cast<const social_t*>(
               &_social_t_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  void Swap(social_t* other);
  friend void swap(social_t& a, social_t& b) {
    a.Swap(&b);
  }

  // implements Message ----------------------------------------------

  inline social_t* New() const final {
    return CreateMaybeMessage<social_t>(NULL);
  }

  social_t* New(::google::protobuf::Arena* arena) const final {
    return CreateMaybeMessage<social_t>(arena);
  }
  void CopyFrom(const ::google::protobuf::Message& from) final;
  void MergeFrom(const ::google::protobuf::Message& from) final;
  void CopyFrom(const social_t& from);
  void MergeFrom(const social_t& from);
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
  void InternalSwap(social_t* other);
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

  // repeated string msgs = 16;
  int msgs_size() const;
  void clear_msgs();
  static const int kMsgsFieldNumber = 16;
  const ::std::string& msgs(int index) const;
  ::std::string* mutable_msgs(int index);
  void set_msgs(int index, const ::std::string& value);
  #if LANG_CXX11
  void set_msgs(int index, ::std::string&& value);
  #endif
  void set_msgs(int index, const char* value);
  void set_msgs(int index, const char* value, size_t size);
  ::std::string* add_msgs();
  void add_msgs(const ::std::string& value);
  #if LANG_CXX11
  void add_msgs(::std::string&& value);
  #endif
  void add_msgs(const char* value);
  void add_msgs(const char* value, size_t size);
  const ::google::protobuf::RepeatedPtrField< ::std::string>& msgs() const;
  ::google::protobuf::RepeatedPtrField< ::std::string>* mutable_msgs();

  // repeated uint64 msgtimes = 17;
  int msgtimes_size() const;
  void clear_msgtimes();
  static const int kMsgtimesFieldNumber = 17;
  ::google::protobuf::uint64 msgtimes(int index) const;
  void set_msgtimes(int index, ::google::protobuf::uint64 value);
  void add_msgtimes(::google::protobuf::uint64 value);
  const ::google::protobuf::RepeatedField< ::google::protobuf::uint64 >&
      msgtimes() const;
  ::google::protobuf::RepeatedField< ::google::protobuf::uint64 >*
      mutable_msgtimes();

  // optional string name = 4;
  bool has_name() const;
  void clear_name();
  static const int kNameFieldNumber = 4;
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

  // optional string verify = 14;
  bool has_verify() const;
  void clear_verify();
  static const int kVerifyFieldNumber = 14;
  const ::std::string& verify() const;
  void set_verify(const ::std::string& value);
  #if LANG_CXX11
  void set_verify(::std::string&& value);
  #endif
  void set_verify(const char* value);
  void set_verify(const char* value, size_t size);
  ::std::string* mutable_verify();
  ::std::string* release_verify();
  void set_allocated_verify(::std::string* verify);

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

  // optional uint64 target_guid = 3;
  bool has_target_guid() const;
  void clear_target_guid();
  static const int kTargetGuidFieldNumber = 3;
  ::google::protobuf::uint64 target_guid() const;
  void set_target_guid(::google::protobuf::uint64 value);

  // optional int32 cup = 5;
  bool has_cup() const;
  void clear_cup();
  static const int kCupFieldNumber = 5;
  ::google::protobuf::int32 cup() const;
  void set_cup(::google::protobuf::int32 value);

  // optional int32 avatar = 7;
  bool has_avatar() const;
  void clear_avatar();
  static const int kAvatarFieldNumber = 7;
  ::google::protobuf::int32 avatar() const;
  void set_avatar(::google::protobuf::int32 value);

  // optional int32 toukuang = 8;
  bool has_toukuang() const;
  void clear_toukuang();
  static const int kToukuangFieldNumber = 8;
  ::google::protobuf::int32 toukuang() const;
  void set_toukuang(::google::protobuf::int32 value);

  // optional int32 region_id = 9;
  bool has_region_id() const;
  void clear_region_id();
  static const int kRegionIdFieldNumber = 9;
  ::google::protobuf::int32 region_id() const;
  void set_region_id(::google::protobuf::int32 value);

  // optional int32 level = 10;
  bool has_level() const;
  void clear_level();
  static const int kLevelFieldNumber = 10;
  ::google::protobuf::int32 level() const;
  void set_level(::google::protobuf::int32 value);

  // optional int32 sex = 11;
  bool has_sex() const;
  void clear_sex();
  static const int kSexFieldNumber = 11;
  ::google::protobuf::int32 sex() const;
  void set_sex(::google::protobuf::int32 value);

  // optional int32 stype = 12;
  bool has_stype() const;
  void clear_stype();
  static const int kStypeFieldNumber = 12;
  ::google::protobuf::int32 stype() const;
  void set_stype(::google::protobuf::int32 value);

  // optional int32 sflag = 13;
  bool has_sflag() const;
  void clear_sflag();
  static const int kSflagFieldNumber = 13;
  ::google::protobuf::int32 sflag() const;
  void set_sflag(::google::protobuf::int32 value);

  // optional int32 gold = 15;
  bool has_gold() const;
  void clear_gold();
  static const int kGoldFieldNumber = 15;
  ::google::protobuf::int32 gold() const;
  void set_gold(::google::protobuf::int32 value);

  // optional int32 name_color = 19;
  bool has_name_color() const;
  void clear_name_color();
  static const int kNameColorFieldNumber = 19;
  ::google::protobuf::int32 name_color() const;
  void set_name_color(::google::protobuf::int32 value);

  // optional uint64 ttime = 18;
  bool has_ttime() const;
  void clear_ttime();
  static const int kTtimeFieldNumber = 18;
  ::google::protobuf::uint64 ttime() const;
  void set_ttime(::google::protobuf::uint64 value);

  // optional int32 achieve_point = 21;
  bool has_achieve_point() const;
  void clear_achieve_point();
  static const int kAchievePointFieldNumber = 21;
  ::google::protobuf::int32 achieve_point() const;
  void set_achieve_point(::google::protobuf::int32 value);

  // optional int32 max_score = 22;
  bool has_max_score() const;
  void clear_max_score();
  static const int kMaxScoreFieldNumber = 22;
  ::google::protobuf::int32 max_score() const;
  void set_max_score(::google::protobuf::int32 value);

  // optional int32 max_sha = 23;
  bool has_max_sha() const;
  void clear_max_sha();
  static const int kMaxShaFieldNumber = 23;
  ::google::protobuf::int32 max_sha() const;
  void set_max_sha(::google::protobuf::int32 value);

  // optional int32 max_lsha = 24;
  bool has_max_lsha() const;
  void clear_max_lsha();
  static const int kMaxLshaFieldNumber = 24;
  ::google::protobuf::int32 max_lsha() const;
  void set_max_lsha(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:dhc.social_t)
 private:
  void set_has_guid();
  void clear_has_guid();
  void set_has_player_guid();
  void clear_has_player_guid();
  void set_has_target_guid();
  void clear_has_target_guid();
  void set_has_name();
  void clear_has_name();
  void set_has_cup();
  void clear_has_cup();
  void set_has_avatar();
  void clear_has_avatar();
  void set_has_toukuang();
  void clear_has_toukuang();
  void set_has_region_id();
  void clear_has_region_id();
  void set_has_level();
  void clear_has_level();
  void set_has_sex();
  void clear_has_sex();
  void set_has_stype();
  void clear_has_stype();
  void set_has_sflag();
  void clear_has_sflag();
  void set_has_verify();
  void clear_has_verify();
  void set_has_gold();
  void clear_has_gold();
  void set_has_ttime();
  void clear_has_ttime();
  void set_has_name_color();
  void clear_has_name_color();
  void set_has_achieve_point();
  void clear_has_achieve_point();
  void set_has_max_score();
  void clear_has_max_score();
  void set_has_max_sha();
  void clear_has_max_sha();
  void set_has_max_lsha();
  void clear_has_max_lsha();

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::internal::HasBits<1> _has_bits_;
  mutable ::google::protobuf::internal::CachedSize _cached_size_;
  ::google::protobuf::RepeatedPtrField< ::std::string> msgs_;
  ::google::protobuf::RepeatedField< ::google::protobuf::uint64 > msgtimes_;
  ::google::protobuf::internal::ArenaStringPtr name_;
  ::google::protobuf::internal::ArenaStringPtr verify_;
  ::google::protobuf::uint64 guid_;
  ::google::protobuf::uint64 player_guid_;
  ::google::protobuf::uint64 target_guid_;
  ::google::protobuf::int32 cup_;
  ::google::protobuf::int32 avatar_;
  ::google::protobuf::int32 toukuang_;
  ::google::protobuf::int32 region_id_;
  ::google::protobuf::int32 level_;
  ::google::protobuf::int32 sex_;
  ::google::protobuf::int32 stype_;
  ::google::protobuf::int32 sflag_;
  ::google::protobuf::int32 gold_;
  ::google::protobuf::int32 name_color_;
  ::google::protobuf::uint64 ttime_;
  ::google::protobuf::int32 achieve_point_;
  ::google::protobuf::int32 max_score_;
  ::google::protobuf::int32 max_sha_;
  ::google::protobuf::int32 max_lsha_;
  friend struct ::protobuf_social_2eproto::TableStruct;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// social_t

// optional uint64 guid = 1;
inline bool social_t::has_guid() const {
  return (_has_bits_[0] & 0x00000004u) != 0;
}
inline void social_t::set_has_guid() {
  _has_bits_[0] |= 0x00000004u;
}
inline void social_t::clear_has_guid() {
  _has_bits_[0] &= ~0x00000004u;
}
inline void social_t::clear_guid() {
  set_changed();
  guid_ = GOOGLE_ULONGLONG(0);
  clear_has_guid();
}
inline ::google::protobuf::uint64 social_t::guid() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.guid)
  return guid_;
}
inline void social_t::set_guid(::google::protobuf::uint64 value) {
  set_changed();
  set_has_guid();
  guid_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.guid)
}

// optional uint64 player_guid = 2;
inline bool social_t::has_player_guid() const {
  return (_has_bits_[0] & 0x00000008u) != 0;
}
inline void social_t::set_has_player_guid() {
  _has_bits_[0] |= 0x00000008u;
}
inline void social_t::clear_has_player_guid() {
  _has_bits_[0] &= ~0x00000008u;
}
inline void social_t::clear_player_guid() {
  set_changed();
  player_guid_ = GOOGLE_ULONGLONG(0);
  clear_has_player_guid();
}
inline ::google::protobuf::uint64 social_t::player_guid() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.player_guid)
  return player_guid_;
}
inline void social_t::set_player_guid(::google::protobuf::uint64 value) {
  set_changed();
  set_has_player_guid();
  player_guid_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.player_guid)
}

// optional uint64 target_guid = 3;
inline bool social_t::has_target_guid() const {
  return (_has_bits_[0] & 0x00000010u) != 0;
}
inline void social_t::set_has_target_guid() {
  _has_bits_[0] |= 0x00000010u;
}
inline void social_t::clear_has_target_guid() {
  _has_bits_[0] &= ~0x00000010u;
}
inline void social_t::clear_target_guid() {
  set_changed();
  target_guid_ = GOOGLE_ULONGLONG(0);
  clear_has_target_guid();
}
inline ::google::protobuf::uint64 social_t::target_guid() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.target_guid)
  return target_guid_;
}
inline void social_t::set_target_guid(::google::protobuf::uint64 value) {
  set_changed();
  set_has_target_guid();
  target_guid_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.target_guid)
}

// optional string name = 4;
inline bool social_t::has_name() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void social_t::set_has_name() {
  _has_bits_[0] |= 0x00000001u;
}
inline void social_t::clear_has_name() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void social_t::clear_name() {
  set_changed();
  name_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  clear_has_name();
}
inline const ::std::string& social_t::name() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.name)
  return name_.GetNoArena();
}
inline void social_t::set_name(const ::std::string& value) {
  set_changed();
  set_has_name();
  name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:dhc.social_t.name)
}
#if LANG_CXX11
inline void social_t::set_name(::std::string&& value) {
  set_changed();
  set_has_name();
  name_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:dhc.social_t.name)
}
#endif
inline void social_t::set_name(const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  set_has_name();
  name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:dhc.social_t.name)
}
inline void social_t::set_name(const char* value, size_t size) {
  set_changed();
  set_has_name();
  name_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:dhc.social_t.name)
}
inline ::std::string* social_t::mutable_name() {
  set_changed();
  set_has_name();
  // @@protoc_insertion_point(field_mutable:dhc.social_t.name)
  return name_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* social_t::release_name() {
  set_changed();
  // @@protoc_insertion_point(field_release:dhc.social_t.name)
  if (!has_name()) {
    return NULL;
  }
  clear_has_name();
  return name_.ReleaseNonDefaultNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void social_t::set_allocated_name(::std::string* name) {
  set_changed();
  if (name != NULL) {
    set_has_name();
  } else {
    clear_has_name();
  }
  name_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), name);
  // @@protoc_insertion_point(field_set_allocated:dhc.social_t.name)
}

// optional int32 cup = 5;
inline bool social_t::has_cup() const {
  return (_has_bits_[0] & 0x00000020u) != 0;
}
inline void social_t::set_has_cup() {
  _has_bits_[0] |= 0x00000020u;
}
inline void social_t::clear_has_cup() {
  _has_bits_[0] &= ~0x00000020u;
}
inline void social_t::clear_cup() {
  set_changed();
  cup_ = 0;
  clear_has_cup();
}
inline ::google::protobuf::int32 social_t::cup() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.cup)
  return cup_;
}
inline void social_t::set_cup(::google::protobuf::int32 value) {
  set_changed();
  set_has_cup();
  cup_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.cup)
}

// optional int32 avatar = 7;
inline bool social_t::has_avatar() const {
  return (_has_bits_[0] & 0x00000040u) != 0;
}
inline void social_t::set_has_avatar() {
  _has_bits_[0] |= 0x00000040u;
}
inline void social_t::clear_has_avatar() {
  _has_bits_[0] &= ~0x00000040u;
}
inline void social_t::clear_avatar() {
  set_changed();
  avatar_ = 0;
  clear_has_avatar();
}
inline ::google::protobuf::int32 social_t::avatar() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.avatar)
  return avatar_;
}
inline void social_t::set_avatar(::google::protobuf::int32 value) {
  set_changed();
  set_has_avatar();
  avatar_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.avatar)
}

// optional int32 toukuang = 8;
inline bool social_t::has_toukuang() const {
  return (_has_bits_[0] & 0x00000080u) != 0;
}
inline void social_t::set_has_toukuang() {
  _has_bits_[0] |= 0x00000080u;
}
inline void social_t::clear_has_toukuang() {
  _has_bits_[0] &= ~0x00000080u;
}
inline void social_t::clear_toukuang() {
  set_changed();
  toukuang_ = 0;
  clear_has_toukuang();
}
inline ::google::protobuf::int32 social_t::toukuang() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.toukuang)
  return toukuang_;
}
inline void social_t::set_toukuang(::google::protobuf::int32 value) {
  set_changed();
  set_has_toukuang();
  toukuang_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.toukuang)
}

// optional int32 region_id = 9;
inline bool social_t::has_region_id() const {
  return (_has_bits_[0] & 0x00000100u) != 0;
}
inline void social_t::set_has_region_id() {
  _has_bits_[0] |= 0x00000100u;
}
inline void social_t::clear_has_region_id() {
  _has_bits_[0] &= ~0x00000100u;
}
inline void social_t::clear_region_id() {
  set_changed();
  region_id_ = 0;
  clear_has_region_id();
}
inline ::google::protobuf::int32 social_t::region_id() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.region_id)
  return region_id_;
}
inline void social_t::set_region_id(::google::protobuf::int32 value) {
  set_changed();
  set_has_region_id();
  region_id_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.region_id)
}

// optional int32 level = 10;
inline bool social_t::has_level() const {
  return (_has_bits_[0] & 0x00000200u) != 0;
}
inline void social_t::set_has_level() {
  _has_bits_[0] |= 0x00000200u;
}
inline void social_t::clear_has_level() {
  _has_bits_[0] &= ~0x00000200u;
}
inline void social_t::clear_level() {
  set_changed();
  level_ = 0;
  clear_has_level();
}
inline ::google::protobuf::int32 social_t::level() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.level)
  return level_;
}
inline void social_t::set_level(::google::protobuf::int32 value) {
  set_changed();
  set_has_level();
  level_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.level)
}

// optional int32 sex = 11;
inline bool social_t::has_sex() const {
  return (_has_bits_[0] & 0x00000400u) != 0;
}
inline void social_t::set_has_sex() {
  _has_bits_[0] |= 0x00000400u;
}
inline void social_t::clear_has_sex() {
  _has_bits_[0] &= ~0x00000400u;
}
inline void social_t::clear_sex() {
  set_changed();
  sex_ = 0;
  clear_has_sex();
}
inline ::google::protobuf::int32 social_t::sex() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.sex)
  return sex_;
}
inline void social_t::set_sex(::google::protobuf::int32 value) {
  set_changed();
  set_has_sex();
  sex_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.sex)
}

// optional int32 stype = 12;
inline bool social_t::has_stype() const {
  return (_has_bits_[0] & 0x00000800u) != 0;
}
inline void social_t::set_has_stype() {
  _has_bits_[0] |= 0x00000800u;
}
inline void social_t::clear_has_stype() {
  _has_bits_[0] &= ~0x00000800u;
}
inline void social_t::clear_stype() {
  set_changed();
  stype_ = 0;
  clear_has_stype();
}
inline ::google::protobuf::int32 social_t::stype() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.stype)
  return stype_;
}
inline void social_t::set_stype(::google::protobuf::int32 value) {
  set_changed();
  set_has_stype();
  stype_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.stype)
}

// optional int32 sflag = 13;
inline bool social_t::has_sflag() const {
  return (_has_bits_[0] & 0x00001000u) != 0;
}
inline void social_t::set_has_sflag() {
  _has_bits_[0] |= 0x00001000u;
}
inline void social_t::clear_has_sflag() {
  _has_bits_[0] &= ~0x00001000u;
}
inline void social_t::clear_sflag() {
  set_changed();
  sflag_ = 0;
  clear_has_sflag();
}
inline ::google::protobuf::int32 social_t::sflag() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.sflag)
  return sflag_;
}
inline void social_t::set_sflag(::google::protobuf::int32 value) {
  set_changed();
  set_has_sflag();
  sflag_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.sflag)
}

// optional string verify = 14;
inline bool social_t::has_verify() const {
  return (_has_bits_[0] & 0x00000002u) != 0;
}
inline void social_t::set_has_verify() {
  _has_bits_[0] |= 0x00000002u;
}
inline void social_t::clear_has_verify() {
  _has_bits_[0] &= ~0x00000002u;
}
inline void social_t::clear_verify() {
  set_changed();
  verify_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  clear_has_verify();
}
inline const ::std::string& social_t::verify() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.verify)
  return verify_.GetNoArena();
}
inline void social_t::set_verify(const ::std::string& value) {
  set_changed();
  set_has_verify();
  verify_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:dhc.social_t.verify)
}
#if LANG_CXX11
inline void social_t::set_verify(::std::string&& value) {
  set_changed();
  set_has_verify();
  verify_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:dhc.social_t.verify)
}
#endif
inline void social_t::set_verify(const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  set_has_verify();
  verify_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:dhc.social_t.verify)
}
inline void social_t::set_verify(const char* value, size_t size) {
  set_changed();
  set_has_verify();
  verify_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:dhc.social_t.verify)
}
inline ::std::string* social_t::mutable_verify() {
  set_changed();
  set_has_verify();
  // @@protoc_insertion_point(field_mutable:dhc.social_t.verify)
  return verify_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* social_t::release_verify() {
  set_changed();
  // @@protoc_insertion_point(field_release:dhc.social_t.verify)
  if (!has_verify()) {
    return NULL;
  }
  clear_has_verify();
  return verify_.ReleaseNonDefaultNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void social_t::set_allocated_verify(::std::string* verify) {
  set_changed();
  if (verify != NULL) {
    set_has_verify();
  } else {
    clear_has_verify();
  }
  verify_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), verify);
  // @@protoc_insertion_point(field_set_allocated:dhc.social_t.verify)
}

// optional int32 gold = 15;
inline bool social_t::has_gold() const {
  return (_has_bits_[0] & 0x00002000u) != 0;
}
inline void social_t::set_has_gold() {
  _has_bits_[0] |= 0x00002000u;
}
inline void social_t::clear_has_gold() {
  _has_bits_[0] &= ~0x00002000u;
}
inline void social_t::clear_gold() {
  set_changed();
  gold_ = 0;
  clear_has_gold();
}
inline ::google::protobuf::int32 social_t::gold() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.gold)
  return gold_;
}
inline void social_t::set_gold(::google::protobuf::int32 value) {
  set_changed();
  set_has_gold();
  gold_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.gold)
}

// repeated string msgs = 16;
inline int social_t::msgs_size() const {
  return msgs_.size();
}
inline void social_t::clear_msgs() {
  set_changed();
  msgs_.Clear();
}
inline const ::std::string& social_t::msgs(int index) const {
  // @@protoc_insertion_point(field_get:dhc.social_t.msgs)
  return msgs_.Get(index);
}
inline ::std::string* social_t::mutable_msgs(int index) {
  set_changed();
  // @@protoc_insertion_point(field_mutable:dhc.social_t.msgs)
  return msgs_.Mutable(index);
}
inline void social_t::set_msgs(int index, const ::std::string& value) {
  set_changed();
  // @@protoc_insertion_point(field_set:dhc.social_t.msgs)
  msgs_.Mutable(index)->assign(value);
}
#if LANG_CXX11
inline void social_t::set_msgs(int index, ::std::string&& value) {
  set_changed();
  // @@protoc_insertion_point(field_set:dhc.social_t.msgs)
  msgs_.Mutable(index)->assign(std::move(value));
}
#endif
inline void social_t::set_msgs(int index, const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  msgs_.Mutable(index)->assign(value);
  // @@protoc_insertion_point(field_set_char:dhc.social_t.msgs)
}
inline void social_t::set_msgs(int index, const char* value, size_t size) {
  set_changed();
  msgs_.Mutable(index)->assign(
    reinterpret_cast<const char*>(value), size);
  // @@protoc_insertion_point(field_set_pointer:dhc.social_t.msgs)
}
inline ::std::string* social_t::add_msgs() {
  set_changed();
  // @@protoc_insertion_point(field_add_mutable:dhc.social_t.msgs)
  return msgs_.Add();
}
inline void social_t::add_msgs(const ::std::string& value) {
  set_changed();
  msgs_.Add()->assign(value);
  // @@protoc_insertion_point(field_add:dhc.social_t.msgs)
}
#if LANG_CXX11
inline void social_t::add_msgs(::std::string&& value) {
  set_changed();
  msgs_.Add(std::move(value));
  // @@protoc_insertion_point(field_add:dhc.social_t.msgs)
}
#endif
inline void social_t::add_msgs(const char* value) {
  set_changed();
  GOOGLE_DCHECK(value != NULL);
  msgs_.Add()->assign(value);
  // @@protoc_insertion_point(field_add_char:dhc.social_t.msgs)
}
inline void social_t::add_msgs(const char* value, size_t size) {
  set_changed();
  msgs_.Add()->assign(reinterpret_cast<const char*>(value), size);
  // @@protoc_insertion_point(field_add_pointer:dhc.social_t.msgs)
}
inline const ::google::protobuf::RepeatedPtrField< ::std::string>&
social_t::msgs() const {
  // @@protoc_insertion_point(field_list:dhc.social_t.msgs)
  return msgs_;
}
inline ::google::protobuf::RepeatedPtrField< ::std::string>*
social_t::mutable_msgs() {
  set_changed();
  // @@protoc_insertion_point(field_mutable_list:dhc.social_t.msgs)
  return &msgs_;
}

// repeated uint64 msgtimes = 17;
inline int social_t::msgtimes_size() const {
  return msgtimes_.size();
}
inline void social_t::clear_msgtimes() {
  set_changed();
  msgtimes_.Clear();
}
inline ::google::protobuf::uint64 social_t::msgtimes(int index) const {
  // @@protoc_insertion_point(field_get:dhc.social_t.msgtimes)
  return msgtimes_.Get(index);
}
inline void social_t::set_msgtimes(int index, ::google::protobuf::uint64 value) {
  set_changed();
  msgtimes_.Set(index, value);
  // @@protoc_insertion_point(field_set:dhc.social_t.msgtimes)
}
inline void social_t::add_msgtimes(::google::protobuf::uint64 value) {
  set_changed();
  msgtimes_.Add(value);
  // @@protoc_insertion_point(field_add:dhc.social_t.msgtimes)
}
inline const ::google::protobuf::RepeatedField< ::google::protobuf::uint64 >&
social_t::msgtimes() const {
  // @@protoc_insertion_point(field_list:dhc.social_t.msgtimes)
  return msgtimes_;
}
inline ::google::protobuf::RepeatedField< ::google::protobuf::uint64 >*
social_t::mutable_msgtimes() {
  set_changed();
  // @@protoc_insertion_point(field_mutable_list:dhc.social_t.msgtimes)
  return &msgtimes_;
}

// optional uint64 ttime = 18;
inline bool social_t::has_ttime() const {
  return (_has_bits_[0] & 0x00008000u) != 0;
}
inline void social_t::set_has_ttime() {
  _has_bits_[0] |= 0x00008000u;
}
inline void social_t::clear_has_ttime() {
  _has_bits_[0] &= ~0x00008000u;
}
inline void social_t::clear_ttime() {
  set_changed();
  ttime_ = GOOGLE_ULONGLONG(0);
  clear_has_ttime();
}
inline ::google::protobuf::uint64 social_t::ttime() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.ttime)
  return ttime_;
}
inline void social_t::set_ttime(::google::protobuf::uint64 value) {
  set_changed();
  set_has_ttime();
  ttime_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.ttime)
}

// optional int32 name_color = 19;
inline bool social_t::has_name_color() const {
  return (_has_bits_[0] & 0x00004000u) != 0;
}
inline void social_t::set_has_name_color() {
  _has_bits_[0] |= 0x00004000u;
}
inline void social_t::clear_has_name_color() {
  _has_bits_[0] &= ~0x00004000u;
}
inline void social_t::clear_name_color() {
  set_changed();
  name_color_ = 0;
  clear_has_name_color();
}
inline ::google::protobuf::int32 social_t::name_color() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.name_color)
  return name_color_;
}
inline void social_t::set_name_color(::google::protobuf::int32 value) {
  set_changed();
  set_has_name_color();
  name_color_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.name_color)
}

// optional int32 achieve_point = 21;
inline bool social_t::has_achieve_point() const {
  return (_has_bits_[0] & 0x00010000u) != 0;
}
inline void social_t::set_has_achieve_point() {
  _has_bits_[0] |= 0x00010000u;
}
inline void social_t::clear_has_achieve_point() {
  _has_bits_[0] &= ~0x00010000u;
}
inline void social_t::clear_achieve_point() {
  set_changed();
  achieve_point_ = 0;
  clear_has_achieve_point();
}
inline ::google::protobuf::int32 social_t::achieve_point() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.achieve_point)
  return achieve_point_;
}
inline void social_t::set_achieve_point(::google::protobuf::int32 value) {
  set_changed();
  set_has_achieve_point();
  achieve_point_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.achieve_point)
}

// optional int32 max_score = 22;
inline bool social_t::has_max_score() const {
  return (_has_bits_[0] & 0x00020000u) != 0;
}
inline void social_t::set_has_max_score() {
  _has_bits_[0] |= 0x00020000u;
}
inline void social_t::clear_has_max_score() {
  _has_bits_[0] &= ~0x00020000u;
}
inline void social_t::clear_max_score() {
  set_changed();
  max_score_ = 0;
  clear_has_max_score();
}
inline ::google::protobuf::int32 social_t::max_score() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.max_score)
  return max_score_;
}
inline void social_t::set_max_score(::google::protobuf::int32 value) {
  set_changed();
  set_has_max_score();
  max_score_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.max_score)
}

// optional int32 max_sha = 23;
inline bool social_t::has_max_sha() const {
  return (_has_bits_[0] & 0x00040000u) != 0;
}
inline void social_t::set_has_max_sha() {
  _has_bits_[0] |= 0x00040000u;
}
inline void social_t::clear_has_max_sha() {
  _has_bits_[0] &= ~0x00040000u;
}
inline void social_t::clear_max_sha() {
  set_changed();
  max_sha_ = 0;
  clear_has_max_sha();
}
inline ::google::protobuf::int32 social_t::max_sha() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.max_sha)
  return max_sha_;
}
inline void social_t::set_max_sha(::google::protobuf::int32 value) {
  set_changed();
  set_has_max_sha();
  max_sha_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.max_sha)
}

// optional int32 max_lsha = 24;
inline bool social_t::has_max_lsha() const {
  return (_has_bits_[0] & 0x00080000u) != 0;
}
inline void social_t::set_has_max_lsha() {
  _has_bits_[0] |= 0x00080000u;
}
inline void social_t::clear_has_max_lsha() {
  _has_bits_[0] &= ~0x00080000u;
}
inline void social_t::clear_max_lsha() {
  set_changed();
  max_lsha_ = 0;
  clear_has_max_lsha();
}
inline ::google::protobuf::int32 social_t::max_lsha() const {
  // @@protoc_insertion_point(field_get:dhc.social_t.max_lsha)
  return max_lsha_;
}
inline void social_t::set_max_lsha(::google::protobuf::int32 value) {
  set_changed();
  set_has_max_lsha();
  max_lsha_ = value;
  // @@protoc_insertion_point(field_set:dhc.social_t.max_lsha)
}

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__

// @@protoc_insertion_point(namespace_scope)

}  // namespace dhc

// @@protoc_insertion_point(global_scope)

#endif  // PROTOBUF_INCLUDED_social_2eproto