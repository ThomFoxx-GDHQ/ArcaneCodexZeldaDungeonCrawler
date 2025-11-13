using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using System.Text;
using System.Runtime.CompilerServices;

public class DialogueHandler : MonoSingleton<DialogueHandler>
{
    [SerializeField] GameObject _panel;
    [SerializeField] TMP_Text _text;
    [SerializeField] Image _speakerImage;
    [SerializeField] TMP_Text _speakerName;
    [SerializeField] Image _canAdvanceImage;
    [SerializeField] float _dialogueTypeSpeed;
    float _speed;

    InputSystem_Actions _input;

    DialogueDictionary _dialogue;
    [SerializeField] DialogueSequence _sequence;
    StringBuilder _sentenceBuilder = new StringBuilder();

    int _counter = 0;
    Coroutine _dialogueRoutine;
    bool _isDone = false;

    public delegate void OnComplete();
    public static event OnComplete OnDialogueComplete;

    public override void Init()
    {
        transform.parent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _input = new InputSystem_Actions();
        _input.Dialogue.Enable();
        //_input.Player.Disable();

        _input.Dialogue.Submit.performed += Submit_performed;
        _input.Dialogue.SubmitHold.performed += SubmitHold_performed;
        _input.Dialogue.SubmitHold.canceled += SubmitHold_canceled;

        _dialogue = DialogueService.Instance.Dictionary;

        if (_dialogue == null)
            Debug.LogError("Dialogue is Null");

       /* //testing and debugging
        if (_dialogue.ById.Count > 0)
            LoadDialogue(_sequence);*/

        _canAdvanceImage.enabled = false;
        _speed = _dialogueTypeSpeed;
        _isDone = false;
    }

    private void Start()
    {       
        /*LoadDialogue(_sequence);*/
    }

    private void Submit_performed(InputAction.CallbackContext obj)
    {
        if (_dialogueRoutine == null && _isDone == false)
            AdvanceThroughSequence();
        /*else if (_isDone == true)
        {
            _panel.SetActive(false);
            _isDone = false;
        }*/
    }

    private void SubmitHold_canceled(InputAction.CallbackContext obj)
    {
        _speed = _dialogueTypeSpeed;
    }

    private void SubmitHold_performed(InputAction.CallbackContext obj)
    {
        _speed = .01f;
    }

    public void LoadDialogue(DialogueSequence sequence)
    {
        _counter = 0;
        _isDone = false;
        _sequence = sequence;
        gameObject.SetActive(true);
        if (_dialogueRoutine == null)
            _dialogueRoutine = StartCoroutine(LoadDialogueText(_counter));
    }

    public void LoadSpeakerInfo(string name, Sprite portrait)
    {
        _speakerName.text = name;
        _speakerImage.sprite = portrait;
    }

    IEnumerator LoadDialogueText(int id)
    {
        _canAdvanceImage.enabled = false;
        yield return null;
        string sentence = _dialogue.ById[_sequence.dialogueIDs[id]];
        _sentenceBuilder.Clear();
        int count = 0;
        while (count < sentence.Length)
        {
            _sentenceBuilder.Append(sentence[count]);
            _text.text = _sentenceBuilder.ToString();
            count++;
            yield return Helpers.GetWait(_speed);
        }
        _dialogueRoutine = null;
        _canAdvanceImage.enabled = true;
    }

    public void AdjustTypingSpeed(float speed)
    {
        _dialogueTypeSpeed = speed;
    }

    private void AdvanceThroughSequence()
    {
        _counter++;
        _counter %= _sequence.dialogueIDs.Count;
        if (_counter == 0)
        {
            if (OnDialogueComplete != null)
            {
                OnDialogueComplete();
                _isDone = true; 
                //_panel.transform.parent.gameObject.SetActive(false);
            }
            //else
                //_panel.transform.parent.gameObject.SetActive(false);

            return;
        }

        if (_dialogueRoutine == null)
            _dialogueRoutine = StartCoroutine(LoadDialogueText(_counter));
    }

    private void OnDisable()
    {
        _input?.Dialogue.Disable();
        //_input.Player.Enable();
        _text.text = string.Empty;

        if (_input != null)
            _input.Dialogue.Submit.performed -= Submit_performed;

        if (_dialogueRoutine != null)
        {
            StopCoroutine(_dialogueRoutine);
            _dialogueRoutine = null;
        }

    }

    public void ClearOnCompleteEvent()
    {
        OnDialogueComplete = null;
    }
}
