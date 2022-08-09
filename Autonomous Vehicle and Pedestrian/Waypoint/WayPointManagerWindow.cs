using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPointManagerWindow : EditorWindow
{
    
    
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WayPointManagerWindow>();
        
    }

    public Transform waypointRoot;
    public float roadWidth=2.5f;

    private void OnGUI()
    {
        SerializedObject obj=new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));
        EditorGUILayout.PropertyField(obj.FindProperty("roadWidth"));

        if(waypointRoot==null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform", MessageType.Warning);
        }
        else 
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }
        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {   
        if(GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
        if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if(GUILayout.Button("Add Branch Waypoint"))
            {
                CreateBranch();
            }
            if(GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if(GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if(GUILayout.Button("Create Stop Waypoint"))
            {
                CreateWaypointStop();
            }
            if(GUILayout.Button("Create Reverse Waypoint"))
            {
                CreateWaypointReverse();
            }
            if(GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }
    }

    void CreateWaypoint()
    {
        GameObject waypointObject=new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot,false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        waypoint.width=roadWidth;
        if(waypointRoot.childCount > 1)
        {
            waypoint.previousWaypoint=waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint.nextWaypoint=waypoint;

            //Place the waypoint at the last position
            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
        }

        Selection.activeGameObject=waypoint.gameObject;
    
    }

    void CreateWaypointAfter()
    {
        GameObject waypointObject=new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot,false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        newWaypoint.width=roadWidth;

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        newWaypoint.previousWaypoint = selectedWaypoint;

        if(selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint=newWaypoint;
            newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
        }

        selectedWaypoint.nextWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void CreateWaypointBefore()
    {
        GameObject waypointObject=new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot,false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        newWaypoint.width=roadWidth;
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;


        if(selectedWaypoint.previousWaypoint != null)
        {   
            newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            selectedWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
            
        }

        newWaypoint.nextWaypoint = selectedWaypoint;
        selectedWaypoint.previousWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject=newWaypoint.gameObject;
    }

    void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if(selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            Selection.activeGameObject = selectedWaypoint.previousWaypoint.gameObject;
        }
        if(selectedWaypoint.previousWaypoint != null)
        {
            selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            Selection.activeGameObject = selectedWaypoint.previousWaypoint.gameObject;
        }
        
        DestroyImmediate(selectedWaypoint.gameObject);
    }

    void CreateBranch()
    {
        GameObject waypointObject=new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot,false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        waypoint.width=roadWidth;

        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        if(waypoint == null){
            EditorGUILayout.HelpBox("null error", MessageType.Warning);
        }
        else{
            branchedFrom.branches.Add(waypoint);

            waypoint.transform.position = branchedFrom.transform.position;
            waypoint.transform.forward = branchedFrom.transform.forward;

            Selection.activeGameObject = waypoint.gameObject;
        }

    }

    void CreateWaypointStop()
    {
        GameObject waypointObject=new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot,false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        newWaypoint.width=roadWidth;

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        selectedWaypoint.Stop = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void CreateWaypointReverse()
    {
        GameObject waypointObject=new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot,false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        newWaypoint.width=roadWidth;

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;


        selectedWaypoint.Reverse = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }
}
