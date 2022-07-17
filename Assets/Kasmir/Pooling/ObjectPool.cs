using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling {
  public class ObjectPool : MonoBehaviour {
    #region Variables
    [Header("Pool - Main Settings")]
    [SerializeField]
    protected GameObject _poolPrefab;
    [SerializeField]
    [Tooltip("Parent of the Pool Objects. If null it's the ObjectPool")]
    protected Transform _parent;

    [Header("Pool Size")]
    [SerializeField]
    [Tooltip("This is the amount of Objects created initially")]
    protected int _initialSize = 0;
    [SerializeField]
    [Tooltip("The maximum amount of concurrent Objects in this Pool")]
    protected int _maxSize = 10;
    [SerializeField]
    [Tooltip("The amount of dynamic existing Objects over maxSize which are destroyed if not necessary")]
    protected int _overflowSize = 0;
    [SerializeField]
    [Tooltip("If set then the ObjectPool will recycle the oldest Object when full. Even though the object is still active." +
             "\nIf false then no object will be created.")]
    protected bool _recycleActiveObjects = true;

    /// <summary>
    /// All Objects in this ObjectPool
    /// </summary>
    protected List<GameObject> _poolObjects;
    #endregion

    #region Getter and Setter
    public GameObject PoolPrefab {
      get => _poolPrefab;
      set {
        _poolPrefab = value;

        if (_poolObjects.Count > 0) {
          Debug.LogWarning("Changed Pool Prefab on Runtime.");
        }
      }
    }
    public Transform Parent { 
      get { 
        if (_parent != null) 
          return _parent; 

        return transform; 
      }
      set => _parent = value; 
    }
    public int Count => _poolObjects.Count;
    public int MaxSize {
      get => _maxSize;
      set {
        _maxSize = value;

        if (Count > _maxSize) {
          // TODO: Clear all PoolObjects above MaxSize
        }
      }
    }
    #endregion

    #region Unity Methods
    protected void Awake() {
      _poolObjects = new List<GameObject>(_maxSize);

      Init();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes the ObjectPool
    /// </summary>
    protected void Init() {
      for (int i = 0; i < _initialSize; i++) {
        GameObject obj = Create(false);
      }
    }

    /// <summary>
    /// Creates a new PoolObject and enables it if specified
    /// </summary>
    protected GameObject Create(bool enabled) {
      GameObject obj = Instantiate(_poolPrefab, Parent);
      IPoolable poolable = obj.GetComponent<IPoolable>();

      if(poolable == null) {
        Debug.LogWarning("Can't create Pool Object which doesn't implement IPoolable! Destroying pool...");
        Destroy(gameObject);
      }

      poolable.Finished += ObjectFinishedEvent;

      // Insert new object on index 0
      _poolObjects.Insert(0, obj);

      if (enabled) {
        Enable(obj);
      } else {
        obj.gameObject.SetActive(false);
      }

      // _poolObjects.Insert(0, obj);

      return obj;
    }

    /// <summary>
    /// Returns an unused pool object or instantiates one if necessary (Or null if recycling active objects is off)
    /// </summary>
    /// <param name="forceFetch">if set then it will return a new pool object even if it's full</param>
    public GameObject Fetch(bool forceFetch = false) {
      // Object Pool is empty: Create a object and return
      if (_poolObjects.Count == 0)
        return Create(true);

      // Return the last object of the pool which should be inactive if not all in use
      GameObject poolObject = _poolObjects[_poolObjects.Count - 1];
      if (!poolObject.gameObject.activeInHierarchy) {
        Enable(poolObject);
        return poolObject;
      }

      // Pool is full
      if (_poolObjects.Count >= _maxSize) {

        // Check if overflow pool is full
        if (_poolObjects.Count >= _maxSize + _overflowSize) {
          // Recycle oldest object even though it's still active
          if (_recycleActiveObjects) {
            Enable(poolObject);
            return poolObject;

            // No Space for a new Object and all objects in use. Return null
          } else {
            // If set then we have to create a new Object even though the pool is full
            if (forceFetch) {
              return Create(true);
            // No space. Return null
            } else {
              return null;
            }
          }


          // Overflow pool is not full. Create new object
        } else {
          return Create(true);
        }
      }

      // Pool is not full: Create 
      return Create(true);
    }


    protected void Enable(GameObject obj) {
      // Shift to start of list
      _poolObjects.MoveItem(obj, 0);

      if(obj.gameObject.activeInHierarchy) {
        //obj.OnPoolableDisable();

      } else {
        // Set the poolable gameObject active
        obj.gameObject.SetActive(true);
      }

      // Reset the poolable
      IPoolable poolable = obj.GetComponent<IPoolable>();
      poolable.ResetPoolable();
    }

    /// <summary>
    /// Event called when the pool object is disabled
    /// </summary>
    private void ObjectFinishedEvent(GameObject gameObject) {
      Disable(gameObject);
    }

    /// <summary>
    /// Disables the object from the pool (Or destroys it if overflowing)
    /// </summary>
    protected void Disable(GameObject obj) {
      // Safety check
      if (!_poolObjects.Contains(obj)) {
        Debug.LogWarning($"Pool didn't contain {obj}. This should not happen!");
        return;
      }

      // Set Inactive
      if (!obj.gameObject.activeInHierarchy) {
        Debug.LogWarning($"Pool Object {obj.name} is already inactive in hierarchy. Let this be handled by the ObjectPool");
      } else {
        obj.gameObject.SetActive(false);
      }

      // Pool has more elements than maxSize (overflow)
      if (_poolObjects.Count > _maxSize) {
        _poolObjects.Remove(obj);
        // Destroy
        Destroy(obj);

      } else {
        // Shift object to the end of the list
        _poolObjects.MoveItem(obj, Count - 1);
      }
    }
    #endregion
  }
}