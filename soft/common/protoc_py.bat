.\etools\protoc.exe -I=.\protocol --python_out=..\server\sub_server\common --python_out=..\server_python\snowballback\engine .\protocol\msg_pipe.proto
.\etools\protoc.exe -I=.\protocol --python_out=..\server\sub_server\common .\protocol\rpc.proto
pause