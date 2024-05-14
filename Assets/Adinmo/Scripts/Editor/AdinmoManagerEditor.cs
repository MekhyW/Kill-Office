using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Adinmo
{
    [CustomEditor(typeof(AdinmoManager))]
    [CanEditMultipleObjects]
    public class AdinmoManagerEditor : Editor
    {
        private Texture2D AdinmoLogo;
        void OnEnable()
        {
            string editorScriptPath = AdinmoEditorCommon.getEditorScriptPath();
            
            AdinmoManager.m_iOSAdMobAppId = AdinmoEditorCommon.getiOSCode();
            AdinmoLogo = (Texture2D)AssetDatabase.LoadAssetAtPath(editorScriptPath + "/Textures/AdinMoLogo.png", typeof(Texture2D));
        }

        public override void OnInspectorGUI() //This controls how the GUI should be rendered in inspector
        {
            //Render the common AdinmoManagerUI
            Rect rect = new Rect();
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            AdinmoEditorCommon.RenderAdinmoManagerUI(serializedObject, AdinmoLogo, ref rect, false,AdinmoEditorCommon.getEditorScriptPath());
            //Display the version number
            var versionStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerRight, fontSize = 10 };
            EditorGUILayout.LabelField("", "v" + AdinmoSender.GetVersion() + " s" + AdinmoSender.GetSdkSource(), versionStyle);
        }
    }
}
