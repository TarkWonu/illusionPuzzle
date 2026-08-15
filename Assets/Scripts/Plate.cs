using DG.Tweening;
using UnityEngine;

public enum PlateMode { Momentary, Toggle }

public class Plate : MonoBehaviour
{
    [Header("모드")]
    [SerializeField] private PlateMode mode = PlateMode.Momentary;

    [Header("이 발판이 놓여있는 길")]
    [SerializeField] private Roads plateRoad;

    [Header("발판을 누르면 이동할 오브젝트")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 posOff;
    [SerializeField] private Vector3 posOn;
    [SerializeField] private float moveDuration = 0.3f;

    [Header("발판 자체의 눌림 연출 (선택)")]
    [SerializeField] private Transform plateVisual;
    [SerializeField] private float plateUpY;
    [SerializeField] private float plateDownY;

    private bool isPressed;
    private bool isLocked;
    private GameStates prevState;
    private Tween targetTween;
    private Tween plateTween;

    void Start()
    {
        target.position = posOff;
        if (plateVisual != null)
        {
            Vector3 p = plateVisual.localPosition;
            p.y = plateUpY;
            plateVisual.localPosition = p;
        }

        prevState = RoadConnectManager.Instance.state;
        EvaluatePlayerOnPlate(); // 씬 시작 시 이미 이 칸 위에 있는 경우 대비
    }

    void Update()
    {
        GameStates cur = RoadConnectManager.Instance.state;

        // 방금 막 idle로 전환된 프레임(=이동이 완전히 끝난 순간)에만 판정
        if (prevState != GameStates.idle && cur == GameStates.idle)
        {
            EvaluatePlayerOnPlate();
        }
        prevState = cur;
    }

    void EvaluatePlayerOnPlate()
    {
        if (mode == PlateMode.Toggle && isLocked) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        bool playerHere = playerObj != null && playerObj.transform.parent == plateRoad.transform;

        if (playerHere == isPressed) return;

        Press(playerHere);
    }

    void Press(bool pressed)
    {
        isPressed = pressed;
        if (mode == PlateMode.Toggle && pressed) isLocked = true;

        targetTween?.Kill();
        Vector3 dest = pressed ? posOn : posOff;
        targetTween = target.DOMove(dest, moveDuration).SetEase(Ease.OutQuad)
            .OnComplete(() => RoadConnectManager.Instance.connectRoad?.Invoke());

        if (plateVisual != null)
        {
            plateTween?.Kill();
            float destY = pressed ? plateDownY : plateUpY;
            plateTween = plateVisual.DOLocalMoveY(destY, moveDuration * 0.5f).SetEase(Ease.OutQuad);
        }
    }
}