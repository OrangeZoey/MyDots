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
        //��ȡѡ�����Դ
        string[] sprteGUIDs = Selection.assetGUIDs;

        //�жϳ���
        if (sprteGUIDs == null || sprteGUIDs.Length <= 1) return;
        //��������  ��ȡ���������
        List<string> spritePathList = new List<string>(sprteGUIDs.Length);
        for (int i = 0; i < sprteGUIDs.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(sprteGUIDs[i]); //���ʲ���GUID��ȫ��Ψһ��ʶ����ת��Ϊ��Ӧ���ʲ�·��
            spritePathList.Add(assetPath);
        }
        //����
        spritePathList.Sort();

        //��һ��
        Texture2D firstTex = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[0]);
        int unitHieght = firstTex.height;
        int unitWidth = firstTex.width;

        Texture2D outputTex = new Texture2D(unitWidth * spritePathList.Count, unitHieght);
        for (int i = 0; i < spritePathList.Count; i++)
        {
            Texture2D temp = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[i]);
            Color[] colors = temp.GetPixels();//��ȡ����
            outputTex.SetPixels(i * unitWidth, 0, unitWidth, unitHieght, colors); //�������� ���
        }

        byte[] bytes = outputTex.EncodeToPNG();
        File.WriteAllBytes(spritePathList[0].Remove(spritePathList[0].LastIndexOf(firstTex.name)) + "MergeSprite.png", bytes);//�ϲ�
        AssetDatabase.SaveAssets();//����
        AssetDatabase.Refresh();//ˢ��
    }
}
