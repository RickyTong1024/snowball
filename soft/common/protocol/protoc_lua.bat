del ..\..\client\Assets\Lua\protobuf\*.lua

..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf acc.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf player.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf role.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf social.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf rank.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf battle_his.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf battle_result.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_connect.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_login.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_hall.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_hall1.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_battle.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_battle1.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_team.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_social.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf post.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf post_new.proto
..\etools\protoc.exe --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=..\..\client\Assets\Lua\protobuf msg_rank.proto

pause
