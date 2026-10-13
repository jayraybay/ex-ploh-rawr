using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
    //public Material newMaterial = null;
    //public Texture newImage = null;
    //public bool setAlsoHeight = false;
    //TODO: make the texture change to be able to change to a different image
    private MaterialPropertyBlock mpb;
    //
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //if (newMaterial != null)
        //{
        //    gameObject.GetComponent<MeshRenderer>()
        //}
        mpb = new MaterialPropertyBlock();
        //if (newImage != null)
        //{
        //    Debug.Log("textruresd detected");
        //    mpb.SetTexture("_BaseMap", newImage);
        //    if (setAlsoHeight)
        //    {
        //        //mpb.SetTexture("_Texture", newImage);
        //    }
        //}
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