using System.Collections.Generic;
using UnityEngine;

//MultiTag is used to allow multiple tags on one object. Likely to be removed since I have moved to interfaces
public class MultiTag : MonoBehaviour
{
    //Holds the different tags
    public List<string> Tags = new List<string>();
}
