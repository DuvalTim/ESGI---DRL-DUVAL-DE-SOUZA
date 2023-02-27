using UnityEngine;
using UnityEngine.UI;

public class CaseDisplay
{
    public GameObject GameObject;
    public RawImage RawImage;
    public Text Text;

    public CaseDisplay(GameObject obj, RawImage rawImage, Text text)
    {
        GameObject = obj;
        RawImage = rawImage;
        Text = text;
    }

    internal void SetActive(bool v)
    {
        GameObject.SetActive(v);
    }

    internal void SetColor(Color color)
    {
        RawImage.color = color;
    }

    internal void SetName(string name)
    {
        GameObject.name = name;
    }

    internal void SetPosition(Vector3 vector3)
    {
        GameObject.GetComponent<RectTransform>().localPosition = vector3;
    }

    internal void SetText(string v)
    {
        Text.text = v;
    }
}
