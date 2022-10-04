using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class KitbashControl : MonoBehaviour
{
    [MenuItem("GameObject/Kitbash Selected")]
    static void KitbashSelectedMethod(MenuCommand menuCommand)
    {
        GameObject gop = new GameObject("Kitbash Group");
        //foreach (GameObject go in Selection.gameObjects) {}
        //Display Prompt for Counts and Axis Locks?
        int collisions = 0;
        for (int i = 0; i < 100; ++i)
        {
            int select = Random.Range(0, Selection.gameObjects.Length);
            GameObject go = GameObject.Instantiate(Selection.gameObjects[select]);
            go.name = go.name.Substring(0, go.name.IndexOf("(Clone)"));
            //go.AddComponent<MeshCollider>();
            //MeshCollider co = go.GetComponent<MeshCollider>();
            bool collision = false;
            //while (true) //<-- this is causing CPU and GPU lock-ups!!! there are obviously better solutions but it's not finding them
            for (int j = 0; j < 100; ++j)
            {
                go.transform.position = new Vector3(Random.Range(-10, 10),
                                                    Random.Range(-10, 10),
                                                    Random.Range(-10, 10));
                go.transform.eulerAngles = new Vector3(Random.Range(-10, 10),
                                                       Random.Range(-10, 10),
                                                       Random.Range(-10, 10));
                go.AddComponent<MeshCollider>();
                MeshCollider co = go.GetComponent<MeshCollider>();
                for (int k = 0; k < gop.transform.childCount; ++k)
                {
                    GameObject go2 = gop.transform.GetChild(k).gameObject;
                    MeshCollider co2 = go2.GetComponent<MeshCollider>();
                    //if (co2.Intersects(co)) { collision = true; break; }
                    if (co.bounds.Intersects(co2.bounds)) { collision = true; break; } //only bounds, not mesh? this doesn't work...
                }
                if (!collision) { break; }
            }
            if (collision) { ++collisions; GameObject.DestroyImmediate(go); } else { go.transform.parent = gop.transform; }
        }
        if (collisions > 0) { Debug.Log("[WARN]: Collisions Detected: " + collisions.ToString()); }
    }
}
