using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello?");

        PostCard tst = new PostCard("Kevin", System.DateTime.Now);

        tst.displayContent();
    }
}
