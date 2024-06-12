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

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => Random.Range(RANDOM_RANGE_MIN, RANDOM_RANGE_MAX)).FirstOrDefault();
    }

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

    private bool IsInBounds(int x, int z)
    {
        return x >= 0 && x < _mazeWidth && z >= 0 && z < _mazeDepth;
    }

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
