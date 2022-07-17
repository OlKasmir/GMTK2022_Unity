using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimatorSimple : MonoBehaviour {
  #region Events
  public delegate void OnAnimatorEnd();
  public event OnAnimatorEnd AnimatorEnd;
  #endregion

  #region References
  #endregion

  #region Variables
  [SerializeField]
  private float _duration;

  [SerializeField]
  private bool _destroyWhenFinished = false;

  [SerializeField]
  private Texture2D _tex;

  [SerializeField]
  private Sprite[] _sprites;

  private SpriteRenderer _spriteRenderer;
  private int index = 0;
  private float timer = 0;
  //private double _editorStartTime;
  private double _editorLastTickTime;
  #endregion

  #region Getter and Setter
  #endregion

  #region Unity Methods


  void Start() {
    _spriteRenderer = GetComponent<SpriteRenderer>();

    //if(Application.isEditor)
    //  _editorStartTime = EditorApplication.timeSinceStartup;
  }

  private void OnValidate() {
    if (_tex == null)
      return;

    string texturePath = AssetDatabase.GetAssetPath(_tex);
    _sprites = AssetDatabase.LoadAllAssetsAtPath(texturePath).OfType<Sprite>().ToArray();
  }

  private void OnRenderObject() {
    UpdateAnimation();
  }

  private void Update() {
    UpdateAnimation();
  }
  #endregion

  #region Methods
  public void Show() {
    index = 0;
    _spriteRenderer.enabled = true;
  }

  public void Hide() {
    _spriteRenderer.enabled = false;
  }

  public void UpdateAnimation() {
    if (!_spriteRenderer.enabled)
      return;
    
    if (_sprites == null)
      return;

    if (_sprites.Length == 0)
      return;

    bool indexReset = false;
    if (index > _sprites.Length) {
      index = 0;
      indexReset = true;
    }

    if (Application.isEditor && !Application.isPlaying) {
      //if (_editorLastTickTime == 0)
      //  _editorLastTickTime = EditorApplication.timeSinceStartup;

      if (EditorApplication.timeSinceStartup - _editorLastTickTime >= (_duration / _sprites.Length)) {
        _editorLastTickTime = EditorApplication.timeSinceStartup;
        _spriteRenderer.sprite = _sprites[index];
        index = (index + 1) % _sprites.Length;
      }

    } else {
      timer += Time.deltaTime;
      if (timer >= (_duration / _sprites.Length)) {
        if (_destroyWhenFinished && index == _sprites.Length - 1) {
          // Debug.Log("Destroying...");
          Destroy(gameObject);
        }

        timer = 0;
        _spriteRenderer.sprite = _sprites[index];
        index = (index + 1) % _sprites.Length;

        if (index == 0) {
          indexReset = true;
        }
      }
    }

    if (indexReset) {
      AnimatorEnd?.Invoke();
    }
  }
  #endregion

  #region Coroutines
  #endregion

  #region Operators
  #endregion
}