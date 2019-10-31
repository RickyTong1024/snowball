#include "room_center_operation.h"
#include "utils.h"

std::string tmp = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

std::string RoomCenterOperation::make_code()
{
	std::string s;
	for (int i = 0; i < 32; ++i)
	{
		s += tmp[Utils::get_int32(0, tmp.size() - 1)];
	}
	return s;
}
