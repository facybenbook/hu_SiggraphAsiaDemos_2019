using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Devdog.General.Editors;
using UnityEditor.Callbacks;

namespace Devdog.SciFiDesign.Editors
{
    [InitializeOnLoad]
    public class SciFiDesignGettingStarted : GettingStartedEditorBase
    {
        private const string MenuItemPath = SciFiDesignConstants.ToolsMenuPath + "Getting started";

        public SciFiDesignGettingStarted()
        {
            version = SciFiDesignConstants.Version;
            productName = SciFiDesignConstants.ProductName;
            documentationUrl = SciFiDesignConstants.ProductUrl;
            productsFetchApiUrl = "http://devdog.io/unity/editorproducts.php?product=" + SciFiDesignConstants.ProductName;
            reviewProductUrl = "https://www.assetstore.unity3d.com/en/#!/content/55270";
        }

        [MenuItem(MenuItemPath, false, 1)] // Always at bottom
        protected static void ShowWindowInternal()
        {
            window = GetWindow<SciFiDesignGettingStarted>();
            window.ShowWindow();
        }

        public override void ShowWindow()
        {
            window = GetWindow<SciFiDesignGettingStarted>();
            window.GetImages();
            window.GetProducts();

            window.ShowUtility();
        }

        [InitializeOnLoadMethod]
        protected static void InitializeOnLoadMethod()
        {
            window = GetWindow<SciFiDesignGettingStarted>();
            window.showOnStart = EditorPrefs.GetBool(window.editorPrefsKey, true);
            if (window.showOnStart == false)
            {
                window.Close();
            }

            window.DoUpdate();
        }

        [DidReloadScripts]
        protected static void DidReloadScripts()
        {
            window.didReloadScripts = true;
        }

        protected override void DrawGettingStarted()
        {
            DrawBox(0, 0, "Documentation", "The official documentation has a detailed description of all components and code examples.", documentationIcon, () =>
            {
                Application.OpenURL(documentationUrl);
            });

            DrawBox(1, 0, "Video tutorials", "The video tutorials cover all interfaces and a complete set up.", videoTutorialsIcon, () =>
            {
                Application.OpenURL(youtubeUrl);
            });

            DrawBox(2, 0, "Forums", "Check out the " + productName + " forums for some community power.", forumIcon, () =>
            {
                Application.OpenURL(forumUrl);
            });

            DrawBox(3, 0, "Integrations", "Combine the power of assets and enable integrations.", integrationsIcon, () =>
            {
                IntegrationHelperEditor.ShowWindow();
            });

            DrawBox(4, 0, "Rate / Review", "Like " + productName + "? Share the experience :)", reviewIcon, () =>
            {
                Application.OpenURL(reviewProductUrl);
            });

            base.DrawGettingStarted();
        }
    }
}