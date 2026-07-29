using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DebugConsole : Singleton<DebugConsole>
{
    [Header("설정")]
    [SerializeField] private KeyCode consoleKey = KeyCode.F1;
    [SerializeField] private KeyCode viewModeKey = KeyCode.F2;
    [Tooltip("아이콘이 길 위로 떠 있는 높이")]
    [SerializeField] private float iconHeight = 1.2f;
    [Tooltip("다리 상태 변화를 아이콘에 반영하는 갱신 주기(초)")]
    [SerializeField] private float refreshInterval = 0.3f;

    /// <summary>연결 보기 모드 여부. ObjClickManager가 클릭을 이동으로 넘길지 판단할 때 사용</summary>
    public static bool ConnectionViewMode { get; private set; }

    private bool consoleOpen = false;
    private Roads selectedRoad;         // 현재 검사 중인 길
    private float refreshTimer;
    private Camera cam;

    
    private readonly List<TextMesh> markerPool = new();
    private int activeMarkers = 0;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 바뀌면 이전 씬의 Roads 참조는 파괴되므로 선택을 초기화
        selectedRoad = null;
        HideAllMarkers();
        cam = Camera.main;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(consoleKey)) consoleOpen = !consoleOpen;
        if (Input.GetKeyDown(viewModeKey)) ToggleViewMode();

        // 다리 회전/엘리베이터 이동으로 간선이 바뀔 수 있으므로 주기적으로 아이콘 갱신
        if (ConnectionViewMode && selectedRoad != null)
        {
            refreshTimer -= Time.unscaledDeltaTime;
            if (refreshTimer <= 0f)
            {
                refreshTimer = refreshInterval;
                ShowConnections(selectedRoad);
            }
        }
    }

    void LateUpdate()
    {
        // 아이콘이 항상 카메라를 바라보도록 (빌보드)
        if (activeMarkers == 0) return;
        if (cam == null) { cam = Camera.main; if (cam == null) return; }

        for (int i = 0; i < activeMarkers; i++)
        {
            Transform t = markerPool[i].transform;
            t.rotation = Quaternion.LookRotation(t.position - cam.transform.position);
        }
    }

    void ToggleViewMode()
    {
        ConnectionViewMode = !ConnectionViewMode;
        if (!ConnectionViewMode)
        {
            selectedRoad = null;
            HideAllMarkers();
        }
    }


    public void ShowConnections(Roads clicked)
    {
        if (clicked == null) return;
        selectedRoad = clicked;
        refreshTimer = refreshInterval;

        if (RoadConnectManager.ExistNow &&
            RoadConnectManager.Instance.state == GameStates.idle)
        {
            RoadConnectManager.Instance.connectRoad?.Invoke();
        }

        HideAllMarkers();

        // 선택한 길 : ▼ (노랑)
        PlaceMarker(clicked.transform.position, "▼", Color.yellow);

        if (clicked.connectRoad == null) return;

        foreach (var next in clicked.connectRoad)
        {
            if (next == null) continue; // 파괴된 오브젝트 방어

            bool illusion = clicked.IsIllusionConnectedTo(next);
            PlaceMarker(next.transform.position,
                        illusion ? "★" : "●",
                        illusion ? new Color(0.6f, 0.4f, 1f) : Color.cyan);
        }
    }

    void PlaceMarker(Vector3 roadPos, string symbol, Color color)
    {
        TextMesh tm = GetMarkerFromPool();
        tm.text = symbol;
        tm.color = color;
        tm.transform.position = roadPos + Vector3.up * iconHeight;
        tm.gameObject.SetActive(true);
    }

    TextMesh GetMarkerFromPool()
    {
        // 풀에 여유가 있으면 재사용, 없으면 1개 생성
        if (activeMarkers < markerPool.Count)
        {
            return markerPool[activeMarkers++];
        }

        GameObject go = new GameObject("DebugMarker");
        go.transform.SetParent(transform); // 콘솔과 함께 관리
        TextMesh tm = go.AddComponent<TextMesh>();
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.characterSize = 0.12f;
        tm.fontSize = 80;
        tm.fontStyle = FontStyle.Bold;

        markerPool.Add(tm);
        activeMarkers++;
        return tm;
    }

    void HideAllMarkers()
    {
        foreach (var tm in markerPool)
        {
            if (tm != null) tm.gameObject.SetActive(false);
        }
        activeMarkers = 0;
    }

    
    void OnGUI()
    {
       
        if (GUI.Button(new Rect(10, 10, 120, 28), consoleOpen ? "디버그 닫기" : "디버그 열기(F1)"))
        {
            consoleOpen = !consoleOpen;
        }

        if (!consoleOpen) return;

        GUILayout.BeginArea(new Rect(10, 44, 340, 280), GUI.skin.box);

        GUILayout.Label("<b>== 디버그 콘솔 ==</b>", RichLabel());

       
        if (GUILayout.Button("레벨 강제 스킵 (이전 스테이지)", GUILayout.Height(30)))
        {
            ClearManager.Instance.MovePrevScene();
        }
        if (GUILayout.Button("레벨 강제 스킵 (다음 스테이지)", GUILayout.Height(30)))
        {
            ClearManager.Instance.MoveNextScene();
        }

        
        string modeLabel = ConnectionViewMode ? "연결 보기 모드 끄기 (F2)" : "연결 보기 모드 켜기 (F2)";
        if (GUILayout.Button(modeLabel, GUILayout.Height(30)))
        {
            ToggleViewMode();
        }

        string state = RoadConnectManager.ExistNow
            ? RoadConnectManager.Instance.state.ToString()
            : "(매니저 없음)";
        GUILayout.Label($"게임 상태 : <b>{state}</b>", RichLabel());
        GUILayout.Label($"현재 씬 : {SceneManager.GetActiveScene().name}");

        if (ConnectionViewMode)
        {
            GUILayout.Space(4);
            GUILayout.Label("<b><color=#ffd24a>[연결 보기 모드 ON]</color></b>", RichLabel());
            GUILayout.Label("길을 클릭하면 연결된 길 위에 아이콘 표시\n▼ 선택한 길   ● 일반 간선   ★ 환각 간선", RichLabel());

            if (selectedRoad != null)
            {
                int count = selectedRoad.connectRoad != null ? selectedRoad.connectRoad.Count : 0;
                GUILayout.Label($"선택 : <b>{selectedRoad.name}</b>  (간선 {count}개)", RichLabel());
            }
        }

        GUILayout.EndArea();
    }

    

    GUIStyle RichLabel()
    {
        return new GUIStyle(GUI.skin.label) { richText = true };
    }
}