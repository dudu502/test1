using Newtonsoft.Json.Linq;
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
        public Image ImgModel;
        JToken m_OpInfo;
        void Start()
        {

        }
        public void SetOpInfo(JToken info)
        {
            m_OpInfo = info;
            ImgModel.sprite = App.Instance.FindSprite(info["image"]["sha1"].ToString());
            var rectTrans = GetComponent<RectTransform>();
            
            Debug.LogWarning("Index:"+Index+" Rect:"+GetComponent<RectTransform>().rect);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}