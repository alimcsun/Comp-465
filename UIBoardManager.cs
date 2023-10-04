using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class UIBoardManager : MonoBehaviour
{
    #region Custom Events
    public delegate void ChangeShip(int id, int size);
    public static event ChangeShip OnChangeShip;

    public delegate void ChangeOrientation(bool orientation);
    public static event ChangeOrientation OnChangeOrientation;
    #endregion

    [Header("Player Pieces References")]
    public List<Button> collectionOfPlayerPieceButtons;

    [Header("Game Button References")]
    public Button butOrientation;
    public Button butUIReset;
    public Button butExit;

    [Header("Sprite References")]
    public Sprite textureHorizontal;
    public Sprite textureVertical;

    [Header("Basic Flag Settings...")]
    public bool DisplayInstruction;
    public bool Orientation = false;

    // This dictionary is a data structure to store
    // the player's piece ID and the size of the unit
    public Dictionary<int, int> boardPieces = new Dictionary<int, int>
    {
        {0, 2}, // patrol boat
        {1, 3}, // destoryer
        {2, 3}, // submarine
        {3, 4}, // battleship 
        {4, 5}  // aircraft carrier
    };

    private void OnEnable()
    {
        BoardManager.OnBoardPiecePlaced += BoardVer1_OnBoardPiecePlaced;
    }

    private void OnDisable()
    {
        BoardManager.OnBoardPiecePlaced -= BoardVer1_OnBoardPiecePlaced;
    }

    private void BoardVer1_OnBoardPiecePlaced(int id)
    {
        Debug.Log($"id = {id}");
        //disable the button representing the piece
        collectionOfPlayerPieceButtons[id].gameObject.SetActive(false);
    }

    void Start()
    {
        UpdateOrientationUI();
    }

    public void butBoardPieceSelected(int shipID)
    {
        //checking in the dictionary
        int size = boardPieces.ContainsKey(shipID) ? boardPieces[shipID] : -1;

        if (size == -1)
            Debug.LogWarning($"{shipID} does not exist in the collection");
        else 
        {
            // pass the data
            OnChangeShip?.Invoke(shipID, size);
        }

    }

    public void butChangeOrientation()
    {
        Orientation = !Orientation;
        UpdateOrientationUI();
    }

    void UpdateOrientationUI()
    {
        if (Orientation)
        {
            butOrientation.image.sprite = textureVertical;
        }
        else
        {
            butOrientation.image.sprite = textureHorizontal;
        }

        OnChangeOrientation?.Invoke(Orientation);
    }

}
