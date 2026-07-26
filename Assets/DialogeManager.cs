using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class DialogeManager : MonoBehaviour
{
    [Serializable]
    private class DialogeContext
    {
        public string name;
        [TextArea(3,5)]
        public string context;

    }

    [SerializeField] private List<DialogeContext> list;

    [SerializeField] TMP_Text contextText;
    [SerializeField] TextEffect effect;
    [SerializeField] TMP_Text nameText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(func());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator func()
    {
        foreach(var item in list)
        {
            contextText.text = item.context;
            nameText.text = item.name;
            yield return StartCoroutine(effect.TypeEffect(0.05f));

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
    }

    
}
