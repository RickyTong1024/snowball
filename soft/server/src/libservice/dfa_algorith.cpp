#include "dfa_algorith.h"
#include "scheme.h"

DFAState::DFAState(DFAState* parent)
{
	parent_ = parent;
	failure_ = 0;
}

DFAState* DFAState::add_goto(char c)
{
	if (goto_.find(c) == goto_.end())
	{
		goto_[c] = new DFAState(this);
		return goto_[c];
	}
	else
	{
		return goto_[c];
	}
}

DFA::DFA()
{
	start_state_ = new DFAState(0);
	start_state_->failure_ = start_state_;
}

DFA::~DFA()
{
	this->close_dfa();
}

int DFA::init(Scheme *scheme)
{
	DBCFile * dbfile = scheme->get_dbc("t_prohibitword.txt");

	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		this->add_keyword(dbfile->Get(i, 0)->pString);
	}
	this->rebuild_dfa();

	return 0;
}

int DFA::init_ex()
{
	this->rebuild_dfa();
	return 0;
}

int DFA::search(const std::string& text)
{
	DFAState* curState = start_state_;
	for (int i = 0; i < text.length(); ++i)
	{
		/// 查看状态机中当前状态下该字符对应的下一状态，如果在当前状态下找不到满足该个字符的状态路线，
		/// 则返回到当前状态的失败状态下继续寻找，直到初始状态
		while (curState->goto_.find(text.at(i)) == curState->goto_.end())
		{
			if (curState->failure_ != start_state_)
			{
				if (curState == curState->failure_)
				{ //陷入死循环了...
					printf("cant find");
					break;
				}
				curState = curState->failure_; // 返回到当前状态的失败状态
			}
			else
			{
				curState = start_state_;
				break;
			}
		}
		/// 如果当前状态下能找到该字符对应的下一状态，则跳到下一状态m，
		/// 如果状态m包含了output_，表示匹配到了关键词，具体原因请继续往下看
		DFAState::GotoIte iter = curState->goto_.find(text.at(i));
		if (iter != curState->goto_.end())
		{
			curState = (*iter).second;
			if (curState->output_.size() != 0)
			{
				return -1;
			}
		}
	}
	return 0;
}

void DFA::change(std::string& text)
{
	DFAState* curState = start_state_;
	for (int i = 0; i < text.length(); ++i)
	{
		/// 查看状态机中当前状态下该字符对应的下一状态，如果在当前状态下找不到满足该个字符的状态路线，
		/// 则返回到当前状态的失败状态下继续寻找，直到初始状态
		while (curState->goto_.find(text.at(i)) == curState->goto_.end())
		{
			if (curState->failure_ != start_state_)
			{
				if (curState == curState->failure_)
				{ //陷入死循环了...
					printf("cant find");
					break;
				}
				curState = curState->failure_; // 返回到当前状态的失败状态
			}
			else
			{
				curState = start_state_;
				break;
			}
		}
		/// 如果当前状态下能找到该字符对应的下一状态，则跳到下一状态m，
		/// 如果状态m包含了output_，表示匹配到了关键词，具体原因请继续往下看
		DFAState::GotoIte iter = curState->goto_.find(text.at(i));
		if (iter != curState->goto_.end())
		{
			curState = (*iter).second;
			if (curState->output_.size() != 0)
			{
				for (std::list<std::string>::iterator it = curState->output_.begin(); it != curState->output_.end(); ++it)
				{
					std::string t = *it;
					std::string::size_type pos = 0;
					std::string::size_type l = t.size();
					pos = text.find(t, pos);
					while ((pos != std::string::npos))
					{
						text.replace(pos, l, "***");
						pos = text.find(t, (pos + 3));
					}
				}
			}
		}
	}
}

//////////////////////////////////////////////////////////////////////////

void DFA::close_dfa()
{
	this->clean_states(start_state_);
}

void DFA::add_keyword(const std::string& keyword)
{
	key_words_[keyword]= 0;
}

void DFA::delete_keyword(const std::string& keyword)
{
	std::map<std::string, int>::iterator iter = key_words_.find(keyword);
	if(iter != key_words_.end())
	{
		key_words_.erase(iter);
		this->rebuild_dfa();
	}
}

void DFA::do_add_word(const std::string& keyword)
{
	int i = 0;
	DFAState* curState = start_state_;

	for (i = 0; i < keyword.length(); i++)
	{
		curState = curState->add_goto(keyword.at(i));
	}

	curState->output_.push_back(keyword);
}

void DFA::rebuild_dfa()
{
	this->clean_states(start_state_);

	start_state_ = new DFAState(0);
	start_state_->failure_ = start_state_;
	/// add all keywords
	for (KeyWordIte iter = key_words_.begin();iter != key_words_.end();++iter)
	{
		this->do_add_word(iter->first);
	}
	/// 为每个状态节点设置失败跳转的状态节点
	this->do_failure();
}

void DFA::clean_states(DFAState* state)
{
	if(!state)
	{
		return;
	}
	DFAState::GotoIte iter = state->goto_.begin();
	for (;iter != state->goto_.end();++iter)
	{
		this->clean_states((*iter).second);
	}
	delete state;
	state = 0;
}

void DFA::do_failure()
{
	std::list<DFAState*> q;
	/// 首先设置起始状态下的所有子状态,设置他们的failure_为起始状态，并将他们添加到q中
	for (DFAState::GotoIte ite = start_state_->goto_.begin();ite != start_state_->goto_.end();++ite)
	{
		q.push_back((*ite).second);
		(*ite).second->failure_ = start_state_;
	}

	while (q.size() != 0)
	{
		/// 获得q的第一个element，并获取它的子节点，为每个子节点设置失败跳转的状态节点
		DFAState* r = q.front();
		DFAState* state;
		q.pop_front();
		for (DFAState::GotoIte itr = r->goto_.begin() ;itr != r->goto_.end(); ++itr)
		{
			q.push_back((*itr).second);
			/// 从父节点的failure_(m1)开始，查找包含字符c对应子节点的节点，
			/// 如果m1找不到，则到m1的failure_查找，依次类推
			state = r->failure_;
			while (state->goto_.find((*itr).first) == state->goto_.end())
			{
				state = state->failure_;
				if (state == start_state_)
				{
					break;
				}
			}
			/// 如果找到了，设置该子节点的failure_为找到的目标节点(m2)，
			/// 并把m2对应的关键词列表添加到该子节点中
			DFAState::GotoIte  seniter = state->goto_.find((*itr).first);
			if (seniter != state->goto_.end())
			{
				(*itr).second->failure_ = (*seniter).second;
				for (std::list<std::string>::iterator thiter = (*itr).second->failure_->output_.begin(); thiter != (*itr).second->failure_->output_.end();++thiter)
				{
					(*itr).second->output_.push_back(*thiter);
				}
			}
			else
			{
				/// 找不到，设置该子节点的failure_为初始节点
				(*itr).second->failure_ = start_state_;
			}
		}
	}
}
