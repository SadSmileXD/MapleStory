using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TMPFontAssetBuilder : EditorWindow
{
    private Font targetFont;
    private string savePath = "Assets/Fonts";
    private int samplingPointSize = 36;
    private int atlasWidth = 2048;
    private int atlasHeight = 2048;
    private int padding = 7;

    [MenuItem("Tools/TextMeshPro/TMP Font Asset Builder")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontAssetBuilder>("TMP Font Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("TMP Font Asset Auto Builder", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetFont = (Font)EditorGUILayout.ObjectField("Source TTF Font", targetFont, typeof(Font), false);
        savePath = EditorGUILayout.TextField("Save Directory", savePath);

        EditorGUILayout.Space();
        samplingPointSize = EditorGUILayout.IntField("Sampling Size", samplingPointSize);
        padding = EditorGUILayout.IntField("Padding", padding);
        atlasWidth = EditorGUILayout.IntField("Atlas Width", atlasWidth);
        atlasHeight = EditorGUILayout.IntField("Atlas Height", atlasHeight);

        EditorGUILayout.Space();

        if (GUILayout.Button("1. Extract Used Characters from Project Assets"))
        {
            string extractedChars = ExtractCharactersFromProject();
            GUIUtility.systemCopyBuffer = extractedChars;
            Debug.Log($"[TMP Font Builder] Extracted {extractedChars.Length} unique characters. Copied to Clipboard!");
        }

        if (GUILayout.Button("2. Create Dynamic TMP Font Asset"))
        {
            CreateDynamicFontAsset();
        }
    }

    private string ExtractCharactersFromProject()
    {
        HashSet<char> charSet = new HashSet<char>();

        // 기본 ASCII 문자는 무조건 포함
        for (char c = (char)32; c <= (char)126; c++)
        {
            charSet.Add(c);
        }

        // 프로젝트 내 .txt, .json, .csv 파일에서 문자를 수집
        string[] fileExtensions = new[] { "*.txt", "*.json", "*.csv" };
        foreach (string ext in fileExtensions)
        {
            string[] files = Directory.GetFiles(Application.dataPath, ext, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string text = File.ReadAllText(file, Encoding.UTF8);
                foreach (char c in text)
                {
                    if (!char.IsControl(c)) charSet.Add(c);
                }
            }
        }

        List<char> sortedChars = charSet.ToList();
        sortedChars.Sort();
        return new string(sortedChars.ToArray());
    }

    private void CreateDynamicFontAsset()
    {
        if (targetFont == null)
        {
            EditorUtility.DisplayDialog("Error", "Source Font File(TTF/OTF)을 먼저 할당해 주세요.", "OK");
            return;
        }

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string assetPath = Path.Combine(savePath, $"{targetFont.name} SDF.asset");

        // Dynamic 모드로 Font Asset 생성 (런타임 추가 글자 대응)
        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            targetFont,
            samplingPointSize,
            padding,
            UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
            atlasWidth,
            atlasHeight,
            AtlasPopulationMode.Dynamic
        );

        if (fontAsset == null)
        {
            Debug.LogError("[TMP Font Builder] Font Asset 생성 실패. 폰트 세그먼트 데이터를 확인해 주세요.");
            return;
        }

        AssetDatabase.CreateAsset(fontAsset, assetPath);

        // 생성된 아틀라스 텍스처 및 머티리얼 서브에셋 결합
        if (fontAsset.atlasTexture != null)
        {
            fontAsset.atlasTexture.name = $"{targetFont.name} Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
        }

        if (fontAsset.material != null)
        {
            fontAsset.material.name = $"{targetFont.name} Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Complete", $"TMP Font Asset이 성공적으로 생성되었습니다!\n경로: {assetPath}", "OK");
        Selection.activeObject = fontAsset;
    }
}