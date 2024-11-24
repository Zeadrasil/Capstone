using UnityEngine;
//Allows you to fade various different types of renderers such that it will be transparent in a given number of seconds
public class FadingRenderer : MonoBehaviour
{
    //Renderer reference type storage
    public Renderer renderer;
    private LineRenderer lineReference;
    private SpriteRenderer spriteReference;

    //Fading details
    public float fadeDuration;
    private float[] initialOpacity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Check if it is a line renderer
        lineReference = renderer as LineRenderer;
        if(lineReference != null)
        {
            initialOpacity = new float[] { lineReference.startColor.a, lineReference.endColor.a };
        }
        else
        {
            //Check if it is a sprite renderer
            spriteReference = renderer as SpriteRenderer;
            if (spriteReference != null)
            {
                initialOpacity = new float[] { spriteReference.color.a };
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Fading for line renderers
        if(lineReference != null)
        {
            lineReference.startColor = new Color(lineReference.startColor.r, lineReference.startColor.g, lineReference.startColor.b, lineReference.startColor.a - initialOpacity[0] * Time.deltaTime / fadeDuration);
            lineReference.endColor = new Color(lineReference.endColor.r, lineReference.endColor.g, lineReference.endColor.b, lineReference.endColor.a - initialOpacity[1] * Time.deltaTime / fadeDuration);
        }
        //Fading for sprite renderers
        else if(spriteReference != null)
        {
            spriteReference.color = new Color(spriteReference.color.r, spriteReference.color.g, spriteReference.color.b, spriteReference.color.a - initialOpacity[0] * Time.deltaTime / fadeDuration);
        }
    }
}
