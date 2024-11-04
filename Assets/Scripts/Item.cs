using Coffee.UIExtensions;
using Newtonsoft.Json.Linq;
using nickeltin.SDF.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameSpace.Core
{
    public class Item : MonoBehaviour
    {
        public int Index;
        public Image ImgBg;
        public SDFImage ImgModel;
        public Animator ImgModelAnimator;
        public UIDissolve ImgBgDissolve;
        public CanvasGroup ImgModelCanvas;
        JToken m_OpInfo;
        public Vector2 PosUv;

        void Awake()
        {
        }
        void Start()
        {
            ImgModel.RenderOutline = false;
            ImgModel.RenderShadow = false;


        }
        
        public void SetImageModelAlpha(float a)
        {
            var color = ImgModel.color;
            color.a = a;
            ImgModel.color = color;
        }
        public void ShowModelShadow()
        {
            ImgModel.RenderShadow = true;
        }
        public void HideModelShadow()
        {
            ImgModel.RenderShadow = false;
        }
        public void ShowModelOutline()
        {
            ImgModel.RenderOutline = true;
        }
        public void HideModelOutline()
        {
            ImgModel.RenderOutline= false;
        }
        public void PlayModelPressed()
        {
            ImgModelAnimator.Play("Pressed");
        }
        public void PlayModelFadeIn()
        {
            ImgModelAnimator.Play("FadeIn");
        }
        public void PlayModelFadeOut()
        {
            ImgModelAnimator.Play("FadeOut");
        }
        public void PlayModelWrongEffect()
        {
            ImgModelAnimator.Play("Wrong");
        }
        public void SetImageBgAlpha(float a)
        {
            var color = ImgBg.color;
            color.a = a;
            ImgBg.color = color;
        }
        public void OnClickModel()
        {
            if(ImgModel.color.a==1)
            {
                Debug.LogWarning("OnClick Model "+Index);
                EventDispatcher<ItemEvent, Item>.DispatchEvent(ItemEvent.ClickModel, this);
            }
        }
        public void SetOpInfo(JToken info)
        {
            m_OpInfo = info;
            var imgSprite = App.Instance.FindSprite(info["image"]["sha1"].ToString());
            ImgModel.SDFSpriteReference = imgSprite;

            var rectTrans = GetComponent<RectTransform>();
            
            Debug.LogWarning("Index:"+Index+" Rect:"+GetComponent<RectTransform>().rect + "Row "+GetRowIndex()+" Col "+GetColIndex());

            Debug.LogWarning("ToScreen Pos "+RectTransformUtility.WorldToScreenPoint(null,transform.position)+"  Screen "+Screen.width+" "+Screen.height);

            var screenPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);
            PosUv = new Vector2(screenPos.x/Screen.width,screenPos.y/Screen.height);
        }

        public int GetRowIndex()
        {
            return Convert.ToInt32(m_OpInfo["rowIndex"]);
        }
        public int GetColIndex()
        {
            return Convert.ToInt32(m_OpInfo["colIndex"]);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}