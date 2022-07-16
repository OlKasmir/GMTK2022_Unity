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
  private List<GameObject> _keepOnSceneChange;
  [SerializeField, Tooltip("All Levels in the order supposed to be played")]
  private List<string> _sceneNamesInOrder;
  [SerializeField, HideInInspector]
  private int _currentSceneCount = 0;

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

  public void FinishScene() {
    if (_loadingScene)
      return;

    if(_currentSceneCount >= _sceneNamesInOrder.Count) {
      FinishGame();
      return;
    }

    _loadingScene = true;
    string sceneName = _sceneNamesInOrder[_currentSceneCount];
    LoadScene(sceneName);
    _currentSceneCount++;

    Debug.Log($"Now in Scene {sceneName}");

    StartCoroutine(InvokeDelayed(0.1f));
    // InitScene();
  }

  public void FinishGame() {
    Debug.Log("Game Won");
  }

  public IEnumerator InvokeDelayed(float delay) {
    yield return new WaitForSeconds(delay);
    InitScene();


    _loadingScene = false;
  }

  public void InitScene() {
    _spawn  = GameObject.FindGameObjectWithTag("Respawn");
    _finish = GameObject.FindGameObjectWithTag("Finish");

    _player.transform.position = _spawn.transform.position;
  }

  public void LoadScene(string sceneName) {
    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
  }
}
