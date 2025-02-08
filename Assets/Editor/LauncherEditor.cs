#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class LauncherEditor : EditorWindow
{
    [MenuItem("Tools/Lancer Test Room")]
    public static void LaunchTestRoom()
    {
        if (EditorApplication.isPlaying)
            ExecuteInPlayMode();
        else
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.isPlaying = true;
        }
    }
    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            ExecuteInPlayMode();
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }
    }
    private static void ExecuteInPlayMode()
    {
        Launcher launcher = Object.FindFirstObjectByType<Launcher>();

        if (launcher != null)
            launcher.StartCoroutine(ExecuteAfterDelay(launcher));
    }
    private static IEnumerator ExecuteAfterDelay(Launcher launcher)
    {
        yield return new WaitForSeconds(1f); 
        launcher.ONTESTROOM(); 
    }
}
#endif
