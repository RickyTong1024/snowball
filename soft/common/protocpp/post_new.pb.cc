// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: post_new.proto

#include "post_new.pb.h"

#include <algorithm>

#include <google/protobuf/stubs/common.h>
#include <google/protobuf/stubs/port.h>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/wire_format_lite_inl.h>
#include <google/protobuf/descriptor.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/reflection_ops.h>
#include <google/protobuf/wire_format.h>
// This is a temporary google only hack
#ifdef GOOGLE_PROTOBUF_ENFORCE_UNIQUENESS
#include "third_party/protobuf/version.h"
#endif
// @@protoc_insertion_point(includes)

namespace dhc {
class post_new_tDefaultTypeInternal {
 public:
  ::google::protobuf::internal::ExplicitlyConstructed<post_new_t>
      _instance;
} _post_new_t_default_instance_;
}  // namespace dhc
namespace protobuf_post_5fnew_2eproto {
static void InitDefaultspost_new_t() {
  GOOGLE_PROTOBUF_VERIFY_VERSION;

  {
    void* ptr = &::dhc::_post_new_t_default_instance_;
    new (ptr) ::dhc::post_new_t();
    ::google::protobuf::internal::OnShutdownDestroyMessage(ptr);
  }
  ::dhc::post_new_t::InitAsDefaultInstance();
}

::google::protobuf::internal::SCCInfo<0> scc_info_post_new_t =
    {{ATOMIC_VAR_INIT(::google::protobuf::internal::SCCInfoBase::kUninitialized), 0, InitDefaultspost_new_t}, {}};

void InitDefaults() {
  ::google::protobuf::internal::InitSCC(&scc_info_post_new_t.base);
}

::google::protobuf::Metadata file_level_metadata[1];

const ::google::protobuf::uint32 TableStruct::offsets[] GOOGLE_PROTOBUF_ATTRIBUTE_SECTION_VARIABLE(protodesc_cold) = {
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, _has_bits_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, _internal_metadata_),
  ~0u,  // no _extensions_
  ~0u,  // no _oneof_case_
  ~0u,  // no _weak_field_map_
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, pid_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, player_guid_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, send_date_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, title_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, text_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, sender_name_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, type_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, value1_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, value2_),
  GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(::dhc::post_new_t, value3_),
  3,
  4,
  5,
  0,
  1,
  2,
  ~0u,
  ~0u,
  ~0u,
  ~0u,
};
static const ::google::protobuf::internal::MigrationSchema schemas[] GOOGLE_PROTOBUF_ATTRIBUTE_SECTION_VARIABLE(protodesc_cold) = {
  { 0, 15, sizeof(::dhc::post_new_t)},
};

static ::google::protobuf::Message const * const file_default_instances[] = {
  reinterpret_cast<const ::google::protobuf::Message*>(&::dhc::_post_new_t_default_instance_),
};

void protobuf_AssignDescriptors() {
  AddDescriptors();
  AssignDescriptors(
      "post_new.proto", schemas, file_default_instances, TableStruct::offsets,
      file_level_metadata, NULL, NULL);
}

void protobuf_AssignDescriptorsOnce() {
  static ::google::protobuf::internal::once_flag once;
  ::google::protobuf::internal::call_once(once, protobuf_AssignDescriptors);
}

void protobuf_RegisterTypes(const ::std::string&) GOOGLE_PROTOBUF_ATTRIBUTE_COLD;
void protobuf_RegisterTypes(const ::std::string&) {
  protobuf_AssignDescriptorsOnce();
  ::google::protobuf::internal::RegisterAllTypes(file_level_metadata, 1);
}

void AddDescriptorsImpl() {
  InitDefaults();
  static const char descriptor[] GOOGLE_PROTOBUF_ATTRIBUTE_SECTION_VARIABLE(protodesc_cold) = {
      "\n\016post_new.proto\022\003dhc\"\261\001\n\npost_new_t\022\013\n\003"
      "pid\030\001 \001(\004\022\023\n\013player_guid\030\002 \001(\004\022\021\n\tsend_d"
      "ate\030\003 \001(\004\022\r\n\005title\030\004 \001(\t\022\014\n\004text\030\005 \001(\t\022\023"
      "\n\013sender_name\030\006 \001(\t\022\014\n\004type\030\007 \003(\005\022\016\n\006val"
      "ue1\030\010 \003(\005\022\016\n\006value2\030\t \003(\005\022\016\n\006value3\030\n \003("
      "\005"
  };
  ::google::protobuf::DescriptorPool::InternalAddGeneratedFile(
      descriptor, 201);
  ::google::protobuf::MessageFactory::InternalRegisterGeneratedFile(
    "post_new.proto", &protobuf_RegisterTypes);
}

void AddDescriptors() {
  static ::google::protobuf::internal::once_flag once;
  ::google::protobuf::internal::call_once(once, AddDescriptorsImpl);
}
// Force AddDescriptors() to be called at dynamic initialization time.
struct StaticDescriptorInitializer {
  StaticDescriptorInitializer() {
    AddDescriptors();
  }
} static_descriptor_initializer;
}  // namespace protobuf_post_5fnew_2eproto
namespace dhc {

// ===================================================================

void post_new_t::InitAsDefaultInstance() {
}
#if !defined(_MSC_VER) || _MSC_VER >= 1900
const int post_new_t::kPidFieldNumber;
const int post_new_t::kPlayerGuidFieldNumber;
const int post_new_t::kSendDateFieldNumber;
const int post_new_t::kTitleFieldNumber;
const int post_new_t::kTextFieldNumber;
const int post_new_t::kSenderNameFieldNumber;
const int post_new_t::kTypeFieldNumber;
const int post_new_t::kValue1FieldNumber;
const int post_new_t::kValue2FieldNumber;
const int post_new_t::kValue3FieldNumber;
#endif  // !defined(_MSC_VER) || _MSC_VER >= 1900

post_new_t::post_new_t()
  : ::google::protobuf::Message(), _internal_metadata_(NULL) {
  ::google::protobuf::internal::InitSCC(
      &protobuf_post_5fnew_2eproto::scc_info_post_new_t.base);
  SharedCtor();
  // @@protoc_insertion_point(constructor:dhc.post_new_t)
}
post_new_t::post_new_t(const post_new_t& from)
  : ::google::protobuf::Message(),
      _internal_metadata_(NULL),
      _has_bits_(from._has_bits_),
      type_(from.type_),
      value1_(from.value1_),
      value2_(from.value2_),
      value3_(from.value3_) {
  _internal_metadata_.MergeFrom(from._internal_metadata_);
  title_.UnsafeSetDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  if (from.has_title()) {
    title_.AssignWithDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), from.title_);
  }
  text_.UnsafeSetDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  if (from.has_text()) {
    text_.AssignWithDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), from.text_);
  }
  sender_name_.UnsafeSetDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  if (from.has_sender_name()) {
    sender_name_.AssignWithDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), from.sender_name_);
  }
  ::memcpy(&pid_, &from.pid_,
    static_cast<size_t>(reinterpret_cast<char*>(&send_date_) -
    reinterpret_cast<char*>(&pid_)) + sizeof(send_date_));
  // @@protoc_insertion_point(copy_constructor:dhc.post_new_t)
}

void post_new_t::SharedCtor() {
  title_.UnsafeSetDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  text_.UnsafeSetDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  sender_name_.UnsafeSetDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  ::memset(&pid_, 0, static_cast<size_t>(
      reinterpret_cast<char*>(&send_date_) -
      reinterpret_cast<char*>(&pid_)) + sizeof(send_date_));
}

post_new_t::~post_new_t() {
  // @@protoc_insertion_point(destructor:dhc.post_new_t)
  SharedDtor();
}

void post_new_t::SharedDtor() {
  title_.DestroyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  text_.DestroyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  sender_name_.DestroyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}

void post_new_t::SetCachedSize(int size) const {
  _cached_size_.Set(size);
}
const ::google::protobuf::Descriptor* post_new_t::descriptor() {
  ::protobuf_post_5fnew_2eproto::protobuf_AssignDescriptorsOnce();
  return ::protobuf_post_5fnew_2eproto::file_level_metadata[kIndexInFileMessages].descriptor;
}

const post_new_t& post_new_t::default_instance() {
  ::google::protobuf::internal::InitSCC(&protobuf_post_5fnew_2eproto::scc_info_post_new_t.base);
  return *internal_default_instance();
}


void post_new_t::Clear() {
// @@protoc_insertion_point(message_clear_start:dhc.post_new_t)
  ::google::protobuf::uint32 cached_has_bits = 0;
  // Prevent compiler warnings about cached_has_bits being unused
  (void) cached_has_bits;

  set_changed();
  type_.Clear();
  set_changed();
  value1_.Clear();
  set_changed();
  value2_.Clear();
  set_changed();
  value3_.Clear();
  cached_has_bits = _has_bits_[0];
  if (cached_has_bits & 7u) {
    if (cached_has_bits & 0x00000001u) {
      set_changed();
      title_.ClearNonDefaultToEmptyNoArena();
    }
    if (cached_has_bits & 0x00000002u) {
      set_changed();
      text_.ClearNonDefaultToEmptyNoArena();
    }
    if (cached_has_bits & 0x00000004u) {
      set_changed();
      sender_name_.ClearNonDefaultToEmptyNoArena();
    }
  }
  if (cached_has_bits & 56u) {
    ::memset(&pid_, 0, static_cast<size_t>(
        reinterpret_cast<char*>(&send_date_) -
        reinterpret_cast<char*>(&pid_)) + sizeof(send_date_));
  }
  _has_bits_.Clear();
  _internal_metadata_.Clear();
}

bool post_new_t::MergePartialFromCodedStream(
    ::google::protobuf::io::CodedInputStream* input) {
#define DO_(EXPRESSION) if (!GOOGLE_PREDICT_TRUE(EXPRESSION)) goto failure
  ::google::protobuf::uint32 tag;
  // @@protoc_insertion_point(parse_start:dhc.post_new_t)
  for (;;) {
    ::std::pair<::google::protobuf::uint32, bool> p = input->ReadTagWithCutoffNoLastTag(127u);
    tag = p.first;
    if (!p.second) goto handle_unusual;
    switch (::google::protobuf::internal::WireFormatLite::GetTagFieldNumber(tag)) {
      // optional uint64 pid = 1;
      case 1: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(8u /* 8 & 0xFF */)) {
          set_has_pid();
          DO_((::google::protobuf::internal::WireFormatLite::ReadPrimitive<
                   ::google::protobuf::uint64, ::google::protobuf::internal::WireFormatLite::TYPE_UINT64>(
                 input, &pid_)));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // optional uint64 player_guid = 2;
      case 2: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(16u /* 16 & 0xFF */)) {
          set_has_player_guid();
          DO_((::google::protobuf::internal::WireFormatLite::ReadPrimitive<
                   ::google::protobuf::uint64, ::google::protobuf::internal::WireFormatLite::TYPE_UINT64>(
                 input, &player_guid_)));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // optional uint64 send_date = 3;
      case 3: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(24u /* 24 & 0xFF */)) {
          set_has_send_date();
          DO_((::google::protobuf::internal::WireFormatLite::ReadPrimitive<
                   ::google::protobuf::uint64, ::google::protobuf::internal::WireFormatLite::TYPE_UINT64>(
                 input, &send_date_)));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // optional string title = 4;
      case 4: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(34u /* 34 & 0xFF */)) {
          DO_(::google::protobuf::internal::WireFormatLite::ReadString(
                input, this->mutable_title()));
          ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
            this->title().data(), static_cast<int>(this->title().length()),
            ::google::protobuf::internal::WireFormat::PARSE,
            "dhc.post_new_t.title");
        } else {
          goto handle_unusual;
        }
        break;
      }

      // optional string text = 5;
      case 5: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(42u /* 42 & 0xFF */)) {
          DO_(::google::protobuf::internal::WireFormatLite::ReadString(
                input, this->mutable_text()));
          ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
            this->text().data(), static_cast<int>(this->text().length()),
            ::google::protobuf::internal::WireFormat::PARSE,
            "dhc.post_new_t.text");
        } else {
          goto handle_unusual;
        }
        break;
      }

      // optional string sender_name = 6;
      case 6: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(50u /* 50 & 0xFF */)) {
          DO_(::google::protobuf::internal::WireFormatLite::ReadString(
                input, this->mutable_sender_name()));
          ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
            this->sender_name().data(), static_cast<int>(this->sender_name().length()),
            ::google::protobuf::internal::WireFormat::PARSE,
            "dhc.post_new_t.sender_name");
        } else {
          goto handle_unusual;
        }
        break;
      }

      // repeated int32 type = 7;
      case 7: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(56u /* 56 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadRepeatedPrimitive<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 1, 56u, input, this->mutable_type())));
        } else if (
            static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(58u /* 58 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadPackedPrimitiveNoInline<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 input, this->mutable_type())));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // repeated int32 value1 = 8;
      case 8: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(64u /* 64 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadRepeatedPrimitive<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 1, 64u, input, this->mutable_value1())));
        } else if (
            static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(66u /* 66 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadPackedPrimitiveNoInline<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 input, this->mutable_value1())));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // repeated int32 value2 = 9;
      case 9: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(72u /* 72 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadRepeatedPrimitive<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 1, 72u, input, this->mutable_value2())));
        } else if (
            static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(74u /* 74 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadPackedPrimitiveNoInline<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 input, this->mutable_value2())));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // repeated int32 value3 = 10;
      case 10: {
        if (static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(80u /* 80 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadRepeatedPrimitive<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 1, 80u, input, this->mutable_value3())));
        } else if (
            static_cast< ::google::protobuf::uint8>(tag) ==
            static_cast< ::google::protobuf::uint8>(82u /* 82 & 0xFF */)) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadPackedPrimitiveNoInline<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 input, this->mutable_value3())));
        } else {
          goto handle_unusual;
        }
        break;
      }

      default: {
      handle_unusual:
        if (tag == 0) {
          goto success;
        }
        DO_(::google::protobuf::internal::WireFormat::SkipField(
              input, tag, _internal_metadata_.mutable_unknown_fields()));
        break;
      }
    }
  }
success:
  // @@protoc_insertion_point(parse_success:dhc.post_new_t)
  return true;
failure:
  // @@protoc_insertion_point(parse_failure:dhc.post_new_t)
  return false;
#undef DO_
}

void post_new_t::SerializeWithCachedSizes(
    ::google::protobuf::io::CodedOutputStream* output) const {
  // @@protoc_insertion_point(serialize_start:dhc.post_new_t)
  ::google::protobuf::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  cached_has_bits = _has_bits_[0];
  // optional uint64 pid = 1;
  if (cached_has_bits & 0x00000008u) {
    ::google::protobuf::internal::WireFormatLite::WriteUInt64(1, this->pid(), output);
  }

  // optional uint64 player_guid = 2;
  if (cached_has_bits & 0x00000010u) {
    ::google::protobuf::internal::WireFormatLite::WriteUInt64(2, this->player_guid(), output);
  }

  // optional uint64 send_date = 3;
  if (cached_has_bits & 0x00000020u) {
    ::google::protobuf::internal::WireFormatLite::WriteUInt64(3, this->send_date(), output);
  }

  // optional string title = 4;
  if (cached_has_bits & 0x00000001u) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->title().data(), static_cast<int>(this->title().length()),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "dhc.post_new_t.title");
    ::google::protobuf::internal::WireFormatLite::WriteStringMaybeAliased(
      4, this->title(), output);
  }

  // optional string text = 5;
  if (cached_has_bits & 0x00000002u) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->text().data(), static_cast<int>(this->text().length()),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "dhc.post_new_t.text");
    ::google::protobuf::internal::WireFormatLite::WriteStringMaybeAliased(
      5, this->text(), output);
  }

  // optional string sender_name = 6;
  if (cached_has_bits & 0x00000004u) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->sender_name().data(), static_cast<int>(this->sender_name().length()),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "dhc.post_new_t.sender_name");
    ::google::protobuf::internal::WireFormatLite::WriteStringMaybeAliased(
      6, this->sender_name(), output);
  }

  // repeated int32 type = 7;
  for (int i = 0, n = this->type_size(); i < n; i++) {
    ::google::protobuf::internal::WireFormatLite::WriteInt32(
      7, this->type(i), output);
  }

  // repeated int32 value1 = 8;
  for (int i = 0, n = this->value1_size(); i < n; i++) {
    ::google::protobuf::internal::WireFormatLite::WriteInt32(
      8, this->value1(i), output);
  }

  // repeated int32 value2 = 9;
  for (int i = 0, n = this->value2_size(); i < n; i++) {
    ::google::protobuf::internal::WireFormatLite::WriteInt32(
      9, this->value2(i), output);
  }

  // repeated int32 value3 = 10;
  for (int i = 0, n = this->value3_size(); i < n; i++) {
    ::google::protobuf::internal::WireFormatLite::WriteInt32(
      10, this->value3(i), output);
  }

  if (_internal_metadata_.have_unknown_fields()) {
    ::google::protobuf::internal::WireFormat::SerializeUnknownFields(
        _internal_metadata_.unknown_fields(), output);
  }
  // @@protoc_insertion_point(serialize_end:dhc.post_new_t)
}

::google::protobuf::uint8* post_new_t::InternalSerializeWithCachedSizesToArray(
    bool deterministic, ::google::protobuf::uint8* target) const {
  (void)deterministic; // Unused
  // @@protoc_insertion_point(serialize_to_array_start:dhc.post_new_t)
  ::google::protobuf::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  cached_has_bits = _has_bits_[0];
  // optional uint64 pid = 1;
  if (cached_has_bits & 0x00000008u) {
    target = ::google::protobuf::internal::WireFormatLite::WriteUInt64ToArray(1, this->pid(), target);
  }

  // optional uint64 player_guid = 2;
  if (cached_has_bits & 0x00000010u) {
    target = ::google::protobuf::internal::WireFormatLite::WriteUInt64ToArray(2, this->player_guid(), target);
  }

  // optional uint64 send_date = 3;
  if (cached_has_bits & 0x00000020u) {
    target = ::google::protobuf::internal::WireFormatLite::WriteUInt64ToArray(3, this->send_date(), target);
  }

  // optional string title = 4;
  if (cached_has_bits & 0x00000001u) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->title().data(), static_cast<int>(this->title().length()),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "dhc.post_new_t.title");
    target =
      ::google::protobuf::internal::WireFormatLite::WriteStringToArray(
        4, this->title(), target);
  }

  // optional string text = 5;
  if (cached_has_bits & 0x00000002u) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->text().data(), static_cast<int>(this->text().length()),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "dhc.post_new_t.text");
    target =
      ::google::protobuf::internal::WireFormatLite::WriteStringToArray(
        5, this->text(), target);
  }

  // optional string sender_name = 6;
  if (cached_has_bits & 0x00000004u) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->sender_name().data(), static_cast<int>(this->sender_name().length()),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "dhc.post_new_t.sender_name");
    target =
      ::google::protobuf::internal::WireFormatLite::WriteStringToArray(
        6, this->sender_name(), target);
  }

  // repeated int32 type = 7;
  target = ::google::protobuf::internal::WireFormatLite::
    WriteInt32ToArray(7, this->type_, target);

  // repeated int32 value1 = 8;
  target = ::google::protobuf::internal::WireFormatLite::
    WriteInt32ToArray(8, this->value1_, target);

  // repeated int32 value2 = 9;
  target = ::google::protobuf::internal::WireFormatLite::
    WriteInt32ToArray(9, this->value2_, target);

  // repeated int32 value3 = 10;
  target = ::google::protobuf::internal::WireFormatLite::
    WriteInt32ToArray(10, this->value3_, target);

  if (_internal_metadata_.have_unknown_fields()) {
    target = ::google::protobuf::internal::WireFormat::SerializeUnknownFieldsToArray(
        _internal_metadata_.unknown_fields(), target);
  }
  // @@protoc_insertion_point(serialize_to_array_end:dhc.post_new_t)
  return target;
}

size_t post_new_t::ByteSizeLong() const {
// @@protoc_insertion_point(message_byte_size_start:dhc.post_new_t)
  size_t total_size = 0;

  if (_internal_metadata_.have_unknown_fields()) {
    total_size +=
      ::google::protobuf::internal::WireFormat::ComputeUnknownFieldsSize(
        _internal_metadata_.unknown_fields());
  }
  // repeated int32 type = 7;
  {
    size_t data_size = ::google::protobuf::internal::WireFormatLite::
      Int32Size(this->type_);
    total_size += 1 *
                  ::google::protobuf::internal::FromIntSize(this->type_size());
    total_size += data_size;
  }

  // repeated int32 value1 = 8;
  {
    size_t data_size = ::google::protobuf::internal::WireFormatLite::
      Int32Size(this->value1_);
    total_size += 1 *
                  ::google::protobuf::internal::FromIntSize(this->value1_size());
    total_size += data_size;
  }

  // repeated int32 value2 = 9;
  {
    size_t data_size = ::google::protobuf::internal::WireFormatLite::
      Int32Size(this->value2_);
    total_size += 1 *
                  ::google::protobuf::internal::FromIntSize(this->value2_size());
    total_size += data_size;
  }

  // repeated int32 value3 = 10;
  {
    size_t data_size = ::google::protobuf::internal::WireFormatLite::
      Int32Size(this->value3_);
    total_size += 1 *
                  ::google::protobuf::internal::FromIntSize(this->value3_size());
    total_size += data_size;
  }

  if (_has_bits_[0 / 32] & 63u) {
    // optional string title = 4;
    if (has_title()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::StringSize(
          this->title());
    }

    // optional string text = 5;
    if (has_text()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::StringSize(
          this->text());
    }

    // optional string sender_name = 6;
    if (has_sender_name()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::StringSize(
          this->sender_name());
    }

    // optional uint64 pid = 1;
    if (has_pid()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::UInt64Size(
          this->pid());
    }

    // optional uint64 player_guid = 2;
    if (has_player_guid()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::UInt64Size(
          this->player_guid());
    }

    // optional uint64 send_date = 3;
    if (has_send_date()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::UInt64Size(
          this->send_date());
    }

  }
  int cached_size = ::google::protobuf::internal::ToCachedSize(total_size);
  SetCachedSize(cached_size);
  return total_size;
}

void post_new_t::MergeFrom(const ::google::protobuf::Message& from) {
// @@protoc_insertion_point(generalized_merge_from_start:dhc.post_new_t)
  GOOGLE_DCHECK_NE(&from, this);
  const post_new_t* source =
      ::google::protobuf::internal::DynamicCastToGenerated<const post_new_t>(
          &from);
  if (source == NULL) {
  // @@protoc_insertion_point(generalized_merge_from_cast_fail:dhc.post_new_t)
    ::google::protobuf::internal::ReflectionOps::Merge(from, this);
  } else {
  // @@protoc_insertion_point(generalized_merge_from_cast_success:dhc.post_new_t)
    MergeFrom(*source);
  }
}

void post_new_t::MergeFrom(const post_new_t& from) {
// @@protoc_insertion_point(class_specific_merge_from_start:dhc.post_new_t)
  GOOGLE_DCHECK_NE(&from, this);
  _internal_metadata_.MergeFrom(from._internal_metadata_);
  ::google::protobuf::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  type_.MergeFrom(from.type_);
  value1_.MergeFrom(from.value1_);
  value2_.MergeFrom(from.value2_);
  value3_.MergeFrom(from.value3_);
  cached_has_bits = from._has_bits_[0];
  if (cached_has_bits & 63u) {
    if (cached_has_bits & 0x00000001u) {
      set_has_title();
      title_.AssignWithDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), from.title_);
    }
    if (cached_has_bits & 0x00000002u) {
      set_has_text();
      text_.AssignWithDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), from.text_);
    }
    if (cached_has_bits & 0x00000004u) {
      set_has_sender_name();
      sender_name_.AssignWithDefault(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), from.sender_name_);
    }
    if (cached_has_bits & 0x00000008u) {
      pid_ = from.pid_;
    }
    if (cached_has_bits & 0x00000010u) {
      player_guid_ = from.player_guid_;
    }
    if (cached_has_bits & 0x00000020u) {
      send_date_ = from.send_date_;
    }
    _has_bits_[0] |= cached_has_bits;
  }
}

void post_new_t::CopyFrom(const ::google::protobuf::Message& from) {
// @@protoc_insertion_point(generalized_copy_from_start:dhc.post_new_t)
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

void post_new_t::CopyFrom(const post_new_t& from) {
// @@protoc_insertion_point(class_specific_copy_from_start:dhc.post_new_t)
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

bool post_new_t::IsInitialized() const {
  return true;
}

void post_new_t::Swap(post_new_t* other) {
  if (other == this) return;
  InternalSwap(other);
}
void post_new_t::InternalSwap(post_new_t* other) {
  using std::swap;
  type_.InternalSwap(&other->type_);
  value1_.InternalSwap(&other->value1_);
  value2_.InternalSwap(&other->value2_);
  value3_.InternalSwap(&other->value3_);
  title_.Swap(&other->title_, &::google::protobuf::internal::GetEmptyStringAlreadyInited(),
    GetArenaNoVirtual());
  text_.Swap(&other->text_, &::google::protobuf::internal::GetEmptyStringAlreadyInited(),
    GetArenaNoVirtual());
  sender_name_.Swap(&other->sender_name_, &::google::protobuf::internal::GetEmptyStringAlreadyInited(),
    GetArenaNoVirtual());
  swap(pid_, other->pid_);
  swap(player_guid_, other->player_guid_);
  swap(send_date_, other->send_date_);
  swap(_has_bits_[0], other->_has_bits_[0]);
  _internal_metadata_.Swap(&other->_internal_metadata_);
}

::google::protobuf::Metadata post_new_t::GetMetadata() const {
  protobuf_post_5fnew_2eproto::protobuf_AssignDescriptorsOnce();
  return ::protobuf_post_5fnew_2eproto::file_level_metadata[kIndexInFileMessages];
}


// @@protoc_insertion_point(namespace_scope)
}  // namespace dhc
namespace google {
namespace protobuf {
template<> GOOGLE_PROTOBUF_ATTRIBUTE_NOINLINE ::dhc::post_new_t* Arena::CreateMaybeMessage< ::dhc::post_new_t >(Arena* arena) {
  return Arena::CreateInternal< ::dhc::post_new_t >(arena);
}
}  // namespace protobuf
}  // namespace google

// @@protoc_insertion_point(global_scope)
