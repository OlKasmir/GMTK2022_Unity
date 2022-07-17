using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public static class MethodExtensions {
  public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume, AudioSource audioSourceToDestroyOnEnd = null) {
    float currentTime = 0;
    float currentVol;
    audioMixer.GetFloat(exposedParam, out currentVol);
    currentVol = Mathf.Pow(10, currentVol / 20);
    float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

    while (currentTime < duration) {
      currentTime += Time.deltaTime;
      float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
      audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
      yield return null;
    }

    if (audioSourceToDestroyOnEnd) {
      Object.Destroy(audioSourceToDestroyOnEnd.gameObject);
    }
    yield break;
  }

  /// <summary>
  /// Gets a component in a specific child, defined by name.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="mono"></param>
  /// <param name="childName"></param>
  /// <returns></returns>
  public static T GetComponentInChildren<T>(this MonoBehaviour mono, string childName) where T : Component {
    List<T> childComps = new List<T>(mono.GetComponentsInChildren<T>());
    foreach (var comp in childComps) {
      if (comp.gameObject.name == childName) {
        return comp;
      }
    }
    Debug.LogWarning($"Component of type {typeof(T)} could not be found in any children of {mono.gameObject.name} named {childName}.");
    return null;
  }

  /// <summary>
  /// Sets the X position of a transform.
  /// </summary>
  /// <param name="transform"></param>
  /// <param name="value"></param>
  public static void SetPositionX(this Transform transform, float value) {
    transform.position = new Vector3(value, transform.position.y, transform.position.z);
  }

  /// <summary>
  /// Sets the Y position of a transform.
  /// </summary>
  /// <param name="transform"></param>
  /// <param name="value"></param>
  public static void SetPositionY(this Transform transform, float value) {
    transform.position = new Vector3(transform.position.x, value, transform.position.z);
  }

  /// <summary>
  /// Sets the Z position of a transform.
  /// </summary>
  /// <param name="transform"></param>
  /// <param name="value"></param>
  public static void SetPositionZ(this Transform transform, float value) {
    transform.position = new Vector3(transform.position.x, transform.position.y, value);
  }


  /// <summary>
  /// Snaps the transform to a grid of specified gridsize. (if centered is true it centers the position on the closest center of the grid cell)
  /// </summary>
  /// <param name="transform"></param>
  /// <param name="gridSize"></param>
  /// <param name="centered"></param>
  public static void SnapToGrid(this Transform transform, float gridSize = 1.0f, bool centered = true) {
    Vector3 pos = transform.position;

    float offsetX = 0.0f;
    float offsetY = 0.0f;

    // If set to centered get the offset to the closest grid cell center
    if (centered) {
      if (pos.x % gridSize >= ((pos.x < 0) ? -1 : 1) * gridSize / 2) {
        offsetX -= (gridSize / 2);
      } else {
        offsetX += (gridSize / 2);
      }
      if (pos.y % gridSize >= ((pos.y < 0) ? -1 : 1) * gridSize / 2) {
        offsetY -= (gridSize / 2);
      } else {
        offsetY += (gridSize / 2);
      }
    }

    pos.x = Mathf.Round(pos.x / gridSize) * gridSize + offsetX;
    pos.y = Mathf.Round(pos.y / gridSize) * gridSize + offsetY;
    transform.position = pos;
  }

  /// <summary>
  /// Rotates a Vector3.
  /// </summary>
  /// <param name="vector"></param>
  /// <param name="angle"></param>
  /// <returns></returns>
  public static Vector2 Rotate(this Vector2 vector, float angle) {
    return Quaternion.Euler(0, 0, angle) * vector;
  }

  public static Bounds OrthographicBounds(this Camera camera) {
    float screenAspect = (float)Screen.width / (float)Screen.height;
    float cameraHeight = camera.orthographicSize * 2;
    Bounds bounds = new Bounds(
        camera.transform.position,
        new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
    return bounds;
  }


  public static T AssignReturn<T>(ref T field, T value) {
    field ??= value;
    return field;
  }

  /// <summary>
  /// https://stackoverflow.com/questions/7040289/converting-integers-to-roman-numerals
  /// </summary>
  public static string ToRoman(this int num) {
    var result = string.Empty;
    var map = new Dictionary<string, int>
    {
        {"M", 1000 },
        {"CM", 900},
        {"D", 500},
        {"CD", 400},
        {"C", 100},
        {"XC", 90},
        {"L", 50},
        {"XL", 40},
        {"X", 10},
        {"IX", 9},
        {"V", 5},
        {"IV", 4},
        {"I", 1}
    };

    foreach (var pair in map) {
      result += string.Join(string.Empty, Enumerable.Repeat(pair.Key, num / pair.Value));
      num %= pair.Value;
    }
    return result;
  }

  /// <summary>
  /// Destroys the supplied monobehavior's GameObject after a delay. If destroyParent or destroyRoot are true, the parent or root GameObject
  /// will be destroyed instead.
  /// </summary>
  /// <param name="monoBehavior"></param>
  /// <param name="seconds"></param>
  /// <param name="destroyParent"></param>
  /// <param name="destroyRoot"></param>
  public static void DestroyGameObjectAfterTime(this MonoBehaviour monoBehavior, float seconds, bool destroyParent = false, bool destroyRoot = false) {
    IEnumerator DelayedDestruction() {
      yield return new WaitForSeconds(seconds);
      GameObject toDestroy = monoBehavior.gameObject;
      if (destroyParent) toDestroy = monoBehavior.transform.parent.gameObject;
      if (destroyRoot) toDestroy = monoBehavior.transform.root.gameObject;
      Object.Destroy(toDestroy);
    }

    monoBehavior.StartCoroutine(DelayedDestruction());
  }

  /// <summary>
  /// Orders the MonoBehavior to destroy a supplied UnityObject after a set duration.
  /// </summary>
  /// <param name="destroyer"></param>
  /// <param name="toDestroy"></param>
  /// <param name="seconds"></param>
  public static void DestroyObjectAfterTime(this MonoBehaviour destroyer, Object toDestroy, float seconds) {
    IEnumerator DelayedDestruction() {
      yield return new WaitForSeconds(seconds);
      Object.Destroy(toDestroy);
    }

    destroyer.StartCoroutine(DelayedDestruction());
  }


  public static Vector3 RandomPointInRange(this Vector3 origin, float radius) {
    return ((Vector2)origin).RandomPointInRange(radius);
  }

  public static Vector2 RandomPointInRange(this Vector2 origin, float radius) {
    if (radius == 0f) return origin;

    radius = Mathf.Max(0f, radius);

    for (int i = 0; i < 100; i++) {
      float rndX = UnityEngine.Random.Range(-radius, radius);
      float rndY = UnityEngine.Random.Range(-radius, radius);

      Vector2 rndPoint = new Vector2(origin.x + rndX, origin.y + rndY);

      if (Vector2.Distance(origin, rndPoint) <= radius) {
        return rndPoint;
      }
    }

    return origin;
  }


  /// <summary>
  /// The squared distance from a to b
  /// </summary>
  public static float DistanceSqr(this Vector2 a, Vector2 b) {
    return (b - a).sqrMagnitude;
  }

  /// <summary>
  /// if distance between a and b is smaller or equal to distance it returns true
  /// </summary>
  public static bool InDistance(this Vector2 a, Vector2 b, float distance) {
    return (b - a).sqrMagnitude <= distance * distance;
  }

  /// <summary>
  /// The squared distance from a to b
  /// </summary>
  public static float DistanceSqr(this Vector3 a, Vector3 b) {
    return (b - a).sqrMagnitude;
  }

  /// <summary>
  /// if distance between a and b is smaller or equal to distance it returns true
  /// </summary>
  public static bool InDistance(this Vector3 a, Vector3 b, float distance) {
    return (b - a).sqrMagnitude <= distance * distance;
  }
}
