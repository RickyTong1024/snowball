-- Generated By protoc-gen-lua Do not Edit
local protobuf = require "protobuf/protobuf"

local RANK_T = protobuf.Descriptor();
local RANK_T_GUID_FIELD = protobuf.FieldDescriptor();
local RANK_T_PLAYER_GUID_FIELD = protobuf.FieldDescriptor();
local RANK_T_NAME_FIELD = protobuf.FieldDescriptor();
local RANK_T_SEX_FIELD = protobuf.FieldDescriptor();
local RANK_T_LEVEL_FIELD = protobuf.FieldDescriptor();
local RANK_T_AVATAR_FIELD = protobuf.FieldDescriptor();
local RANK_T_TOUKUANG_FIELD = protobuf.FieldDescriptor();
local RANK_T_REGION_ID_FIELD = protobuf.FieldDescriptor();
local RANK_T_NAME_COLOR_FIELD = protobuf.FieldDescriptor();
local RANK_T_VALUE_FIELD = protobuf.FieldDescriptor();

RANK_T_GUID_FIELD.name = "guid"
RANK_T_GUID_FIELD.full_name = ".dhc.rank_t.guid"
RANK_T_GUID_FIELD.number = 1
RANK_T_GUID_FIELD.index = 0
RANK_T_GUID_FIELD.label = 1
RANK_T_GUID_FIELD.has_default_value = false
RANK_T_GUID_FIELD.default_value = 0
RANK_T_GUID_FIELD.type = 4
RANK_T_GUID_FIELD.cpp_type = 4

RANK_T_PLAYER_GUID_FIELD.name = "player_guid"
RANK_T_PLAYER_GUID_FIELD.full_name = ".dhc.rank_t.player_guid"
RANK_T_PLAYER_GUID_FIELD.number = 2
RANK_T_PLAYER_GUID_FIELD.index = 1
RANK_T_PLAYER_GUID_FIELD.label = 3
RANK_T_PLAYER_GUID_FIELD.has_default_value = false
RANK_T_PLAYER_GUID_FIELD.default_value = {}
RANK_T_PLAYER_GUID_FIELD.type = 4
RANK_T_PLAYER_GUID_FIELD.cpp_type = 4

RANK_T_NAME_FIELD.name = "name"
RANK_T_NAME_FIELD.full_name = ".dhc.rank_t.name"
RANK_T_NAME_FIELD.number = 3
RANK_T_NAME_FIELD.index = 2
RANK_T_NAME_FIELD.label = 3
RANK_T_NAME_FIELD.has_default_value = false
RANK_T_NAME_FIELD.default_value = {}
RANK_T_NAME_FIELD.type = 9
RANK_T_NAME_FIELD.cpp_type = 9

RANK_T_SEX_FIELD.name = "sex"
RANK_T_SEX_FIELD.full_name = ".dhc.rank_t.sex"
RANK_T_SEX_FIELD.number = 4
RANK_T_SEX_FIELD.index = 3
RANK_T_SEX_FIELD.label = 3
RANK_T_SEX_FIELD.has_default_value = false
RANK_T_SEX_FIELD.default_value = {}
RANK_T_SEX_FIELD.type = 5
RANK_T_SEX_FIELD.cpp_type = 1

RANK_T_LEVEL_FIELD.name = "level"
RANK_T_LEVEL_FIELD.full_name = ".dhc.rank_t.level"
RANK_T_LEVEL_FIELD.number = 5
RANK_T_LEVEL_FIELD.index = 4
RANK_T_LEVEL_FIELD.label = 3
RANK_T_LEVEL_FIELD.has_default_value = false
RANK_T_LEVEL_FIELD.default_value = {}
RANK_T_LEVEL_FIELD.type = 5
RANK_T_LEVEL_FIELD.cpp_type = 1

RANK_T_AVATAR_FIELD.name = "avatar"
RANK_T_AVATAR_FIELD.full_name = ".dhc.rank_t.avatar"
RANK_T_AVATAR_FIELD.number = 6
RANK_T_AVATAR_FIELD.index = 5
RANK_T_AVATAR_FIELD.label = 3
RANK_T_AVATAR_FIELD.has_default_value = false
RANK_T_AVATAR_FIELD.default_value = {}
RANK_T_AVATAR_FIELD.type = 5
RANK_T_AVATAR_FIELD.cpp_type = 1

RANK_T_TOUKUANG_FIELD.name = "toukuang"
RANK_T_TOUKUANG_FIELD.full_name = ".dhc.rank_t.toukuang"
RANK_T_TOUKUANG_FIELD.number = 7
RANK_T_TOUKUANG_FIELD.index = 6
RANK_T_TOUKUANG_FIELD.label = 3
RANK_T_TOUKUANG_FIELD.has_default_value = false
RANK_T_TOUKUANG_FIELD.default_value = {}
RANK_T_TOUKUANG_FIELD.type = 5
RANK_T_TOUKUANG_FIELD.cpp_type = 1

RANK_T_REGION_ID_FIELD.name = "region_id"
RANK_T_REGION_ID_FIELD.full_name = ".dhc.rank_t.region_id"
RANK_T_REGION_ID_FIELD.number = 8
RANK_T_REGION_ID_FIELD.index = 7
RANK_T_REGION_ID_FIELD.label = 3
RANK_T_REGION_ID_FIELD.has_default_value = false
RANK_T_REGION_ID_FIELD.default_value = {}
RANK_T_REGION_ID_FIELD.type = 5
RANK_T_REGION_ID_FIELD.cpp_type = 1

RANK_T_NAME_COLOR_FIELD.name = "name_color"
RANK_T_NAME_COLOR_FIELD.full_name = ".dhc.rank_t.name_color"
RANK_T_NAME_COLOR_FIELD.number = 9
RANK_T_NAME_COLOR_FIELD.index = 8
RANK_T_NAME_COLOR_FIELD.label = 3
RANK_T_NAME_COLOR_FIELD.has_default_value = false
RANK_T_NAME_COLOR_FIELD.default_value = {}
RANK_T_NAME_COLOR_FIELD.type = 5
RANK_T_NAME_COLOR_FIELD.cpp_type = 1

RANK_T_VALUE_FIELD.name = "value"
RANK_T_VALUE_FIELD.full_name = ".dhc.rank_t.value"
RANK_T_VALUE_FIELD.number = 10
RANK_T_VALUE_FIELD.index = 9
RANK_T_VALUE_FIELD.label = 3
RANK_T_VALUE_FIELD.has_default_value = false
RANK_T_VALUE_FIELD.default_value = {}
RANK_T_VALUE_FIELD.type = 5
RANK_T_VALUE_FIELD.cpp_type = 1

RANK_T.name = "rank_t"
RANK_T.full_name = ".dhc.rank_t"
RANK_T.nested_types = {}
RANK_T.enum_types = {}
RANK_T.fields = {RANK_T_GUID_FIELD, RANK_T_PLAYER_GUID_FIELD, RANK_T_NAME_FIELD, RANK_T_SEX_FIELD, RANK_T_LEVEL_FIELD, RANK_T_AVATAR_FIELD, RANK_T_TOUKUANG_FIELD, RANK_T_REGION_ID_FIELD, RANK_T_NAME_COLOR_FIELD, RANK_T_VALUE_FIELD}
RANK_T.is_extendable = false
RANK_T.extensions = {}

module('rank_pb')

rank_t = protobuf.Message(RANK_T)

