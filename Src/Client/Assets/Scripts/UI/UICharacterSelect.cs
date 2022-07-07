using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
public class UICharacterSelect : MonoBehaviour {

    public GameObject panelCreate;
    public GameObject panelSelect;

    public GameObject btnCreateCancel;

    public InputField charName;
    CharacterClass charClass;

    public Transform uiCharList;
    public GameObject uiCharInfo;

    public List<GameObject> uiChars = new List<GameObject>();

    public Image[] titles;

    public Text descs;

    //public Text descs;


    public Text[] names;

    private int selectCharacterIdx = -1;

    public UICharacterView characterView;

    // Use this for initialization
    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
        DataManager.Instance.Load();
        OnSelectCharacter(0);


    }


    public void InitCharacterSelect(bool init)
    {
        panelCreate.SetActive(false);
        panelSelect.SetActive(true);

        if (init)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();
            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                GameObject go = Instantiate(uiCharInfo, this.uiCharList);
                UICharInfo charInfo = go.GetComponent<UICharInfo>();
                charInfo.info = User.Instance.Info.Player.Characters[i];

                Button button = go.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() =>
                {
                    OnSelectCharacter(idx);
                });

                uiChars.Add(go);
                go.SetActive(true);
            }

        }
    }

    public void InitCharacterCreate()
    {
        panelCreate.SetActive(true);
        panelSelect.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickCreate()
    {
        
        if (string.IsNullOrEmpty(this.charName.text))
        {
            
            MessageBox.Show("请输入角色名");
            return;
        }      
        UserService.Instance.SendCharacterCreate(this.charName.text,this.charClass);
        InitCharacterSelect(true);


    }

    public void OnSelectClass(int charClass)
    {
       this.charClass = (CharacterClass)charClass;

        characterView.CurrectCharacter = charClass - 1;
        UpdateTitle(charClass - 1);
        descs.text = DataManager.Instance.Characters[charClass].Description;
        for (int i = 0; i < 3; i++)
        {
            names[i].text = DataManager.Instance.Characters[i + 1].Name;

        }

    }



    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);

        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    public void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        //User.Instance.CurrentCharacter = cha;
        //characterView.CurrectCharacter = idx;
        characterView.CurrectCharacter = (System.Int32)cha.Class - 1;

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            ci.Seleceted = idx == i;
        }

    }

    public void UpdateTitle(int idx)
    {
        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == idx);
        }
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }

    
}
