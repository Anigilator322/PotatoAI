#if UNITY_EDITOR
using UnityEditor;
namespace Assets.Scripts.Utils 
{ 
    public class DomainReloader
    {
        [MenuItem("Tools/Reload Domain")]
        public static void ReloadDomain()
        {
            EditorUtility.RequestScriptReload();
        }
    }
}
#endif
