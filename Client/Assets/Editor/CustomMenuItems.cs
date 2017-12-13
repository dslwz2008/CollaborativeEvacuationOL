using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class CustomMenuItems : MonoBehaviour
{
    private static List<string> pathes = new List<string>();

    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("Custom/Generate Resources Manifest")]
    static void GenerateResourcesManifest()
    {
        string path = Application.dataPath + "/Resources/";
        DirSearch(path, path.Length);
        StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/manifest.txt");
        foreach (string file in pathes)
        {
            sw.WriteLine(file);
        }
        sw.Close();
    }

    private static void DirSearch(string sDir, int baseLength)
    {
        try
        {
            foreach (string d in Directory.GetDirectories(sDir))
            {
                Debug.Log(sDir);
                foreach (string f in Directory.GetFiles(d))
                {
                    string ext = Path.GetExtension(f);
                    if (ext == ".xml" || ext==".fbx")
                    {
                        string filepath = f.Replace('\\', '/');
                        pathes.Add(filepath.Substring(baseLength));
                        Debug.Log(filepath.Substring(baseLength));
                    }
                }
                DirSearch(d, baseLength);
            }
        }
        catch (System.Exception excpt)
        {
            Debug.Log(excpt.Message);
        }
    }
    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("Custom/Modify FBX Material")]
    static void ModifyFBXMaterial()
    {
        string rootDir = Application.dataPath + "/Resources/jskjc/Data/";
        try
        {
            foreach (string d in Directory.GetDirectories(rootDir))
            {
                Debug.Log(d);
                foreach (string f in Directory.GetFiles(d))
                {
                    string ext = Path.GetExtension(f);
                    if (ext == ".fbx")
                    {
                        
                    }
                }
            }
        }
        catch (System.Exception excpt)
        {
            Debug.Log(excpt.Message);
        }
    }

}
