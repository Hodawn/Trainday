using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System.Reflection;

namespace VideoPlayer360
{
    [CustomEditor(typeof(MediaPlayerController))]
    public class MediaPlayerControllerEditor : Editor
    {
        private SerializedObject SO_Target;

        private SerializedProperty SP_videoPlayerType;
        private SerializedProperty SP_playListMode;
        private SerializedProperty SP_currentVideo;
        private SerializedProperty SP_videoPlaylist;
        private SerializedProperty SP_target;
        private SerializedProperty SP_playOnStart;
        private SerializedProperty SP_menuSpawnMode;
        private SerializedProperty SP_menuPrefab;
        private SerializedProperty SP_menuSpawnPosition;
        private SerializedProperty SP_iconPlay, SP_iconPause, SP_iconPrevious, SP_iconNext, SP_iconVideoWall;

        public void OnEnable()
        {
            SO_Target = new SerializedObject(target);

            SP_videoPlayerType = SO_Target.FindProperty("videoPlayerType");
            SP_playListMode = SO_Target.FindProperty("playListMode");
            SP_currentVideo = SO_Target.FindProperty("currentVideo");
            SP_videoPlaylist = SO_Target.FindProperty("videoPlaylist");
            SP_target = SO_Target.FindProperty("target");
            SP_playOnStart = SO_Target.FindProperty("playOnStart");

            SP_menuSpawnMode = SO_Target.FindProperty("menuSpawnMode");
            SP_menuPrefab = SO_Target.FindProperty("menuPrefab");
            SP_menuSpawnPosition = SO_Target.FindProperty("menuSpawnPosition");

            SP_iconPlay = SO_Target.FindProperty("iconPlay");
            SP_iconPause = SO_Target.FindProperty("iconPause");
            SP_iconNext = SO_Target.FindProperty("iconNext");
            SP_iconPrevious = SO_Target.FindProperty("iconPrevious");
            SP_iconVideoWall = SO_Target.FindProperty("iconVideoWall");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Video Player", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(SP_videoPlayerType);
            EditorGUILayout.PropertyField(SP_playListMode);

            if ((MediaPlayerController.ePlayListMode)SP_playListMode.enumValueIndex == MediaPlayerController.ePlayListMode.SingleVideo)
            {
                EditorGUILayout.PropertyField(SP_currentVideo, true);
            }
            else
            {
                /*EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PrefixLabel("Current Video");
                SP_fileName.stringValue = EditorGUILayout.TextField(SP_fileName.stringValue);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();*/

                EditorGUILayout.PropertyField(SP_videoPlaylist, true);
            }

            EditorGUILayout.PropertyField(SP_target);
            EditorGUILayout.PropertyField(SP_playOnStart);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Menu", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(SP_menuSpawnMode);
            EditorGUILayout.PropertyField(SP_menuPrefab);
            EditorGUILayout.PropertyField(SP_menuSpawnPosition);
            EditorGUILayout.PropertyField(SP_iconPlay);
            EditorGUILayout.PropertyField(SP_iconPause);
            EditorGUILayout.PropertyField(SP_iconPrevious);
            EditorGUILayout.PropertyField(SP_iconNext);
            EditorGUILayout.PropertyField(SP_iconVideoWall);

            if (EditorGUI.EndChangeCheck())
            {
                SO_Target.ApplyModifiedProperties();
            }
        }
    }
}