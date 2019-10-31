#ifndef __WINDUMPER_H__
#define __WINDUMPER_H__

#include <string>

#ifdef WIN32

#ifndef DEBUG_LOG
#define DEBUG_LOG printf
#endif

#ifndef INFO_FORMAT_LOG
#define INFO_FORMAT_LOG printf
#endif

#ifndef ERROR_FORMAT_LOG
#define ERROR_FORMAT_LOG printf
#endif

#ifndef ERROR_LOG
#define ERROR_LOG printf
#endif

#ifndef INFO_LOG
#define INFO_LOG printf
#endif

#include <Windows.h>
#include <dbghelp.h>

namespace Windows {

	typedef BOOL (WINAPI *MINIDUMPWRITEDUMP)(HANDLE hProcess, DWORD dwPid, HANDLE hFile, MINIDUMP_TYPE DumpType,
		CONST PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
		CONST PMINIDUMP_USER_STREAM_INFORMATION UserStreamParam,
		CONST PMINIDUMP_CALLBACK_INFORMATION CallbackParam);

	class CoreDumper
	{
	public:
		enum Flags{timestamp_file_name = 0x0001};

		CoreDumper();
	public:
		static LONG WINAPI TopLevelFilter(struct _EXCEPTION_POINTERS *pExceptionInfo);
	public:
		static void filePath(const std::string & file_path);
		static void fileName(const std::string & file_name);
		static int  addFlags(Flags flags);
	private:
		static void disableUnhandleFilter();
		static std::string newFilePath();
	private:
		static int m_flags;
		static std::string m_dump_file_path;
		static std::string m_dump_file_name;
	};

}

#else

namespace Windows {

	class CoreDumper {};

}

#endif

#endif
