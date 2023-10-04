using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class BoardAI : Board
{
    GameObject cubePrefab;
    int[] aiShipsSizes = new int[5] {2, 3, 3, 4, 5};

    public BoardAI(GameObject unitPrefab, GameObject prefab)
    {
        BoardUnitPrefab = unitPrefab;
        cubePrefab = prefab;

        ClearBoard();
    }

    public void CreateAIBoard()
    {
        int row = 1;
        int col = 1;
        for (int i = 11; i < 21; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject tmp = GameObject.Instantiate(BoardUnitPrefab,
                                                    new Vector3(i, 0, j),
                                                    BoardUnitPrefab.transform.rotation) as GameObject;

                BoardUnit tmpUI = tmp.GetComponentInChildren<BoardUnit>();
                string name = string.Format("B2:[{0:00},{0:00}]", row, col);
                tmpUI.tmpBoardUnitLabel.text = name;
                tmpUI.col = col - 1;
                tmpUI.row = row - 1;

                board[tmpUI.row, tmpUI.col] = tmp;

                tmp.name = name;

                col++;
            }
            col = 1;
            row++;
        }
    }

    public void PlaceShips()
    {
        for(int i = 0; i < aiShipsSizes.Length; i++)
        {
            Debug.Log("I AM IN THE PLACE ENEMY SHIP FUNCTION");

            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);

            bool ori = (Random.Range(0, 9) > 5) ? true : false;

            Debug.Log(string.Format("Placing ship {0} at location {1}, {2} with orientation: {3}", i, row, col, ori));

            CheckBoardForPlacement(row, col, aiShipsSizes[i], ori);

        }
    }

    private void CheckBoardForPlacement(int row, int col, int size, bool hor)
    {
        Debug.Log("STARTING VERIFICATION PROCESS");

        //the logic:
        // 1. we need to check and see that at the given location, if the ai can place the ship
        // 2. if it is avaiable, then go ahead and put it in the location
        GameObject checkUnit = board[row, col];

        var bu = checkUnit.GetComponentInChildren<BoardUnit>();

        if (bu.occupied || (row + size > 9) || (col + size > 9))
        {
            Debug.Log(string.Format("LOCATION OCCUPIED AT [{0},{1}]", row, col));

            //it is occupied, generate a new random row/col and call function
            int r1 = Random.Range(0, 9);
            int c1 = Random.Range(0, 9);
            Debug.Log(string.Format("RETRY WITH NEW COORDINATE AT [{0},{1}]", r1, c1));
            CheckBoardForPlacement(r1, c1, size, hor);
            return;
        }

        //one pass for verification
        bool okToPlace = true;

        Debug.Log(string.Format("STARTING PASS 1 for PLACEMENT. ON?{0}", okToPlace));

        if (!hor && (row + size < 10))
        {
            for (int i = 0; i < size; i++)
            {
                GameObject bp = board[row + i, col];
                BoardUnit bpUI = bp.GetComponentInChildren<BoardUnit>();
                if (!bpUI.occupied)
                {
                    //visual.GetComponment<Renderer>().material.color = Color.magenta;
                    //okToPlace = true;
                }
                else
                {
                    //visual.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f)
                    //visual.GetComponment<Renderer>().material.color = Color.yellow;
                    okToPlace = false;
                }

                //visual.transform.parent = this.tmpBlockHolder.transform
            }
        }

        if (hor && (col + size < 10))
        {
            for (int i = 0; i < size; i++)
            {
                GameObject bp = board[row, col + i];
                BoardUnit bpUI = bp.GetComponentInChildren<BoardUnit>();
                if (!bpUI.occupied)
                {
                    //visual.GetComponment<Renderer>().material.color = Color.magenta;
                    //okToPlace = true;
                }
                else
                {
                    //visual.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f)
                    //visual.GetComponment<Renderer>().material.color = Color.yellow;
                    okToPlace = false;
                }
                
                //visual.transform.parent = this.tmpBlockHolder.transform;
            }
        }

        Debug.Log(string.Format("END PASS 1 for PLACEMENT. ON?{0}", okToPlace));

        if (okToPlace)
        {
            if (!hor)
            {
                for (int i = 0; i < size; i++)
                {
                    // these game objects can be removed since they are only places for visual debugging
                    GameObject visual = GameObject.Instantiate(cubePrefab,
                                                               new Vector3(row + i + 11, 0.9f, col),
                                                               cubePrefab.transform.rotation) as GameObject;
                    visual.GetComponent<Renderer>().material.color = Color.yellow;

                    GameObject sB = board[row + i, col];
                    sB.GetComponentInChildren<BoardUnit>().occupied = true;
                    board[row + i, col] = sB;

                    visual.gameObject.name = string.Format("EN-R-[{0},{1}]", row + i, col);

                    Debug.Log(string.Format("Enemy ship will be placed at location[{0}, {1}]", row + i, col));

                }
            }

            if (hor)
            {
                for (int i = 0; i < size; i++)
                {
                    // these game objects can be removed since they are only places for visual debugging
                    GameObject visual = GameObject.Instantiate(cubePrefab,
                                                               new Vector3(row + 11, 0.9f, col + i),
                                                               cubePrefab.transform.rotation) as GameObject;
                    visual.GetComponent<Renderer>().material.color = Color.magenta;

                    GameObject sB = board[row, col + i];
                    sB.GetComponentInChildren<BoardUnit>().occupied = true;
                    board[row, col + i] = sB;

                    visual.gameObject.name = string.Format("EN-C-[{0},{1}]", row, col + i);

                    Debug.Log(string.Format("Enemy ship will be placed at location[{0}, {1}]", row, col + i));

                }
            }
        }
        else
        {
            int r1 = Random.Range(0, 9);
            int c1 = Random.Range(0, 9);

            Debug.Log(string.Format("Placement was {2}, Starting again, New Location [{0}, {1}]", r1, c1, okToPlace));

            CheckBoardForPlacement(r1, c1, size, hor);
        }
    }
}
