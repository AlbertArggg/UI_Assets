using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PropollyGDS_UI_Pack.Editor.Custom_Menu_Items;
using UnityEngine;

namespace PropollyGDS_UI_Pack.Editor.CustomMenuItems.CreateFile
{
    public class TemplateManager
    {
        private Dictionary<string, Template> templates;

        public TemplateManager()
        {
            templates = new Dictionary<string, Template>();
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            templates["Abstract Class"] = new Template(Constants.ScriptingTemplates.ABSTRACT_CLASS);
            templates["C#"] = new Template(Constants.ScriptingTemplates.CSHARP_CLASS);
            templates["Interface"] = new Template(Constants.ScriptingTemplates.INTERFACE);
            templates["MonoBehaviour"] = new Template(Constants.ScriptingTemplates.MONOBEHAVIOUR);
            templates["Popup"] = new Template(Constants.ScriptingTemplates.POPUP);
            templates["Scriptable Object"] = new Template(Constants.ScriptingTemplates.SCRIPTABLE_OBJECT);
            templates["Singleton"] = new Template(Constants.ScriptingTemplates.SINGLETON);
            templates["UI Document"] = new Template(Constants.ScriptingTemplates.UI_DOCUMENT);
        }

        public Template GetTemplate(string templateType)
        {
            if (templates.TryGetValue(templateType, out var template)) return template;
            
            Debug.LogError($"Template not found: {templateType}");
            return null;
        }
    }

    public class Template
    {
        public string Content { get; private set; }
        public IEnumerable<string> Sections { get; private set; }

        public Template(string resourcePath)
        {
            LoadTemplate(resourcePath);
            ParseSections();
        }

        private void LoadTemplate(string resourcePath)
        {
            var textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset != null)
            {
                Content = textAsset.text;
            }
            else
            {
                Debug.LogError($"Failed to load template: {resourcePath}");
            }
        }

        private void ParseSections()
        {
            var sectionNames = new List<string>();
            var regex = new Regex("// \\[SECTION:(.*?)\\]");
            var matches = regex.Matches(Content);

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count > 1)
                {
                    sectionNames.Add(match.Groups[1].Value);
                }
            }

            Sections = sectionNames.Distinct();
        }
    }
}
