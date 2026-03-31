using System.Collections;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    private float lifetime = 1f;

    public void Setup(int value, Vector3 position, PopupType type)
    {
        textMeshPro.text = value.ToString();
        switch (type)
        {
            case PopupType.Damage:
                textMeshPro.text = value.ToString();
                textMeshPro.color = Color.red;
                break;
            case PopupType.Heal:
                textMeshPro.text = "+" + value.ToString();
                textMeshPro.color = Color.green;
                break;
            case PopupType.Buff:
                textMeshPro.text = "Buff!";
                textMeshPro.color = Color.blue;
                break;
            case PopupType.Debuff:
                textMeshPro.text = "Debuff!";
                textMeshPro.color = Color.yellow;
                break;
        }
        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        float elapsed = 0;
        Vector3 startPos = transform.position;
        Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 1.5f, 0); 

        while (elapsed < lifetime)
        {
            transform.position = Vector3.Lerp(startPos, startPos + offset, elapsed / lifetime);
            
            Color c = textMeshPro.color;
            c.a = Mathf.Lerp(1, 0, elapsed / lifetime);
            textMeshPro.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
