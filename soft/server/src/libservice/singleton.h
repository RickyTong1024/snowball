#ifndef __SINGLETON_H__
#define __SINGLETON_H__

template<typename _Type>
class Singleton
{
public:
	static _Type * instance (void)
	{
		static _Type obj;
		return &obj;
	}

private:
	Singleton (void) {}
};
#endif
