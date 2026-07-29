using UnityEngine;

using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TextEffect : MonoBehaviour
{
    private TMP_Text textComponent;
    
    

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            EffectData effect = GetEffectForCharacter(textInfo, i);
            if (effect == null) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 offset = Vector3.zero;

            if (effect.type == "wave")
            {
                float speed = effect.GetFloat("speed", 5f);
                float height = effect.GetFloat("height", 5f);
                offset.y = Mathf.Sin(Time.time * speed + i * 0.5f) * height;
            }
            else if (effect.type == "shake")
            {
                float amount = effect.GetFloat("amount", 2f);
                offset.x += Random.Range(-amount, amount);
                offset.y += Random.Range(-amount, amount);
            }

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    // 효과 이름 + 파라미터를 담는 클래스
    class EffectData
    {
        public string type;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();

        public float GetFloat(string key, float defaultValue)
        {
            if (parameters.TryGetValue(key, out string value) && float.TryParse(value, out float result))
                return result;
            return defaultValue;
        }
    }

    // "wave:speed=8,height=15" 같은 문자열을 파싱
    EffectData ParseLinkID(string linkID)
    {
        EffectData data = new EffectData();
        string[] mainParts = linkID.Split(':');
        data.type = mainParts[0];

        if (mainParts.Length > 1)
        {
            string[] paramPairs = mainParts[1].Split(',');
            foreach (string pair in paramPairs)
            {
                string[] kv = pair.Split('=');
                if (kv.Length == 2)
                    data.parameters[kv[0]] = kv[1];
            }
        }

        return data;
    }

    EffectData GetEffectForCharacter(TMP_TextInfo textInfo, int charIndex)
    {
        for (int l = 0; l < textInfo.linkCount; l++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[l];
            if (charIndex >= linkInfo.linkTextfirstCharacterIndex &&
                charIndex < linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength)
            {
                return ParseLinkID(linkInfo.GetLinkID());
            }
        }
        return null;
    }

    public IEnumerator TypeEffect(float time)
    {
        textComponent.maxVisibleCharacters = 0;
        textComponent.ForceMeshUpdate();
        
        
        int totalVisible = textComponent.textInfo.characterCount;

        for(int i = 0; i <= totalVisible; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(time);

        }
        
        
    }

    
}
