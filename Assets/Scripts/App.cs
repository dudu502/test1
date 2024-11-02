using GameSpace.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameSpace.Core
{
    public class App : MonoBehaviour
    {
        public TextAsset m_ConfigJsonText;
        public Sprite[] m_AllModelSprites;
        public Item[] m_ItemControllers;
        public GameObject m_GameEnterPointObj;
        public Material m_GameMaterial;
        public Shader m_GameShader;
        private ListIterator<JToken> Questions;
        public static App Instance
        {
            set;get;
        }
        void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            m_GameMaterial = m_GameEnterPointObj.GetComponent<Image>().material;
            Utils.ClearAllMatEffect(m_GameMaterial);
            AddEvents();
            string jsonText = m_ConfigJsonText.text;
            JToken obj = JsonConvert.DeserializeObject(jsonText) as JToken;

            JArray questionsArray = obj[0]["Activity"]["Questions"] as JArray;

            Questions = new ListIterator<JToken>(questionsArray.ToArray<JToken>());
            Questions.SetCurrentIndex(0);
            Debug.LogWarning(questionsArray);
            ShowCurrentQuestion();
        }
        private void OnDestroy()
        {
            Utils.ClearAllMatEffect(m_GameMaterial);
        }
        void AddEvents()
        {
            EventDispatcher<ItemEvent, Item>.AddListener(ItemEvent.ClickModel, OnClickModel);
        }
        public Sprite FindSprite(string name)
        {
            foreach (var sprite in m_AllModelSprites)
                if (sprite.name == name)
                    return sprite;
            return null;
        }
        
        private void ShowCurrentQuestion()
        {
            if (Questions.GetCurrent() != null)
            {
                JToken question = Questions.GetCurrent();

                if (question != null)
                {
                    JArray answers = question["Body"]["answers"] as JArray;
                    JArray options = question["Body"]["options"] as JArray;
                    if (options != null)
                    {
                        for(int i = 0; i < options.Count; ++i)
                        {
                            m_ItemControllers[i].SetOpInfo(options[i]);
                        }
                    }
                    int theFirstStep = Convert.ToInt32(answers[0][0].ToString());
                    Debug.LogWarning("The First step is "+theFirstStep);

                    DrawItemRectEffect(m_ItemControllers[theFirstStep].PosUv, SelectedAnswers.Count);
                    SelectedAnswers.Push(theFirstStep);
                    UpdateItemsVisible();
                }
            }
        }
        public Stack<int> SelectedAnswers = new Stack<int>();
        void DrawItemRectEffect(Vector2 posUv,int numId)
        {
            m_GameMaterial.SetVector("_RectPosition"+ numId, new Vector4(posUv.x,posUv.y));
            m_GameMaterial.SetVector("_RectSize"+ numId, new Vector4(0.07f, 0.09f));
            m_GameMaterial.SetFloat("_RectRadii"+ numId,0.05f);
        }
        void OnClickModel(Item item)
        {
            if (!SelectedAnswers.Contains(item.Index))
            {
                DrawItemRectEffect(item.PosUv, SelectedAnswers.Count);
                SelectedAnswers.Push(item.Index);
                UpdateItemsVisible();
            }
        }
        void UpdateItemsVisible()
        {
            foreach(var item in m_ItemControllers)
            {
                item.SetImageModelAlpha(.5f);
            }
            foreach(var stacked in SelectedAnswers)
            {
                m_ItemControllers[stacked].SetImageBgAlpha(0);
                m_ItemControllers[stacked].SetImageModelAlpha(1);
            }
            if(SelectedAnswers.TryPeek(out int id))
            {
                Item peekItem = m_ItemControllers[id];
                peekItem.SetImageModelAlpha(1);

                List<int> neibors =  Utils.GetNeibors(peekItem.GetRowIndex()-1, peekItem.GetColIndex()-1);
                foreach(var neiborIndex in neibors)
                {
                    m_ItemControllers[neiborIndex].SetImageModelAlpha(1);
                }
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
