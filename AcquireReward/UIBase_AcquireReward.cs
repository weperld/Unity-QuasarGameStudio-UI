using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UIParam.AcquireReward;

public class UIBase_AcquireReward : UIBase
{
    #region Inspector
    [SerializeField] private Button btn_Screen;
    [SerializeField] private UI_AcquireReward_ItemList rewardList_00;
    [SerializeField] private UI_AcquireReward_ItemList rewardList_01;
    [SerializeField] private TextMeshProUGUI tmpu_Title;
    #endregion


    #region Variables
    private UIParam.AcquireReward.UIParameterAcquireRewardSetup setupParam;
    private bool showingListNumIs00 = false;
    private bool isFirstShow = true;
    private Animator _AcquireRewardAnim => GetComponent<Animator>();
    private List<UIParam.Common.Reward> showingRewardList;
    #endregion

    public System.Action OnHide;





    // UI 초기화 하려 할 때 호출 하여 사용
    //public override void Setup(object param)
    //{
    //    base.Setup(param);

    //    setupParam = param as UIParameterAcquireRewardSetup;
    //    if (setupParam == null || (setupParam.list_Reward == null || setupParam.list_Reward.Count == 0)) return;

    //    if (tmpu_Title != null) tmpu_Title.text = setupParam.isFirstReward ? "최초 보상" : "획득 보상";
    //    PrepareRewardListToShow();
    //    isFirstShow = true;
    //    var tmp = showingListNumIs00 ? rewardList_00 : rewardList_01;
    //    tmp.gameObject.SetActive(false);
    //}

   

    // UI 활성화 할 때 실행
    public override void Show(object param = null)
    {
        base.Show();
        rewardList_00?.ResetList();
        rewardList_01?.ResetList();
        setupParam = param as UIParameterAcquireRewardSetup;
        if (setupParam == null || (setupParam.list_Reward == null || setupParam.list_Reward.Count == 0)) return;
        if (btn_Screen != null) { btn_Screen.onClick.RemoveAllListeners(); btn_Screen.onClick.AddListener(OnClickScreen); }
        if (tmpu_Title != null) tmpu_Title.text = setupParam.thumbnail_Type == UIParam.Common.Reward.RewardItemType.CARD ? "최초 보상" : "획득 보상";
        PrepareRewardListToShow();
        isFirstShow = true;
        var tmp = showingListNumIs00 ? rewardList_00 : rewardList_01;
        tmp.gameObject.SetActive(false);


        //if (!IsActive())
        //UIManager._instance.SetToTop(this);
    }

    // UI 비활성화 할 때 실행
    public override void Hide()
    {
        //UIDialogueWindow.ShowDialogue(Data.Enum.Tutorial_Trigger.GAME_START);
        OnHide?.Invoke();
        base.Hide();
    }

    private void OnClickScreen()
    {
        if (!PrepareRewardListToShow()) { _AcquireRewardAnim.SetTrigger("Disappear"); return; }
        ShowRewardList();
    }

    private bool PrepareRewardListToShow()
    {
        if (setupParam.list_Reward.Count < 1) return false;
        else
        {
            var tmp = showingListNumIs00 ? rewardList_01 : rewardList_00;

            showingRewardList = new List<UIParam.Common.Reward>();
            for (int i = 0; i < setupParam.list_Reward.Count && i < (setupParam.thumbnail_Type == UIParam.Common.Reward.RewardItemType.CARD ? 5 : 10); i++)
                showingRewardList.Add(setupParam.list_Reward[i]);

            _ = setupParam.list_Reward.RemoveAll(r => showingRewardList.Contains(r));

            tmp.Set(setupParam.thumbnail_Type, showingRewardList);

            return true;
        }
    }

    public void ShowRewardList()
    {
        var tmp = showingListNumIs00 ? rewardList_01 : rewardList_00;
        var tmp2 = showingListNumIs00 ? rewardList_00 : rewardList_01;
        showingListNumIs00 = !showingListNumIs00;

        tmp.Show();
        if (!isFirstShow) tmp2.Out();
    }
}
