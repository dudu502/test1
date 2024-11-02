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
        private Timer m_AnimationTimer;
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
            m_AnimationTimer = new Timer();
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

                    DrawItemRectEffect(m_ItemControllers[theFirstStep], SelectedAnswers.Count);
                    SelectedAnswers.Push(theFirstStep);
                    UpdateItemsVisible();
                }
            }
        }
        public Stack<int> SelectedAnswers = new Stack<int>();
        void DrawItemRectEffect(Item item, int numId)
        {
            if (numId == 0)
            {
                m_GameMaterial.SetVector("_RectPosition" + numId, new Vector4(item.PosUv.x, item.PosUv.y));
                m_GameMaterial.SetVector("_RectSize" + numId, new Vector4(0.07f, 0.09333f));
                m_GameMaterial.SetFloat("_RectRadii" + numId, 0.05f);
            }
            else
            {
                PlayRectEffect(item, numId);
            }
        }

        void PlayRectEffect(Item item, int numId)
        {
            StartCoroutine(PlayRectEffectCor(item));
        }
        const float animationTime = 0.4f;
        IEnumerator PlayRectEffectCor(Item item)
        {
            m_AnimationTimer.Reset();
            int currentSelectedAnsersCount = SelectedAnswers.Count;
            var currentPosUv = m_ItemControllers[SelectedAnswers.Peek()].PosUv;
            var toPosUv = item.PosUv;
            while (m_AnimationTimer <= animationTime)
            {
                m_GameMaterial.SetVector("_RectPosition" + currentSelectedAnsersCount, Vector4.Lerp(currentPosUv, toPosUv, m_AnimationTimer / animationTime));
                m_GameMaterial.SetVector("_RectSize" + currentSelectedAnsersCount, new Vector4(0.07f, 0.09333f));
                m_GameMaterial.SetFloat("_RectRadii" + currentSelectedAnsersCount, 0.05f);

                m_GameMaterial.SetVector("_RectPosition" + (currentSelectedAnsersCount - 1) + "_" + currentSelectedAnsersCount, Vector4.Lerp(currentPosUv, (toPosUv + currentPosUv) / 2, m_AnimationTimer / animationTime));
                m_GameMaterial.SetVector("_RectSize" + (currentSelectedAnsersCount - 1) + "_" + currentSelectedAnsersCount, new Vector4(0.07f, 0.09333f));
                m_GameMaterial.SetFloat("_RectRadii" + (currentSelectedAnsersCount - 1) + "_" + currentSelectedAnsersCount, 0.05f);

                yield return null;
            }

        }
        void OnClickModel(Item item)
        {
            if (!SelectedAnswers.Contains(item.Index))
            {
                DrawItemRectEffect(item, SelectedAnswers.Count);
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
