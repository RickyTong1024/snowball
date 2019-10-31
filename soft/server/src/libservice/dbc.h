/*
|==========================================
|	DBC���ݿ��ļ���
|		(������/�ͻ���ͨ��)
|==========================================
|
|		--------------------
|		|  ���ݿ��ļ���ʽ  |
|		--------------------
|
|		Offset |	Type  |  Description  
|		-------+----------+------------------
|	Head
|		0X000	  int		DBC File Identity�� always 0XDDBBCC00
|		0X004	  int      Number of records in the file 
|		0X008     int      Number of 4-byte fields per record
|		0X010     int      String block size 
|   FieldType
|		0X014     int[FieldNum]   
|							  The type fo fields(0-int, 1-double, 2-string)
|   FieldBlock
|				  int[FieldNum*RecordNum]
|							  DataBlock
|	StringBlock
|				  char[StringSize]
|							  StringBlock
|
*/
#ifndef __DBC_H__
#define __DBC_H__

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <vector>
#include <map>

class DBCFile	
{
public:
	//�ļ�ͷ
	struct FILE_HEAD
	{
		unsigned int		m_Identify;				//��ʾ	0XDDBBCC00
		int			m_nFieldsNum;			//����
		int			m_nRecordsNum;			//����
		int			m_nStringBlockSize;		//�ַ�������С
	};

	//�ֶ���������
	enum FIELD_TYPE
	{
		T_INT		= 0,	//����
		T_DOUBLE	= 1,	//������
		T_STRING	= 2,	//�ַ���
		T_UNKNOW	= 3,	//��ȷ������
	};

	//���ݿ��ʽ����
	typedef std::vector< FIELD_TYPE >	FILEDS_TYPE;

	//���ݶ�
	struct FIELD
	{
		union {
			double		dValue;
			int			iValue;
			int			wValue;
			char		cValue;
			const char*	pString;	// Just for runtime!
		};

		bool isempty;

		//Construct
		FIELD() {}
		FIELD(int value) { iValue = value; }
		FIELD(double value) { dValue = value; }
		FIELD(const char* value) { pString = value; }
	};
	//������
	typedef std::vector< FIELD >		DATA_BUF;

public:
	//���ı��ļ�������һ�����ݿ�
	bool					OpenFromTXT(const char* szFileName);
	//�����ڴ��е��ļ���
	bool					OpenFromMemory(const char* pMemory, const char* pDeadEnd, const char* szFileName=0);
protected:
	//��ȡ�ı���ʽ����
	bool					OpenFromMemoryImpl_Text(const char* pMemory, const char* pDeadEnd, const char* szFileName=0);
	//��ȡ�����Ƹ�ʽ����
	bool					OpenFromMemoryImpl_Binary(const char* pMemory, const char* pDeadEnd, const char* szFileName=0);

public:
	//����������(��һ��Ϊ����)
	virtual const FIELD*	Get(int nValue) const;
	//����λ�ò���
	virtual const FIELD*	Get(int nRecordLine, int nColumNum) const;
	//����ĳ�е���ָ��ֵ�ĵ�һ��
	virtual const FIELD*	Get(int nColumnNum, const FIELD& value) const;

public:
	//ȡ��ID
	int GetID(void) const				{ return m_ID; }
	//ȡ������
	int	GetFieldsNum(void) const	    { return m_nFieldsNum; }
	//ȡ�ü�¼������
	int GetRecordsNum(void) const		{ return m_nRecordsNum; }
	//����������
	void CreateIndex(int nColum = 0, const char* szFileName=0);

protected:
	typedef std::map< int, FIELD*>	FIELD_HASHMAP;
	//���ݿ��ʽ�ļ���
	int			m_ID;
	//���ݿ��ʽ����
	FILEDS_TYPE				m_theType;
	//����
	int						m_nRecordsNum;
	//����
	int						m_nFieldsNum;
	//������
	DATA_BUF				m_vDataBuf;		//size = m_nRecordsNum*m_nFieldsNum
	//�ַ�����
	char*					m_pStringBuf;
	//�ַ�������С
	int						m_nStringBufSize;
	//������
	FIELD_HASHMAP			m_hashIndex;
	//������
	int						m_nIndexColum;

public:

	static int			_ConvertStringToVector(const char* strStrintgSource, std::vector< std::string >& vRet, const char* szKey, bool bOneOfKey, bool bIgnoreEmpty);
	//���ڴ����ַ�����ȡһ���ı�(���ջ��з�)
	static const char*	_GetLineFromMemory(char* pStringBuf, int nBufSize, const char* pMemory, const char* pDeadEnd);
	//�Ƚ�����ֵ�Ƿ����
	template < FIELD_TYPE T>	
	static bool			_FieldEqu(const FIELD& a, const FIELD& b);

public:
	DBCFile(int id);
	virtual ~DBCFile();
};

#endif
