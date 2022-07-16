using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ListExtensions {
  /// <summary>
  /// Geklaut vom Herrn Allgaier persönlich.
  /// Beschreibung gibt es nicht.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="list"></param>
  public static void Shuffle<T>(this List<T> list) {
    for (int i = 0; i < list.Count - 1; i++) {
      int k = UnityEngine.Random.Range(i, list.Count);
      (list[i], list[k]) = (list[k], list[i]);
    }
  }

  /// <summary>
  /// Adds an item to a list if it doesn't already exist.
  /// Returns true if item got added. false if it was already added
  /// </summary>
  public static bool AddUnique<T>(this List<T> list, T item) {
    if (!list.Contains(item)) {
      list.Add(item);
      return true;
    }

    return false;
  }

  /// <summary>
  /// Moves the obj to a new index in a list
  /// </summary>
  public static void MoveItem<T>(this List<T> list, T item, int index) {
    list.Remove(item);
    list.Insert(index, item);
  }

  /// <summary>
  /// Moves the object at oldIndex to newIndex in the list
  /// </summary>
  public static void MoveItem<T>(this List<T> list, int oldIndex, int newIndex) {
    T item = list[oldIndex];
    list.RemoveAt(oldIndex);
    list.Insert(newIndex, item);
  }

  /// <summary>
  /// Removes all duplicates of a list
  /// </summary>
  public static void RemoveDuplicates<T>(this List<T> list) {
    List<T> uniques = new List<T>();
    foreach (T item in list) {
      uniques.AddUnique(item);
    }

    // There might be a better way but it's good enough
    list.Clear();
    list.AddRange(uniques);
  }

  /// <summary>
  /// Removes all empty entries of the list
  /// </summary>
  public static void RemoveEmpties<T>(this List<T> list) {
    for (int i = list.Count - 1; i >= 0; i--) {
      // The ToString() == null check is necessary because sometimes the item is not returned as null even though it says null when the reference is deleted (e.g. File deleted)
      if (list[i] == null || list[i].ToString() == "null")
        list.RemoveAt(i);
    }
  }

  /// <summary>
  /// Gets a random element from within a list.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="list"></param>
  /// <returns></returns>
  public static T RandomElement<T>(this List<T> list) {
    if (list.Count <= 0)
      return default;
    int rndIdx = UnityEngine.Random.Range(0, list.Count);
    return list[rndIdx];
  }

  /// <summary>
  /// Gets a random element from list of value/weight tuples. Uses the weight to determine prevalence.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="list"></param>
  /// <returns></returns>
  public static T RandomElementWeighted<T>(this List<(T value, float weight)> list) {
    if (list.Count <= 0)
      return default;
    float accWeight = 0;
    foreach (var (value, weight) in list) {
      accWeight += weight;
    }

    float rnd = UnityEngine.Random.Range(0, accWeight);
    foreach (var (value, weight) in list) {
      accWeight -= weight;
      if (accWeight <= rnd) {
        return value;
      }
    }

    return default;
  }

  public static string ToStringList<T>(this List<T> list, bool colored = false) {
    Color currentColor = Color.HSVToRGB(0.0f, 0.4f, 0.9f);
    int count = list.Count;
    
    StringBuilder builder = new StringBuilder();
    for (int i = 0; i < list.Count; i++) {
      if(colored) {
        builder.Append($"[{currentColor.ColorString(i.ToString())}]: {currentColor.ColorString(list[i].ToString())}; ");
        Color.RGBToHSV(currentColor, out float H, out float S, out float V);
        H += 1.0f / count;
        if (H > 1.0f)
          H -= 1.0f;

        currentColor = Color.HSVToRGB(H, S, V);

      } else {
        builder.Append($"[{i}]: {list[i]}; ");
      }
    }
    return builder.ToString();
  }

  public static List<T> Find<T>() {
    List<T> interfaces = new List<T>();
    GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    foreach (var rootGameObject in rootGameObjects) {
      T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
      foreach (var childInterface in childrenInterfaces) {
        interfaces.Add(childInterface);
      }
    }
    return interfaces;
  }

  /// <summary>
  /// Loads and adds all ScriptableObject assets of type T. Only use this in editor mode.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="templateList"></param>
  public static void LoadTemplates<T>(this List<T> templateList) where T : ScriptableObject {
#if UNITY_EDITOR
    if (templateList != null) {
      templateList.Clear();
    } else {
      templateList = new List<T>();
    }

    int countLoaded = 0;
    var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
    foreach (var asset in assets) {
      string path = AssetDatabase.GUIDToAssetPath(asset);
      // Debug.Log($"Adding { path }");
      templateList.AddUnique(AssetDatabase.LoadAssetAtPath<T>(path));
      countLoaded++;
    }
    Debug.Log($"<color=#99ff66>Loaded {countLoaded} Assets of Type {typeof(T).Name} ...</color>");
#endif
  }
}