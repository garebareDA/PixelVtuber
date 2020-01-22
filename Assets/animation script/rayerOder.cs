using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayerOder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        var render = GetComponent<MeshRenderer>();
        if (render != null)
        {
            render.sortingOrder = 6;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
