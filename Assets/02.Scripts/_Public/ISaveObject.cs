using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveObject {
    void SaveState(bool selfState, bool selfActive,Vector2 pos);
    bool LoadState();
}
