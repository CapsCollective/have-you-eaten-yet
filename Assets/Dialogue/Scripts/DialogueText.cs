using System;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogueText : MonoBehaviour
{
    [SerializeField] private Sprite downArrowSprite, upArrowSprite;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image boxImage;
    [SerializeField] private float fadeInTime = 1.0f;
    [SerializeField] private float sineStrength = 3.0f;
    
    private const float DumplingFadeOutTime = 1.5f;
    private const float RestaurantFadeOutTime = 3.0f;
    private const float DumplingFadeOutDistance = 5f;
    private const float RestaurantFadeOutDistance = 35f;

    private bool _inRestaurant;
    
    private bool _startFloat;
    private float _sine;
    private float _startX;
    private CanvasGroup _canvasGroup;
    private RestaurantDialogueView _dialogueView;
    private Action _dialogueAction;
    private bool _flipY;
    
    
    public void Setup(RestaurantDialogueView dv)
    {
        _dialogueView = dv;
        _dialogueView.OnNewRestaurantDialogue += OnNewDialogue;
        _inRestaurant = true;
    }
    
    public void Setup(DumplingDialogueView dv)
    {
        DumplingDialogueView.OnNewDumplingDialogue += OnNewDialogue;
        _inRestaurant = false;
    }
    
    public void HookAction(Action onDialogueFinished)
    {
        _dialogueAction = onDialogueFinished;
    }

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.DOFade(1, fadeInTime);
    }

    private void OnNewDialogue()
    {
        if(_dialogueView != null)
            _dialogueView.OnNewRestaurantDialogue -= OnNewDialogue;
        if (_startFloat) return;
        _startFloat = true;

        _startX = transform.localPosition.x;
        float fadeOutDistance = (_flipY ? -1 : 1) * (_inRestaurant ? RestaurantFadeOutDistance : DumplingFadeOutDistance);
        float fadeOutTime = _inRestaurant ? RestaurantFadeOutTime : DumplingFadeOutTime;
        transform.DOLocalMoveY(fadeOutDistance, fadeOutTime).SetEase(Ease.InSine).OnUpdate(() => {
            transform.localPosition = new Vector3(_startX + (Mathf.Sin(_sine) * sineStrength), transform.localPosition.y);
        });
        GetComponent<CanvasGroup>().DOFade(0, fadeOutTime).SetEase(Ease.InCubic).OnComplete(() => 
        {
            _dialogueAction.Invoke();
            Destroy(gameObject);
        });
    }

    public void SetSpriteFlip(bool x, bool y)
    {
        transform.localScale = new Vector3(x ? -1 : 1, 1, 1);
        text.transform.localScale = new Vector3(x ? -1 : 1, 1, 1);
        boxImage.sprite = y ? upArrowSprite : downArrowSprite;
        text.margin = new Vector4(0, y ? 4 : 0, 0, y ? 0 : 4);
        _flipY = y;
    }

    public void SetText(string t)
    {
        text.text = t;
    }

    public void SetFont(TMP_FontAsset font)
    {
        if (font) text.font = font;
    }

    public void Update()
    {
        if(_startFloat)
            _sine += Time.deltaTime * 8 / (_inRestaurant ? RestaurantFadeOutTime : DumplingFadeOutTime);
    }

    public void RevealText(int pos)
    {
        text.maxVisibleCharacters = pos;
    }
}
