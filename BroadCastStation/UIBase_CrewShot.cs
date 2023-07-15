using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UIBase_CrewShot : UIBase
{
    private bool isShowUI = true;
    private List<Material> materials = new List<Material>();

    public Image imgBg;
    public Image imgMark;
    public TextMeshProUGUI tmpuName;
    public Transform tfCrews;

    public GameObject goUI;

    public Button btnScreen;
    public Button btnBack;
    public Button btnHome;

    private void Start()
    {
        btnScreen.onClick.AddListener(OnClickScreen);
        btnBack.onClick.AddListener(OnClickBack);
        btnHome.onClick.AddListener(OnClickHome);
    }

    public override void Show(object param)
    {
        base.Show(param);

        isShowUI = true;
        goUI.SetActive(isShowUI);

        if (param is UIParam.BroadCast.BroadCastCrewShotSetupParam p)
        {
            for(int i = 0; i < tfCrews.childCount; i++)
            {
                tfCrews.GetChild(i).gameObject.SetActive(false);
            }

            Transform child = tfCrews.Find(p.crewData._Enum_Id);
            if(child != null)
            {
                child.gameObject.SetActive(true);
                Image[] imgCharacters = child.GetComponentsInChildren<Image>();
                for (int i = 0; i < p.crewData._Crew_Member_Data.Length; i++)
                {
                    if (imgCharacters.Length == i) break;

                    var member = p.crewData._Crew_Member_Data[i];
                    if (member != null)
                    {
                        bool isActive = false;
                        string characterEnumId = member._Enum_Id;
                        if (User._CharacterCollection.TryGetValue(characterEnumId, out CharacterCollectionInfo info))
                        {
                            isActive = info.Count > 0;
                        }
                        var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
                        if (character != null)
                        {
                            var resData = character._Resource_List_Data;
                            if (resData != null)
                            {
                                imgCharacters[i].gameObject.SetActive(true);
                                imgCharacters[i].sprite = resData._Night_LD_Reference_Data.GetSpriteFromSMTable(this);
                                Material mat = Instantiate(imgCharacters[i].material);
                                if (!isActive)
                                {
                                    mat.SetFloat("_Hue", 0f);
                                    mat.SetFloat("_Saturation", 0f);
                                    mat.SetFloat("_Brightness", -0.5f);
                                    mat.SetFloat("_Contrast", 0.5f);
                                }
                                imgCharacters[i].material = mat;
                                materials.Add(mat);
                            }
                        }
                    }
                }
            }

            if(p.crewData._Crew_Group_Photo_Reference_Data != null)
            {
                imgBg.sprite = p.crewData._Crew_Group_Photo_Reference_Data.GetSpriteFromSMTable(this);
            }

            if (p.crewData._Crew_Reference_Data != null)
            {
                imgMark.sprite = p.crewData._Crew_Reference_Data.GetSpriteFromSMTable(this);
            }

            tmpuName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, p.crewData._Enum_Id);
        }
    }

    public override void Hide()
    {
        foreach (var mat in materials)
        {
            Destroy(mat);
        }
        materials.Clear();
        base.Hide();
    }

    public void OnClickBack()
    {
        Hide();
    }

    public void OnClickHome()
    {
        UIManager._instance.HideAll();
    }

    public void OnClickScreen()
    {
        isShowUI = !isShowUI;
        goUI.SetActive(isShowUI);
    }
}
