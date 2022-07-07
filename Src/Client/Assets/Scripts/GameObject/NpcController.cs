using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using System;
using Models;
/// <summary>
/// NPC控制器，主要负责处理角色点击NPC后的后续动作及操作，提升用户体验，实际交互调用NPCManager内Interactive函数
/// </summary>
public class NpcController : MonoBehaviour {

    public int npcID;
    private SkinnedMeshRenderer renderer;//因为是骨骼，所以获取其皮肤渲染器
    public Animator anim;
    Color orignColor;//材质原始颜色

    private bool inInteractive = false;//判断是否正在交互

    NpcDefine npc;
	// Use this for initialization
	void Start () {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponent<Animator>();
        orignColor = renderer.sharedMaterial.color;
        npc = NPCManager.Instance.GetNpcDefine(npcID);

	}
	
    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive)
                yield return new WaitForSeconds(2f);//交互中，每次等待2s
            else
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f,10f));//未在交互中，每次等待5至10s；

            this.Relax();
        }
    }

    

    // Update is called once per frame
    void Update () {
		
	}

    private void Relax()
    {
        anim.SetTrigger("Relax");
    }

    void Interaction()
    {
        if (!inInteractive)//防止连点
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());

        }
    }

    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();//朝向玩家
        if (NPCManager.Instance.Interactive(npc))//执行实际交互
        {
            anim.SetTrigger("Talk");//交互成功才执行动画
        }
        yield return new WaitForSeconds(3f);
        inInteractive = false;//3s后恢复可交互
    }

    IEnumerator FaceToPlayer()//朝向玩家函数
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward,faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward,faceTo,Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        Interaction();
    }

    private void OnMouseOver()
    {
        Highlight(true);
    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }
    private void Highlight(bool highlight)//高光函数，用来标识鼠标进入与离开，提升用户体验
    {
        if (highlight)
        {
            if (renderer.sharedMaterial.color != Color.white)
                renderer.sharedMaterial.color = Color.white;
        }
        else
        {
            if (renderer.sharedMaterial.color != orignColor)
                renderer.sharedMaterial.color = orignColor;
        }
    }
}
