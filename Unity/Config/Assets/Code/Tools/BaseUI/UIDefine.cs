using UnityEngine;
using System.Collections.Generic;

public class UIDefine : Singleton<UIDefine> {

    public const int iTipsBeginDepth = 50;

    public const int iLevelUI_1 = -1;
    public const int iLevelUI0 = 0;
    public const int iLevelUI1 = 1;
    public const int iLevelUI2 = 2;
    public const int iLevelUI3 = 3;
    public const int iLevelUI9 = 9;

    public const string IconSkill = "Icon/Skill/";
    public const string IconGoods = "Icon/Goods/";
    public const string IconHero = "Icon/HeroIcon/";
    public const string IconPet = "Icon/PetIcon/";
    public const string IconFuBen = "Icon/FubenIcon/";
    public const string IconMainUI = "Icon/MainUI/";
    public const string IconEquip = "Icon/Equipment/";

    public readonly string[] IconPaths = new string[] {
        IconSkill,
        IconGoods,
        IconHero,
        IconPet,
        IconFuBen,
        IconMainUI,
        IconEquip,
    };

    #region 背包模块
    public const string BagPanelView = "BagViewAtlas/BagViewPanelPrefab";
    public const string CommonItemShow = "BagViewAtlas/CommonItemShowPanelPrefab";
    public const string DiammonItemShow = "BagViewAtlas/DiamondItemShowPanelPrefab";
    public const string EquipmentItemShow = "BagViewAtlas/EquipmentItemShowPanelPrefab";
    #endregion

    private Dictionary<GUILevelEnum, string> _dicUI;
    public Dictionary<GUILevelEnum, string> DicUI
    {
        get { return _dicUI; }
        set { _dicUI = value; }
    }

    public const string MainCharacterView = "CharacterAtlas/CharacterView";

    public UIDefine()
    {
        DicUI = new Dictionary<GUILevelEnum, string>();

        //Login
#if USE_AB
        DicUI.Add(GUILevelEnum.eLogin, "DLLoginUI@LoginAtlas");
        DicUI.Add(GUILevelEnum.eLoadingUI, "LoaingUI@loadingAtlas");
        DicUI.Add(GUILevelEnum.eAnnUI, "GameAnnUI@AnnAtlas");
#else
        DicUI.Add(GUILevelEnum.eLogin, "LoginAtlas/DLLoginUI");
        DicUI.Add(GUILevelEnum.eDiSanFangLogin, "LoginAtlas/DiSanFangLoginUI");
        DicUI.Add(GUILevelEnum.eLoadingUI, "loadingAtlas/LoaingUI");
        DicUI.Add(GUILevelEnum.eAnnUI, "AnnAtlas/GameAnnUI");
#endif
        DicUI.Add(GUILevelEnum.eReg, "LoginAtlas/DLRegisterUI");
        DicUI.Add(GUILevelEnum.eSelServer, "AnnAtlas/SelServerUI");
        DicUI.Add(GUILevelEnum.eCreate, "CreateUI/createUI");
        DicUI.Add(GUILevelEnum.eNotice, "AnnAtlas/GameAnnUI");
        DicUI.Add(GUILevelEnum.eLoadingInit, "Loading/Loading");

        DicUI.Add(GUILevelEnum.eUpgradeUI, "UpgradeUI/UpgradeUI");


        DicUI.Add(GUILevelEnum.eMainUI, "MainUI/MainView");

        DicUI.Add(GUILevelEnum.eCharacterUI, "CharacterAtlas/CharacterView");
        DicUI.Add(GUILevelEnum.eCharacterUI_Forward, "CharacterAtlas/CharacterView");
        DicUI.Add(GUILevelEnum.eActivity, "Activity/ActivityView1");
        DicUI.Add(GUILevelEnum.eShangDian, "ShangChengUI/ShangChengView2");
        DicUI.Add(GUILevelEnum.eFubenUI, "FuBenAtlas/FuBenView");
        DicUI.Add(GUILevelEnum.eFubenRes, "FuBenAtlas/FuBenResult");
        DicUI.Add(GUILevelEnum.eTuiChuFuBenUI, "Tips/TuiChuFuBenTip");

        DicUI.Add(GUILevelEnum.eDengJiFuBenViewUI, "FuBenAtlas/DengJiFuBenView");
        DicUI.Add(GUILevelEnum.eBattleResultFuBenViewUI, "BattleResultAtlas/BattleResultFuBenView");
        DicUI.Add(GUILevelEnum.eFanPaiAwardUI, "BattleResultAtlas/FanPaiAward");
        DicUI.Add(GUILevelEnum.eBagViewUI, "BagViewAtlas/BagViewPanelPrefab");
        DicUI.Add(GUILevelEnum.eBagViewNormalHitUI, "BagViewAtlas/CommonItemShowPanelPrefab");
        DicUI.Add(GUILevelEnum.eResourceFetchUI, "BagViewAtlas/RourceFetchUrlUI");

        DicUI.Add(GUILevelEnum.eTask, "TaskAtlas/TaskUI");
        DicUI.Add(GUILevelEnum.eBagViewDiamondHitUI, "BagViewAtlas/DiamondItemShowPanelPrefab");
        DicUI.Add(GUILevelEnum.eBangPai, "FactionAtlas/FactionUI");
        DicUI.Add(GUILevelEnum.eBangPaiJianZhu, "FactionAtlas/FactionBuildingsUI");
        DicUI.Add(GUILevelEnum.eBagViewEquipHitUI, "BagViewAtlas/EquipmentItemShowPanelPrefab");
        DicUI.Add(GUILevelEnum.eMap, "MapUI/MapUI");
        DicUI.Add(GUILevelEnum.eHuoDong, "JingJiChangAtlas/JingJiChangUI");
        DicUI.Add(GUILevelEnum.eSkill, "CharacterAtlas/SkillView");

        //聊天
        DicUI.Add(GUILevelEnum.eHuaYu, "ChatAtlas/ChatView");
        DicUI.Add(GUILevelEnum.eBaoBao, "PetAtlas/PetUI");
        DicUI.Add(GUILevelEnum.eHuangBang, "PaiHangBang/PaiHangBang");
        DicUI.Add(GUILevelEnum.eMoNiZhanResultView, "MoNiZhanUI/MoNiZhanResultView");
        //添加对话框
        DicUI.Add(GUILevelEnum.eDialog, "DialogboxAtlas/DialogBoxUI");

        DicUI.Add(GUILevelEnum.eReliveTips, "Tips/ReliveTip");

        //设置
        DicUI.Add(GUILevelEnum.eSheZhi, "SettingAtlas/SettingUI");
        DicUI.Add(GUILevelEnum.eHuanXian, "SettingAtlas/LineUI");
        DicUI.Add(GUILevelEnum.eGuideUI, "GuideUI/GuideUI");
        DicUI.Add(GUILevelEnum.eGuideContent, "GuideUI/content");
        DicUI.Add(GUILevelEnum.eQuickEqupTips, "QuickEquipTips/QuickEquipTips");

        //好友
        DicUI.Add(GUILevelEnum.eFriends, "FriendUI/FriendUI");
        DicUI.Add(GUILevelEnum.eFriendRatify, "Tips/FriendRatifyTip");
        DicUI.Add(GUILevelEnum.eFriends_Forward, "FriendUI/FriendUI");

        //邮件
        DicUI.Add(GUILevelEnum.eEmail, "MailUI/MailView");

        DicUI.Add(GUILevelEnum.eDaoZao, "EquipUI/EquipView");



        DicUI.Add(GUILevelEnum.ePlotUI, "PlotUI/PlotUI");
        DicUI.Add(GUILevelEnum.eScenePlotUI, "PlotUI/ScenePlotUI");
        DicUI.Add(GUILevelEnum.eNetLoading, "Loading/NetLoading");
        DicUI.Add(GUILevelEnum.eWarning, "Loading/Warning");
        DicUI.Add(GUILevelEnum.eDisConnected, "Tips/DisconnectedTip");

        //Tips

        DicUI.Add(GUILevelEnum.eConfirmQuitTip, "Tips/ConfirmQuitTip");
        DicUI.Add(GUILevelEnum.eCommodifyBuyTip, "Tips/CommodityBuyTip");
        DicUI.Add(GUILevelEnum.eCommodifySellTip, "Tips/CommoditySellTip");


        DicUI.Add(GUILevelEnum.eOKTip, "Tips/OKTip");
        DicUI.Add(GUILevelEnum.ePetChangeNameTip, "PetAtlas/ChangeNameTip");
        DicUI.Add(GUILevelEnum.eTopTips, "TopTips/TipsFactoryUI");
        DicUI.Add(GUILevelEnum.eFightingTips, "Tips/FightingTips");
        DicUI.Add(GUILevelEnum.eNewSkillUI, "Skill/NewSkillPanel");
        //宝石
        DicUI.Add(GUILevelEnum.eBaoShi, "DiamondAtlas/DiamondView");
        DicUI.Add(GUILevelEnum.eBaoShi_Forward, "DiamondAtlas/DiamondView");

        //组队
        DicUI.Add(GUILevelEnum.eTeamDetails, "Tips/TeamMemberInfoTip");
        DicUI.Add(GUILevelEnum.eTeamRatify, "Tips/TeamRatifyTip");
        DicUI.Add(GUILevelEnum.eTeam, "TeamUI/TeamUI");
        DicUI.Add(GUILevelEnum.eTeamSceneUI, "TeamUI/TeamSceneUI");

        DicUI.Add(GUILevelEnum.eMoneyUI, "MainUI/MoneyUI");
        DicUI.Add(GUILevelEnum.eChoujiang, "ChouJiang/ChouJiangUI");
        DicUI.Add(GUILevelEnum.eLoginSign, "FuLiUI/FuliUI");

        //Vip
        DicUI.Add(GUILevelEnum.eVip, "VIPUI/NewVIPView");
        //招募
        DicUI.Add(GUILevelEnum.eRecruit, "RecruitUI/RecruitView");
        //荣誉
        DicUI.Add(GUILevelEnum.eHonor, "HonorUI/HonorUI");
        //秘境副本
        DicUI.Add(GUILevelEnum.eSecretArea, "SecretAreaUI/SecretArea");
        //目标界面
        DicUI.Add(GUILevelEnum.eTarget, "TargetUI/TargetUI");
        //充值
        DicUI.Add(GUILevelEnum.eRecharge, "Recharge/RechargeView");

        DicUI.Add(GUILevelEnum.eCommonTips, "Tips/CommonTips");
        DicUI.Add(GUILevelEnum.eChengJiuTips, "Tips/ChengjiuTips");

        //通用提示框
        DicUI.Add(GUILevelEnum.eCommonNotify, "Tips/CommonNotify");

        //变强
        DicUI.Add(GUILevelEnum.eBecomeStrong, "BecomeStrongUI/BecomeStrongUI");

        DicUI.Add(GUILevelEnum.eWarCountry, "MainUI/WarTopUI");
        DicUI.Add(GUILevelEnum.eMoNiZhanMainView, "MainUI/MoNiZhanMainView");

        DicUI.Add(GUILevelEnum.eQuestionTip, "Tips/QuestionTips");
        DicUI.Add(GUILevelEnum.eSelCamera, "SelCamera/SelCamera");

        DicUI.Add(GUILevelEnum.eOnAttacked, "Tips/OnAttackedUI");
        DicUI.Add(GUILevelEnum.eFeiJian, "FlyPetsUI/FlyPetsUIOld");

        DicUI.Add(GUILevelEnum.eChongZhi, "Recharge/RechargeViewNew");
        DicUI.Add(GUILevelEnum.eRechargeRank, "Recharge/RechargeRank");
        //其他角色信息
        DicUI.Add(GUILevelEnum.eOtherInfo, "Tips/OtherPplayerInformation");
        //宠物信息
        DicUI.Add(GUILevelEnum.ePetInfo, "Tips/OtherPlayerPetInformation");
        DicUI.Add(GUILevelEnum.eTiaoZhan, "TiaoZhanUI/TiaoZhanUI");
    }
}

public enum GUILevelEnum
{
    eDefaultUI = 1000,

    #region -1级
    //login
    eLogin = -1005,
    eReg = -1006,
    eCreate = -1007,
    eSelServer = -1008,
    eAnnUI = -1009,
    eDiSanFangLogin = -1010,
    /// <summary>
    /// 主界面
    /// </summary>
    eMainUI = -1000,

    eLoadingUI = -1050,
    eLoadingInit = -1051,

    // 强制更新
    eUpgradeUI = -1060,
    ePlotUI,
    #endregion

    #region 登录线 0级UI
    eActivity = 300,
    eTarget,
    eBecomeStrong,
    #endregion

    #region 主模块 1级UI
    eDialog = 1904,//对话框
    eTask = 1010,

    eDengJiFuBenViewUI = 1200,
    eBangPai = 1201,
    eBangPaiJianZhu = 2061,
    eBattleResultFuBenViewUI = 2200,
    eFanPaiAwardUI = 3100,
    //主UI按钮界面

    /// <summary>
    /// 副本
    /// </summary>
    eFubenUI = 1100,
    eFubenRes = 1102,
    eTuiChuFuBenUI = 2111,
    /// <summary>
    /// 背包
    /// </summary>
    eBagViewUI = 1101,
    eBagViewEquipHitUI = 9101,
    eBagViewDiamondHitUI = 3102,
    eBagViewNormalHitUI = 3103,
    eDaoZao = 2104,
    eBaoShi_Forward = 2105,
    eBaoShi = 1219,
    eNotice = 2106,
    //宝宝
    eBaoBao,
    eChoujiang = 2108,
    eSkill,
    eFeiJian,

    eSheZhi = 1121,
    eEmail,
    eFriends,
    eYuYin,
    eTeam,
    eFriends_Forward = 2231,
    //聊天

    eMarket,
    eQunLiao,

    eMoNiZhanResultView = 1300,

    //地图
    eMap = 1401,

    eHuanXian = 2403,
    //七日领取奖励
    eLoginSign = 2404,
    //挑战它
    eTiaoZhan = 2406,

    //top
    /// <summary>
    /// 人物属性
    /// </summary>
    eCharacterUI = 1500,
    eCharacterUI_Forward = 2405,
    eHuoDong = 1600,
    eFuLi,
    eShangDian = 1800,
    eChongZhi,

    eHuangBang = 1900,
    eShiButton,
    eShouJi,


    eConfirmQuitTip = 3800,
    ePetChangeNameTip = 3801,
    eCommodifyBuyTip = 3802,
    eCommodifySellTip = 3803,
    eOKTip = 3900,

    //组队
    eTeamDetails,
    eTeamSceneUI,
    //vip
    eVip,
    eRecruit = 3904,
    eRechargeRank,
    eRecharge,
    eHonor,
    //秘境副本
    eSecretArea,
    eOtherInfo,



    #endregion


    // 如果自治界面可以被覆盖，使用GUITipControl做基类，UIPanel Depth 托管在50层以上
    // 实现的代码里必须调用基类的InitData和Close方法。
    #region 自治UI 9开头; 简单的说就是弹任何其他面板，这个都不销毁，也不隐藏。
    /// <summary>
    /// Tips
    /// </summary>
    eScenePlotUI,
    eQuickEqupTips = 9499,
    eHuaYu,
    eReliveTips,
    eResourceFetchUI,
    eNewSkillUI,
    eTeamRatify,
    eChengJiuTips,
    eMoneyUI,
    eCommonTips,
    eNetLoading,
    eDisConnected,
    eTopTips,
    eWarning,
    eFriendRatify,
    eCommonNotify,
    eFightingTips,
    eGuideUI,
    eGuideContent,
    eWarCountry,
    eQuestionTip,
    eSelCamera,
    /// <summary>
    /// 模拟战主界面
    /// </summary>

    ePetInfo,
    eMoNiZhanMainView,
    eOnAttacked,
    #endregion
}

public enum GUIEventType
{
    Chat,
    MainViewChat,
    ChatView,
    ChatViewChat,
    //主界面雷达
    MainUI_Randar_Add, //添加
    MainUI_Randar_Rem, //移除
}
