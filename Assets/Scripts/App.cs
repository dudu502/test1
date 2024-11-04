using GameSpace.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
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
        public Button m_NextBtn;
        public Button m_ResetBtn;
        private Timer m_AnimationTimer;
        private ListIterator<JToken> Questions;
        private JArray m_CurrentAnswers;
        private Stack<int> m_UserSelectedAnswers = new Stack<int>(); 
        private float rect_w => 0.068f * 800 / Screen.width;
        private float rect_h => rect_w * Screen.width / Screen.height;
        private float rect_round => 0.05f * 800 / Screen.width;

        private int m_wrong_count = 0;
        public static App Instance
        {
            set;get;
        }
        void Awake()
        {
            Instance = this;
            Debug.LogWarning($"Screen Size {Screen.width} {Screen.height}");
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

        public void OnClickReset()
        {
            if (CanReset())
            {
                PlayUndoEffect(0,null);
            }
        }

        /// <summary>
        /// The index of selected answer array.
        /// </summary>
        /// <param name="toNumberIndex"></param>
        private void PlayUndoEffect(int toNumberIndex,Action complete)
        {
            StartCoroutine(PlayUndoEffectCor(toNumberIndex, complete));
        }

        public int GetAnswersCount()
        {
            return ((JArray)m_CurrentAnswers[0]).Count;
        }

        public void OnClickNext()
        {
            if (CanCheckAnswer())
            {
                var userSelected = m_UserSelectedAnswers.ToList();
                userSelected.Reverse();
                JArray answers = m_CurrentAnswers[0] as JArray;  
                int wrongIndex = -1;
                for (int i = 0; i < answers.Count; ++i)
                {
                    if(answers[i].ToString() != userSelected[i].ToString())
                    {

                        wrongIndex = i;
                        break;
                    }
                }

                // wrong answers!....
                if (wrongIndex > -1)
                {
                    m_wrong_count++;
                    Debug.LogWarning("Wrong Index "+wrongIndex);
                    for (int j = wrongIndex; j < userSelected.Count; ++j)
                    {
                        m_ItemControllers[userSelected[j]].PlayModelWrongEffect();
                    }

                    PlayUndoEffect(wrongIndex-1, () => {
                        if (m_wrong_count >= 2)
                        {
                            // Auto show the correct answers!
                            ShowCorrectAnswers();
                        }
                    });
                }
                // Move to next question.
            }
        }

        private void ShowCorrectAnswers()
        {
            StartCoroutine(ShowCorrectAnswersCor());
        }

        IEnumerator ShowCorrectAnswersCor()
        {
            JArray answers = m_CurrentAnswers[0] as JArray;
            for (int i = 0; i < answers.Count; ++i)
            {
                yield return new WaitForSeconds(0.5f);
                OnClickModel(m_ItemControllers[Convert.ToInt32(answers[i].ToString())]);
                yield return new WaitForSeconds(0.5f);
            }
        }
        public Sprite FindSprite(string name)
        {
            foreach (var sprite in m_AllModelSprites)
                if (sprite.name == name)
                    return sprite;
            return null;
        }

        private bool CanReset()
        {
            return m_UserSelectedAnswers.Count > 1;
        }

        private void UpdateResetButtonStatus()
        {
            m_ResetBtn.interactable = CanReset();
        }

        private void UpdateNextButtonStatus()
        {
            if (CanCheckAnswer())
            {
                m_NextBtn.GetComponentInChildren<TMPro.TMP_Text>().text = "Next";
                m_NextBtn.interactable = true;
            }
            else
            {
                m_NextBtn.GetComponentInChildren<TMPro.TMP_Text>().text = $"{m_UserSelectedAnswers.Count}/{((JArray)m_CurrentAnswers[0]).Count}";
                m_NextBtn.interactable = false;
            }
        }

        private bool CanCheckAnswer()
        {
            return m_UserSelectedAnswers.Count == ((JArray)m_CurrentAnswers[0]).Count;
        }
        private void ShowCurrentQuestion()
        {
            if (Questions.GetCurrent() != null)
            {
                JToken question = Questions.GetCurrent();

                if (question != null)
                {
                    m_CurrentAnswers = question["Body"]["answers"] as JArray;
                    JArray options = question["Body"]["options"] as JArray;
                    if (options != null)
                    {
                        for(int i = 0; i < options.Count; ++i)
                        {
                            m_ItemControllers[i].SetOpInfo(options[i]);
                        }
                    }
                    int theFirstStep = Convert.ToInt32(m_CurrentAnswers[0][0].ToString());
                    Debug.LogWarning("The First step is "+theFirstStep);

                    DrawItemRectEffect(m_ItemControllers[theFirstStep], m_UserSelectedAnswers.Count);
                    m_UserSelectedAnswers.Push(theFirstStep);
                    UpdateItemsVisible();
                    UpdateNextButtonStatus();
                    UpdateResetButtonStatus();
                }
            }
        }

        void DrawItemRectEffect(Item item, int numId)
        {
            if (numId == 0)
            {
                m_GameMaterial.SetVector("_RectPosition" + numId, new Vector4(item.PosUv.x, item.PosUv.y));
                m_GameMaterial.SetVector("_RectSize" + numId, new Vector4(rect_w, rect_h));
                m_GameMaterial.SetFloat("_RectRadii" + numId, rect_round);

                item.ImgBgDissolve.location = 1;
            }
            else
            {
                PlayRectEffect(item, numId);
            }
        }

        void PlayRectEffect(Item item, int numId)
        {
            StartCoroutine(PlayAdvanceRectEffectCor(item));
        }
        const float animationTime = 0.4f;
        IEnumerator PlayAdvanceRectEffectCor(Item item)
        {
            m_AnimationTimer.Reset();
            int currentSelectedAnswersCount = m_UserSelectedAnswers.Count;
            var currentPosUv = m_ItemControllers[m_UserSelectedAnswers.Peek()].PosUv;
            var toPosUv = item.PosUv;
            while (m_AnimationTimer <= animationTime)
            {
                m_GameMaterial.SetVector("_RectPosition" + currentSelectedAnswersCount, Vector4.Lerp(currentPosUv, toPosUv, m_AnimationTimer / animationTime));
                m_GameMaterial.SetVector("_RectSize" + currentSelectedAnswersCount, new Vector4(rect_w, rect_h));
                m_GameMaterial.SetFloat("_RectRadii" + currentSelectedAnswersCount, rect_round);

                m_GameMaterial.SetVector("_RectPosition" + (currentSelectedAnswersCount - 1) + "_" + currentSelectedAnswersCount, Vector4.Lerp(currentPosUv, (toPosUv + currentPosUv) / 2, m_AnimationTimer / animationTime));
                m_GameMaterial.SetVector("_RectSize" + (currentSelectedAnswersCount - 1) + "_" + currentSelectedAnswersCount, new Vector4(rect_w, rect_h));
                m_GameMaterial.SetFloat("_RectRadii" + (currentSelectedAnswersCount - 1) + "_" + currentSelectedAnswersCount, rect_round);

                item.ImgBgDissolve.location = Mathf.Lerp(0,1f,m_AnimationTimer/animationTime);
                yield return null;
            }
            m_GameMaterial.SetVector("_RectPosition" + currentSelectedAnswersCount, toPosUv);
            m_GameMaterial.SetVector("_RectSize" + currentSelectedAnswersCount, new Vector4(rect_w, rect_h));
            m_GameMaterial.SetFloat("_RectRadii" + currentSelectedAnswersCount, rect_round);

            m_GameMaterial.SetVector("_RectPosition" + (currentSelectedAnswersCount - 1) + "_" + currentSelectedAnswersCount, (toPosUv + currentPosUv) / 2f);
            m_GameMaterial.SetVector("_RectSize" + (currentSelectedAnswersCount - 1) + "_" + currentSelectedAnswersCount, new Vector4(rect_w, rect_h));
            m_GameMaterial.SetFloat("_RectRadii" + (currentSelectedAnswersCount - 1) + "_" + currentSelectedAnswersCount, rect_round);

            item.ImgBgDissolve.location = 1;
        }
        IEnumerator PlayUndoEffectCor(int toNumberIndex, Action complete)
        {
            int currentSelectedAnswersIndex = m_UserSelectedAnswers.Count - 1;
            while (currentSelectedAnswersIndex > toNumberIndex)
            {

                m_AnimationTimer.Reset();
                var currentItem = m_ItemControllers[m_UserSelectedAnswers.Peek()];
                m_UserSelectedAnswers.Pop();
                var toItem = m_ItemControllers[m_UserSelectedAnswers.Peek()];


                while (m_AnimationTimer <= animationTime)
                {
                    m_GameMaterial.SetVector("_RectPosition" + currentSelectedAnswersIndex, Vector4.Lerp(currentItem.PosUv, toItem.PosUv, m_AnimationTimer / animationTime));
                    m_GameMaterial.SetVector("_RectSize" + currentSelectedAnswersIndex, new Vector4(rect_w, rect_h));
                    m_GameMaterial.SetFloat("_RectRadii" + currentSelectedAnswersIndex, rect_round);

                    m_GameMaterial.SetVector("_RectPosition" + (currentSelectedAnswersIndex - 1) + "_" + currentSelectedAnswersIndex, Vector4.Lerp((toItem.PosUv + currentItem.PosUv) / 2, toItem.PosUv, m_AnimationTimer / animationTime));
                    m_GameMaterial.SetVector("_RectSize" + (currentSelectedAnswersIndex - 1) + "_" + currentSelectedAnswersIndex, new Vector4(rect_w, rect_h));
                    m_GameMaterial.SetFloat("_RectRadii" + (currentSelectedAnswersIndex - 1) + "_" + currentSelectedAnswersIndex, rect_round);

                    currentItem.ImgBgDissolve.location = Mathf.Lerp(1, 0, m_AnimationTimer / animationTime);
                    yield return null;
                }
                m_GameMaterial.SetVector("_RectPosition" + currentSelectedAnswersIndex, new Vector4());
                m_GameMaterial.SetVector("_RectSize" + currentSelectedAnswersIndex, new Vector4());
                m_GameMaterial.SetFloat("_RectRadii" + currentSelectedAnswersIndex, rect_round);

                m_GameMaterial.SetVector("_RectPosition" + (currentSelectedAnswersIndex - 1) + "_" + currentSelectedAnswersIndex, new Vector4());
                m_GameMaterial.SetVector("_RectSize" + (currentSelectedAnswersIndex - 1) + "_" + currentSelectedAnswersIndex, new Vector4());
                m_GameMaterial.SetFloat("_RectRadii" + (currentSelectedAnswersIndex - 1) + "_" + currentSelectedAnswersIndex, rect_round);
                currentItem.ImgBgDissolve.location = 0;
                currentSelectedAnswersIndex--; 
                UpdateItemsVisible();
            }
            UpdateItemsVisible();
            UpdateNextButtonStatus();
            UpdateResetButtonStatus();
            complete?.Invoke();
        }
        void OnClickModel(Item item)
        {
            if (!m_UserSelectedAnswers.Contains(item.Index)&&m_UserSelectedAnswers.Count < GetAnswersCount())
            {
                item.PlayModelPressed();
                DrawItemRectEffect(item, m_UserSelectedAnswers.Count);
                m_UserSelectedAnswers.Push(item.Index);
                UpdateItemsVisible();
                UpdateNextButtonStatus();
                UpdateResetButtonStatus();
            }
        }
        void UpdateItemsVisible()
        {
            if (m_UserSelectedAnswers.Count < GetAnswersCount()+1)
            {
                Item peekItem = m_ItemControllers[m_UserSelectedAnswers.Peek()];
                List<int> neibors = Utils.GetNeibors(peekItem.GetRowIndex() - 1, peekItem.GetColIndex() - 1);
                foreach (var item in m_ItemControllers)
                {
                    bool isInSelected = m_UserSelectedAnswers.Contains(item.Index);
                    bool isInNeibor = neibors.Contains(item.Index);
                    item.HideModelShadow();
                  
                    if (isInSelected || isInNeibor)
                    {
                        if(m_UserSelectedAnswers.Count<5)
                            item.PlayModelFadeIn();
                        //else
                        //    item.PlayModelFadeOut();
                        if (isInNeibor && !isInSelected)
                        {           
                            item.ShowModelShadow();
                        }
                 
                    }
                    else
                    {
                        item.PlayModelFadeOut();
                    }
                }
            }

            foreach(var itemId in m_UserSelectedAnswers)
            {
                m_ItemControllers[itemId].ShowModelOutline();
            }

        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
