using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Battleship : MonoBehaviour
{

    public GameObject[] Cell1;  // ячейки первого корпуса
    public GameObject[] Cell2;  // ячейки первого корпуса
    public GameObject[] Cell3;  // ячейки первого корпуса
    public GameObject[] Cell4;  // ячейки первого корпуса

    public GameObject[,] Battlefield;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        Debug.Log("click");
    }

    public void Shot(int row, int cell)
    {
        Debug.Log(row + " " + cell);
    }
}
