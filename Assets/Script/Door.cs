using UnityEngine;

public class Door : MonoBehaviour, TriggeredObject
{
    private Animator _anim;
    [SerializeField] AnimationClip _animation;
    private bool _isOpen;
    [SerializeField] private Collider _collider;

    private void Start()
    {
        _anim = GetComponent<Animator>();       
    }

    public void TriggeredAction()
    {
        _anim.Play(_animation.name);
        _isOpen = true;
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isOpen)
        {
            Debug.Log("Exit Floor");
        }
        else
        {
            Debug.Log("Key not found");
        }
    }
}
