require "Common/define"

Game = {}

function Game.Init()
	Config.Init()
	Message.Init()
	GUIRoot.Init()
	State.ChangeState(State.state.ss_login)
end
