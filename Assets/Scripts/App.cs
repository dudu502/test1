using GameSpace.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSpace.Core
{
    public class App : MonoBehaviour
    {
        public TextAsset m_ConfigJsonText;
        public Sprite[] m_AllModelSprites;
        public Item[] m_ItemControllers;
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
            string jsonText = m_ConfigJsonText.text;
            JToken obj = JsonConvert.DeserializeObject(jsonText) as JToken;

            JArray questionsArray = obj[0]["Activity"]["Questions"] as JArray;

            Questions = new ListIterator<JToken>(questionsArray.ToArray<JToken>());
            Questions.SetCurrentIndex(0);
            Debug.LogWarning(questionsArray);
            ShowCurrentQuestion();
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
                    int theFirstStep = Convert.ToInt32( answers[0][0].ToString());
                    Debug.LogWarning("The First step is "+theFirstStep);
                }
            }
        }



        // Update is called once per frame
        void Update()
        {

        }
    }
}
