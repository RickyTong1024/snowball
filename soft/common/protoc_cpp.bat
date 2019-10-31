del .\protocpp\*.pb.h
del .\protocpp\*.pb.cc

.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\acc.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\player.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\role.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\battle_his.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\battle_result.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\gtool.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=..\server\src\libservice .\protocol\rpc.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_pipe.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_connect.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_login.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_hall.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_hall1.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_battle.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_battle1.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_team.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\post.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\post_new.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\share.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\recharge.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\name.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_name.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\social.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\social_list.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_social.proto 
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\rank.proto
.\etools\protoc.exe -I=.\protocol --cpp_out=.\protocpp .\protocol\msg_rank.proto 
pause
