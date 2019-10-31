#ifndef __UTILS_H__
#define __UTILS_H__

#include <vector>
#include "typedefs.h"

namespace Utils
{
	int32_t  get_int32 (int32_t min, int32_t max);

	template <typename T>
	void get_vector (const std::vector<T> &ovec, int num, std::vector<T> &nvec)
	{
		if (num <= 0 || num > ovec.size())
		{
			return;
		}
		std::vector<T> tmpvec = ovec;
		for (int i = 0; i < num; ++i)
		{
			int r = get_int32(i, ovec.size() - 1);
			nvec.push_back(tmpvec[r]);
			tmpvec[r] = tmpvec[i];
		}
	}
}

#endif
