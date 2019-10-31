#ifndef __DFA_ALGORITH_H__
#define __DFA_ALGORITH_H__

#include <list>
#include <map>
#include <string>

class DFAState 
{
public:
	DFAState(DFAState* parent);

	DFAState* add_goto(char c);

	typedef std::map<char, DFAState*> Goto;
	typedef Goto::iterator GotoIte;
	Goto goto_;
	DFAState* failure_;
	DFAState* parent_;
	std::list<std::string> output_;
};

class Scheme;

class DFA
{
public: 
	DFA();

	~DFA();

	int init(Scheme *scheme);

	int init_ex();

	/// �����ַ���text�Ƿ�����ؼ��ʣ�������outputCallback����ƥ��Ĺؼ���
	int search(const std::string& text);

	void change(std::string& text);

protected:
	/// �������DFA״̬
	void close_dfa();

	/// ���ӹؼ��ʣ�ͬʱ�ؽ�DFA
	void add_keyword(const std::string& keyword);

	/// ɾ���ؼ��ʣ�ͬʱ�ؽ�DFA
	void delete_keyword(const std::string& keyword);

	/// ��ӹؼ��ʵ�״̬����ʵ�ʲ���������״̬�ڵ㣬�������������ڵ��output_
	void do_add_word(const std::string& keyword);

	/// �ؽ�״̬��
	void rebuild_dfa();

	/// ���state�µ�����״̬�ڵ�
	void clean_states(DFAState* state);

	/// Ϊÿ��״̬�ڵ�����ʧ����ת��״̬�ڵ�
	void do_failure();

private:
	/// ��ʼ״̬
	DFAState* start_state_;

	/// ��¼�˵�ǰ�ؼ��ʼ���
	std::map<std::string, int> key_words_;

	typedef std::map<std::string, int>::iterator KeyWordIte;
};

#endif
