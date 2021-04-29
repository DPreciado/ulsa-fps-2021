using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadInput : MonoBehaviour
{
    private string input;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadStringInput(string s)
    {
        input = s;
        Debug.Log(input);
    }

    public void StartServer()
    {
        Debug.Log("server iniciado");
    }

    public void StartClient()
    {
        Debug.Log("Cliente iniciado");
    }
}
