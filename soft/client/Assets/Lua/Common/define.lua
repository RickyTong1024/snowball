resMgr = LuaHelper.GetResManager()
gameMgr = LuaHelper.GetGameManager()
panelMgr = LuaHelper.GetPanelManager()
soundMgr = LuaHelper.GetSoundManager()
networkMgr = LuaHelper.GetNetManager()
messageMgr = LuaHelper.GetMessageManager()
mapMgr = LuaHelper.GetMapManager()
timerMgr = LuaHelper.GetTimerManager()
twnMgr = LuaHelper.GetTweenManager()
shareMgr = LuaHelper.GetShareManager()
toolMgr = LuaHelper.GetToolManager()

Application = UnityEngine.Application
WWW = UnityEngine.WWW
WWWForm = UnityEngine.WWWForm
GameObject = UnityEngine.GameObject
PlayerPrefs = UnityEngine.PlayerPrefs
Input = UnityEngine.Input
KeyCode = UnityEngine.KeyCode
QualitySettings = UnityEngine.QualitySettings

require "Common/functions"
require "Common/opcodes"
require "Common/Message"
require "Common/Config"
require "Common/PlayerData"
require "Common/Joy"
require "Common/State"
require "Common/crc32"
require "Common/profiler"
require "Common/LuaAchieve"

require "Net/ConnectTcp"
require "Net/GameTcp"
require "Net/BattleTcp"
require "Net/BattleStateTcp"

require "View/LoginPanel"
require "View/MessagePanel"
require "View/SelectPanel"
require "View/HallPanel"
require "View/TopPanel"
require "View/ItemPanel"
require "View/RolePanel"
require "View/ZonePanel"
require "View/ChatPanel"
require "View/BackPanel"
require "View/HallScene"
require "View/MaskPanel"
require "View/LoadPanel"
require "View/GainPanel"
require "View/SellPanel"
require "View/UsePanel"
require "View/BuyPanel"
require "View/AvatarPanel"
require "View/DetailPanel"
require "View/CupPanel"
require "View/SetPanel"
require "View/MailPanel"
require "View/ShopPanel"
require "View/ChestPanel"
require "View/SharePanel"
require "View/IconPanel"
require "View/AvaIconPanel"
require "View/NoticePanel"
require "View/RoleGetPanel"
require "View/SignPanel"
require "View/StartPanel"
require "View/TeamPanel"
require "View/ChestShowPanel"
require "View/FriendPanel"
require "View/AchieveAnimation"
require "View/LevelTask"
require "VIew/LevelUpPanel"
require "View/NewAchievePanel"
require "View/RankPanel"

require "protobuf/acc_pb"
require "protobuf/msg_connect_pb"
require "protobuf/msg_hall_pb"
require "protobuf/msg_team_pb"
require "protobuf/msg_social_pb"
require "protobuf/msg_login_pb"
require "protobuf/player_pb"
require "protobuf/rank_pb"
require "protobuf/msg_rank_pb"

require "GUIRoot"

cjson_safe = require "cjson.safe"

self = PlayerData
self.guid = 0
self.battle_code = 0
self.game_code = 0

math.randomseed(os.time())