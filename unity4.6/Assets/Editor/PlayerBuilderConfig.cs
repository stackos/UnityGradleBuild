using System.Collections.Generic;

public class PlayerBuilderConfig {
    public const string BUILD_PATH = "PlayerBuild";
    public const string TEMPLATE_PATH = "AndroidTemplates";
    public const string OUT_PATH = "bin";
    public const string PLATFORM_NAME_ANDROID = "Android";
    public const string PLATFORM_NAME_IOS = "iOS";

#if UNITY_ANDROID
    public static Channel[] GetChannels() {
        List<Channel> channels = new List<Channel>();
        Channel ch;

        ///  ----------------------
        ///  add channel info begin
        ///  ----------------------

        ///  YouMe
        ch = new Channel();
        ch.name = "SparklingGame";
        ch.template_project_name = "AndroidStudioProjectBase";
        ch.enable = true;
        channels.Add(ch);

        ///  ----------------------
        ///  add channel info end
        ///  ----------------------

        return channels.ToArray();
    }
#endif

#if UNITY_IPHONE
    public static Channel[] GetChannels() {
        List<Channel> channels = new List<Channel>();
        Channel ch;

        ///  ----------------------
        ///  add channel info begin
        ///  ----------------------

        ///  Test
        ch = new Channel();
        ch.name = "Test";
        ch.template_project_name = "test";
        ch.enable = true;
        channels.Add(ch);

        ///  ----------------------
        ///  add channel info end
        ///  ----------------------

        return channels.ToArray();
    }
#endif

    public class Channel {
        public string name;
        public string template_project_name;
        public bool enable;
    }
}