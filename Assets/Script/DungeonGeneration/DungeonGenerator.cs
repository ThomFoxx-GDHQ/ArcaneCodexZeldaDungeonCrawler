using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class DungeonGenerator : MonoBehaviour
{
    private Room[,] _roomGrid;
    [SerializeField] private int _gridXSize;
    [SerializeField] private int _gridYSize;
    private int _gridSize;
    [SerializeField] private List<Room> _rooms = new List<Room>();

    private Vector2Int _currentRoom;
    private Vector2Int _targetRoom;
    [SerializeField]
    private List<Room> _visitedRooms = new List<Room>();
    private List<Vector2Int> _path = new List<Vector2Int>();

    private Vector2Int _falseRoom = new Vector2Int(-1, -1); //Subsitutes for null checks

    [SerializeField] private Transform _rootObject;
    [SerializeField] private GameObject _roomObject;
    [SerializeField] private int _roomSize = 1;

    [SerializeField] private GameObject _player;

    Stopwatch _stopWatch;

    [SerializeField] private float _openWallChance = .1f;
    [SerializeField] private float _voidRoomChance = .1f;
    List<RoomController> _roomControllers;
    private void Start()
    {
        _roomGrid = new Room[_gridXSize, _gridYSize];
        _gridSize = _gridXSize * _gridYSize;
        
        for (int row = 0; row < _gridXSize;row++)
        {
            for (int col = 0; col < _gridYSize;col++)
            {
                _roomGrid[row,col] = new Room($"{row}-{col}", row, col);
               /* if (Random.value <= _voidRoomChance)
                {
                    _roomGrid[row, col].CreateVoid();
                    _visitedRooms.Add(_roomGrid[row, col]);
                }*/

                _rooms.Add(_roomGrid[row,col]); //Just so we can look at in the inspector
            }
        }

        /*foreach (Room r in _rooms)
            Debug.Log($"Room: {r.Name}");*/
        _stopWatch = new Stopwatch();
        GenerateDungeon();
    }

    //Pick start Position

    //Pick a random direction
    //check if random Direction valid
    //Valid is within grid borders and not visited
    //Check target against grid Borders 
    //Check if Target Visited
    //Move to Target
    //Mark target as visited
    //Repeat

    //If no valid Direction Got back to previous

    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        int rndX = Random.Range(0, _gridXSize);
        int rndY = Random.Range(0, _gridYSize);
        _currentRoom = new Vector2Int(rndX,rndY);
        _visitedRooms.Clear();
        _visitedRooms.Add(_roomGrid[_currentRoom.x, _currentRoom.y]);
        //_visitedRooms[0].VisitRoom();
        _path.Clear();
        _path.Add(_currentRoom);    

        Debug.Log($"Starting Room is {_roomGrid[_currentRoom.x, _currentRoom.y].Name}.");

        StartCoroutine(DungeonRoutine());
    }

    IEnumerator DungeonRoutine()
    {
        _stopWatch.Start();
        while (_visitedRooms.Count < _gridSize && _path.Count > 0)
        {
            _targetRoom = ValidRandomDirection(_currentRoom);
            if (_targetRoom != _falseRoom)
            {
                _roomGrid[_currentRoom.x, _currentRoom.y].VisitRoom(GetDirection(_currentRoom, _targetRoom));
                _roomGrid[_targetRoom.x, _targetRoom.y].VisitRoom(GetDirection(_targetRoom, _currentRoom));
                _path.Add(_targetRoom);
                _currentRoom = _targetRoom;
                _visitedRooms.Add(_roomGrid[_currentRoom.x, _currentRoom.y]);
            }
            else
            {
                _path.Remove(_currentRoom);
                _currentRoom = _path[_path.Count - 1];
            }
        }

        RandomOpenPaths();

        StartCoroutine(BuildDungeon(_visitedRooms));

        //Time to Build Metrics
        _stopWatch.Stop();

        yield return null;
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = _stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Debug.Log($"Build Time: {elapsedTime}");
    }

    private void RandomOpenPaths()
    {
        foreach (Room room in _rooms)
        {
            for (int i = 0; i < 4; i++)
            {
                switch ((DungeonDirections)i)
                {
                    case DungeonDirections.North:
                        if (room.Y < _gridYSize - 1 && Random.value <= _openWallChance)
                        {
                            room.OpenExit((DungeonDirections)i);
                            _roomGrid[room.X, room.Y + 1].OpenExit(GetDirection(_roomGrid[room.X, room.Y + 1].GetGridPosition(), room.GetGridPosition()));
                        }
                        break;
                    case DungeonDirections.East:
                        if (room.X < _gridXSize - 1 && Random.value <= _openWallChance)
                        {
                            room.OpenExit((DungeonDirections)i);
                            _roomGrid[room.X + 1, room.Y].OpenExit(GetDirection(_roomGrid[room.X + 1, room.Y].GetGridPosition(), room.GetGridPosition()));
                        }
                        break;
                    case DungeonDirections.South:
                        if (room.Y > 0 && Random.value <= _openWallChance)
                        {
                            room.OpenExit((DungeonDirections)i);
                            _roomGrid[room.X, room.Y - 1].OpenExit(GetDirection(_roomGrid[room.X, room.Y - 1].GetGridPosition(), room.GetGridPosition()));
                        }
                        break;
                    case DungeonDirections.West:
                        if (room.X > 0 && Random.value <= _openWallChance)
                        {
                            room.OpenExit((DungeonDirections)i);
                            _roomGrid[room.X - 1, room.Y].OpenExit(GetDirection(_roomGrid[room.X - 1, room.Y].GetGridPosition(), room.GetGridPosition()));
                        }
                        break;
                }
            }
        }
    }

    private Vector2Int ValidRandomDirection(Vector2Int currentPOS)
    {
        List<Vector2Int> directions = new List<Vector2Int>();
        Vector2Int CheckingDirection = currentPOS;
        for (int i = 0; i < 4; i++)
        {
            CheckingDirection = currentPOS;
            switch ((DungeonDirections)i)
            {
                case DungeonDirections.North:
                    if (CheckingDirection.y + 1 < _gridYSize &&
                        !_roomGrid[CheckingDirection.x, CheckingDirection.y + 1].Visited)
                    {
                        CheckingDirection.y += 1;
                        directions.Add(CheckingDirection);
                    }
                    break;
                case DungeonDirections.East:
                    if (CheckingDirection.x + 1 < _gridXSize && !_roomGrid[CheckingDirection.x + 1, CheckingDirection.y].Visited )
                    {
                        CheckingDirection.x += 1;
                        directions.Add(CheckingDirection);
                    }
                    break;
                case DungeonDirections.South:
                    if (CheckingDirection.y - 1 >= 0 &&
                        !_roomGrid[CheckingDirection.x, CheckingDirection.y - 1].Visited)
                    {
                        CheckingDirection.y -= 1;
                        directions.Add(CheckingDirection);
                    }
                    break;
                case DungeonDirections.West:
                    if (CheckingDirection.x - 1 >= 0 &&
                        !_roomGrid[CheckingDirection.x - 1, CheckingDirection.y].Visited)
                    {
                        CheckingDirection.x -= 1;
                        directions.Add(CheckingDirection);
                    }
                    break;
                default:
                    break;
            }
        }


        return directions.Count > 0 ? directions[Random.Range(0, directions.Count)] : _falseRoom;
    }

    IEnumerator BuildDungeon(List<Room> grid)
    {
        yield return null;
        _roomControllers = new List<RoomController>();
        Vector3 pos = Vector3.zero;
        foreach (Room room in grid)
        {

            pos.x = room.X * _roomSize;
            pos.z = room.Y * _roomSize;
            RoomController rc = Instantiate(_roomObject, pos, Quaternion.identity,_rootObject).GetComponent<RoomController>();
            rc.SetRoom(_roomGrid[room.X, room.Y]);
            _roomControllers.Add(rc);
        }

        if (_roomControllers.Count > 0)
        {
            yield return StartCoroutine(_roomControllers[0].BuildNavMesh());

            foreach (RoomController rc in _roomControllers)
                rc.RandomizeEnvironment();
        }


        if (_player != null)
        {
            pos.x = grid[0].X * _roomSize;
            pos.z = grid[0].Y * _roomSize;
            pos.y = 1.05f;

            _player.GetComponent<CharacterController>().enabled = false;
            _player.transform.position = pos;
            _player.GetComponent<CharacterController>().enabled = true;
        }
    }

    private DungeonDirections GetDirection(Vector2Int current, Vector2Int target)
    {
        var diff = target - current;

        return diff.x switch
        {
            1 => DungeonDirections.East,
            -1 => DungeonDirections.West,
            _ => diff.y switch
            {
                1 => DungeonDirections.North,
                -1 => DungeonDirections.South,
                _ => DungeonDirections.North
            }
        };
    }
}
