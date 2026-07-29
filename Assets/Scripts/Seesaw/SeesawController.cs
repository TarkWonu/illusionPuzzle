using DG.Tweening;
using UnityEngine;

public enum SeesawState { LeftUp, RightUp }

public class SeesawController : MonoBehaviour
{
    [Header("연결된 블록 2개")]
    [SerializeField] private SeesawPlatform leftPlatform;
    [SerializeField] private SeesawPlatform rightPlatform;

    [Header("레버")]
    [SerializeField] private Transform lever;
    [SerializeField] private Vector3 leverPosUp;
    [SerializeField] private Vector3 leverPosDown;

    [Header("동작 설정")]
    [SerializeField] private float snapDuration = 0.25f;
    [Tooltip("레버를 끝까지 당겨야할 값")]
    [SerializeField] private float dragPixelRange = 250f;

    public SeesawState state = SeesawState.LeftUp;

    private bool dragging;
    private float dragStartY;
    private Sequence activeSeq;

    void Start()
    {
        ApplyImmediate(state);
    }

    void ApplyImmediate(SeesawState s)
    {
        leftPlatform.transform.position = s == SeesawState.LeftUp ? leftPlatform.PosUp : leftPlatform.PosDown;
        rightPlatform.transform.position = s == SeesawState.LeftUp ? rightPlatform.PosDown : rightPlatform.PosUp;
        if (lever != null)
            lever.localPosition = s == SeesawState.LeftUp ? leverPosUp : leverPosDown;
    }

    bool CanInteract()
    {
        return RoadConnectManager.Instance.state == GameStates.idle;
    }

    public void BeginDrag()
    {
        if (!CanInteract()) return;

        activeSeq?.Kill();
        dragging = true;
        dragStartY = Input.mousePosition.y;
        RoadConnectManager.Instance.state = GameStates.bridgeMoving;
    }

    public void UpdateDrag()
    {
        if (!dragging) return;
        float t = GetDragT();

        Vector3 leftFrom = state == SeesawState.LeftUp ? leftPlatform.PosUp : leftPlatform.PosDown;
        Vector3 leftTo = state == SeesawState.LeftUp ? leftPlatform.PosDown : leftPlatform.PosUp;
        leftPlatform.transform.position = Vector3.Lerp(leftFrom, leftTo, t);

        Vector3 rightFrom = state == SeesawState.LeftUp ? rightPlatform.PosDown : rightPlatform.PosUp;
        Vector3 rightTo = state == SeesawState.LeftUp ? rightPlatform.PosUp : rightPlatform.PosDown;
        rightPlatform.transform.position = Vector3.Lerp(rightFrom, rightTo, t);

        if (lever != null)
        {
            Vector3 leverFrom = state == SeesawState.LeftUp ? leverPosUp : leverPosDown;
            Vector3 leverTo = state == SeesawState.LeftUp ? leverPosDown : leverPosUp;
            lever.localPosition = Vector3.Lerp(leverFrom, leverTo, t);
        }
    }

    public void EndDrag()
    {
        if (!dragging) return;
        dragging = false;

        float t = GetDragT();
        bool flip = t >= 0.98f;
        SeesawState target = flip ? (state == SeesawState.LeftUp ? SeesawState.RightUp : SeesawState.LeftUp) : state;

        Vector3 leftTarget = target == SeesawState.LeftUp ? leftPlatform.PosUp : leftPlatform.PosDown;
        Vector3 rightTarget = target == SeesawState.LeftUp ? rightPlatform.PosDown : rightPlatform.PosUp;

        activeSeq = DOTween.Sequence();
        activeSeq.Join(leftPlatform.transform.DOMove(leftTarget, snapDuration).SetEase(Ease.OutQuad));
        activeSeq.Join(rightPlatform.transform.DOMove(rightTarget, snapDuration).SetEase(Ease.OutQuad));

        if (lever != null)
        {
            Vector3 leverTarget = target == SeesawState.LeftUp ? leverPosUp : leverPosDown;
            activeSeq.Join(lever.DOLocalMove(leverTarget, snapDuration).SetEase(Ease.OutQuad));
        }

        activeSeq.OnComplete(() =>
        {
            state = target;
            RoadConnectManager.Instance.state = GameStates.idle;
            if (flip) RoadConnectManager.Instance.connectRoad?.Invoke();
        });
    }

    float GetDragT()
    {
        float delta = Input.mousePosition.y - dragStartY;
        float dragDir = state == SeesawState.LeftUp ? -1f : 1f;
        return Mathf.Clamp01((delta * dragDir) / dragPixelRange);
    }
}