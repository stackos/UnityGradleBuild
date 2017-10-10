using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlayerBuilder : EditorWindow {
    PlayerBuilderConfig.Channel[] m_channels = PlayerBuilderConfig.GetChannels();
    bool m_android_toggle = false;
    bool m_ios_toggle = false;
    //bool m_compile_script_only = false;

    [MenuItem("SparklingGame/Player Builder")]
    static void Init() {
        var window = (PlayerBuilder) GetWindow(typeof(PlayerBuilder), true, "Player Builder", true);

        if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
            window.m_android_toggle = true;
        }
        if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone) {
            window.m_ios_toggle = true;
        }
    }

    void OnGUI() {
        EditorGUILayout.Space();

        EditorGUI.indentLevel = 0;
        if(EditorGUILayout.Foldout(m_android_toggle, PlayerBuilderConfig.PLATFORM_NAME_ANDROID)) {
            DrawUI(PlayerBuilderConfig.PLATFORM_NAME_ANDROID);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.indentLevel = 0;
        if(EditorGUILayout.Foldout(m_ios_toggle, PlayerBuilderConfig.PLATFORM_NAME_IOS)) {
            DrawUI(PlayerBuilderConfig.PLATFORM_NAME_IOS);
        }
    }

    void DrawUI(string platform) {
        EditorGUI.indentLevel = 1;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Select Channels");

        EditorGUILayout.Space();

        EditorGUI.indentLevel = 2;
        EditorGUILayout.BeginHorizontal();
        for(int i = 0; i < m_channels.Length; i++) {
            if(i % 3 == 0 && i != 0) {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
            }

            m_channels[i].enable = EditorGUILayout.ToggleLeft(m_channels[i].name, m_channels[i].enable);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel = 1;

        EditorGUILayout.Space();

#if UNITY_ANDROID
        if(platform == PlayerBuilderConfig.PLATFORM_NAME_ANDROID) {
			//m_debug_key = EditorGUILayout.Toggle("Debug Key", m_debug_key);
		}
#endif

		EditorGUI.indentLevel = 1;

        //EditorGUILayout.Space();
        //m_compile_script_only = EditorGUILayout.Toggle("Compile Script Only", m_compile_script_only);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if(GUILayout.Button("Build " + platform.ToString(), GUILayout.Height(40))) {
            if(platform == PlayerBuilderConfig.PLATFORM_NAME_ANDROID) {
#if UNITY_ANDROID
				EditorApplication.delayCall += BuildAndroid;
#endif
			} else {
#if UNITY_IPHONE
				EditorApplication.delayCall += BuildIOS;
#endif
			}
		}
    }

#if UNITY_ANDROID
    bool m_debug_key = true;

    /// 输出工程的assets目录会被覆盖，模板工程的assets里面不要放自定义资源
    void BuildAndroid() {
        try {
            const string platform = "Android";
            string project_out_path = Application.dataPath + "/../" + PlayerBuilderConfig.BUILD_PATH + "/" + platform;
            string out_path_local = PlayerBuilderConfig.BUILD_PATH + "/" + platform;

            string project_path = project_out_path + "/" + PlayerSettings.productName;
            if(Directory.Exists(project_path)) {
                Directory.Delete(project_path, true);
            }

			var scenes = EditorBuildSettings.scenes;
			List<string> scene_names = new List<string>();
			foreach(var i in scenes) {
				scene_names.Add(i.path);
			}
	
			BuildPipeline.BuildPlayer(scene_names.ToArray(), out_path_local, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);

			string apk_out_path = project_out_path + "/" + PlayerBuilderConfig.OUT_PATH;
            if(!Directory.Exists(apk_out_path)) {
                Directory.CreateDirectory(apk_out_path);
            }

            for(int i = 0; i < m_channels.Length; i++) {
                if(m_channels[i].enable) {
                    EditorUtility.DisplayProgressBar("Building APK", m_channels[i].name, i / (float) m_channels.Length);

                    string template = PlayerBuilderConfig.TEMPLATE_PATH + "/" + m_channels[i].template_project_name;
                    string project_path_temp = apk_out_path + "/temp/" + m_channels[i].template_project_name;
					if(Directory.Exists(project_path_temp)) {
						Directory.Delete(project_path_temp, true);
					}

					/// copy channel template
					DirectoryCopy(template, project_path_temp);

                    DirectoryCopy(project_path + "/assets", project_path_temp + "/src/main/assets");
					DirectoryCopy(project_path + "/libs", project_path_temp + "/src/main/jniLibs");
					DirectoryCopy(project_path + "/res", project_path_temp + "/src/main/res");

					var unity_jar_name = "unity-classes.jar";
					var unity_jar = project_path_temp + "/src/main/jniLibs/" + unity_jar_name;
					if(File.Exists(unity_jar)) {
						File.Move(unity_jar, project_path_temp + "/libs/" + unity_jar_name);
					}

					/// build apk
					BuildAPKGradle(project_path_temp, apk_out_path + "/" + m_channels[i].name, m_channels[i].template_project_name);
				}
            }
        } finally {
            EditorUtility.ClearProgressBar();
        }

        EditorUtility.DisplayDialog("Player Builder", "Build Complete.", "OK");
    }

	void BuildAPKGradle(string project_path, string out_path, string project_name) {
		System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
		info.FileName = "cmd.exe";
		info.Arguments = "/c gradle build";
		info.WorkingDirectory = project_path;
		System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);
		p.WaitForExit();

		string apk;
		if(m_debug_key) {
			apk = project_path + "/build/outputs/apk/" + project_name + "-debug.apk";
			out_path += "-Debug.apk";
		} else {
			apk = project_path + "/build/outputs/apk/" + project_name + "-release-unsigned.apk";
			out_path += "-Release.apk";
		}

		if(File.Exists(apk)) {
			File.Copy(apk, out_path, true);
		} else {
			Debug.LogError("Build failed: " + project_path);
		}
	}

	void BuildAPKAnt(string project_path, string out_path, string project_name) {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
        info.FileName = "cmd.exe";
        if(m_debug_key) {
            info.Arguments = "/c ant debug";
        } else {
            info.Arguments = "/c ant release";
        }
        info.WorkingDirectory = project_path;
        System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);
        p.WaitForExit();

        string ant_apk;
        if(m_debug_key) {
            ant_apk = project_path + "/bin/" + project_name + "-debug.apk";
            out_path += "-Debug.apk";
        } else {
            ant_apk = project_path + "/bin/" + project_name + "-release.apk";
            out_path += "-Release.apk";
        }

        if(File.Exists(ant_apk)) {
            File.Copy(ant_apk, out_path, true);
        } else {
            Debug.LogError("Build error: " + project_name);
        }
    }
#endif

#if UNITY_IPHONE
	/// 输出工程的Classes目录会被覆盖，模板工程的Classes里面不要放自定义代码
	/// 输出工程的Data目录会被覆盖，模板工程的Data里面不要放自定义资源
	void BuildIOS() {
        try {
            string project_out_path = Application.dataPath + "/../" + PlayerBuilderConfig.BUILD_PATH + "/" + PlayerBuilderConfig.PLATFORM_NAME_IOS;
            string out_path_local = PlayerBuilderConfig.BUILD_PATH + "/" + PlayerBuilderConfig.PLATFORM_NAME_IOS + "/" + PlayerSettings.productName;

            string project_path = project_out_path + "/" + PlayerSettings.productName;
            if(Directory.Exists(project_path)) {
                Directory.Delete(project_path, true);
            }

            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, out_path_local, BuildTarget.iOS, BuildOptions.None);

            string ipa_out_path = project_out_path + "/" + PlayerBuilderConfig.OUT_PATH;
            if(!Directory.Exists(ipa_out_path)) {
                Directory.CreateDirectory(ipa_out_path);
            }

            for(int i = 0; i < m_channels.Length; i++) {
                if(m_channels[i].enable) {
                    EditorUtility.DisplayProgressBar("Building IPA", m_channels[i].name, i / (float) m_channels.Length);

                    string template = project_out_path + "/" + PlayerBuilderConfig.TEMPLATE_PATH + "/" + m_channels[i].template_project_name;
                    string project_path_temp = ipa_out_path + "/temp";

                    /// copy channel template
                    DirectoryCopy(template, project_path_temp);
                    /// replace Classes
                    DirectoryCopy(project_path + "/Classes", project_path_temp + "/Classes");
                    /// replace Data
                    DirectoryCopy(project_path + "/Data", project_path_temp + "/Data");
                    /// build ipa
                    BuildIPA(project_path_temp, ipa_out_path + "/" + m_channels[i].name, m_channels[i].template_project_name);
                }
            }
        } finally {
            EditorUtility.ClearProgressBar();
        }

        EditorUtility.DisplayDialog("Player Builder", "Build Complete.", "OK");
    }

    void BuildIPA(string project_path, string out_path, string project_name) {
        out_path += ".ipa";

        Debug.Log(project_path);
        Debug.Log(out_path);
        Debug.Log(project_name);
    }
#endif

	void DirectoryCopy(string src, string dest) {
        if(!Directory.Exists(src)) {
            return;
        }

        if(Directory.Exists(dest)) {
            Directory.Delete(dest, true);
        }
        Directory.CreateDirectory(dest);

        var files = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories);
        foreach(var i in files) {
            string dest_file = dest + NormalizePath(i.Substring(src.Length));
            var info = new FileInfo(dest_file);
            if(!info.Directory.Exists) {
                Directory.CreateDirectory(info.DirectoryName);
            }
            File.Copy(i, dest_file, true);
        }

		var dirs = Directory.GetDirectories(src, "*", SearchOption.AllDirectories);
		foreach(var i in dirs) {
			string dest_dir = dest + NormalizePath(i.Substring(src.Length));
			if(!Directory.Exists(dest_dir)) {
				Directory.CreateDirectory(dest_dir);
			}
		}
	} 

    string NormalizePath(string path) {
        return path.Replace("\\", "/");
    }
}