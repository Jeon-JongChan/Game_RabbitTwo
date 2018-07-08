using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISaveObject {
    void SaveInitState();
    void SaveState();
    void SaveLoad();
    void LoadInitState();
}
