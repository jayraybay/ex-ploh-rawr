using UnityEditor.TerrainTools;
using UnityEngine;

public class CCBaseColor : MonoBehaviour
{
    public int matIndex = 0; //Check under mesh renderer > materials for index number
    public Color newColor = new Color(1, 0, 1);
    public bool realTimeChange;
    public bool rgbMode = false;
    public bool randomRGBStart = false;
    private float currentH;
    private float currentS;
    private float currentV;
    private MaterialPropertyBlock mpb;
    //
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mpb = new MaterialPropertyBlock();
        changeCol(newColor);
        Color.RGBToHSV(newColor, out currentH, out currentS, out currentV);
        if (randomRGBStart)
        {
            currentH = Random.Range(0, 360);
            currentH /= 360;
        }
    }
    private void Update()
    {
        if (realTimeChange&&(!rgbMode))
        {
            changeCol(newColor);
        }
        if (rgbMode)
        {
            currentH *= 360;
            if (currentH == 0)
            {
                currentH = 1;
            }
            else if (currentH >= 360)
            {
                currentH = 0;
            }
            else
            {
                currentH++;
            }
            currentH /= 360;
            newColor = Color.HSVToRGB(currentH, currentS, currentV);
            changeCol(newColor);   
        }
    }
    void changeCol(Color c)
    {
        mpb.SetColor("_BaseColor", c);
        mpb.SetColor("_EmissionColor", c);
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(mpb, matIndex);
    }
}