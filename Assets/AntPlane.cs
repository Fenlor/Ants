using UnityEngine;

public class AntPlane : MonoBehaviour
{
    //take in ground cell object
    public GameObject cellObject;
    public int gridSize = 256;
    private GameObject[,] cellGrid;
    public Material colourOneMaterial;
    public Material colourTwoMaterial;
    private GameObject activeCell;
    private int activeRow;
    private int activeColumn;    
    public GameObject antObject;
    private bool isRowMove = false;
    private bool isPosMove = false;
    private int rowDir = -1;
    private int colDir = -1;

    //use timer after it works
    public float moveTimeDelta;
    private float moveTimer;
    private float stride;

    private enum DIRECTION
    {
        UP,
        DOWN, 
        LEFT, 
        RIGHT
    }
    DIRECTION dir = DIRECTION.UP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        if (cellObject != null)
        {
            cellGrid = new GameObject[gridSize, gridSize];

            //Debug.Log("AntPlane - Start() - cellGrid Length:" + cellGrid.Length);
            //Debug.Log("AntPlane - Start() - cellGrid[0] Length:" + cellGrid[0].Length);
            //cellGrid[gridSize] = new GameObject[gridSize];
            float cellSize = cellObject.GetComponent<MeshRenderer>().bounds.size.x;
            float halfCellSize = cellSize * 0.5f;
            //assume square for cell?
            //use middle of this object as middle of grid
            Vector3 startPos = transform.position; 
            float startX = transform.position.x - ((gridSize * 0.5f) * cellSize);
            float startY = transform.position.z - ((gridSize * 0.5f) * cellSize);
            startPos.x = startX;
            startPos.z = startY;

            //create 2D array of grid objects so we can change material and stuff?

            stride = cellObject.GetComponent<MeshRenderer>().bounds.size.x * 0.5f;
            for (int gridX = 0; gridX < cellGrid.GetLength(0); gridX++)
            {
                for (int gridY = 0; gridY < cellGrid.GetLength(1); gridY++)
                {
                    //cellGrid[gridX] = new GameObject[gridY];
                    //cellGrid[gridX][gridY] = new GameObject();
                    cellGrid[gridX, gridY] = Instantiate(cellObject, startPos, transform.rotation);
                    cellGrid[gridX, gridY].SetActive(false);
                    startPos.x += cellSize;                    
                }
                startPos.x = startX;
                startPos.z += cellSize;
            }

            //ant to find cell near center to start on
            //ant will always move from cell center to cell center, teleport to start. Lerp is easy though, might be cool
            //use cell count to find cell? sounds good
            int halfCells = (int)(gridSize * 0.5f);
            activeCell = cellGrid[halfCells, halfCells];
            activeRow = halfCells;
            activeColumn = halfCells;
            //Debug.Log("AntPlane - Start() - activeRow, activeColumn: " + activeRow + ", " + activeColumn);
            //Debug.Log("AntPlane - Start() - cellGrid Length:" + cellGrid.Length);
            //Debug.Log("AntPlane - Start() - cellGrid[0][0] Object:" + cellGrid[0,0]);
        }

        colourOneMaterial.SetOverrideTag("SurfaceType", "ColourOne");
        colourTwoMaterial.SetOverrideTag("SurfaceType", "ColourTwo");
    }

    // Update is called once per frame
    void Update()
    {
        //ant need to look at active cell, use tag in material? cant see how atm, maybe just name
        //then change colour and turn facing both depending on current colour
        //then move forward 1 cell, how to tell if facing row or coloum, could just pick one to start with and keep a hold of it

        moveTimer += Time.deltaTime;
        if (moveTimer > moveTimeDelta)
        {
            Debug.Log("AntMove - ACTING!");
            moveTimer = 0f;
            if (activeCell != null)
            {
                AntActOnActiveCell();
            }
        }
    }
    
    private void AntActOnActiveCell()
    {
        //Debug.Log("AntMove - AntActOnActiveCell, material.name: " + activeCell.GetComponent<MeshRenderer>().material.name);
        //Debug.Log("AntMove - AntActOnActiveCell, material: " + activeCell.GetComponent<MeshRenderer>().material);
        //Debug.Log("AntMove - Before row col change, isRowMove, isPosMove: " + isRowMove + ", " + isPosMove);
        //Debug.Log("AntMove - Before row col change, activeRow, activeColumn: " + activeRow + ", " + activeColumn);

        if (activeCell.GetComponent<MeshRenderer>().material.GetTag("SurfaceType", false, default) == "ColourOne")
        {
            Debug.Log("AntMove - AntActOnActiveCell, CellColourWhite, changing to black:");

            if (!activeCell.activeSelf)
            {
                activeCell.SetActive(true);
            }
            activeCell.GetComponent<MeshRenderer>().material = colourTwoMaterial;
            Vector3 pos = activeCell.transform.position;
            switch (dir)
            {
                case DIRECTION.UP:
                    activeColumn += 1;
                    dir = DIRECTION.RIGHT;
                    break;
                case DIRECTION.DOWN:
                    activeColumn -= 1;
                    dir = DIRECTION.LEFT;
                    break;
                case DIRECTION.LEFT:                    
                    pos.y += stride * 2f;
                    activeCell.transform.position = pos;
                    activeRow -= 1;
                    dir = DIRECTION.UP;
                    break;
                case DIRECTION.RIGHT:                    
                    pos.y -= stride * 2f;
                    activeCell.transform.position = pos;
                    activeRow += 1;
                    dir = DIRECTION.DOWN;
                    break;
                default:
                    break;
            }
        }
        else if (activeCell.GetComponent<MeshRenderer>().material.GetTag("SurfaceType", false, default) == "ColourTwo")
        {
            //Debug.Log("AntMove - AntActOnActiveCell, CellColourBLACK, changing to white:");
            if (!activeCell.activeSelf)
            {
                activeCell.SetActive(true);
            }
            activeCell.GetComponent<MeshRenderer>().material = colourOneMaterial;
            Vector3 pos = activeCell.transform.position;
  
            switch (dir)
            {
                case DIRECTION.UP:
                    activeColumn -= 1;
                    dir = DIRECTION.LEFT;
                    break;
                case DIRECTION.DOWN:
                    activeColumn += 1;
                    dir = DIRECTION.RIGHT;
                    break;
                case DIRECTION.LEFT:
                    //move height down, do we also move row? I think not so it changes , try both though
                    activeRow += 1;                    
                    pos.y -= stride;
                    activeCell.transform.position = pos;

                    pos.y -= stride * 2f;
                    activeCell.transform.position = pos;
                    dir = DIRECTION.DOWN;
                    break;
                case DIRECTION.RIGHT:
                    activeRow -= 1;
                    pos.y -= stride * 2f;
                    activeCell.transform.position = pos;
                    dir = DIRECTION.UP;
                    break;
                default:
                    break;
            }
        }
        //Debug.Log("AntMove - about to AntMove(), isRowMove, isPosMove: " + isRowMove + ", " + isPosMove);
        //Debug.Log("AntMove - about to AntMove(), activeRow, activeColumn: " + activeRow + ", " + activeColumn);
        AntMove();
    }
    
    private void OriginalMovement()
    {
        switch (dir)
        {
            case DIRECTION.UP:
                activeColumn -= 1;
                dir = DIRECTION.LEFT;
                break;
            case DIRECTION.DOWN:
                activeColumn += 1;
                dir = DIRECTION.RIGHT;
                break;
            case DIRECTION.LEFT:
                activeRow += 1;
                dir = DIRECTION.DOWN;
                break;
            case DIRECTION.RIGHT:
                activeRow -= 1;
                dir = DIRECTION.UP;
                break;
            default:
                break;
        }
    }
    private void AntMove()
    {
        Debug.Log("AntMove - activeRow, activeColumn: " + activeRow + ", " + activeColumn);
        if (isRowMove)
        {
            if (activeRow >= 1 && activeRow < gridSize - 1)
            {              
                activeCell = cellGrid[activeRow, activeColumn];
            }
        }
        else
        {
            if (activeColumn >= 1 && activeColumn < gridSize-1)
            {               
                activeCell = cellGrid[activeRow, activeColumn];
            }
        }        
    }
}
