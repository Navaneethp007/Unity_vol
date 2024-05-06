using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace UnityVolumeRendering
{

public class Label : MonoBehaviour
{
    public VolumeRenderedObject targetObject;
    private static Label instance;
    private Rect windowRect = new Rect(150, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

    private const int WINDOW_WIDTH = 400;
    private const int WINDOW_HEIGHT = 200;
    private int windowID;
    private string displaytext = "";
    private LineRenderer lineRenderer;
    private void Awake()
        {
            windowID = WindowGUID.GetUniqueWindowID();
        }
    // Start is called before the first frame update
    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        Material lineMaterial = new Material(Shader.Find("Standard"));
        lineMaterial.SetColor("_Color", Color.red);
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.003f;
        lineRenderer.endWidth = 0.003f;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Camera.main != null && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                VolumeRaycaster raycaster = new VolumeRaycaster();
                if (raycaster.RaycastScene(ray, out RaycastHit hit))
                {
                    //Debug.DrawLine(ray.origin, hit.point, Color.red, 10.0f, true);
                    lineRenderer.SetPosition(0, lineRenderer.GetPosition(1));
                    lineRenderer.SetPosition(1, hit.point);
                    Vector3 nosePoint = new Vector3(0.0f, 0.1f, -0.5f);
                    float distanceThreshold = 0.1f; 

                    if (Vector3.Distance(hit.point, nosePoint) < distanceThreshold)
                    {
                        displaytext = "Nasal region";
                    }
                    Vector3 leftEyePoint = new Vector3(-0.2f, 0.2f, -0.4f);
                    if (Vector3.Distance(hit.point, leftEyePoint) < distanceThreshold)
                    {
                        displaytext = "Left eye region";
                    }
                    Vector3 rightEyePoint = new Vector3(0.0f, 0.2f, -0.5f);
                    if (Vector3.Distance(hit.point, rightEyePoint) < distanceThreshold)
                    {
                        displaytext = "Right eye region";
                    }
                    //displaytext = "Hit point: " + hit.point.ToString();
                    
                }
            }
    }
    public static void ShowWindow()
        {
            if(instance != null)
                GameObject.Destroy(instance);

            GameObject obj = new GameObject("Label");
            instance = obj.AddComponent<Label>();
        }

        private void OnGUI()
        {
            windowRect = GUI.Window(windowID, windowRect, UpdateWindow, "Display labels");
        }

        private void UpdateWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUILayout.BeginVertical();

            GUILayout.Label(displaytext);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // Show close button
            if (GUILayout.Button("Close"))
            {
                GameObject.Destroy(this.gameObject);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        private void OnLoadTransferFunction(RuntimeFileBrowser.DialogResult result)
        {
            if(!result.cancelled)
            {
                string extension = Path.GetExtension(result.path);
                if(extension == ".tf")
                {
                    TransferFunction tf = TransferFunctionDatabase.LoadTransferFunction(result.path);
                    if (tf != null)
                    {
                        targetObject.transferFunction = tf;
                        targetObject.SetTransferFunctionMode(TFRenderMode.TF1D);
                    }
                }
                if (extension == ".tf2d")
                {
                    TransferFunction2D tf = TransferFunctionDatabase.LoadTransferFunction2D(result.path);
                    if (tf != null)
                    {
                        targetObject.transferFunction2D = tf;
                        targetObject.SetTransferFunctionMode(TFRenderMode.TF2D);
                    }
                }
            }
        }

    }
}
