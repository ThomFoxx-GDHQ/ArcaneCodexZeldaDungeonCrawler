using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _roomCamera;
    [SerializeField] private List<Renderer> _renderers = new List<Renderer>();

    [SerializeField] private Transform[] _environmentObjects;
    [SerializeField, Tooltip("Exits should be in order of N,E,S,W.")] private ExitController[] _exits;
    private Room _roomInfo;
    private NavMeshSurface _navMeshSurface;
    [SerializeField] private GameObject _enemyPrefab;
    BaseEnemyAI _enemyAi = null;

    private void OnEnable()
    {
        if (_roomCamera == null)
            Debug.LogWarning($"{transform.name}'s Camera is Null", this.gameObject);
        _renderers.AddRange(GetComponentsInChildren<Renderer>(true));
        _navMeshSurface = GetComponent<NavMeshSurface>();
        //RenderRoom(false);
        //RandomizeEnvironment();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RenderRoom(true);
            _roomCamera.Priority = 12;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RenderRoom(false);
            _roomCamera.Priority = 10;
        }
    }

    private void RenderRoom(bool active)
    {
        foreach (var renderer in _renderers)
        {
            renderer.enabled = active;
            _enemyAi = renderer.GetComponentInParent<BaseEnemyAI>();
            if (_enemyAi != null)
            {
                //Debug.Log("Found EnemyAi");
                _enemyAi.EnableAttack(active);
            }
        }
    }

    public void RandomizeEnvironment()
    {
        float rng = 0;

        //Spawn Enemies in the Room

        Transform[] points = transform.Find("Patrol Route").GetComponentsInChildren<Transform>();

        /*Transform spawnPoint = points[Random.Range(0, points.Length)];
        _renderers.AddRange(Instantiate(_enemyPrefab, spawnPoint.position, Quaternion.identity, transform).GetComponentsInChildren<Renderer>());*/ 

        foreach(Transform t in _environmentObjects)
        {
            rng = Random.value;
            if (rng >= 0.75f)
                t.gameObject.SetActive(true);
            else t.gameObject.SetActive(false);
        }
        RenderRoom(false);
    }

    public void SetRoom(Room room)
    {
        _roomInfo = room;
        for (int i = 0;  i < _roomInfo.Exits.Length; i++)
        {
            _exits[i].SetExit(_roomInfo.Exits[i]);
        }
    }

    public IEnumerator BuildNavMesh()
    {
        yield return new WaitForEndOfFrame();
        _navMeshSurface.BuildNavMesh();
    }

    public void RemoveRenderer(Renderer renderer)
    {
        if (_renderers.Contains(renderer))
            _renderers.Remove(renderer);
    }
}
