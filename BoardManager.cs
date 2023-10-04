using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region Custom Events
    public delegate void StartGame(bool value);
    public static event StartGame OnStartGame;

    public delegate void BoardPiecePlaced(int id);
    public static event BoardPiecePlaced OnBoardPiecePlaced;
    #endregion

    public int ShipSize = 2;
    public bool Vertical = true;

    //accpeting the player and ai board
    public BoardPlayer boardPlayer;
    public BoardAI boardEnemy;

    //accepting the prefab from unity such as board, cube and attack
    public GameObject BoardUnitPrefab;
    public GameObject BoardUnitAttachPrefab;

    public GameObject BlockVisualizerPrefab;

    [Header("Player piece Model Reference")]
    public List<GameObject> boardPiecesPref;

    
    [Header("----")]

    //can we place the block or not
    bool PLACE_BLOCK = true;

    [SerializeField]

    //coming from the ui whats the current ship id
    private int currentShipID;

    GameObject tmpHighlight = null;
    RaycastHit tmpHitHighlight;

    GameObject tmpBlockHolder = null;

    //is it ok the place
    private bool OK_TO_PLACE = true;

    [SerializeField]

    //counter to check if user has place all the pieces
    private int count = 0;

    //a flag to tell the program that the player has place their
    //pieces and now place the enemy's ship
    bool placeEnemyShips = true;

    GameObject tmpAttackHighlight = null;
    GameObject tmpAttackHitHighlight;


    //enable and disable are the event handler
    //changing the ship or orientation
    private void OnEnable()
    {
        UIBoardManager.OnChangeShip += UIBoardManager_OnChangeShip;
        UIBoardManager.OnChangeOrientation += UIBoardManager_OnChangeOrientation;
    }
    
    private void OnDisable()
    {
        UIBoardManager.OnChangeShip -= UIBoardManager_OnChangeShip;
        UIBoardManager.OnChangeOrientation -= UIBoardManager_OnChangeOrientation;
    }

    private void UIBoardManager_OnChangeOrientation(bool orientation)
    {
        Vertical = !Vertical;
    }

    private void UIBoardManager_OnChangeShip(int id, int size)
    {

        currentShipID = id;
        ShipSize = size;
    }

    //start is called before the first frame update
    void Start()
    {
        //creating the board themselves player and ai
        boardPlayer = new BoardPlayer(BoardUnitPrefab);
        boardPlayer.CreatePlayerBoard();

        boardEnemy = new BoardAI(BoardUnitAttachPrefab, BlockVisualizerPrefab);
        boardEnemy.CreateAIBoard();

        currentShipID = 0;
        ShipSize = 0;
    }

    //update is called once per frame
    void Update()
    {
        //this is to see if it is still the player turn or ai turn
        if (IsBusy)
            return;

        if (count < 5)
        {
            PlacePlayerPieces();
        }
        else
        {
            if (placeEnemyShips)
            {
                placeEnemyShips = false;
                boardEnemy.PlaceShips();
                OnStartGame?.Invoke(true);
            }
        }
    }

    private void PlacePlayerPieces()
    {
        //capture the mouse position and cast a ray to see what object is hit
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.mousePosition != null )
        {
            //this is where we check if it did or not
            if (Physics.Raycast(ray, out tmpHitHighlight, 100))
            {
                //we get the transform of the boardunit and save it into a variable
                BoardUnit tmpUI = tmpHitHighlight.transform.GetComponent<BoardUnit>();
               

                if (tmpHitHighlight.transform.tag.Equals("BoardUnit") && !tmpUI.occupied)
                {
                    //we retrieve the data of the board in 2 dim array
                    BoardUnit boardData =
                        boardPlayer.board[tmpUI.row, tmpUI.col].transform.GetComponentInChildren<BoardUnit>();

                    if (tmpHighlight != null)
                    {
                        if(boardData.occupied)
                            tmpHighlight.GetComponent<MeshRenderer>().material.color = Color.red;
                        else
                            tmpHighlight.GetComponent<MeshRenderer>().material.color = Color.white;
                    }

                    if (tmpBlockHolder != null)
                    {
                        Destroy(tmpBlockHolder);
                    }

                    if (PLACE_BLOCK)
                    {
                        Debug.Log("PLACE_BLOCK");

                        tmpBlockHolder = new GameObject();
                        OK_TO_PLACE = true;

                        if (!Vertical && (tmpUI.row <= 10 - ShipSize))
                        {

                            for (int i = 0; i < ShipSize; i++)
                            {
                                GameObject visual = GameObject.Instantiate(BlockVisualizerPrefab,
                                                                           new Vector3(tmpUI.row + i, BlockVisualizerPrefab.transform.position.y, tmpUI.col),
                                                                           BlockVisualizerPrefab.transform.rotation) as GameObject;

                                GameObject bp = boardPlayer.board[tmpUI.row + i, tmpUI.col];
                                BoardUnit bpUI = bp.transform.GetComponentInChildren<BoardUnit>();

                                if (!bpUI.occupied)
                                {
                                    visual.GetComponent<MeshRenderer>().material.color = Color.gray; //ok to place
                                }
                                else
                                {
                                    visual.GetComponent<MeshRenderer>().material.color = Color.yellow; //not ok to place
                                    OK_TO_PLACE = false;
                                }

                                visual.transform.parent = tmpBlockHolder.transform;
                            }
                        }

                        if (Vertical && (tmpUI.col <= 10 - ShipSize))
                        {
                            Debug.Log("Vertivcal visual");

                            for (int i = 0; i < ShipSize; i++)
                            {
                                GameObject visual = GameObject.Instantiate(BlockVisualizerPrefab,
                                                                           new Vector3(tmpUI.row, BlockVisualizerPrefab.transform.position.y, tmpUI.col + i),
                                                                            BlockVisualizerPrefab.transform.rotation) as GameObject;

                                GameObject bp = boardPlayer.board[tmpUI.row, tmpUI.col + i];
                                BoardUnit bpUI = bp.transform.GetComponentInChildren<BoardUnit>();

                                if (!bpUI.occupied)
                                {
                                    visual.GetComponent<MeshRenderer>().material.color = Color.gray; //ok to place
                                }
                                else
                                {
                                    visual.GetComponent<MeshRenderer>().material.color = Color.yellow; //not ok to place
                                    OK_TO_PLACE = false;
                                }

                                visual.transform.parent = tmpBlockHolder.transform;
                            }
                        }
                    }
                }
            }
        }

        #region MOUSE CLICK
        if (Input.GetMouseButton(0))
        {
            //capture the mouse position and cast a ray to see what object is hit
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100 )) 
            {
                if (hit.transform.tag.Equals("BoardUnit"))
                {
                    BoardUnit tmpUI = hit.transform.GetComponentInChildren<BoardUnit>();

                    if (PLACE_BLOCK && OK_TO_PLACE) 
                    {
                        //placing the model into the board
                        if(!Vertical)
                        {
                            for (int i = 0; i < ShipSize; i++)
                            {
                                GameObject sB = boardPlayer.board[tmpUI.row + i, tmpUI.col];

                                BoardUnit bu = sB.transform.GetComponentInChildren<BoardUnit>();
                                bu.occupied = true;
                                bu.GetComponent<MeshRenderer>().material.color= Color.green;

                                boardPlayer.board[tmpUI.row + i, tmpUI.col] = sB;

                            }
                        }

                        if (Vertical)
                        {
                            for (int i = 0; i < ShipSize; i++)
                            {
                                GameObject sB = boardPlayer.board[tmpUI.row, tmpUI.col + i];

                                BoardUnit bu = sB.transform.GetComponentInChildren<BoardUnit>();
                                bu.occupied = true;

                                bu.GetComponent<MeshRenderer>().material.color = Color.green;

                                boardPlayer.board[tmpUI.row, tmpUI.col + i] = sB;

                            }
                        }

                        //check which ship we placed
                        CheckWhichShipWasPlaced(tmpUI.row, tmpUI.col);

                        OK_TO_PLACE = true;
                        tmpHighlight = null;

                    }

                    //check and destory the temp block holder
                    //this is used to destory the last tmp blocks after we have placed the last
                    //block group on the board
                    if (count >= 5)
                    {
                        if (tmpBlockHolder != null)
                        {
                            Destroy(tmpBlockHolder);
                        }
                    }
                }
            }
        }
        #endregion MOUSE CLICK
    }

    private void CheckWhichShipWasPlaced(int row, int col)
    {
        switch (currentShipID)
        {
            case 0:
                {
                    if (!Vertical)
                    {
                        //place it as vertical
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row + 0.5f,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                        testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                    }
                    else
                    {
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col + 0.5f),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    }

                    count++;
                    break;
                }
            case 1:
                {
                    if (!Vertical)
                    {
                        //place it as vertical
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row + 1,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                        testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                    }
                    else
                    {
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col + 1),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    }

                    count++;
                    break;
                }
            case 2:
                {
                    if (!Vertical)
                    {
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row + 1,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                        testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                    }
                    else
                    {
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col + 1),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    }

                    count++;
                    break;
                }
            case 3:
                {
                    if (!Vertical)
                    {
                        //place it as vertical
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row + 1.5f,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                        testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                    }
                    else
                    {
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col + 1.5f),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    }

                    count++;
                    break;
                }
            case 4:
                {
                    if (!Vertical)
                    {
                        //place it as vertical
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row + 2,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                        testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                    }
                    else
                    {
                        GameObject testingVisual = GameObject.Instantiate(boardPiecesPref[currentShipID],
                                                                          new Vector3(row,
                                                                                      boardPiecesPref[currentShipID].transform.position.y,
                                                                                      col + 2),
                                                                           boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    }

                    count++;
                    break;
                }
        }

        OnBoardPiecePlaced?.Invoke(currentShipID);

        StartCoroutine(Wait4Me(0.5f));

        //clear internal data
        currentShipID = -1;
        ShipSize = 0;
    }

    public static bool IsBusy = false;

    IEnumerator Wait4Me(float seconds = 0.5f)
    {
        IsBusy = true;
        yield return new WaitForSeconds(seconds);
        IsBusy = false;
    }
}
