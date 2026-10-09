using System.Drawing;
using UnityEditor.TerrainTools;
using UnityEngine;

public class CCBaseColor : MonoBehaviour
{
    public float red = 1f;
    public float green = 0f;
    public float blue = 1f;
    public int matIndex = 0; //Check under mesh renderer > materials for index number
    public bool changeEmissive = false;
    //
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_BaseColor", new UnityEngine.Color(red,green, blue));
        if (changeEmissive)
        {
            mpb.SetColor("_EmissionColor", new UnityEngine.Color(red, green, blue));
        }
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(mpb, matIndex);
    }
}