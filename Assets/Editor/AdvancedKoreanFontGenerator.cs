using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;
using System.Text;

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
        Selection.activeObject = null;
        // 1. 폰트 에셋 생성 
        // Note: 11,172자를 담기 위해 Atlas 해상도를 4096x4096으로 확장하고, Sampling Point Size를 50으로 조정
        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceFont,
            50,                 // Sampling Point Size (11,172자 수용을 위해 50 정도가 적당함)
            9,                  // Padding
            UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, // SDFF8 -> SDFAA로 수정
            4096, 4096          // Atlas Width/Height (1024 -> 4096)
        );

        if (fontAsset == null)
        {
            EditorUtility.DisplayDialog("오류", "폰트 에셋 생성에 실패했습니다.", "확인");
            return;
        }

        // 2. 경로 설정 및 에셋 저장
        string path = AssetDatabase.GetAssetPath(sourceFont);
        string dir = Path.GetDirectoryName(path);
        string assetPath = Path.Combine(dir, sourceFont.name + "_Full_SDF.asset");

        AssetDatabase.CreateAsset(fontAsset, assetPath);

        // 3. 완성형 한글 11,172자 + 기본 ASCII 문자열 조합
        StringBuilder sb = new StringBuilder();

        // ASCII 기본 문법 (숫자, 영문, 특수문자)
        for (int i = 32; i <= 126; i++)
        {
            sb.Append((char)i);
        }

        // 한글 완성형 11,172자 (AC00 ~ D7A3)
        for (int i = 0xAC00; i <= 0xD7A3; i++)
        {
            sb.Append((char)i);
        }

        // 4. 글자 데이터를 폰트 에셋에 추가 및 아틀라스 갱신
        fontAsset.TryAddCharacters(sb.ToString());

        // 5. 변경사항 저장 및 리프레시
        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[성공] 정상적인 TMP 폰트 에셋 생성 완료: {assetPath}");
        EditorUtility.DisplayDialog("성공", "한글 폰트 에셋이 정상적으로 생성되었습니다!", "확인");
    }
}