using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    // GameManager instance
    private GameManager gm;
    // Array of Gem prefabs
    public GameObject[] AR_GemPF;
    // 2D Array for mapping prefabs to the grid
    private GameObject[,] GO_AR_Graphics;    
    // 2D Array for the base grid
    private int[,] mAR_IN_Board;
    // Integer for private load time
    private int IN_Load = 1;
    // Current selected gem
    private GameObject selectedGem;
    // Latest selected gem
    private GameObject lastGem;
    // List of game objects for matched gems to be removed
    private List<GameObject> matchHorizontal = new List<GameObject>();
    private List<GameObject> matchVertical = new List<GameObject>();    
    // Boolean to check if player can interact with board
    public bool mBL_Interact;    

    void Start()
    {
        // Reference to game manager
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // If the game has started, run board interactivity
        if (gm.mBL_Start)
        {
            InteractiveBoard();
        }        
    }    

    public void StartGame()
    {
        // Disables interaction with the game board
        mBL_Interact = false;
        // Sets pause/resume start button to false while game is loading
        gm.startButton.interactable = false;
        // Initialises 2D base grid array and 2D prefab array
        mAR_IN_Board = new int[8, 8];
        GO_AR_Graphics = new GameObject[8, 8];
        // Hides the Title compnent
        gm.titleComp.gameObject.SetActive(false);
        // Shows the Loading compinent
        gm.loadingComp.gameObject.SetActive(true);
        // Sets the game board frame/gems colour to indicate that it is not interactable
        gm.ColourChange(1);
        // Prepares game
        gm.StartCoroutine("PrepareGame");
        // Begins the main Game Loop which is called every 0.2 seconds
        InvokeRepeating("GameLoop", 0F, .2F);
    }
      
    private void GameLoop()
    {
        // Checks if the user's time is up
        if (!gm.mBL_TimeUp)
        {
            // If the grid can Load
            if (gm.mBL_CanLoad)
            {
                // Loads new gems (updating the gem prefabs to the array)
                LoadGems();
            }            
            // Artificial gravity that moves the gems down the board if there is an empty space below
            ArtificialGravity();
            // Spawns gems on the top row if there is an empty space
            SpawnGems();
           
        }
        else
        {
            // Time is up
            print("Time up!");
        }
    }

    

    public void GenerateGrid()
    {      
        // Nested for loop that draws the x and y axis of the grid
        for (int x = 0; x < mAR_IN_Board.GetLength(0); x++)
        {
            for (int y = 0; y < mAR_IN_Board.GetLength(1); y++)
            {
                // Assigns a random gem type to each position on the grid
                mAR_IN_Board[x, y] = Random.Range(1, 8);
            }
        }

        // Loads gem prefabs into the generated grid
        LoadGems();
    }   
    
    private void LoadGems()
    {
        // Nested for loop that iterates through the x and y axis of the grid
        for (int x = 0; x < mAR_IN_Board.GetLength(0); x++)
        {
            for (int y = 0; y < mAR_IN_Board.GetLength(1); y++)
            {
                // Creates a game object variable that represents a single gem
                GameObject GO_Gem;
                // Checks if the type of gem is not empty
                if (mAR_IN_Board[x, y] > 0)
                {
                    // Destroys any current gem in the grid position
                    Destroy(GO_AR_Graphics[x, y]);
                    // Assigns Gem game object to the index of a gem prefab, using the 2D array base grid
                    GO_Gem = AR_GemPF[mAR_IN_Board[x, y] - 1 ];
                    // Instantiates game objects to a position on the prefab graphics array, with an offset of 1.5f
                    GO_AR_Graphics[x, y] = (GameObject) GameObject.Instantiate(GO_Gem, new Vector3(x / 1.5F , y / 1.5F , 0) , GO_Gem.transform.rotation);
                    // Sets the details of each individual gem to their parent gem class
                    GO_AR_Graphics[x, y].transform.GetComponent<GemInfo>().mGem.x = x;
                    GO_AR_Graphics[x, y].transform.GetComponent<GemInfo>().mGem.y = y;
                    GO_AR_Graphics[x, y].transform.GetComponent<GemInfo>().mGem.Type = mAR_IN_Board[x, y];
                }                
            }
        }
    }

    private void SpawnGems()
    {
        // Loop that runs through the first row 
        for (int i = 0; i < mAR_IN_Board.GetLength(0); i++)
        {            
            // Checking for empty spaces
            if (mAR_IN_Board[i, mAR_IN_Board.GetLength(0) - 1] == 0)
            {
                // Sets the empty space to a random gem type
                mAR_IN_Board[i, mAR_IN_Board.GetLength(0) - 1] = Random.Range(1, 8);
            }
        }     
    }

    private void ArtificialGravity()
    {
        // Nested for loop that iterates through the x and y axis of the grid
        for (int x = 0; x < mAR_IN_Board.GetLength(0); x++)
        {
            for (int y = 0; y < mAR_IN_Board.GetLength(1); y++)
            {
                // Limits the y axis to 0 to prevent gems moving off the grid
                if (y > 0)
                {
                    // Checks for empty space below current gem
                    if (mAR_IN_Board[x, y - 1] == 0)
                    {
                        // Sets the current gem type value to the empty space below
                        mAR_IN_Board[x, y - 1] = mAR_IN_Board[x, y];
                        //  Sets the current gem type to 0
                        mAR_IN_Board[x, y] = 0;
                    }
                }
            }
        }
    }    

    private void InteractiveBoard()
    {
        // Checks if the board is interactive
        if (mBL_Interact)
        {
            // Listens for right mouse click input from the user    
            if (Input.GetMouseButtonDown(0))
            {     
                // Generates raycast at the location of the mouse pointer           
                Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);

                // Checks if ray cast hits a collider within the game space
                if (hit)
                {
                    if (selectedGem == hit.collider.gameObject)
                    {
                        // Do nothing
                        return;
                    }

                    // If the selected game object is null
                    if (selectedGem == null)
                    {                     
                        // Assign the hit game object to the selected game object
                        selectedGem = hit.collider.gameObject;
                        // Change colour of selected gem to indicate that it is selected
                        selectedGem.GetComponent<Renderer>().material.color = Color.grey;
                    }
                    else
                    {
                        // Assigns the last gem as the second game object selected by the user
                        lastGem = hit.collider.gameObject;
                        // Float which calculates the distance from both selected game objects
                        float gemDistance = Vector3.Distance(selectedGem.transform.position, lastGem.transform.position);
                        // Custom distance check for orthogonal swaping for one tile only
                        if (gemDistance > 0.6 && gemDistance < 0.7)
                        {
                            // Swaps gems and checks for matches   
                            SwitchGems(selectedGem, lastGem);
                            CheckMatch();                            
                        }
                        else
                        {
                            // Prints hint messsage
                            gm.mST_Note = "Can only swap gems with a distance of one tile";
                            selectedGem.GetComponent<Renderer>().material.color = Color.white;
                            selectedGem = null;
                        }
                    }
                }                
            }
        }
    }

    private void SwitchGems(GameObject gemA, GameObject gemB)
    {        
        // Local variables that store the x and y locations of both gems
        int gemAX = gemA.GetComponent<GemInfo>().mGem.x;
        int gemAY = gemA.GetComponent<GemInfo>().mGem.y;      

        int gemBX = gemB.GetComponent<GemInfo>().mGem.x;
        int gemBY = gemB.GetComponent<GemInfo>().mGem.y;

        // Temporarily stores position of selected gem position
        Vector2 tempPos = gemA.transform.position;
        // Sets last gem to the position of the selected gem
        gemA.transform.position = gemB.transform.position;
        // Sets position from temporary vector 2 to the last gem position
        gemB.transform.position = tempPos;        

        // Switches x component location
        int tempX = gemAX;
        gemAX = gemBX;
        gemBX = tempX;
        // Switches y component location
        int tempY = gemAY;
        gemAY = gemBY;
        gemBY = tempY;        
        // Switches the type of gem
        int temType = mAR_IN_Board[gemAX, gemAY];
        mAR_IN_Board[gemAX, gemAY] = mAR_IN_Board[gemBX, gemBY];
        mAR_IN_Board[gemBX, gemBY] = temType;

        // Assigns the new values to the original component of gemA
        gemA.GetComponent<GemInfo>().mGem.x = gemAX;
        gemA.GetComponent<GemInfo>().mGem.y = gemAY;
        gemA.GetComponent<GemInfo>().mGem.Type = mAR_IN_Board[gemAX, gemAY];
        // Assigns the new values to the original component of gemB
        gemB.GetComponent<GemInfo>().mGem.x = gemBX;
        gemB.GetComponent<GemInfo>().mGem.y = gemBY;
        gemB.GetComponent<GemInfo>().mGem.Type = mAR_IN_Board[gemBX, gemBY];        

    }

    private void CheckMatch()
    {
        // Clears both match lists for fresh new matches
        matchHorizontal.Clear();
        matchVertical.Clear();
        // Checks for matching gems
        GemCheck();
        // Removes gems if the number of matches is greater or equal than 3
        if (matchHorizontal.Count >= 3 || matchVertical.Count >= 3)
        {
            // Increases load time depending on amount of gems to move down
            if (matchVertical.Count >= 5)
            {
                // Increases load time
                IN_Load = 2;
                // Generates new note
                gm.mST_Note = gm.GenerateResponse();
                // Increments turn counter and removes gems
                gm.mIN_Turns++;
                Invoke("RemoveGems", 0.5f);
            }
            else
            {
                // Sets load time to default
                IN_Load = 1;
                // Generates new note
                gm.mST_Note = gm.GenerateResponse();
                // Increments turn counter and removes gems
                gm.mIN_Turns++;
                Invoke("RemoveGems", 0.5f);
            }
                      
        }
        else
        {
            // Resets the selected gem game objects
            gm.mST_Note = "No matches here! Try again!";
            // Switches gems back to original position
            SwitchGems(lastGem, selectedGem); 
            // Deselects gem           
            selectedGem.GetComponent<Renderer>().material.color = Color.white;
            selectedGem = null;
            lastGem = null;
        }

    }

    private void GemCheck()
    {
        // Stores x and y positions of selected gems
        int x = selectedGem.GetComponent<GemInfo>().mGem.x;
        int y = selectedGem.GetComponent<GemInfo>().mGem.y;
        int prevX = lastGem.GetComponent<GemInfo>().mGem.x;
        int prevY = lastGem.GetComponent<GemInfo>().mGem.y;

        // Position search 1 left of the grid item
        int in_searchLeft = x - 1;
        // Position search 1 right of the grid item
        int in_searchRight = x + 1;
        // Position search 1 above the grid item
        int in_searchUp = y + 1;
        // Position search 1 below the grid item
        int in_searchDown = y - 1;

        // Adds current gem to be removed if successful
        matchHorizontal.Add(GO_AR_Graphics[prevX, prevY].gameObject);
        matchVertical.Add(GO_AR_Graphics[prevX, prevY].gameObject);

        // While loop that checks for id matches left of the grid item
        while (in_searchLeft >= 0 && mAR_IN_Board[x, y] == mAR_IN_Board[in_searchLeft, y])
        {
            // Adds the grid item position to the List
            matchHorizontal.Add(GO_AR_Graphics[in_searchLeft, y].gameObject);
            // Decrements the search to the previous grid item
            in_searchLeft--;
        }

        // While loop that checks for id matches right of the grid item
        while (in_searchRight < mAR_IN_Board.GetLength(0) && mAR_IN_Board[x, y] == mAR_IN_Board[in_searchRight, y])
        {
            // Adds the grid item position to the List
            matchHorizontal.Add(GO_AR_Graphics[in_searchRight, y].gameObject);
            // Increments the search to the previous grid item
            in_searchRight++;
        }

        // While loop that checks for id matches above the grid item
        while (in_searchUp < mAR_IN_Board.GetLength(0) && mAR_IN_Board[x, y] == mAR_IN_Board[x, in_searchUp])
        {
            // Adds the grid item position to the List
            matchVertical.Add(GO_AR_Graphics[x, in_searchUp].gameObject);
            // Increments the search to the previous grid item
            in_searchUp++;
            
        }

        // While loop that checks for id matches below the grid item
        while (in_searchDown >= 0 && mAR_IN_Board[x, y] == mAR_IN_Board[x, in_searchDown])
        {
            // Adds the grid item position to the List
            matchVertical.Add(GO_AR_Graphics[x, in_searchDown].gameObject);
            // Decrements the search to the previous grid item
            in_searchDown--;
        }

    }

    public void GridCheck()
    {
        // Nested for loop that iterates through the x and y axis of the grid
        for (int x = 0; x < mAR_IN_Board.GetLength(0); x++)
        {
            for (int y = 0; y < mAR_IN_Board.GetLength(1); y++)
            {
                // X axis checks for limiting highest and lowest available number
                if (x > 1 && x < mAR_IN_Board.GetLength(0) - 1)
                {
                    // Checks for matches left and right of curent gem
                    if (mAR_IN_Board[x + 1, y] == mAR_IN_Board[x, y] && mAR_IN_Board[x - 1, y] == mAR_IN_Board[x, y])
                    {
                        // Adds the three gems to the lists for removal
                        matchHorizontal.Add(GO_AR_Graphics[x + 1, y].gameObject);
                        matchHorizontal.Add(GO_AR_Graphics[x, y].gameObject);
                        matchHorizontal.Add(GO_AR_Graphics[x - 1, y].gameObject);
                    }
                }
                // Y axis checks for limiting highest and lowest available number
                if (y > 1 && y < mAR_IN_Board.GetLength(1) - 1)
                {
                    // Checks for matches top and bottom of curent gem
                    if (mAR_IN_Board[x, y + 1] == mAR_IN_Board[x, y] && mAR_IN_Board[x, y - 1] == mAR_IN_Board[x, y])
                    {
                        // Adds the three gems to the lists for removal
                        matchVertical.Add(GO_AR_Graphics[x, y + 1].gameObject);
                        matchVertical.Add(GO_AR_Graphics[x, y].gameObject);
                        matchVertical.Add(GO_AR_Graphics[x, y - 1].gameObject);
                    }
                }
            }
        }

        // Removes gems from game board
        RemoveGems();
          
    }
   

    private void RemoveGems()
    {       
        if (matchHorizontal.Count >= 3)
        {
            // Foreach loops which go through each game object in the list
            foreach (GameObject gem in matchHorizontal)
            {
                // Null check
                if (gem != null)
                {
                    // Temporary x and y variable
                    int x = gem.GetComponent<GemInfo>().mGem.x;
                    int y = gem.GetComponent<GemInfo>().mGem.y;
                    // Sets x and y position to 0 in array
                    mAR_IN_Board[x, y] = 0;
                    // Destroys the gem object                          
                    Destroy(gem);
                    // Adds score only if the game has started (after the initial grid check)
                    if (gm.mBL_Start)
                    {
                        gm.mIN_Score += 100;
                    }

                }
            }
        }

        if (matchVertical.Count >= 3)
        {
            foreach (GameObject gem in matchVertical)
            {
                // Null check
                if (gem != null)
                {
                    // Temporary x and y variable                    
                    int x = gem.GetComponent<GemInfo>().mGem.x;
                    int y = gem.GetComponent<GemInfo>().mGem.y;
                    // Sets x and y position to 0 in array
                    mAR_IN_Board[x, y] = 0;
                    // Destroys the gem object
                    Destroy(gem);
                    // Adds score only if the game has started (after the initial grid check)
                    if (gm.mBL_Start)
                    {
                        gm.mIN_Score += 100;
                    }
                }
            }
        }              

        // Resets the selected gem game objects
        if (selectedGem != null)
        {
            selectedGem.GetComponent<Renderer>().material.color = Color.white;
            selectedGem = null;
            lastGem = null;
        }

        // Grid can load and user will be unable to interact
        gm.mIN_Load = IN_Load;
        gm.mBL_CanLoad = true;
        mBL_Interact = false;
        // Method is invoked after specified delay time
        Invoke("SetupLoad", gm.mIN_Load);
    }

    private void SetupLoad()
    {
        // Switches the boolean variables
        gm.mBL_CanLoad = false;
        mBL_Interact = true;
    }   

   
}
