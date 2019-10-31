#include "utils.h"
#include <boost/random.hpp>
#include <time.h>

namespace Utils {

	int32_t get_int32 (int32_t min, int32_t max)
	{
		using namespace boost;

		uniform_int<> distribution (min, max);
		static mt19937 engine(time(NULL));
		variate_generator<mt19937&, uniform_int<> > rand_gen (engine, distribution);

		return rand_gen ();
	}
}
