using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AcquireReward_ItemList : MonoBehaviour
{
    #region Inspector
    [Header("아이템 프리팹 OLD")]
    //[SerializeField] private UI_Item_Character_Base prefab_CharItem;
    //[SerializeField] private UI_Item_Consumable_Base prefab_ConsumItem;
    //[SerializeField] private UI_Item_StarGem_Base prefab_StarGem;
    //[SerializeField] private UI_Item_Asset_Base prefab_Asset;

    [Header("아이템 프리팹 NEW")]
    [SerializeField] private UI_Thumbnail_Character_Basic_Old thumb_Char;
    [SerializeField] private UI_Thumbnail_Consum_Basic thumb_Consum;
    [SerializeField] private UI_Thumbnail_Asset_Basic_Old thumb_Asset;
    [SerializeField] private UI_Thumbnail_Integrate_Old thumb_Integrate;

    [Header("카드 프리팹")]
    //[SerializeField] private UI_Card_Base_Integration prefab_IntegrationCard;


    [Header("획득 보상 UI 그리드 트랜스폼")]
    [SerializeField] private Transform tf_ItemGrid;
    [SerializeField] private Transform tf_CardGrid;
    #endregion


    #region Variables
    private Animator _ListAnim => GetComponent<Animator>();
    #endregion

    private void Awake()
    {
        if (thumb_Char != null && thumb_Char.gameObject.activeSelf)
            thumb_Char.gameObject.SetActive(false);
        if (thumb_Consum != null && thumb_Consum.gameObject.activeSelf)
            thumb_Consum.gameObject.SetActive(false);
        if (thumb_Asset != null && thumb_Asset.gameObject.activeSelf)
            thumb_Asset.gameObject.SetActive(false);

        //if (prefab_CharItem != null && prefab_CharItem.gameObject.activeSelf) prefab_CharItem.gameObject.SetActive(false);
        //if (prefab_ConsumItem != null && prefab_ConsumItem.gameObject.activeSelf) prefab_ConsumItem.gameObject.SetActive(false);
        //if (prefab_StarGem != null && prefab_StarGem.gameObject.activeSelf) prefab_StarGem.gameObject.SetActive(false);
        //if (prefab_Asset != null && prefab_Asset.gameObject.activeSelf) prefab_Asset.gameObject.SetActive(false);
        //if (prefab_IntegrationCard != null && prefab_IntegrationCard.gameObject.activeSelf) prefab_IntegrationCard.gameObject.SetActive(false);
    }

    // *보상 획득 연출을 빠르게 스킵할 경우 → 전에 스킵했던 보상들이 한 번에 연출되는 문제*
    // -> 창을 너무 빠르게 닫을 때 이쪽이 호출이 안되고 있음.
    // -> 임시로 Set 하는 시점에 child 오브젝트들 삭제처리.
    //private void OnDisable()
    //{
    //    List<GameObject> list_Destroy = new List<GameObject>();
    //    if (tf_ItemGrid != null) foreach (Transform child in tf_ItemGrid) list_Destroy.Add(child.gameObject);
    //    if (tf_CardGrid != null) foreach (Transform child in tf_CardGrid) list_Destroy.Add(child.gameObject);

    //    for (int i = list_Destroy.Count - 1; i >= 0; i--)
    //        Destroy(list_Destroy[i]);
    //}

    //카드면 1 / Integrate면 2 / 둘다 아닐시 0
    public void Set(UIParam.Common.Reward.RewardItemType thumbnail_Type, params UIParam.Common.Reward[] rewards)
    {
        if (tf_ItemGrid != null) tf_ItemGrid.gameObject.SetActive(thumbnail_Type != UIParam.Common.Reward.RewardItemType.CARD);
        if (tf_CardGrid != null) tf_CardGrid.gameObject.SetActive(thumbnail_Type == UIParam.Common.Reward.RewardItemType.CARD);

        Transform rewardParent = (thumbnail_Type == UIParam.Common.Reward.RewardItemType.CARD) ? tf_CardGrid : tf_ItemGrid;
        if (rewardParent == null) return;

        // child 오브젝트들 삭제처리.
        List<GameObject> list_Destroy = new List<GameObject>();
        foreach (Transform child in rewardParent) list_Destroy.Add(child.gameObject);
        for (int i = list_Destroy.Count - 1; i >= 0; i--)
        {
            Destroy(list_Destroy[i]);
        }

        foreach (var reward in rewards)
        {
            if (reward == null) continue;

            if (thumbnail_Type == UIParam.Common.Reward.RewardItemType.NONE)
                switch (reward.type)
                {
                    case Data.Enum.Common_Type.CHARACTER:
                        {
                            //if (prefab_CharItem == null) break;

                            //var obj = Instantiate(prefab_CharItem, rewardParent);
                            //if (obj != null)
                            //    obj.Set(reward.key, 1);
                            if (thumb_Char == null) break;
                            var inst = Instantiate(thumb_Char, rewardParent);
                            var data = Data._characterTable.GetDataFromTable(reward.key);
                            inst.SetData(data);
                            inst.SetActive(true);
                        }
                        break;
                    case Data.Enum.Common_Type.ITEM:
                        {
                            //if (prefab_ConsumItem == null) break;

                            //var obj = Instantiate(prefab_ConsumItem, rewardParent);
                            //if (obj != null)
                            //    obj.Set(reward.key, reward.value);
                            if (thumb_Consum == null) break;
                            var inst = Instantiate(thumb_Consum, rewardParent);
                            var data = Data._itemTable.GetDataFromTable(reward.key);
                            inst.SetData(data);
                            inst.SetActive(true);
                        }
                        break;

                    //스타잼
                    //case Data.Enum.Common_Type.STARGEM:
                    //    {
                    //        if (prefab_StarGem == null) break;

                    //        var obj = Instantiate(prefab_StarGem, rewardParent);
                    //        if (obj != null)
                    //            obj.Set(reward.key, reward.value);
                    //    }
                    //    break;

                    case Data.Enum.Common_Type.ASSET:
                        {
                            //if (prefab_Asset == null) break;

                            //var obj = Instantiate(prefab_Asset, rewardParent);
                            //if (obj != null) obj.Set(reward.key, reward.value);

                            if (thumb_Asset == null) break;
                            var inst = Instantiate(thumb_Asset, rewardParent);
                            var data = Data._assetTable.GetDataFromTable(reward.key);
                            inst.SetData(data);
                            inst.SetActive(true);
                        }
                        break;
                }
            else if (thumbnail_Type == UIParam.Common.Reward.RewardItemType.INTEGRATE)
            {

                if (thumb_Integrate == null) break;
                var inst = Instantiate(thumb_Integrate, rewardParent);
                var data = Data._assetTable.GetDataFromTable(reward.key);
                inst.SetData(new MailInfo.MailInfoReward(reward.key, reward.type.ToString(), reward.value), 0);
                inst.gameObject.SetActive(true);
            }

            //UI_Card_Integration 카드
            //else
            //{
            //    if (prefab_IntegrationCard == null) break;

            //    var obj = Instantiate(prefab_IntegrationCard, rewardParent);
            //    if (obj != null)
            //    {
            //        obj.Setup(reward);
            //        obj.Show();
            //    }
            //}
        }
    }
    public void Set(UIParam.Common.Reward.RewardItemType thumbnail_Type, List<UIParam.Common.Reward> list_Reward) => Set(thumbnail_Type, list_Reward.ToArray());

    public void Show()
    {
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
        if (_ListAnim != null) _ListAnim.SetBool("Show", true);
    }

    public void Out()
    {
        if (_ListAnim != null) _ListAnim.SetBool("Show", false);
    }

    public void Deactive()
    {
        gameObject.SetActive(false);
    }

    public void ResetList()
    {
        List<GameObject> list_Destroy = new List<GameObject>();
        if (tf_ItemGrid != null) foreach (Transform child in tf_ItemGrid) list_Destroy.Add(child.gameObject);
        if (tf_CardGrid != null) foreach (Transform child in tf_CardGrid) list_Destroy.Add(child.gameObject);

        for (int i = list_Destroy.Count - 1; i >= 0; i--)
        {
            Debug.Log(13);
            Destroy(list_Destroy[i]);
        }
    }
}
