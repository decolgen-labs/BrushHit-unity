using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NOOD.Extension
{
    public static class EnumExtension
    {
        public static T AddToEnum<T>(this T sourceEnum, string value, T defaultValue) where T : struct
        {
            if(string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse<T>(value, out result) ? result : defaultValue;
        }
    }

    public static class StringExtension
    {
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse<T>(value, out result) ? result : defaultValue;
        }
    }

    public static class ListExtension
    {
        public static T GetRandom<T>(this List<T> list) where T : class
        {
            T result = null;
            if(list.Count > 0 && list != null)
            {
                int r = UnityEngine.Random.Range(0, list.Count - 1);
                result = list[r];
            }
            return result;
        }
    }

    public static class Vector3Extension
    {
        public static Vector2 ToVector2XY (this Vector3 source)
        {
            return new Vector2(source.x, source.y);
        }
        public static Vector2 ToVector2XZ (this Vector3 source)
        {
            return new Vector2(source.x, source.z);
        }
        public static Vector2 ToVector2YZ (this Vector3 source)
        {
            return new Vector2(source.y, source.z);
        }
        public static Vector3 ToVector3XZ (this Vector3 source)
        {
            return new Vector3(source.x, 0, source.z);
        }
        public static Vector3 ChangeX(this Vector3 source, float value)
        {
            return new Vector3(value, source.y, source.z);
        }
        public static Vector3 ChangeY(this Vector3 source, float value)
        {
            return new Vector3(source.x, value, source.z);
        }
        public static Vector3 ChangeZ(this Vector3 source, float value)
        {
            return new Vector3(source.x, source.y, value);
        }
    }

    public static class Vector2Extension
    {
        public static Vector3 ToVector3XZ (this Vector2 source, float y = 0)
        {
            return new Vector3(source.x, y, source.y);
        }
        public static Vector3 ToVector3XY (this Vector2 source, float z = 0)
        {
            return new Vector3(source.x, source.y, z);
        }
        public static Vector3 ToVector3YZ (this Vector2 source, float x = 0)
        {
            return new Vector3(x, source.x, source.y);
        }
    }

#region Editor Extension
#if UNITY_EDITOR
    public static class EnumCreator
    {
        const string extension = ".cs";
        public static void WriteToEnum<T>(string enumFolderPath, string enumFileName, ICollection<string> data)
        {
            WriteToEnum<T>(enumFolderPath, enumFileName, data, null);
        }
        public static void WriteToEnum<T>(string enumFolderPath, string enumFileName, ICollection<string> data, string _namespace = null)
        {
            if(Directory.Exists(enumFolderPath) == false)
            {
                Directory.CreateDirectory(enumFolderPath);
            }
            using (StreamWriter file = File.CreateText(enumFolderPath + enumFileName + extension))
            {
                if(string.IsNullOrEmpty(_namespace) == false)
                {
                    file.WriteLine("namespace " + _namespace + "\n{ ");
                }
                file.WriteLine("public enum " + typeof(T).Name + "\n{ ");

                int i = 0;
                foreach (var line in data)
                {
                    string lineRep = line.ToString().Replace(" ", string.Empty);
                    if (!string.IsNullOrEmpty(lineRep))
                    {
                        file.WriteLine(string.Format("	{0} = {1},",
                            lineRep, i));
                        i++;
                    }
                }

                file.WriteLine("}");
                AssetDatabase.ImportAsset(enumFolderPath + enumFileName + extension);
                Debug.Log("Create Success " + typeof(T) + " at " + enumFolderPath);
                Debug.Log(Directory.Exists(enumFolderPath));
                if(string.IsNullOrEmpty(_namespace) == false)
                {
                    file.WriteLine("}");
                }
            }
        }
    }
#endif

#if UNITY_EDITOR
    public static class RootPathExtension<T> 
    {
        public static string RootPath
        {
            get 
            {
                var g = AssetDatabase.FindAssets($"t:Script {typeof(T).Name}");
                return AssetDatabase.GUIDToAssetPath(g[0]);
            }
        }
    }
#endif
#endregion

    #region File Extension
    public static class FileExtension
    {
        public static FileStream CreateFile(string folderPath, string fileName, string extension)
        {
            string filePath = Path.Combine(folderPath, fileName + extension);
            return File.Create(filePath);
        }
        public static void WriteToFile(string filePath, string text)
        {
            CreateFolderIfNeed(Path.GetDirectoryName(filePath));

            using(StreamWriter file = File.CreateText(filePath))
            {
                file.Write(text);
            }
        }
        public static string ReadFile(string filePath, string extension)
        {
            return File.ReadAllText(filePath + extension);
        }
        public static void CreateFolderIfNeed(string directory)
        {
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        public static bool IsExitFileInDefaultFolder(string fileName)
        {
            UnityEngine.Object resources = Resources.Load(Path.Combine("Datas", fileName));
            if (resources == null) return false;
            return true;
        }
    }
    #endregion
    
    public static class InputActionExtension
    {
        // public static void SaveKeyBinding(this InputAction inputAction)
        // {
        //     PlayerPrefs.SetString(inputAction.name, inputAction.SaveBindingOverridesAsJson());
        //     PlayerPrefs.Save();
        // }
        // public static void LoadKeyBinding(this InputAction inputAction)
        // {
        //     if(PlayerPrefs.HasKey(inputAction.name))
        //     {
        //         inputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(inputAction.name));
        //     }
        // }
    }
}
