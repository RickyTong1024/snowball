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
		/// �鿴״̬���е�ǰ״̬�¸��ַ���Ӧ����һ״̬������ڵ�ǰ״̬���Ҳ�������ø��ַ���״̬·�ߣ�
		/// �򷵻ص���ǰ״̬��ʧ��״̬�¼���Ѱ�ң�ֱ����ʼ״̬
		while (curState->goto_.find(text.at(i)) == curState->goto_.end())
		{
			if (curState->failure_ != start_state_)
			{
				if (curState == curState->failure_)
				{ //������ѭ����...
					printf("cant find");
					break;
				}
				curState = curState->failure_; // ���ص���ǰ״̬��ʧ��״̬
			}
			else
			{
				curState = start_state_;
				break;
			}
		}
		/// �����ǰ״̬�����ҵ����ַ���Ӧ����һ״̬����������һ״̬m��
		/// ���״̬m������output_����ʾƥ�䵽�˹ؼ��ʣ�����ԭ����������¿�
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
		/// �鿴״̬���е�ǰ״̬�¸��ַ���Ӧ����һ״̬������ڵ�ǰ״̬���Ҳ�������ø��ַ���״̬·�ߣ�
		/// �򷵻ص���ǰ״̬��ʧ��״̬�¼���Ѱ�ң�ֱ����ʼ״̬
		while (curState->goto_.find(text.at(i)) == curState->goto_.end())
		{
			if (curState->failure_ != start_state_)
			{
				if (curState == curState->failure_)
				{ //������ѭ����...
					printf("cant find");
					break;
				}
				curState = curState->failure_; // ���ص���ǰ״̬��ʧ��״̬
			}
			else
			{
				curState = start_state_;
				break;
			}
		}
		/// �����ǰ״̬�����ҵ����ַ���Ӧ����һ״̬����������һ״̬m��
		/// ���״̬m������output_����ʾƥ�䵽�˹ؼ��ʣ�����ԭ����������¿�
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
	/// Ϊÿ��״̬�ڵ�����ʧ����ת��״̬�ڵ�
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
	/// ����������ʼ״̬�µ�������״̬,�������ǵ�failure_Ϊ��ʼ״̬������������ӵ�q��
	for (DFAState::GotoIte ite = start_state_->goto_.begin();ite != start_state_->goto_.end();++ite)
	{
		q.push_back((*ite).second);
		(*ite).second->failure_ = start_state_;
	}

	while (q.size() != 0)
	{
		/// ���q�ĵ�һ��element������ȡ�����ӽڵ㣬Ϊÿ���ӽڵ�����ʧ����ת��״̬�ڵ�
		DFAState* r = q.front();
		DFAState* state;
		q.pop_front();
		for (DFAState::GotoIte itr = r->goto_.begin() ;itr != r->goto_.end(); ++itr)
		{
			q.push_back((*itr).second);
			/// �Ӹ��ڵ��failure_(m1)��ʼ�����Ұ����ַ�c��Ӧ�ӽڵ�Ľڵ㣬
			/// ���m1�Ҳ�������m1��failure_���ң���������
			state = r->failure_;
			while (state->goto_.find((*itr).first) == state->goto_.end())
			{
				state = state->failure_;
				if (state == start_state_)
				{
					break;
				}
			}
			/// ����ҵ��ˣ����ø��ӽڵ��failure_Ϊ�ҵ���Ŀ��ڵ�(m2)��
			/// ����m2��Ӧ�Ĺؼ����б���ӵ����ӽڵ���
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
				/// �Ҳ��������ø��ӽڵ��failure_Ϊ��ʼ�ڵ�
				(*itr).second->failure_ = start_state_;
			}
		}
	}
}
