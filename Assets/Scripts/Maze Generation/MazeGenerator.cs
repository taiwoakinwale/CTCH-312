using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _mazeCellPrefab;

    [SerializeField] private int _mazeWidth;

    [SerializeField] private int _mazeDepth;

    [SerializeField] private int _seed;

    [SerializeField] private bool _useSeed;

    private MazeCell[,] _mazeGrid;

    private const int RANDOM_RANGE_MIN = 1;
    private const int RANDOM_RANGE_MAX = 10;

    [SerializeField] private int START_X;
    [SerializeField] private int START_Z;

    void Start()
    {
        InitializeRandomSeed();
        InitializeMazeGrid();
        GenerateMaze(null, _mazeGrid[0, 0]);
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    // This give a random seed for the maze and if allows the possiblitity to input a spacific seed to get the same maze again
    private void InitializeRandomSeed()
    {
        if (_useSeed)
        {
            Random.InitState(_seed);
        }
        else
        {
            int randomSeed = Random.Range(1, 1000000);
            Random.InitState(randomSeed);
            Debug.Log(randomSeed);
        }
    }

    // Creates a layout for the size of the maze
    private void InitializeMazeGrid()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                Vector3 position = new Vector3(START_X + x, 0, START_Z + z);
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, position, Quaternion.identity, transform);
                _mazeGrid[x, z].transform.localPosition = position;
            }
        }
    }

    // Goes through and creates a maze through a path finding algorithm
    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    //If the maze generator runs into a dead end, it will return to the last maze on it's path that still has unvisited directions
    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => Random.Range(RANDOM_RANGE_MIN, RANDOM_RANGE_MAX)).FirstOrDefault();
    }

    // Coroutine to help the GetNextUnvisitedCell
    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.localPosition.x - START_X;
        int z = (int)currentCell.transform.localPosition.z - START_Z;

        if (IsInBounds(x + 1, z) && !_mazeGrid[x + 1, z].IsVisited)
        {
            yield return _mazeGrid[x + 1, z];
        }

        if (IsInBounds(x - 1, z) && !_mazeGrid[x - 1, z].IsVisited)
        {
            yield return _mazeGrid[x - 1, z];
        }

        if (IsInBounds(x, z + 1) && !_mazeGrid[x, z + 1].IsVisited)
        {
            yield return _mazeGrid[x, z + 1];
        }

        if (IsInBounds(x, z - 1) && !_mazeGrid[x, z - 1].IsVisited)
        {
            yield return _mazeGrid[x, z - 1];
        }
    }

    // Checks to make sure we don't try to create a maze that has a greater width or height than was originally defined
    private bool IsInBounds(int x, int z)
    {
        return x >= 0 && x < _mazeWidth && z >= 0 && z < _mazeDepth;
    }


    // As we go through createing paths in the maze we want to clear the wall that we originally came from
    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.localPosition.x < currentCell.transform.localPosition.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
        }
        else if (previousCell.transform.localPosition.x > currentCell.transform.localPosition.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
        }
        else if (previousCell.transform.localPosition.z < currentCell.transform.localPosition.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
        }
        else if (previousCell.transform.localPosition.z > currentCell.transform.localPosition.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
        }
    }
}
