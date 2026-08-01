using DG.Tweening;
using UnityEngine;

public enum PlateMode { Hold , Toggle }

public class Plate : MonoBehaviour
{
    [Header("모드")]
    [SerializeField] private PlateMode mode = PlateMode.Hold ;

    [Header("발판을 누르면 이동할 오브젝트")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 posOff;
    [SerializeField] private Vector3 posOn;
    [SerializeField] private float moveDuration = 0.3f;

    [Header("눌릴 발판")]
    [SerializeField] private Transform plateVisual;
    [SerializeField] private float plateUpY;
    [SerializeField] private float plateDownY;

    private bool isPressed;
    private bool isLocked; // Toggle 모드에서 한번 눌리면 true로 고정
    private bool hasPending;
    private bool pendingPressed;
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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7) return;
        RequestState(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 7) return;

        if (mode == PlateMode.Toggle) return; // 고정 모드는 벗어나도 무시
        RequestState(false);
    }

    void RequestState(bool pressed)
    {
        if (mode == PlateMode.Toggle && isLocked) return; // 이미 고정됐으면 더 이상 반응 안 함
        if (isPressed == pressed) { hasPending = false; return; }

        pendingPressed = pressed;
        hasPending = true;
    }

    void Update()
    {
        if (!hasPending) return;
        if (RoadConnectManager.Instance.state != GameStates.idle) return;

        hasPending = false;
        Press(pendingPressed);
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