using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // GameBoard instance
    private GameBoard gb;
    // Toggle state for the start button 
    public bool toggleState = false;
    // Text and slider member variables for score, time, and turns
    public Text mTextScore;
    public Text mTextTime;
    public Text mTextTurns;
    public Text mTextNote;
    public Slider mSliderPrepare;
    // Member variables for game objects
    public Button startButton;
    public Button resetButton;
    public GameObject loadingComp;
    public GameObject titleComp;
    public GameObject dataComp;
    public GameObject frameComp;
    private GameObject border1;
    private GameObject border2;
    private GameObject border3;
    private GameObject border4;
    private GameObject[] gemObjects;
    // Integers that holds the technical data
    public int mIN_Load = 1;
    // Variables that hold the data for time, score, turns and notice
    public int mIN_Time = 121;
    public int mIN_Score = 0;
    public int mIN_Turns = 0;
    public string mST_Note = "Welcome to match the gems!";
    // Boolean variables that hold the different game states; start, paused and time up
    public bool mBL_Start = false;
    public bool mBL_Paused = false;
    public bool mBL_TimeUp = false;
    public bool mBL_CanLoad = false;

    // Use this for initialization
    void Start () {

        // Initialises game objects from the heirachy and sets the loading component inactive until player starts the game
        gb = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        startButton = GameObject.Find("Start").GetComponent<Button>();
        resetButton = GameObject.Find("Reset").GetComponent<Button>();
        loadingComp = GameObject.Find("Loading");
        titleComp = GameObject.Find("GameTitle");
        dataComp = GameObject.Find("DataTitle");
        frameComp = GameObject.Find("WhiteFrame");
        border1 = GameObject.Find("border1");
        border2 = GameObject.Find("border2");
        border3 = GameObject.Find("border3");
        border4 = GameObject.Find("border4");        
        loadingComp.gameObject.SetActive(false);
        dataComp.gameObject.SetActive(false);
        startButton.GetComponentInChildren<Text>().text = "Begin Game";
        resetButton.GetComponentInChildren<Text>().text = "Reset Board";
        resetButton.interactable = false;
    }
	
	// Update is called once per frame
	void Update () {

        // If the game has started, update the UI
        if (mBL_Start)
        {
            UpdateUI();
        }
    }

    public void NewGame()
    {
        // Toggle state for the start button click
        if (!toggleState)
        {
            // Changes button text to pause the game            
            startButton.GetComponentInChildren<Text>().text = "Pause Game";
            ColourChange(0);
            GemLock(0);

            // Do only on first time initialisation
            if (!mBL_Start)
            {
                // Calls StartGame method
                gb.StartGame();                
            }
            else
            {
                // User has resumed the game
                mST_Note = "Game has resumed!";
                mBL_Paused = false;
                gb.mBL_Interact = true;
            }

        }
        else
        {
            // Changes button text to resume the game
            mST_Note = "Game has been paused!";
            startButton.GetComponentInChildren<Text>().text = "Resume Game";
            ColourChange(1);
            GemLock(1);
            // User has paused the game
            mBL_Paused = true;
            gb.mBL_Interact = false;
        }

        // Switch states on start button click
        toggleState = !toggleState;
    }

    public void ResetGame()
    {
        // Reloads the game using scene manager
        SceneManager.LoadScene("Scene", LoadSceneMode.Single);
    }

    private void UpdateUI()
    {
        // Updates the time, score and turns every frame
        mTextTime.text = mIN_Time + " seconds";
        mTextScore.text = "Score: " + mIN_Score + " points";
        mTextTurns.text = "Turns: " + mIN_Turns + " turns";
        mTextNote.text = mST_Note;
    }

    public string GenerateResponse()
    {
        // Temporary storage for response and random number
        string response;
        int randonNum;
        // Generates random number between 1 and 5
        randonNum = Random.Range(1, 11);
        // Sends the new number to a switch case statement and returns a predefined response
        switch (randonNum)
        {
            case 1:
                response = "Good work!";
                return response;

            case 2:
                response = "Nice job!";
                return response;

            case 3:
                response = "Awesome match!";
                return response;

            case 4:
                response = "Sweet!";
                return response;

            case 5:
                response = "More points!";
                return response;

            case 6:
                response = "Well done!";
                return response;

            case 7:
                response = "Perfect!";
                return response;

            case 8:
                response = "You're on fire!";
                return response;

            case 9:
                response = "Amazing work!";
                return response;

            case 10:
                response = "You're too good at this game!";
                return response;

            default:
                return "";                
        }
    }

    private void GemLock(int code)
    {
        // Gets all gem game objects on grid and assigns them to an array
        gemObjects = GameObject.FindGameObjectsWithTag("Gem");
        // Switches the code for the lock state of gems
        switch (code)
        {
            case 0:
                // Sets all gem highlight colours to default
                foreach (GameObject gem in gemObjects)
                {
                    gem.GetComponent<Renderer>().material.color = Color.white;
                }
                break;
            case 1:
                // Sets all gem highlight colours to grey
                foreach (GameObject gem in gemObjects)
                {
                    gem.GetComponent<Renderer>().material.color = Color.grey;
                }
                break;
        }
    }

    public void ColourChange(int type)
    {
        // Switch statemnet that changes the colour of the game board, depending on its state
        switch (type)
        {
            case 0:
                // Components for game board frame plus borders
                frameComp.gameObject.GetComponent<Image>().color = Color.white;
                border1.gameObject.GetComponent<Image>().color = Color.white;
                border2.gameObject.GetComponent<Image>().color = Color.white;
                border3.gameObject.GetComponent<Image>().color = Color.white;
                border4.gameObject.GetComponent<Image>().color = Color.white;
                break;
            case 1:
                frameComp.gameObject.GetComponent<Image>().color = Color.grey;
                border1.gameObject.GetComponent<Image>().color = Color.grey;
                border2.gameObject.GetComponent<Image>().color = Color.grey;
                border3.gameObject.GetComponent<Image>().color = Color.grey;
                border4.gameObject.GetComponent<Image>().color = Color.grey;
                break;
            case 2:
                frameComp.gameObject.GetComponent<Image>().color = Color.red;
                border1.gameObject.GetComponent<Image>().color = Color.red;
                border2.gameObject.GetComponent<Image>().color = Color.red;
                border3.gameObject.GetComponent<Image>().color = Color.red;
                border4.gameObject.GetComponent<Image>().color = Color.red;
                break;

        }

    }

    IEnumerator UpdateTime()
    {
        // User has started the game
        mBL_Start = true;

        // Ensures timer does not go below 0
        while (mIN_Time > 0)
        {
            // Checks the paused state of the game
            while (mBL_Paused)
            {
                // Returns null
                yield return null;
            }
            // Otherwise decrements time variable after every second
            mIN_Time--;
            yield return new WaitForSeconds(1);
        }

        if (mIN_Time < 11)
        {
            // Changes colour of timer text to create a sense of urgency
            mST_Note = "Only 10 seconds remaining! Hurry!";
            mTextTime.color = Color.red;
        }

        if (mIN_Time == 0)
        {
            // Sets time up variable to true and ends the game
            mBL_TimeUp = true;
            gb.mBL_Interact = false;
            startButton.interactable = false;
            resetButton.GetComponentInChildren<Text>().text = "Replay Game";
            mST_Note = "You are out of time! Replay?";
            ColourChange(2);
            GemLock(1);
        }
    }

    IEnumerator PrepareGame()
    {
        // Ensures slider does not go above 1
        while (mSliderPrepare.value < 1)
        {
            // Otherwise increments slider progress, matching the time taken to generate the board
            mSliderPrepare.value += 0.035f;
            gb.GenerateGrid();
            yield return new WaitForSeconds(0.05f);
        }

        if (mSliderPrepare.value == 1)
        {
            // Hides the loading component
            loadingComp.gameObject.SetActive(false);
            // Makes the pause/resume start button interactable as well as the reset button
            startButton.interactable = true;
            resetButton.interactable = true;
            // Sets the game board frame colour to indicate that it is interactable
            ColourChange(0);
            gb.mBL_Interact = true;
            // Checks the grid for matches
            gb.GridCheck();
            // Start timer
            dataComp.gameObject.SetActive(true);
            StartCoroutine("UpdateTime");
        }
    }

}
