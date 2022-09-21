#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class CameraControl : MonoBehaviour
{
    static SceneView sv = null;

    [Header("FlyCam")]
    public bool toggle = false;
    public bool reset = false;

    public static float deadzone = 0.1f;
    public static float sensitivity_left = 0.1f;
    public static float sensitivity_right = 0.1f;

    private static float buttonSouth = 0.0f;
    private static float buttonEast = 0.0f;
    private static float buttonWest = 0.0f;
    private static float buttonNorth = 0.0f;
    private static float startButton = 0.0f;
    private static float selectButton = 0.0f;

    private static float leftShoulder = 0.0f;
    private static float rightShoulder = 0.0f;
    private static float leftTrigger = 0.0f;
    private static float rightTrigger = 0.0f;

    private static Vector2 leftStick = new Vector2(0.0f, 0.0f);
    private static Vector2 rightStick = new Vector2(0.0f, 0.0f);

    private bool _block = false;
    private bool _shlock = false;
    private bool _dralock = false;
    private static int spinlock = 0;
    private static int marker_count = 0;

    private GameObject gop = null;

    void Start() { Update(); }

    void Update()
    {
        sv = EditorWindow.GetWindow<SceneView>(); //Debug.Log(sv);
        //Debug.Log(sv.sceneViewState.alwaysRefreshEnabled);
        sv.sceneViewState.alwaysRefresh = true;
        //Debug.Log(sv.sceneViewState.alwaysRefreshEnabled);
        SceneView.RepaintAll();
        SceneView.duringSceneGui -= EditorRefresh; //Might fail normally
        SceneView.duringSceneGui += EditorRefresh; //Gets added multiple times
    }

    void EditorRefresh(object state)
    {
        if (reset)
        {
            reset = false;
            toggle = false;
            spinlock = 0;
            marker_count = 0;
            sv.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        }
        if (spinlock > 0) { return; } spinlock += 1;

        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            leftStick = gamepad.leftStick.ReadValue();
            rightStick = gamepad.rightStick.ReadValue();
            leftStick = new Vector2((Mathf.Abs(leftStick.x) > deadzone) ? leftStick.x * sensitivity_left : 0.0f,
                (Mathf.Abs(leftStick.y) > deadzone) ? leftStick.y * sensitivity_left : 0.0f);
            rightStick = new Vector2((Mathf.Abs(rightStick.x) > deadzone) ? rightStick.x * sensitivity_right : 0.0f,
                (Mathf.Abs(rightStick.y) > deadzone) ? rightStick.y * sensitivity_right : 0.0f);

            buttonSouth = gamepad.buttonSouth.ReadValue();
            buttonEast = gamepad.buttonEast.ReadValue();
            buttonWest = gamepad.buttonWest.ReadValue();
            buttonNorth = gamepad.buttonNorth.ReadValue();
            startButton = gamepad.startButton.ReadValue();
            selectButton = gamepad.selectButton.ReadValue();

            leftShoulder = gamepad.leftShoulder.ReadValue();
            rightShoulder = gamepad.rightShoulder.ReadValue();
            if ((leftShoulder > 0.0f) || (selectButton > 0.0f)) { reset = true; }
            if ((rightShoulder > 0.0f) && (!_shlock)) { toggle = !toggle; _shlock = true; }
            else if ((rightShoulder < 1.0f) && (_shlock)) { _shlock = false; }
            if ((startButton > 0.0f) && (!_dralock)) { toggle = !toggle; _dralock = true; }
            else if ((startButton < 1.0f) && (_dralock)) { _dralock = false; }

            leftTrigger = gamepad.leftTrigger.ReadValue();
            rightTrigger = gamepad.rightTrigger.ReadValue();
            if (leftTrigger > 0.0f) { sv.camera.transform.position = sv.pivot; } //cameraDistance is
            if (rightTrigger > 0.0f) { sv.camera.transform.position = sv.pivot; } //read-only!

            if (toggle)
            {
                Vector3 sva = sv.camera.transform.eulerAngles; //Debug.Log(sva);

                Vector3 svm = new Vector3(leftStick.x, 0.0f, leftStick.y);
                svm = Quaternion.AngleAxis(sva.x, Vector3.right) * svm;
                svm = Quaternion.AngleAxis(sva.y, Vector3.up) * svm;
                svm = Quaternion.AngleAxis(sva.z, Vector3.forward) * svm; //Debug.Log(svm);

                sv.LookAtDirect(new Vector3(sv.pivot.x + svm.x, sv.pivot.y + svm.y, sv.pivot.z + svm.z),
                                Quaternion.Euler(sva.x - rightStick.y, sva.y + rightStick.x, sva.z));

                if ((buttonSouth > 0.0f) && (!_block)) { PlaceMarker(sv.pivot, sva); _block = true; }
                else if ((buttonSouth < 1.0f) && (_block)) { _block = false; }
            }
        }

        spinlock -= 1;
    }

    void PlaceMarker(Vector3 position, Vector3 facing)
    {
        if (marker_count == 0) { gop = new GameObject("Marker Group"); }
        marker_count += 1;
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Marker" + marker_count.ToString();
        go.transform.position = position;
        go.transform.eulerAngles = facing;
        go.transform.parent = gop.transform;
    }
}

#endif