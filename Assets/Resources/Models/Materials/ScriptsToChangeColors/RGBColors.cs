using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class RGBColors : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int matIndex = 0;
    private MaterialPropertyBlock mpb;
    private float currentH;
    private float currentS;
    private float currentV;
    public float offset = 0f;
    public bool randomStarting = false;
    public bool changeEmissive = false;
    float speed = 1.0f;
    void Start()
    {
        mpb = new MaterialPropertyBlock();
        currentH = offset;
        if (randomStarting)
        {
            currentH = Random.Range(0, 360);
            currentH/= 360;
        }
        //Color ogColor = mpb.GetColor("_BaseColor");
        //mpb = GetComponent<MaterialPropertyBlock>();
        //gameObject.GetComponent <MeshRenderer>().GetPropertyBlock(mpb, matIndex);
        //Color.RGBToHSV(mpb.GetColor("_BaseColor"),out currentH, out currentS, out currentV);
        //Debug.Log(currentH +""+ currentS +""+currentV);
        //TODO: get already existing color so S and V values can be transfered over
    }

    // Update is called once per frame
    void Update()
    {
        currentH *= 360;
        if (currentH == 0)
        {
            currentH = 1;
        }
        else if(currentH >= 360)
        {
            currentH = 0;
        }
        else
        {
            currentH++;
        }
        currentH /= 360;
        Color nc = Color.HSVToRGB(currentH, 1, 1);
        mpb.SetColor("_BaseColor", nc);
        if (changeEmissive){
            mpb.SetColor("_EmissionColor", nc);
        }
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(mpb, matIndex);
    }
}
