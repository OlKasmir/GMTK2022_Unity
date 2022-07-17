using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
  #region Singleton
  private static GameManager _instance;
  public static GameManager Instance {
    get {
      if (_instance != null)
        return _instance;

      _instance = FindObjectOfType<GameManager>();
      return _instance;
    }
  }

  public GameObject Player { get => _player; set => _player = value; }
  public GameObject Spawn { get => _spawn; set => _spawn = value; }
  public GameObject Finish { get => _finish; set => _finish = value; }

  private void Awake() {
    if (_instance != null && _instance != this) {
      Destroy(this.gameObject);
    } else {
      _instance = this;
      Init();
    }
  }
  #endregion

  [SerializeField]
  private CanvasGroup canvasGroupGameOver;

  [SerializeField]
  private List<GameObject> _keepOnSceneChange;
  [SerializeField, Tooltip("All Levels in the order supposed to be played")]
  private List<string> _sceneNamesInOrder;
  [SerializeField]
  private List<AudioClip> _musicTracks;
  [SerializeField, HideInInspector]
  private int _currentSceneCount = 0;
  [SerializeField]
  private AudioClip _deathMusic;

  [SerializeField]
  private GameObject _player;

  [SerializeField, HideInInspector]
  private GameObject _spawn;
  [SerializeField, HideInInspector]
  private GameObject _finish;

  private bool _loadingScene = false;

  void Init() {
    DontDestroyOnLoad(gameObject);
    
    foreach(GameObject go in _keepOnSceneChange) {
      DontDestroyOnLoad(go);
    }

    FinishScene();
  }

  public void FinishScene(bool uncompleted = false) {
    if (_loadingScene)
      return;

    if(_currentSceneCount >= _sceneNamesInOrder.Count) {
      FinishGame();
      return;
    }

    if (gameObject.transform.parent != null) {

    } else {
      _loadingScene = true;
      string sceneName = _sceneNamesInOrder[_currentSceneCount];
      StartCoroutine(LoadNextSceneAsync(sceneName, uncompleted));
    }
  }

  public void FinishGame() {
    if (gameObject.transform.parent != null) {
      Player.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
      UpdateMapConfiner();
      return;
    }

    foreach(GameObject go in _keepOnSceneChange) {
      SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
    }
    // SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

    //SceneManager.LoadScene("StartScene", LoadSceneMode.Single);

    StartCoroutine(LoadStartSceneAsync());
  }

  public void Respawn() {
    _currentSceneCount--;
    canvasGroupGameOver.alpha = 0.0f;
    canvasGroupGameOver.blocksRaycasts = false;
    canvasGroupGameOver.interactable = false;

    FinishScene(true);
    Player.GetComponent<PlayerMovement>().ApplyMovementBlockTime(0);

  }

  public void MainMenu() {
    foreach (GameObject go in _keepOnSceneChange) {
      SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
    }

    StartCoroutine(LoadStartSceneAsyncMainMenu());
  }

  public IEnumerator LoadNextSceneAsync(string sceneName, bool uncompleted = false) {
    AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

    while (!async.isDone) {
      yield return null;
    }

    _currentSceneCount++;
    Debug.Log($"Now in Scene {sceneName}");

    InitScene();


    _loadingScene = false;

    //StartCoroutine(InvokeDelayed(0.1f));
    // InitScene();

    if (!uncompleted)
      AudioManager.Instance.PlaySound("LevelComplete");
  }

  public IEnumerator LoadStartSceneAsyncMainMenu() {
    AsyncOperation async = SceneManager.LoadSceneAsync("StartScene", LoadSceneMode.Single);

    while (!async.isDone) {
      yield return null;
    }

    Destroy(gameObject);
  }

  public IEnumerator LoadStartSceneAsync() {
    AsyncOperation async = SceneManager.LoadSceneAsync("StartScene", LoadSceneMode.Single);

    while(!async.isDone) {
      yield return null;
    }

    UiController c = FindObjectOfType<UiController>();
    c.GameWon();

    yield return new WaitForSeconds(0.5f);
    c.GameWon();

    Destroy(gameObject);
  }

  //public IEnumerator InvokeDelayed(float delay) {
  //  yield return new WaitForSeconds(delay);
  //  InitScene();


  //  _loadingScene = false;
  //}

  public void InitScene() {
    Spawn  = GameObject.FindGameObjectWithTag("Respawn");
    Finish = GameObject.FindGameObjectWithTag("Finish");

    UpdateMapConfiner();




    Camera.main.gameObject.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().enabled = false;
    Camera.main.transform.position = Spawn.transform.position;
    _player.transform.position = Spawn.transform.position;
    Camera.main.gameObject.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().enabled = true;

    StartCoroutine(ToSpawnDelayed());

    AudioManager.Instance.PlayMusic(_musicTracks[(_currentSceneCount - 1) % (_musicTracks.Count)]);
  }

  public IEnumerator ToSpawnDelayed() {
    yield return new WaitForSeconds(0.1f);

    Camera.main.gameObject.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().enabled = false;
    Camera.main.transform.position = Spawn.transform.position;
    _player.transform.position = Spawn.transform.position;
    Camera.main.gameObject.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().enabled = true;
  }

  public void UpdateMapConfiner() {
    GameObject mapConfiner = GameObject.FindGameObjectWithTag("CameraBounds");
    if (mapConfiner != null) {
      Collider2D mapConfinerCollider = mapConfiner.GetComponent<Collider2D>();

      if (mapConfinerCollider != null) {
        Cinemachine.CinemachineConfiner2D confiner = Camera.main.GetComponentInChildren<Cinemachine.CinemachineConfiner2D>();
        if (confiner != null) {
          confiner.m_BoundingShape2D = mapConfinerCollider;
        } else {
          Debug.LogWarning("No Confiner Extension found on Camera");
        }
      } else {
        Debug.LogWarning("No Collider on Camera Bounds!");
      }



    } else {
      Debug.LogWarning("No Camera Bounds found");
    }
  }

  public void LoadScene(string sceneName) {
    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
  }

  public void GameOver() {
    AudioManager.Instance.PlaySound("PlayerDeath");

    if (gameObject.transform.parent == null) {
      canvasGroupGameOver.alpha = 1.0f;
      canvasGroupGameOver.blocksRaycasts = true;
      canvasGroupGameOver.interactable = true;
      Player.GetComponent<PlayerMovement>().ApplyMovementBlockTime(1000);
      AudioManager.Instance.PlayMusic(_deathMusic);

    } else {
      Respawn();
    }
  }
}
