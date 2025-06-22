namespace GIKCore.Net
{
    public class IService
    {
        public const int RECEIVE_PACKAGE_FAIL = -1;

        public const int ERROR = 64000;
        public const int PONG = 253;
        public const int MULTI_LANGUAGE = 3030;

        public const int REGISTER = 0;
        public const int LOGIN = 1;
        public const int LOGIN_FAIL = 2;
        public const int LOGOUT = 7;
        public const int DUPLICATE_LOGIN = 8;
        public const int LOGIN_WEB = 9;
        public const int AUTO_LOGIN = 10;
        public const int SET_LANGUAGE = 11;
        public const int LOGIN_3RD = 12;
        public const int LOGIN_GOOGLE = 14;
        public const int CHECK_VERSION = 20;

        public const int GET_USER_HERO_CARD = 100;
        public const int GET_USER_BATTLE_DECK = 101;

        public const int SET_USER_BATTLE_DECK = 102;
        public const int GET_USER_DECK = 103;
        public const int GET_USER_DECK_DETAIL = 104;
        public const int SET_USER_DECK = 105;
        public const int UPDATE_USER_DECK = 106;
        public const int DELETE_USER_DECK = 107;
        public const int GET_PROFILE = 108;
        public const int GET_LEADER_BOARD = 109;
        public const int GET_EVENT = 110;
        public const int GET_USER_EVENT_INFO = 111;
        public const int GET_RANK = 130;
        public const int GET_MODE = 131;
        public const int VIEW_REWARD = 132;
        public const int GET_REWARD = 133;
        public const int GAME_BATTLE_AUTO_JOIN = 351;
        public const int GAME_BATTLE_JOIN = 51;
        public const int GAME_BATTLE_LEAVE = 52;
        public const int GAME_START = 54;
        public const int GAME_DEAL_CARDS = 55;
        public const int GAME_MULLIGAN = 56;
        public const int GAME_FIRST_GOD_SUMMON = 57;
        public const int GAME_MOVE_GOD_SUMMON = 58;
        public const int GAME_STARTUP_CONFIRM = 59;
        public const int GAME_STARTUP_END = 60;
        public const int GAME_START_BATTLE = 61;
        public const int GAME_START_BATTLE_DETAIL = 611;
        public const int GAME_START_BID = 613;
        public const int GAME_UP_BID = 614;
        public const int GAME_BID_RESULT = 615;
        public const int GAME_SUB_BID_RESULT = 6151;
        public const int GAME_BID_COMFIRM = 616;
        public const int GAME_MOVE_CARD_IN_BATTLE = 63;
        public const int GAME_MOVE_CARD_IN_BATTLE_DETAIL = 631;
        public const int GAME_SUMMON_CARD_IN_BATTLE = 62;
        public const int GAME_SUMMON_CARD_IN_BATTLE_DETAIL = 621;
        public const int GAME_CONFIRM_STARTBATTLE = 65;
        public const int GAME_CONFIRM_STARTBATTLE_DETAIL = 651;
        public const int GAME_CHOOSE_WAY_REQUEST = 67;

        public const int GAME_SIMULATE_BATTLE = 68;
        public const int GAME_PREPARE_SIMULATE_BATTLE = 81;
        public const int GAME_SIMULATE_CONFIRM = 69;
        public const int GAME_BATTLE_ATTACK = 70;
        public const int GAME_BATTLE_DEAL_DAMAGE = 71;
        public const int GAME_BATTLE_HERO_DEAD = 72;
        public const int GAME_BATTLE_HERO_TIRED = 75;
        public const int GAME_BATTLE_HERO_READY = 79;
        public const int GAME_BATTLE_ENDROUND = 73;
        public const int GAME_BATTLE_ENDGAME = 74;

        public const int GAME_SIMULATE_SKILLS_ON_BATTLE= 76;
        public const int GAME_SIMULATE_SKILLS = 77;
        public const int GAME_SKILL_EFFECT = 78;
        public const int GAME_DELETE_CARDS = 83;

        public const int GAME_ACTIVE_SKILL = 80;
        public const int GAME_STATUS_SKILL = 801;
        public const int GAME_UPDATE_HERO_MATRIC = 82;
        public const int GET_BALANCE = 112;
        public const int GET_USER_ITEMS = 114;
        public const int GET_SHOP = 115;
        public const int GET_PACK_DETAIL = 116;
        public const int BUY_ITEM = 117;
        public const int OPEN_CHEST = 118;
        public const int GAME_RESUME = 221;
        public const int GET_QUESTS = 150;
        public const int RECHARGE_GEM = 146;
        public const int UPDATE_PROGRESS = 140;
        public const int GET_VERIFY_IAP = 146;
        public const int NEWBIE_EVENT = 155;
        public const int SET_SCREEN_NAME = 156;
        public const int GET_RANDOM_SCREEN_NAME = 157;

        public const int GET_ACCESSTOKEN = 120;

        public const int GET_TRAY = 151;
        public const int GET_TIME_CHEST = 152;
        public const int ACTIVATE_TIME_CHEST = 153;
        public const int OPEN_TIME_CHEST = 154;
        public const int GET_USER_LEVEL = 159;
        public const int GET_MAP = 160;
        public const int CLAIM_LEVEL_REWARD = 161;
        public const int VIEW_UPGRADE = 164;
        public const int VIEW_UPGRADE_DETAIl = 165;
        public const int UPGRADE = 166;
        public const int FIRST_DECK = 170;

        public static bool NeedShowLoading(int serviceId)
        {
            switch (serviceId)
            {
                case PONG:
                case MULTI_LANGUAGE:
                    return false;
            }
            return true;
        }

        public static bool NeedHideLoading(int serviceId)
        {
            switch (serviceId)
            {
                case PONG:
                case MULTI_LANGUAGE:
                    return false;
            }
            return true;
        }
    }
}