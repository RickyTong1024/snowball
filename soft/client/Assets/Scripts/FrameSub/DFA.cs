using UnityEngine;
using System.Collections;
using System.Collections.Generic;
class DFAState
{
	public Dictionary<char,DFAState> goto_ = new Dictionary<char, DFAState>();

	public DFAState failure_ ;
	public DFAState parent_;
	public List<string> output_ = new List<string>();
	public DFAState add_goto(char c)
	{
		if(!goto_.ContainsKey(c))
		{
			goto_[c] = new DFAState(this);
			return goto_[c];
		}
		else
		{
			return goto_[c];
		}
	}
	public DFAState(DFAState parent)
	{
		parent_ = parent;
		failure_ = null;
	}
}
public class DFA 
{
	public static DFA m_instance = null;
	DFAState start_state_;
	Dictionary<string ,int> key_words_ = new Dictionary<string, int>();
	string s;
    dbc m_dbc_prohibitword;

    public DFA()
    {
		start_state_ = new DFAState (null);
		start_state_.failure_ = start_state_;
    }

	public static DFA _instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DFA();
            }
            return m_instance;
        }
	}

	public int init()
	{
        m_dbc_prohibitword = new dbc();
        m_dbc_prohibitword.load_txt("t_prohibitword");

        if (m_dbc_prohibitword == null)
		{
			return -1;
		}
		
		for (int i = 0; i < m_dbc_prohibitword.get_y(); ++i)
		{
			this.add_keyword(m_dbc_prohibitword.get_string(i,0));
		}
		this.rebuild_dfa();
		
		return 0;
	}
	public string search(string text)
	{
		DFAState curState = start_state_;
		s = text;
		int i;
		for (i = 0; i < text.Length; ++i)
		{
			/// 查看状态机中当前状态下该字符对应的下一状态，如果在当前状态下找不到满足该个字符的状态路线，
			/// 则返回到当前状态的失败状态下继续寻找，直到初始状态
			while (!curState.goto_.ContainsKey(text[i]))
			{
				if (curState.failure_ != start_state_)
				{
					if (curState == curState.failure_)
					{ //陷入死循环了...

						break;
					}
					curState = curState.failure_; // 返回到当前状态的失败状态
				}
				else
				{
					curState = start_state_;
					break;
				}
			}
			/// 如果当前状态下能找到该字符对应的下一状态，则跳到下一状态m，
			/// 如果状态m包含了output_，表示匹配到了关键词，具体原因请继续往下看
			if (curState.goto_.ContainsKey(text[i]))
			{
				curState = curState.goto_[text[i]];//可能有错
				if(curState.output_.Count!= 0)
				{
					s = s.Replace(curState.output_[0],"***");
				}

			}

		}
		return s;
	}

	public bool fei_fa(string text)
	{
		DFAState curState = start_state_;
		s = text;
		int i;
		for (i = 0; i < text.Length; ++i)
		{
			/// 查看状态机中当前状态下该字符对应的下一状态，如果在当前状态下找不到满足该个字符的状态路线，
			/// 则返回到当前状态的失败状态下继续寻找，直到初始状态
			while (!curState.goto_.ContainsKey(text[i]))
			{
				if (curState.failure_ != start_state_)
				{
					if (curState == curState.failure_)
					{ //陷入死循环了...
						
						break;
					}
					curState = curState.failure_; // 返回到当前状态的失败状态
				}
				else
				{
					curState = start_state_;
					break;
				}
			}
			/// 如果当前状态下能找到该字符对应的下一状态，则跳到下一状态m，
			/// 如果状态m包含了output_，表示匹配到了关键词，具体原因请继续往下看
			if (curState.goto_.ContainsKey(text[i]))
			{
				curState = curState.goto_[text[i]];//可能有错
				if(curState.output_.Count!= 0)
				{
					return true;
				}
				
			}
			
		}
		return false;
	}

	void close_dfa()
	{
		this.clean_states (start_state_);
	}
	void add_keyword(string keyword)
	{
		if(!key_words_.ContainsKey(keyword))
		{
			key_words_.Add(keyword,0);
		}

	
	}
	void delete_keyword(string keyword)
	{
		if(key_words_.ContainsKey(keyword))
		{
			key_words_.Remove(keyword);
			this.rebuild_dfa();
		}
	
	}
	void do_add_word(string keyword)
	{
		int i = 0;
		DFAState curState = start_state_;
		
		for (i = 0; i < keyword.Length; i++)
		{
			curState = curState.add_goto(keyword[i]);
		}
		
		curState.output_.Add(keyword);

	}
	void rebuild_dfa()
	{
		this.clean_states(start_state_);
		
		start_state_ = new DFAState(null);
		start_state_.failure_ = start_state_;
		/// add all keywords
		foreach (var item  in key_words_)
		{
			this.do_add_word(item.Key);

		}
		/// 为每个状态节点设置失败跳转的状态节点
		this.do_failure();
	}
	void clean_states(DFAState state)
	{
		if(state == null)
		{
			return;
		}
		foreach (var item  in state.goto_)
		{
			this.clean_states(item.Value);
		}
		state = null;
	}
	void do_failure()
	{
		List<DFAState> q = new List<DFAState>();
		/// 首先设置起始状态下的所有子状态,设置他们的failure_为起始状态，并将他们添加到q中
		foreach (var item  in start_state_.goto_)
		{
			q.Add(item.Value);
			item.Value.failure_ = start_state_;
		}
		
		while (q.Count != 0)
		{
			/// 获得q的第一个element，并获取它的子节点，为每个子节点设置失败跳转的状态节点
			DFAState r = q[0];
			DFAState state;
			q.RemoveAt(0);
			foreach (var item  in r.goto_)
			{
				q.Add(item.Value);
				/// 从父节点的failure_(m1)开始，查找包含字符c对应子节点的节点，
				/// 如果m1找不到，则到m1的failure_查找，依次类推
				state = r.failure_;
				while (!state.goto_.ContainsKey(item.Key))
				{
					state = state.failure_;
					if (object.ReferenceEquals(state,start_state_))
					{
						break;
					}
				}
				/// 如果找到了，设置该子节点的failure_为找到的目标节点(m2)，
				/// 并把m2对应的关键词列表添加到该子节点中

				if (state.goto_.ContainsKey(item.Key))
				{
					item.Value.failure_ = state.goto_[item.Key];
					foreach (var item1  in item.Value.failure_.output_)
					{
						item.Value.output_.Add(item1);
					}
				}
				else
				{
					/// 找不到，设置该子节点的failure_为初始节点
					item.Value.failure_ = start_state_;
				}
			}
		
		}
	}


}
