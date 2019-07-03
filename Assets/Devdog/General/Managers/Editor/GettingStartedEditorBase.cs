using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;
using UnityEditor.Callbacks;

namespace Devdog.General.Editors
{
    [InitializeOnLoad]
    public abstract class GettingStartedEditorBase : EditorWindow
    {
        public struct ProductInfo
        {
            public string productName { get; set; }
            public string assetStoreUrl { get; set; }
            public Texture icon { get; set; }
        }

        protected const int SingleColWidth = 400;
        protected static Texture documentationIcon;
        protected static Texture videoTutorialsIcon;
        protected static Texture reviewIcon;
        protected static Texture forumIcon;
        protected static Texture integrationsIcon;
        protected static Texture newsletterIcon;

        public bool showOnStart = true;
        protected int heightExtra;

        protected string productName;
        protected string documentationUrl;
        protected string signupNewsletterApiUrl = "http://devdog.io/unity/mailchimpsignup.php";
        protected string productsFetchApiUrl;
        protected string youtubeUrl;
        protected string forumUrl = "http://forum.devdog.io";
        protected string reviewProductUrl;
        protected string version;

        private const string IconRootPath = "Assets/Devdog/General/EditorStyles/";
        private const string DocumentationIconUri = IconRootPath + "Documentation.png";
        private const string VideoTutorialsIconUri = IconRootPath + "Youtube.png";
        private const string ReviewIconUri = IconRootPath + "Star.png";
        private const string ForumIconUri = IconRootPath + "Forum.png";
        private const string IntegrationIconUri = IconRootPath + "Integration.png";
        private const string NewsletterIconUri = IconRootPath + "MailNotSignedUp.png";
        private const string NewsletterSubscribedIconUri = IconRootPath + "MailSignedUp.png";

        public static GettingStartedEditorBase window { get; protected set; }

        private List<ProductInfo> _productInfo = new List<ProductInfo>();
        public string editorPrefsKey
        {
            get { return "SHOW_" + productName + "_GETTING_STARTED_WINDOW" + version; }
        }

        private static string _email;
        private const string DevdogNewsletterSignupKey = "DEVDOG_SIGNUP_EMAIL";
        private static string subscribedWithEmail
        {
            get
            {
                if (string.IsNullOrEmpty(_email) && EditorPrefs.HasKey(DevdogNewsletterSignupKey))
                {
                    _email = EditorPrefs.GetString(DevdogNewsletterSignupKey);
                }

                return _email;
            }
            set
            {
                EditorPrefs.SetString(DevdogNewsletterSignupKey, value);
                _email = value;
            }
        }

        public bool didReloadScripts = false;
        
        public void DoUpdate()
        {
            if (didReloadScripts == false)
            {
                if (EditorPrefs.GetBool(window.editorPrefsKey, true))
                {
                    window.GetImages();
                    window.GetProducts();
                    window.ShowUtility();
                }

                didReloadScripts = false;
            }
        }

        public void GetImages()
        {
            documentationIcon = AssetDatabase.LoadAssetAtPath<Texture>(DocumentationIconUri);
            videoTutorialsIcon = AssetDatabase.LoadAssetAtPath<Texture>(VideoTutorialsIconUri);
            reviewIcon = AssetDatabase.LoadAssetAtPath<Texture>(ReviewIconUri);
            forumIcon = AssetDatabase.LoadAssetAtPath<Texture>(ForumIconUri);
            integrationsIcon = AssetDatabase.LoadAssetAtPath<Texture>(IntegrationIconUri);

            newsletterIcon = AssetDatabase.LoadAssetAtPath<Texture>(NewsletterIconUri);
            if (EditorPrefs.HasKey(DevdogNewsletterSignupKey))
            {
                newsletterIcon = AssetDatabase.LoadAssetAtPath<Texture>(NewsletterSubscribedIconUri);
            }
        }

        public void GetProducts()
        {
            _productInfo.Clear();
            using (var client = new WebClient())
            {
//                client.DownloadStringCompleted += ClientOnDownloadStringCompleted;
                var s = client.DownloadString(new Uri(productsFetchApiUrl));
                ClientOnDownloadStringCompleted(s);
            }
        }

        private void ClientOnDownloadStringCompleted(string str)
        {
            var arr = str.Split(new string[] { ">>>>" }, StringSplitOptions.None);
            foreach (var s in arr)
            {
                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }

                _productInfo.Add(ParseProductInfo(s));
            }
        }

        protected ProductInfo ParseProductInfo(string s)
        {
            var info = s.Split('|');
            using (var www = new WWW(info[1]))
            {
                var startTime = EditorApplication.timeSinceStartup;
                while (www.isDone == false || EditorApplication.timeSinceStartup - startTime > 5f)
                {
                    // Wait...    
                }
                var icon = www.texture;

                return new ProductInfo()
                {
                    productName = info[0],
                    icon = icon,
                    assetStoreUrl = info[2],
                };
            }
        }

        public abstract void ShowWindow();


        protected void OnGUI()
        {
            heightExtra = 0;

            GUILayout.BeginHorizontal("Toolbar", GUILayout.Width(window.position.size.x));
            GUILayout.Label(productName + " Version: " + version);
            GUILayout.EndVertical();

            GUILayout.BeginArea(new Rect(0, 0, SingleColWidth, window.position.height));
            DrawGettingStarted();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(SingleColWidth + (SingleColWidth / 50), 0, SingleColWidth, 260));
            DrawMailSignupForm();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(SingleColWidth + (SingleColWidth / 50), 260, SingleColWidth, window.position.height - 260));
            DrawOtherProducts();
            GUILayout.EndArea();
        }

        protected void DrawOtherProducts()
        {
            GUILayout.Space(30);

            EditorGUILayout.LabelField("Other great products you should check out.", UnityEditor.EditorStyles.boldLabel);

            for (int i = 0; i < _productInfo.Count; i++)
            {
                DrawProduct(i, _productInfo[i].icon, _productInfo[i].assetStoreUrl);
            }
        }

        protected void DrawProduct(int index, Texture icon, string url)
        {
            var rect = new Rect(70 * index, 64, 64, 64);
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                Application.OpenURL(url);
            }

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            GUI.DrawTexture(rect, icon);
        }

        protected void DrawMailSignupForm()
        {
            GUI.DrawTexture(new Rect(SingleColWidth / 2 - 32, 30, 64, 64), newsletterIcon);
            GUILayout.Space(100);

            EditorGUILayout.LabelField("Never miss a thing, sign up for the newsletter.", UnityEditor.EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Get notified about updates, upgrade guides and new products.");

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            //            EditorGUILayout.LabelField("Name");
            //            _name = EditorGUILayout.TextField(_name);

            EditorGUILayout.LabelField("Email");
            _email = EditorGUILayout.TextField(_email);

            GUILayout.Space(10);

            if (EditorPrefs.HasKey(DevdogNewsletterSignupKey))
            {
                EditorGUILayout.LabelField("Subscribed with " + EditorPrefs.GetString(DevdogNewsletterSignupKey), UnityEditor.EditorStyles.wordWrappedLabel);
            }

            if (string.IsNullOrEmpty(_email))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.color = Color.green;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Subscribe", "LargeButton"))
            {
                SubscribeToMailingList(_email);
            }

            GUI.enabled = true;
            GUI.color = Color.white;
        }

        protected void SubscribeToMailingList(string email)
        {
            string result = "";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                result = client.UploadString(signupNewsletterApiUrl + "?email_address=" + email, "POST");
            }

            if (result.Contains("\"status\":\"subscribed\""))
            {
                subscribedWithEmail = email;
                DevdogLogger.Log("Successfully subscribed to mailing list :)");

                Repaint();
            }
            else
            {
                DevdogLogger.Log("Whoops something went wrong while subscribing");
                DevdogLogger.Log(result);
            }

            GetImages();
        }

        protected virtual void DrawGettingStarted()
        {
            var toggle = GUI.Toggle(new Rect(10, window.position.height - 20, SingleColWidth - 10, 20), showOnStart, "Show " + productName + " getting started on start.");
            if (toggle != showOnStart)
            {
                showOnStart = toggle;
                EditorPrefs.SetBool(editorPrefsKey, toggle);
            }
        }

        protected void DrawBox(int index, int extraHeight, string title, string desc, Texture texture, Action action)
        {
            heightExtra += extraHeight;

            const int spacing = 10;
            const int offset = 30;
            int offsetY = offset + (spacing * index) + (64 * index);

            GUI.BeginGroup(new Rect(10, offsetY, SingleColWidth - 20, 64 + heightExtra), Devdog.General.Editors.EditorStyles.boxStyle);

            var rect = new Rect(0, 0, SingleColWidth - 20, 64 + heightExtra);
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                action();
            }

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            var iconRect = new Rect(15, 7, 50, 50);
            GUI.DrawTexture(iconRect, texture);

            rect.x = 74;
            rect.y += 5;
            rect.width = SingleColWidth - 90;

            GUI.Label(rect, title, UnityEditor.EditorStyles.boldLabel);

            rect.y += 20;
            rect.height = 44;
            GUI.Label(rect, desc, UnityEditor.EditorStyles.wordWrappedLabel);

            GUI.EndGroup();
        }

        private bool Button(GUIContent content)
        {
            return GUILayout.Button(content, GUILayout.Width(128), GUILayout.Height(128));
        }
    }
}