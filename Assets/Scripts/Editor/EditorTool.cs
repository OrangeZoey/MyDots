using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorTool
{
    [MenuItem("Assets/CustomTool/MergeSprite")]
    public static void MergeSprite()
    {
        //获取选择的资源
        string[] sprteGUIDs = Selection.assetGUIDs;

        //判断长度
        if (sprteGUIDs == null || sprteGUIDs.Length <= 1) return;
        //创建集合  获取的是无序的
        List<string> spritePathList = new List<string>(sprteGUIDs.Length);
        for (int i = 0; i < sprteGUIDs.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(sprteGUIDs[i]); //将资产的GUID（全局唯一标识符）转换为对应的资产路径
            spritePathList.Add(assetPath);
        }
        //排序
        spritePathList.Sort();

        //第一个
        Texture2D firstTex = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[0]);
        int unitHieght = firstTex.height;
        int unitWidth = firstTex.width;

        Texture2D outputTex = new Texture2D(unitWidth * spritePathList.Count, unitHieght);
        for (int i = 0; i < spritePathList.Count; i++)
        {
            Texture2D temp = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[i]);
            Color[] colors = temp.GetPixels();//获取像素
            outputTex.SetPixels(i * unitWidth, 0, unitWidth, unitHieght, colors); //设置像素 填充
        }

        byte[] bytes = outputTex.EncodeToPNG();
        File.WriteAllBytes(spritePathList[0].Remove(spritePathList[0].LastIndexOf(firstTex.name)) + "MergeSprite.png", bytes);//合并
        AssetDatabase.SaveAssets();//保存
        AssetDatabase.Refresh();//刷新
    }
}
