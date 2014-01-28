private static var instance:MenuSound;
public static function GetInstance() : MenuSound {
return instance;
}
 
function Awake() {
    if (instance != null && instance != this) {
        Destroy(this.gameObject);
        return;
    } else {
        instance = this;
    }
    DontDestroyOnLoad(this.gameObject);
}