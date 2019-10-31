cd ..\..\common\etools

for /l %%i in (1 1 20) do (
	create_db.exe ..\protocol\player.proto snowball_player%%i 47.91.226.108 root 1qaz2wsx@39299911 1
	create_db.exe ..\protocol\role.proto snowball_player%%i 47.91.226.108 root 1qaz2wsx@39299911 1
	create_db.exe ..\protocol\battle_his.proto snowball_player%%i 47.91.226.108 root 1qaz2wsx@39299911 1
	create_db.exe ..\protocol\post.proto snowball_player%%i 47.91.226.108 root 1qaz2wsx@39299911 1
	create_db.exe ..\protocol\post_new.proto snowball_player%%i 47.91.226.108 root 1qaz2wsx@39299911 1
	create_db.exe ..\protocol\share.proto snowball_player%%i 47.91.226.108 root 1qaz2wsx@39299911 1
	create_db.exe ..\protocol\recharge.proto snowball_player%%i 47.91.226.108 root 1qaz2wsx@39299911 1
)
pause