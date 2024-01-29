using System;
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
        
        public string[] GetTemplateKeys() => 
            templates.Keys.ToArray();
        
        public string GetTemplateContent(string key) => 
            templates.TryGetValue(key, out Template template) ? template.Content : string.Empty;

        public Template GetTemplate(string templateType) =>
            templates.TryGetValue(templateType, out var template) ? template : null;

        public string ParseAndProcessTemplate(string directory, string templateKey, string fileName, 
            bool includeNamespace, Dictionary<string, bool> sectionToggles)
        {
            string generatedClassContent = GetTemplateContent(templateKey);
            generatedClassContent = generatedClassContent.Replace("#SCRIPTNAME#", fileName);
            
            foreach (var section in sectionToggles)
            {
                string pattern = $"// \\[SECTION:{section.Key}\\](.*?)// \\[ENDSECTION\\]";
                Match match = Regex.Match(generatedClassContent, pattern, RegexOptions.Singleline);
                generatedClassContent = match.Success
                    ? section.Value
                        ? generatedClassContent.Replace(match.Value, match.Groups[1].Value)
                        : generatedClassContent.Replace(match.Value, "")
                    : generatedClassContent;
            }
            
            if (!includeNamespace)
            {
                string namespacePattern = @"namespace #NAMESPACE#\s*{((?:[^{}]|{(?<c>)|}(?<-c>))*(?(c)(?!)))}";
                generatedClassContent = Regex.Replace(generatedClassContent, namespacePattern, m =>
                {
                    var content = m.Groups[1].Value;
                    var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    var adjustedLines = lines.Select(line => line.StartsWith("    ") ? line.Substring(4) : line);
                    return string.Join(Environment.NewLine, adjustedLines);
                }, RegexOptions.Singleline);
            }
            else
            {
                generatedClassContent = generatedClassContent.Replace("#NAMESPACE#", directory.GenerateNamespaceFromDirectory());
            }
            
            generatedClassContent = Regex.Replace(generatedClassContent, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            return generatedClassContent;
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
            if (textAsset == null) return;
            
            Content = textAsset.text;
        }

        private void ParseSections()
        {
            var sectionNames = new List<string>();
            var regex = new Regex("// \\[SECTION:(.*?)\\]");
            var matches = regex.Matches(Content);

            foreach (Match match in matches)
                if (match.Success && match.Groups.Count > 1)
                    sectionNames.Add(match.Groups[1].Value);

            Sections = sectionNames.Distinct();
        }
    }
}