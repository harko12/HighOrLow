using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    // credit to http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
    /// </summary>
    public static T CreateAsset<T>(string path = null) where T : ScriptableObject
    {

        /* add this to your scriptableobject cs file
        [MenuItem("Assets/Create/YourClass")]
        public static void CreateAsset ()
        {
            ScriptableObjectUtility.CreateAsset<YourClass> ();
        }        
    */
        T asset = ScriptableObject.CreateInstance<T>();
        if (path == null)
        {
           path = AssetDatabase.GetAssetPath(Selection.activeObject);
        }
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }

    /// <summary>
    /// Create new asset from <see cref="ScriptableObject"/> type with unique name at
    /// selected folder in project window. Asset creation can be cancelled by pressing
    /// escape key when asset is initially being named.
    /// </summary>
    /// <remarks>
    /* add this to a manager class (not the scriptable object
    [MenuItem("Assets/Create/YourScriptableObject")]
    public static void CreateYourScriptableObject()
    {
        ScriptableObjectUtility2.CreateAsset<YourScriptableObject>();
    }
    */
    /// </remarks>
    /// <typeparam name="T">Type of scriptable object.</typeparam>
    public static void CreateAssetInPlace<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
    }
}