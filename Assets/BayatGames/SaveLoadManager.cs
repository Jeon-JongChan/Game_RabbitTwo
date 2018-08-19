using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// using BayatGames.SaveGameFree.Encoders;
using BayatGames.SaveGameFree.Serializers;
using BayatGames.SaveGameFree.Types;
using BayatGames.SaveGameFree;

public class SaveLoadManager : MonoBehaviour {

    [Tooltip("아무것도 없다면 해당 컴포넌트를 가진 OBJ 를 저장합니다.")]
    [SerializeField] Transform[] _saveObj;
    [SerializeField] SaveSetting _setting = new SaveSetting();
    [SerializeField] SaveLoadEvent _saveLoadEvent = new SaveLoadEvent();
	Vector2 _defaultPosition = Vector2.zero;
    private ISaveGameSerializer _serializer;

    public int SaveObjectLen
    {
        get{return _saveObj.Length;}
    }

    protected virtual void Awake ()
    {
        _serializer = new SaveGameJsonSerializer ();


        if(_setting.savePosition)
        {
            if(string.IsNullOrEmpty(_setting.positionIdentifier)) _setting.positionIdentifier = "POS" + transform.name;
            //Debug.Log("save positon ID : " + _setting.positionIdentifier); 
        }
        if(_setting.saveRotation)
        {
            if(string.IsNullOrEmpty(_setting.rotationIdentifier)) _setting.rotationIdentifier = "ROT" + transform.name;
            //Debug.Log("save rotation ID : " + _setting.rotationIdentifier); 
        }
        if(_setting.saveScale)
        {
            if(string.IsNullOrEmpty(_setting.scaleIdentifier)) _setting.scaleIdentifier = "SCALE" + transform.name;
            //Debug.Log("save scale ID : " + _setting.scaleIdentifier); 
        }


        if ( _saveLoadEvent.loadOnAwake )
        {
            Load ();
        }
        if ( _saveLoadEvent.saveOnAwake )
        {
            Save ();
        }
    }

    protected virtual void Start ()
    {
        if ( _saveLoadEvent.loadOnStart )
        {
            Load ();
        }
        if ( _saveLoadEvent.saveOnStart )
        {
            Save ();
        }
    }

    protected virtual void OnEnable ()
    {
        if ( _saveLoadEvent.loadOnEnable )
        {
            Load ();
        }
        if ( _saveLoadEvent.saveOnEnable )
        {
            Save ();
        }
    }

    protected virtual void OnDisable ()
    {
        if ( _saveLoadEvent.saveOnDisable )
        {
            Save ();
        }
    }

    protected virtual void OnApplicationQuit ()
    {
        if ( _saveLoadEvent.saveOnApplicationQuit )
        {
            Save ();
        }
    }

    protected virtual void OnApplicationPause ()
    {
        if ( _saveLoadEvent.saveOnApplicationPause )
        {
            Save ();
        }
    }

    public void Save()
    {
        if(_saveObj.Length > 0)
        {
            foreach(var tf in _saveObj)
            {
                if ( _setting.savePosition )
                {
                    SaveGame.Save<Vector3Save> ( 
                        "POS" + tf.name, 
                        tf.position, 
                        _serializer
                    );
                }
                if ( _setting.saveRotation )
                {
                    SaveGame.Save<QuaternionSave> ( 
                        "ROT" + tf.name, 
                        tf.rotation, 
                        _serializer
                    );
                }
                if ( _setting.saveScale )
                {
                    SaveGame.Save<Vector3Save> (
                    "SCALE" + tf.name,
                        tf.localScale,
                        _serializer
                    );
                }
            }
        }
        else
        {
            if ( _setting.savePosition )
            {
                SaveGame.Save<Vector3Save> ( 
                    _setting.positionIdentifier, 
                    this.transform.position, 
                    _serializer
                );
            }
            if ( _setting.saveRotation )
            {
                SaveGame.Save<QuaternionSave> ( 
                    _setting.rotationIdentifier, 
                    this.transform.rotation, 
                    _serializer
                );
            }
            if ( _setting.saveScale )
            {
                SaveGame.Save<Vector3Save> (
                    _setting.scaleIdentifier,
                    this.transform.localScale,
                    _serializer
                );
            }
        }
        
    }
    public void Load()
    {
        if(_saveObj.Length > 0)
        {
            foreach(var tf in _saveObj)
            {
                if ( _setting.savePosition )
                {
                    tf.position = SaveGame.Load<Vector3Save> ( 
                        "POS" + tf.name,
                        _serializer
                        );
                }
                if ( _setting.saveRotation )
                {
                    tf.rotation = SaveGame.Load<QuaternionSave> ( 
                        "ROT" + tf.name,
                        _serializer
                        );
                }
                if ( _setting.saveScale )
                {
                    tf.localScale = SaveGame.Load<Vector3Save> (
                        "SCALE" + tf.name,
                        _serializer
                        );
                }
            }
        }
        else
        {
            if ( _setting.savePosition )
            {
                this.transform.position = SaveGame.Load<Vector3Save> ( 
                    _setting.positionIdentifier,
                    _serializer
                    );
            }
            if ( _setting.saveRotation )
            {
                this.transform.rotation = SaveGame.Load<QuaternionSave> ( 
                    _setting.rotationIdentifier,
                    _serializer
                    );
            }
            if ( _setting.saveScale )
            {
                this.transform.localScale = SaveGame.Load<Vector3Save> (
                    _setting.scaleIdentifier,
                    _serializer
                    );
            }
        }
    }

    ///<summary>
    /// 외부에서 save를 호출시
    ///</summary>
    /// <param name="saveType">0 - pos \n 1 - rot \n 2 - scale</param>
    public void Save(Transform tf, int saveType)
    {
        if ( saveType == 0 )
        {
            SaveGame.Save<Vector3Save> ( 
                "POS" + tf.name, 
                tf.position, 
                _serializer
            );
            //Debug.Log("외부 호출 성공 pos save" + tf.name);
        }
        else if ( saveType == 1 )
        {
            SaveGame.Save<QuaternionSave> ( 
                "ROT" + tf.name, 
                tf.rotation, 
                _serializer
            );
        }
        else if ( saveType == 2 )
        {
            SaveGame.Save<Vector3Save> (
               "SCALE" + tf.name,
                tf.localScale,
                _serializer
            );
        }
    }
    ///<summary>
    /// 외부에서 Load를 호출시
    ///</summary>
    /// <param name="loadType">0 - pos \n 1 - rot \n 2 - scale</param>
    public void Load(Transform tf, int loadType)
    {
        if ( loadType == 0 )
        {
            tf.position = SaveGame.Load<Vector3Save> ( 
                "POS" + tf.name,
                _serializer
                );
            //Debug.Log("외부 호출 성공 pos load" + tf.name);
        }
        else if ( loadType == 1 )
        {
            tf.rotation = SaveGame.Load<QuaternionSave> ( 
                "ROT" + tf.name,
                _serializer
                );
        }
        else if ( loadType == 2 )
        {
            tf.localScale = SaveGame.Load<Vector3Save> (
                "SCALE" + tf.name,
                _serializer
                );
        }
    }

}
[System.Serializable]
class SaveSetting
{
    [Header ( "What to Save?" )]
    [Space]


    [Tooltip ( "Save Position?" )]
    /// <summary>
    /// The save position.
    /// </summary>
    public bool savePosition = true;

    [Tooltip ( "Save Rotation?" )]
    /// <summary>
    /// The save rotation.
    /// </summary>
    public bool saveRotation = false;

    [Tooltip ( "Save Scale?" )]
    /// <summary>
    /// The save scale.
    /// </summary>
    public bool saveScale = false;

    [Header ( "Settings" )]
    [Space]


    [Tooltip ( "You must specify a value for this to be able to save it." )]
    /// <summary>
    /// The position identifier.
    /// </summary>
    public string positionIdentifier = null;

    [Tooltip ( "You must specify a value for this to be able to save it." )]
    /// <summary>
    /// The rotation identifier.
    /// </summary>
    public string rotationIdentifier = null;

    [Tooltip ( "You must specify a value for this to be able to save it." )]
    /// <summary>
    /// The scale identifier.
    /// </summary>
    public string scaleIdentifier = null;
    
}
[System.Serializable]
class SaveLoadEvent
{
    [Header ( "Save Events" )]
    [Space]


    [Tooltip ( "Save on Awake()" )]
    /// <summary>
    /// The save on awake.
    /// </summary>
    public bool saveOnAwake;

    [Tooltip ( "Save on Start()" )]
    /// <summary>
    /// The save on start.
    /// </summary>
    public bool saveOnStart;

    [Tooltip ( "Save on OnEnable()" )]
    /// <summary>
    /// The save on enable.
    /// </summary>
    public bool saveOnEnable;

    [Tooltip ( "Save on OnDisable()" )]
    /// <summary>
    /// The save on disable.
    /// </summary>
    public bool saveOnDisable = false;

    [Tooltip ( "Save on OnApplicationQuit()" )]
    /// <summary>
    /// The save on application quit.
    /// </summary>
    public bool saveOnApplicationQuit = false;

    [Tooltip ( "Save on OnApplicationPause()" )]
    /// <summary>
    /// The save on application pause.
    /// </summary>
    public bool saveOnApplicationPause;


    [Header ( "Load Events" )]
    [Space]


    [Tooltip ( "Load on Awake()" )]
    /// <summary>
    /// The load on awake.
    /// </summary>
    public bool loadOnAwake;

    [Tooltip ( "Load on Start()" )]
    /// <summary>
    /// The load on start.
    /// </summary>
    public bool loadOnStart = false;

    [Tooltip ( "Load on OnEnable()" )]
    /// <summary>
    /// The load on enable.
    /// </summary>
    public bool loadOnEnable = false;
}
