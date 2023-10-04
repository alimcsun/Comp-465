using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardPlayer : Board
{
    public BoardPlayer(GameObject unitPrefab)
    {
        BoardUnitPrefab = unitPrefab;
        ClearBoard();
    }

    public void CreatePlayerBoard()
    {
        //create a 10x10 board - board 1
        int row = 1; 
        int col = 1;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                //instantiate prefrab and place it properly on the scene
                GameObject tmp = GameObject.Instantiate(BoardUnitPrefab, 
                                                         new Vector3(i, 0, j),
                                                         BoardUnitPrefab.transform.rotation ) as GameObject;

                BoardUnit tmpUI = tmp.GetComponentInChildren<BoardUnit>();
                string name = string.Format("B1:[{0:00},{0:00}]", row, col);
                tmpUI.tmpBoardUnitLabel.text = name;
                tmpUI.col = j;
                tmpUI.row = i;

                board[i, j] = tmp;

                tmp.name = name;

                col++;
            }

            col = 1;
            row++;
        }
    }
}
