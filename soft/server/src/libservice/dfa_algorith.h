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

	/// 检索字符串text是否包含关键词，并调用outputCallback处理匹配的关键词
	int search(const std::string& text);

	void change(std::string& text);

protected:
	/// 清除所有DFA状态
	void close_dfa();

	/// 增加关键词，同时重建DFA
	void add_keyword(const std::string& keyword);

	/// 删除关键词，同时重建DFA
	void delete_keyword(const std::string& keyword);

	/// 添加关键词到状态机的实际操作，建立状态节点，并设置最后结束节点的output_
	void do_add_word(const std::string& keyword);

	/// 重建状态机
	void rebuild_dfa();

	/// 清除state下的所有状态节点
	void clean_states(DFAState* state);

	/// 为每个状态节点设置失败跳转的状态节点
	void do_failure();

private:
	/// 起始状态
	DFAState* start_state_;

	/// 记录了当前关键词集合
	std::map<std::string, int> key_words_;

	typedef std::map<std::string, int>::iterator KeyWordIte;
};

#endif
