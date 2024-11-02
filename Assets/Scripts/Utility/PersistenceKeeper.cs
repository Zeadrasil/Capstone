using UnityEngine;

//PersistenceKeeper is exclusively used to toss onto something to ensure that it does not get destroyed upon loading in
public class PersistenceKeeper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Do not destroy the gameobject
        Debug.Log(gameObject.name);
        DontDestroyOnLoad(gameObject);
    }
}
