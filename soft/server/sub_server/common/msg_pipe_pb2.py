# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: msg_pipe.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='msg_pipe.proto',
  package='protocol.pipe',
  syntax='proto2',
  serialized_options=None,
  serialized_pb=_b('\n\x0emsg_pipe.proto\x12\rprotocol.pipe\"8\n\x0epmsg_req_login\x12\x0b\n\x03uid\x18\x01 \x02(\t\x12\r\n\x05token\x18\x02 \x02(\t\x12\n\n\x02pt\x18\x03 \x02(\t\"0\n\x0epmsg_rep_login\x12\x0e\n\x06\x65rrres\x18\x01 \x02(\x05\x12\x0e\n\x06\x65rrmsg\x18\x02 \x02(\t\"\x1f\n\x0cpmsg_gonggao\x12\x0f\n\x07gonggao\x18\x01 \x02(\t\"+\n\x0epmsg_req_libao\x12\x0c\n\x04\x63ode\x18\x01 \x02(\t\x12\x0b\n\x03use\x18\x02 \x02(\t\"t\n\x0epmsg_rep_libao\x12\x0b\n\x03res\x18\x01 \x02(\x05\x12\n\n\x02pc\x18\x02 \x02(\x05\x12\n\n\x02\x63\x66\x18\x03 \x02(\x05\x12\r\n\x05types\x18\x04 \x03(\x05\x12\x0e\n\x06value1\x18\x05 \x03(\x05\x12\x0e\n\x06value2\x18\x06 \x03(\x05\x12\x0e\n\x06value3\x18\x07 \x03(\x05\"\xa4\x01\n\rpmsg_mail_one\x12\x11\n\tacc_names\x18\x01 \x03(\t\x12\x10\n\x08serverid\x18\x02 \x02(\x05\x12\r\n\x05title\x18\x04 \x02(\t\x12\x0c\n\x04text\x18\x05 \x02(\t\x12\x13\n\x0bsender_name\x18\x06 \x02(\t\x12\x0c\n\x04type\x18\x07 \x03(\x05\x12\x0e\n\x06value1\x18\x08 \x03(\x05\x12\x0e\n\x06value2\x18\t \x03(\x05\x12\x0e\n\x06value3\x18\n \x03(\x05\"\xa0\x01\n\rpmsg_mail_all\x12\r\n\x05level\x18\x01 \x02(\x05\x12\x10\n\x08serverid\x18\x02 \x02(\x05\x12\r\n\x05title\x18\x04 \x02(\t\x12\x0c\n\x04text\x18\x05 \x02(\t\x12\x13\n\x0bsender_name\x18\x06 \x02(\t\x12\x0c\n\x04type\x18\x07 \x03(\x05\x12\x0e\n\x06value1\x18\x08 \x03(\x05\x12\x0e\n\x06value2\x18\t \x03(\x05\x12\x0e\n\x06value3\x18\n \x03(\x05\"-\n\x11pmsg_req_recharge\x12\n\n\x02pt\x18\x01 \x02(\t\x12\x0c\n\x04\x63ode\x18\x02 \x03(\t\"O\n\x11pmsg_recharge_ali\x12\x0c\n\x04guid\x18\x01 \x02(\x04\x12\x0b\n\x03rid\x18\x02 \x02(\x05\x12\x0f\n\x07orderno\x18\x03 \x02(\t\x12\x0e\n\x06\x61mount\x18\x04 \x02(\x02\"E\n\x11pmsg_rep_recharge\x12\x0b\n\x03res\x18\x01 \x02(\x05\x12\x12\n\nproduct_id\x18\x02 \x02(\t\x12\x0f\n\x07orderid\x18\x03 \x02(\t\"L\n\x18pmsg_recharge_simulation\x12\x11\n\tacc_names\x18\x01 \x02(\t\x12\x10\n\x08serverid\x18\x02 \x02(\x05\x12\x0b\n\x03rid\x18\x03 \x02(\x05\"6\n\x19pmsg_recharge_simulation1\x12\x0c\n\x04guid\x18\x01 \x02(\x04\x12\x0b\n\x03rid\x18\x02 \x02(\x05\":\n\x13pmsg_rank_forbidden\x12\x11\n\tacc_names\x18\x01 \x02(\t\x12\x10\n\x08serverid\x18\x02 \x02(\x05\"$\n\x14pmsg_rank_forbidden1\x12\x0c\n\x04guid\x18\x01 \x02(\x04')
)




_PMSG_REQ_LOGIN = _descriptor.Descriptor(
  name='pmsg_req_login',
  full_name='protocol.pipe.pmsg_req_login',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='uid', full_name='protocol.pipe.pmsg_req_login.uid', index=0,
      number=1, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='token', full_name='protocol.pipe.pmsg_req_login.token', index=1,
      number=2, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='pt', full_name='protocol.pipe.pmsg_req_login.pt', index=2,
      number=3, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=33,
  serialized_end=89,
)


_PMSG_REP_LOGIN = _descriptor.Descriptor(
  name='pmsg_rep_login',
  full_name='protocol.pipe.pmsg_rep_login',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='errres', full_name='protocol.pipe.pmsg_rep_login.errres', index=0,
      number=1, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='errmsg', full_name='protocol.pipe.pmsg_rep_login.errmsg', index=1,
      number=2, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=91,
  serialized_end=139,
)


_PMSG_GONGGAO = _descriptor.Descriptor(
  name='pmsg_gonggao',
  full_name='protocol.pipe.pmsg_gonggao',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='gonggao', full_name='protocol.pipe.pmsg_gonggao.gonggao', index=0,
      number=1, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=141,
  serialized_end=172,
)


_PMSG_REQ_LIBAO = _descriptor.Descriptor(
  name='pmsg_req_libao',
  full_name='protocol.pipe.pmsg_req_libao',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='code', full_name='protocol.pipe.pmsg_req_libao.code', index=0,
      number=1, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='use', full_name='protocol.pipe.pmsg_req_libao.use', index=1,
      number=2, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=174,
  serialized_end=217,
)


_PMSG_REP_LIBAO = _descriptor.Descriptor(
  name='pmsg_rep_libao',
  full_name='protocol.pipe.pmsg_rep_libao',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='res', full_name='protocol.pipe.pmsg_rep_libao.res', index=0,
      number=1, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='pc', full_name='protocol.pipe.pmsg_rep_libao.pc', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='cf', full_name='protocol.pipe.pmsg_rep_libao.cf', index=2,
      number=3, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='types', full_name='protocol.pipe.pmsg_rep_libao.types', index=3,
      number=4, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value1', full_name='protocol.pipe.pmsg_rep_libao.value1', index=4,
      number=5, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value2', full_name='protocol.pipe.pmsg_rep_libao.value2', index=5,
      number=6, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value3', full_name='protocol.pipe.pmsg_rep_libao.value3', index=6,
      number=7, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=219,
  serialized_end=335,
)


_PMSG_MAIL_ONE = _descriptor.Descriptor(
  name='pmsg_mail_one',
  full_name='protocol.pipe.pmsg_mail_one',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='acc_names', full_name='protocol.pipe.pmsg_mail_one.acc_names', index=0,
      number=1, type=9, cpp_type=9, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='serverid', full_name='protocol.pipe.pmsg_mail_one.serverid', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='title', full_name='protocol.pipe.pmsg_mail_one.title', index=2,
      number=4, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='text', full_name='protocol.pipe.pmsg_mail_one.text', index=3,
      number=5, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='sender_name', full_name='protocol.pipe.pmsg_mail_one.sender_name', index=4,
      number=6, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='type', full_name='protocol.pipe.pmsg_mail_one.type', index=5,
      number=7, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value1', full_name='protocol.pipe.pmsg_mail_one.value1', index=6,
      number=8, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value2', full_name='protocol.pipe.pmsg_mail_one.value2', index=7,
      number=9, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value3', full_name='protocol.pipe.pmsg_mail_one.value3', index=8,
      number=10, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=338,
  serialized_end=502,
)


_PMSG_MAIL_ALL = _descriptor.Descriptor(
  name='pmsg_mail_all',
  full_name='protocol.pipe.pmsg_mail_all',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='level', full_name='protocol.pipe.pmsg_mail_all.level', index=0,
      number=1, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='serverid', full_name='protocol.pipe.pmsg_mail_all.serverid', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='title', full_name='protocol.pipe.pmsg_mail_all.title', index=2,
      number=4, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='text', full_name='protocol.pipe.pmsg_mail_all.text', index=3,
      number=5, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='sender_name', full_name='protocol.pipe.pmsg_mail_all.sender_name', index=4,
      number=6, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='type', full_name='protocol.pipe.pmsg_mail_all.type', index=5,
      number=7, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value1', full_name='protocol.pipe.pmsg_mail_all.value1', index=6,
      number=8, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value2', full_name='protocol.pipe.pmsg_mail_all.value2', index=7,
      number=9, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value3', full_name='protocol.pipe.pmsg_mail_all.value3', index=8,
      number=10, type=5, cpp_type=1, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=505,
  serialized_end=665,
)


_PMSG_REQ_RECHARGE = _descriptor.Descriptor(
  name='pmsg_req_recharge',
  full_name='protocol.pipe.pmsg_req_recharge',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='pt', full_name='protocol.pipe.pmsg_req_recharge.pt', index=0,
      number=1, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='code', full_name='protocol.pipe.pmsg_req_recharge.code', index=1,
      number=2, type=9, cpp_type=9, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=667,
  serialized_end=712,
)


_PMSG_RECHARGE_ALI = _descriptor.Descriptor(
  name='pmsg_recharge_ali',
  full_name='protocol.pipe.pmsg_recharge_ali',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='guid', full_name='protocol.pipe.pmsg_recharge_ali.guid', index=0,
      number=1, type=4, cpp_type=4, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='rid', full_name='protocol.pipe.pmsg_recharge_ali.rid', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='orderno', full_name='protocol.pipe.pmsg_recharge_ali.orderno', index=2,
      number=3, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='amount', full_name='protocol.pipe.pmsg_recharge_ali.amount', index=3,
      number=4, type=2, cpp_type=6, label=2,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=714,
  serialized_end=793,
)


_PMSG_REP_RECHARGE = _descriptor.Descriptor(
  name='pmsg_rep_recharge',
  full_name='protocol.pipe.pmsg_rep_recharge',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='res', full_name='protocol.pipe.pmsg_rep_recharge.res', index=0,
      number=1, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='product_id', full_name='protocol.pipe.pmsg_rep_recharge.product_id', index=1,
      number=2, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='orderid', full_name='protocol.pipe.pmsg_rep_recharge.orderid', index=2,
      number=3, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=795,
  serialized_end=864,
)


_PMSG_RECHARGE_SIMULATION = _descriptor.Descriptor(
  name='pmsg_recharge_simulation',
  full_name='protocol.pipe.pmsg_recharge_simulation',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='acc_names', full_name='protocol.pipe.pmsg_recharge_simulation.acc_names', index=0,
      number=1, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='serverid', full_name='protocol.pipe.pmsg_recharge_simulation.serverid', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='rid', full_name='protocol.pipe.pmsg_recharge_simulation.rid', index=2,
      number=3, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=866,
  serialized_end=942,
)


_PMSG_RECHARGE_SIMULATION1 = _descriptor.Descriptor(
  name='pmsg_recharge_simulation1',
  full_name='protocol.pipe.pmsg_recharge_simulation1',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='guid', full_name='protocol.pipe.pmsg_recharge_simulation1.guid', index=0,
      number=1, type=4, cpp_type=4, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='rid', full_name='protocol.pipe.pmsg_recharge_simulation1.rid', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=944,
  serialized_end=998,
)


_PMSG_RANK_FORBIDDEN = _descriptor.Descriptor(
  name='pmsg_rank_forbidden',
  full_name='protocol.pipe.pmsg_rank_forbidden',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='acc_names', full_name='protocol.pipe.pmsg_rank_forbidden.acc_names', index=0,
      number=1, type=9, cpp_type=9, label=2,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='serverid', full_name='protocol.pipe.pmsg_rank_forbidden.serverid', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=1000,
  serialized_end=1058,
)


_PMSG_RANK_FORBIDDEN1 = _descriptor.Descriptor(
  name='pmsg_rank_forbidden1',
  full_name='protocol.pipe.pmsg_rank_forbidden1',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='guid', full_name='protocol.pipe.pmsg_rank_forbidden1.guid', index=0,
      number=1, type=4, cpp_type=4, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=1060,
  serialized_end=1096,
)

DESCRIPTOR.message_types_by_name['pmsg_req_login'] = _PMSG_REQ_LOGIN
DESCRIPTOR.message_types_by_name['pmsg_rep_login'] = _PMSG_REP_LOGIN
DESCRIPTOR.message_types_by_name['pmsg_gonggao'] = _PMSG_GONGGAO
DESCRIPTOR.message_types_by_name['pmsg_req_libao'] = _PMSG_REQ_LIBAO
DESCRIPTOR.message_types_by_name['pmsg_rep_libao'] = _PMSG_REP_LIBAO
DESCRIPTOR.message_types_by_name['pmsg_mail_one'] = _PMSG_MAIL_ONE
DESCRIPTOR.message_types_by_name['pmsg_mail_all'] = _PMSG_MAIL_ALL
DESCRIPTOR.message_types_by_name['pmsg_req_recharge'] = _PMSG_REQ_RECHARGE
DESCRIPTOR.message_types_by_name['pmsg_recharge_ali'] = _PMSG_RECHARGE_ALI
DESCRIPTOR.message_types_by_name['pmsg_rep_recharge'] = _PMSG_REP_RECHARGE
DESCRIPTOR.message_types_by_name['pmsg_recharge_simulation'] = _PMSG_RECHARGE_SIMULATION
DESCRIPTOR.message_types_by_name['pmsg_recharge_simulation1'] = _PMSG_RECHARGE_SIMULATION1
DESCRIPTOR.message_types_by_name['pmsg_rank_forbidden'] = _PMSG_RANK_FORBIDDEN
DESCRIPTOR.message_types_by_name['pmsg_rank_forbidden1'] = _PMSG_RANK_FORBIDDEN1
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

pmsg_req_login = _reflection.GeneratedProtocolMessageType('pmsg_req_login', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_REQ_LOGIN,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_req_login)
  ))
_sym_db.RegisterMessage(pmsg_req_login)

pmsg_rep_login = _reflection.GeneratedProtocolMessageType('pmsg_rep_login', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_REP_LOGIN,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_rep_login)
  ))
_sym_db.RegisterMessage(pmsg_rep_login)

pmsg_gonggao = _reflection.GeneratedProtocolMessageType('pmsg_gonggao', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_GONGGAO,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_gonggao)
  ))
_sym_db.RegisterMessage(pmsg_gonggao)

pmsg_req_libao = _reflection.GeneratedProtocolMessageType('pmsg_req_libao', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_REQ_LIBAO,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_req_libao)
  ))
_sym_db.RegisterMessage(pmsg_req_libao)

pmsg_rep_libao = _reflection.GeneratedProtocolMessageType('pmsg_rep_libao', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_REP_LIBAO,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_rep_libao)
  ))
_sym_db.RegisterMessage(pmsg_rep_libao)

pmsg_mail_one = _reflection.GeneratedProtocolMessageType('pmsg_mail_one', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_MAIL_ONE,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_mail_one)
  ))
_sym_db.RegisterMessage(pmsg_mail_one)

pmsg_mail_all = _reflection.GeneratedProtocolMessageType('pmsg_mail_all', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_MAIL_ALL,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_mail_all)
  ))
_sym_db.RegisterMessage(pmsg_mail_all)

pmsg_req_recharge = _reflection.GeneratedProtocolMessageType('pmsg_req_recharge', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_REQ_RECHARGE,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_req_recharge)
  ))
_sym_db.RegisterMessage(pmsg_req_recharge)

pmsg_recharge_ali = _reflection.GeneratedProtocolMessageType('pmsg_recharge_ali', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_RECHARGE_ALI,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_recharge_ali)
  ))
_sym_db.RegisterMessage(pmsg_recharge_ali)

pmsg_rep_recharge = _reflection.GeneratedProtocolMessageType('pmsg_rep_recharge', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_REP_RECHARGE,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_rep_recharge)
  ))
_sym_db.RegisterMessage(pmsg_rep_recharge)

pmsg_recharge_simulation = _reflection.GeneratedProtocolMessageType('pmsg_recharge_simulation', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_RECHARGE_SIMULATION,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_recharge_simulation)
  ))
_sym_db.RegisterMessage(pmsg_recharge_simulation)

pmsg_recharge_simulation1 = _reflection.GeneratedProtocolMessageType('pmsg_recharge_simulation1', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_RECHARGE_SIMULATION1,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_recharge_simulation1)
  ))
_sym_db.RegisterMessage(pmsg_recharge_simulation1)

pmsg_rank_forbidden = _reflection.GeneratedProtocolMessageType('pmsg_rank_forbidden', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_RANK_FORBIDDEN,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_rank_forbidden)
  ))
_sym_db.RegisterMessage(pmsg_rank_forbidden)

pmsg_rank_forbidden1 = _reflection.GeneratedProtocolMessageType('pmsg_rank_forbidden1', (_message.Message,), dict(
  DESCRIPTOR = _PMSG_RANK_FORBIDDEN1,
  __module__ = 'msg_pipe_pb2'
  # @@protoc_insertion_point(class_scope:protocol.pipe.pmsg_rank_forbidden1)
  ))
_sym_db.RegisterMessage(pmsg_rank_forbidden1)


# @@protoc_insertion_point(module_scope)
