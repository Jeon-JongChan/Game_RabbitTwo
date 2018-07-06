using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISaveCurrentState {
    void SaveInitState();
    void SaveState();
    void SaveLoad();
}
