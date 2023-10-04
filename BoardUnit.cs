using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardUnit : MonoBehaviour
{
    public TMP_Text tmpBoardUnitLabel;
    public GameObject CubePrefab;

    public int row;
    public int col;

    public bool occupied = false;
    public bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        tmpBoardUnitLabel.text = $"B[{row},{col}]";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
