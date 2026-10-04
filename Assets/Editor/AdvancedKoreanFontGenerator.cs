using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class AdvancedKoreanFontGenerator : EditorWindow
{
    private Font sourceFont;

    [MenuItem("Tools/Easy to generate Korean Font")]
    public static void ShowWindow()
    {
        GetWindow<AdvancedKoreanFontGenerator>("TMP한글 폰트 생성기");
    }

    private void OnGUI()
    {
        GUILayout.Label("한글 11,172자 포함 폰트 생성기", EditorStyles.boldLabel);
        sourceFont = (Font)EditorGUILayout.ObjectField("폰트 파일", sourceFont, typeof(Font), false);

        if (GUILayout.Button("한글 폰트 생성하기"))
        {
            if (sourceFont != null) GenerateFullKoreanFont();
            else EditorUtility.DisplayDialog("알림", "폰트 파일을 먼저 넣어주세요.", "확인");
        }
    }

    private void GenerateFullKoreanFont()
    {
        // 1. 폰트 에셋 생성 (가장 표준적인 API)
        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);

        // 2. 경로 설정
        string path = AssetDatabase.GetAssetPath(sourceFont);
        string dir = Path.GetDirectoryName(path);
        string assetPath = Path.Combine(dir, sourceFont.name + "_Full_SDF.asset");

        // 3. 생성된 에셋 저장
        AssetDatabase.CreateAsset(fontAsset, assetPath);
        AssetDatabase.SaveAssets();

        // 4. 리프레시 및 안내
        AssetDatabase.Refresh();

        Debug.Log($"[성공] 폰트 생성 완료: {assetPath}");
        EditorUtility.DisplayDialog("성공", "폰트 에셋이 생성되었습니다.\n\n생성된 파일(" + sourceFont.name + "_Full_SDF)을 선택하고, Inspector 창에서 'Font Asset Creator' 메뉴를 통해 Atlas를 생성하십시오.", "확인");
    }
}