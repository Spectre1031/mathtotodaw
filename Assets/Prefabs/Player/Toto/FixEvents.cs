#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class FixEvents : EditorWindow
{
    [MenuItem("Tools/Fix Events")]
    static void Fix()
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) continue;
            AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);
            EditorUtility.SetDirty(clip);
            Debug.Log("Cleared: " + path);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("DONE!");
    }
}
#endif