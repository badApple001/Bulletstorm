using UnityEditor;
using Quark.Asset;
using System.IO;
using UnityEngine;

namespace Quark.Editor
{
    [CustomEditor( typeof( QuarkLauncher ), false )]
    public class QuarkLauncherEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        QuarkLauncher quarkConfig;
        bool encryptionToggle;
        SerializedProperty sp_AutoStartBasedOnConfig;
        SerializedProperty sp_LoadMode;
        SerializedProperty sp_QuarkDataset;
        SerializedProperty sp_QuarkBuildPath;
        SerializedProperty sp_EnableStreamingRelativeBundlePath;
        SerializedProperty sp_StreamingRelativeBundlePath;

        SerializedProperty sp_EnablePersistentRelativeBundlePath;
        SerializedProperty sp_PersistentRelativeBundlePath;

        SerializedProperty sp_EncryptionOffset;
        SerializedProperty sp_ManifestAesEncryptKey;

        string[] streammingAsset_options = null;
        int streammingAsset_options_index = 0;

        public override void OnInspectorGUI( )
        {
            targetObject.Update( );
            sp_AutoStartBasedOnConfig.boolValue = EditorGUILayout.ToggleLeft( "Auto-start based on launcher configuration", sp_AutoStartBasedOnConfig.boolValue );
            if ( sp_AutoStartBasedOnConfig.boolValue )
            {
                EditorGUILayout.Space( 8 );
                sp_LoadMode.enumValueIndex = ( byte ) ( QuarkLoadMode ) EditorGUILayout.EnumPopup( "Load mode", ( QuarkLoadMode ) sp_LoadMode.enumValueIndex );
                switch ( ( QuarkLoadMode ) sp_LoadMode.enumValueIndex )
                {
                    case QuarkLoadMode.AssetDatabase:
                        {
                            sp_QuarkDataset.objectReferenceValue = EditorGUILayout.ObjectField( "QuarkDataset", ( QuarkDataset ) sp_QuarkDataset.objectReferenceValue, typeof( QuarkDataset ), false );

                            if ( sp_QuarkDataset.objectReferenceValue == null )
                            {
                                var assets = AssetDatabase.FindAssets( "t:QuarkDataset", new string[] { "Assets" } );
                                if ( assets.Length > 0 )
                                {
                                    sp_QuarkDataset.objectReferenceValue = AssetDatabase.LoadAssetAtPath<QuarkDataset>( AssetDatabase.GUIDToAssetPath( assets[ 0 ] ) );
                                }
                            }
                        }
                        break;
                    case QuarkLoadMode.AssetBundle:
                        {
                            DrawBuildAssetBundleTab( );
                            //EditorGUILayout.Space( 8 );
                            //EditorGUILayout.LabelField( "Encryption", EditorStyles.boldLabel );
                            //encryptionToggle = EditorGUILayout.Foldout( encryptionToggle, "Encryption" );
                            //if ( encryptionToggle )
                            //{
                            //    DrawOffstEncryption( );
                            //    DrawAESEncryption( );
                            //}
                        }
                        break;
                }
            }
            targetObject.ApplyModifiedProperties( );
        }
        private void OnEnable( )
        {
            quarkConfig = target as QuarkLauncher;
            targetObject = new SerializedObject( quarkConfig );
            sp_AutoStartBasedOnConfig = targetObject.FindProperty( "autoStartBasedOnConfig" );
            sp_LoadMode = targetObject.FindProperty( "loadMode" );
            sp_QuarkDataset = targetObject.FindProperty( "quarkDataset" );
            sp_EncryptionOffset = targetObject.FindProperty( "encryptionOffset" );

            sp_EnableStreamingRelativeBundlePath = targetObject.FindProperty( "enableStreamingRelativeBuildPath" );
            sp_StreamingRelativeBundlePath = targetObject.FindProperty( "streamingRelativeBuildPath" );

            sp_EnablePersistentRelativeBundlePath = targetObject.FindProperty( "enablePersistentRelativeBundlePath" );
            sp_PersistentRelativeBundlePath = targetObject.FindProperty( "persistentRelativeBundlePath" );

            sp_ManifestAesEncryptKey = targetObject.FindProperty( "manifestAesKey" );
            sp_QuarkBuildPath = targetObject.FindProperty( "quarkBuildPath" );


            if ( Directory.Exists( Application.streamingAssetsPath ) )
            {
                var dirs = Directory.GetDirectories( Application.streamingAssetsPath );
                streammingAsset_options = new string[ dirs.Length ];
                for ( int i = 0; i < dirs.Length; i++ )
                {
                    streammingAsset_options[ i ] = dirs[ i ].Split( '\\' )[ 1 ];
                }
                int index = ArrayUtility.IndexOf<string>( streammingAsset_options, Application.version );
                if ( index < 0 )
                    index = 0;
                streammingAsset_options_index = index;
            }

        }



        void DrawBuildAssetBundleTab( )
        {
            EditorGUILayout.Space( 8 );

            EditorGUILayout.LabelField( "AssetBundle path", EditorStyles.boldLabel );

            sp_QuarkBuildPath.enumValueIndex = ( byte ) ( QuarkBuildPath ) EditorGUILayout.EnumPopup( "Path type", ( QuarkBuildPath ) sp_QuarkBuildPath.enumValueIndex );
            var buildType = ( QuarkBuildPath ) sp_QuarkBuildPath.enumValueIndex;
            switch ( buildType )
            {
                case QuarkBuildPath.StreamingAssets:
                    {
                        //sp_EnableStreamingRelativeBundlePath.boolValue = EditorGUILayout.ToggleLeft( "Enable streamingAsset relative path <Nullable>", sp_EnableStreamingRelativeBundlePath.boolValue );
                        //var useRelativePath = sp_EnableStreamingRelativeBundlePath.boolValue;
                        //if ( useRelativePath )
                        //{
                        //    EditorGUILayout.LabelField( "StreamingAsset relative path" );
                        //    //sp_StreamingRelativeBundlePath.stringValue = EditorGUILayout.TextField( sp_StreamingRelativeBundlePath.stringValue.Trim( ) );
                        //    if ( streammingAsset_options != null )
                        //    {

                        //        streammingAsset_options_index = EditorGUILayout.Popup( streammingAsset_options_index, streammingAsset_options );
                        //        if ( streammingAsset_options_index >= 0 && streammingAsset_options_index < streammingAsset_options.Length )
                        //        {
                        //            sp_StreamingRelativeBundlePath.stringValue = streammingAsset_options[ streammingAsset_options_index ];
                        //        }
                        //    }
                        //    else
                        //    {
                        //        EditorGUILayout.LabelField( "You must build assetbundle!" );
                        //    }
                        //}
                    }
                    break;
                case QuarkBuildPath.PersistentDataPath:
                    {
                        sp_EnablePersistentRelativeBundlePath.boolValue = EditorGUILayout.ToggleLeft( "Enable persistentDataPath relative path <Nullable>", sp_EnablePersistentRelativeBundlePath.boolValue );
                        var useRelativePath = sp_EnablePersistentRelativeBundlePath.boolValue;
                        if ( useRelativePath )
                        {
                            EditorGUILayout.LabelField( "PersistentDataPath relative path" );
                            sp_PersistentRelativeBundlePath.stringValue = EditorGUILayout.TextField( sp_PersistentRelativeBundlePath.stringValue.Trim( ) );
                        }
                    }
                    break;
            }
        }
        //void DrawOffstEncryption( )
        //{
        //    EditorGUILayout.LabelField( "Encryption offset", sp_EncryptionOffset.longValue.ToString( ) );
        //    sp_EncryptionOffset.longValue = EditorPrefs.GetInt( nameof( QuarkLauncher ) + ".Encrypt.offset" );
        //}
        //void DrawAESEncryption( )
        //{
        //    EditorGUILayout.Space( 8 );
        //    EditorGUILayout.LabelField( "Manifest aes key", sp_ManifestAesEncryptKey.stringValue );
        //    var keyStr = sp_ManifestAesEncryptKey.stringValue;
        //    var keyLength = System.Text.Encoding.UTF8.GetBytes( keyStr ).Length;
        //    EditorGUILayout.LabelField( $"Current key length is:{keyLength}" );
        //    if ( keyLength != 16 && keyLength != 24 && keyLength != 32 && keyLength != 0 )
        //    {
        //        EditorGUILayout.HelpBox( "Key should be 16,24 or 32 bytes long", MessageType.Error );
        //    }
        //    sp_ManifestAesEncryptKey.stringValue = EditorPrefs.GetString( nameof( QuarkLauncher ) + ".Encrypt.aes" );
        //}
    }
}