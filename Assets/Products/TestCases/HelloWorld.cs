using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print("Hello World!");
        Invoke("Mono", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Mono()
    {
        Debug.Log("Mono");
    }
}
