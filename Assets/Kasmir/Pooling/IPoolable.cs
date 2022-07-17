using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling {
  public interface IPoolable {
    // Hook for the ObjectPool so it knows the object is not in use anymore
    delegate void FinishedEvent(GameObject obj);
    event FinishedEvent Finished;

    public void ResetPoolable();
  }
}