using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using PropollyGDS.Scripts;
using PropollyGDS.Scripts.Extensions;

namespace PropollyGDS.Editor.CustomMenuItems.CreateFile
{
    /// <summary>
    /// Manages the loading and processing of script templates for file generation.
    /// </summary>
    public class TemplateManager
    {
        private readonly Dictionary<string, Template> templates;

        /// <summary>
        /// Initializes a new instance of the TemplateManager, loading predefined templates.
        /// </summary>
        public TemplateManager()
        {
            templates = new Dictionary<string, Template>();
            LoadTemplates();
        }

        /// <summary>
        /// Loads script templates into the manager for later use.
        /// </summary>
        private void LoadTemplates()
        {
            templates["Abstract Class"] = new Template(Constants.ScriptingTemplates.ABSTRACT_CLASS);
            templates["C#"] = new Template(Constants.ScriptingTemplates.CSHARP_CLASS);
            templates["Interface"] = new Template(Constants.ScriptingTemplates.INTERFACE);
            templates["MonoBehaviour"] = new Template(Constants.ScriptingTemplates.MONOBEHAVIOUR);
            templates["Scriptable Object"] = new Template(Constants.ScriptingTemplates.SCRIPTABLE_OBJECT);
            templates["Singleton"] = new Template(Constants.ScriptingTemplates.SINGLETON);
            templates["UI Document"] = new Template(Constants.ScriptingTemplates.UI_DOCUMENT);
        }
        
        /// <summary>
        /// Retrieves all available template keys.
        /// </summary>
        /// <returns>An array of template keys.</returns>
        public string[] GetTemplateKeys() => templates.Keys.ToArray();

        /// <summary>
        /// Gets the content of a template based on a provided key.
        /// </summary>
        /// <param name="key">The key identifying the template.</param>
        /// <returns>The content of the template if found; otherwise, an empty string.</returns>
        private string GetTemplateContent(string key) => templates.TryGetValue(key, out Template template) ? template.Content : string.Empty;

        /// <summary>
        /// Retrieves a Template object by its type.
        /// </summary>
        /// <param name="templateType">The type of the template to retrieve.</param>
        /// <returns>The Template object if found; otherwise, null.</returns>
        public Template GetTemplate(string templateType) => templates.TryGetValue(templateType, out var template) ? template : null;

        /// <summary>
        /// Parses and processes a template, replacing placeholders and toggling sections based on provided parameters.
        /// </summary>
        /// <param name="directory">The directory path for namespace generation.</param>
        /// <param name="templateKey">The key of the template to process.</param>
        /// <param name="fileName">The name of the file to be generated.</param>
        /// <param name="includeNamespace">Indicates whether to include a namespace in the generated content.</param>
        /// <param name="sectionToggles">A dictionary specifying which sections to include in the generated content.</param>
        /// <returns>The processed template content as a string.</returns>
        public string ParseAndProcessTemplate(string directory, string templateKey, string fileName, 
            bool includeNamespace, Dictionary<string, bool> sectionToggles)
        {
            var generatedClassContent = GetTemplateContent(templateKey);
            generatedClassContent = generatedClassContent.Replace("#SCRIPTNAME#", fileName);
            
            foreach (var section in sectionToggles)
            {
                var pattern = $"// \\[SECTION:{section.Key}\\](.*?)// \\[ENDSECTION\\]";
                var match = Regex.Match(generatedClassContent, pattern, RegexOptions.Singleline);
                generatedClassContent = match.Success
                    ? section.Value
                        ? generatedClassContent.Replace(match.Value, match.Groups[1].Value)
                        : generatedClassContent.Replace(match.Value, "")
                    : generatedClassContent;
            }
            
            if (!includeNamespace)
            {
                const string namespacePattern = @"namespace #NAMESPACE#\s*{((?:[^{}]|{(?<c>)|}(?<-c>))*(?(c)(?!)))}";
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

    /// <summary>
    /// Represents a script template, including its content and defined sections.
    /// </summary>
    public class Template
    {
        public string Content { get; private set; }
        public IEnumerable<string> Sections { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Template class, loading its content from a resource path.
        /// </summary>
        /// <param name="resourcePath">The resource path to load the template from.</param>
        public Template(string resourcePath)
        {
            LoadTemplate(resourcePath);
            ParseSections();
        }

        /// <summary>
        /// Loads the template content from a given resource path.
        /// </summary>
        /// <param name="resourcePath">The resource path to load the template from.</param>
        private void LoadTemplate(string resourcePath)
        {
            var textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset == null) return;
            
            Content = textAsset.text;
        }

        /// <summary>
        /// Parses the template content to identify and store section markers.
        /// </summary>
        private void ParseSections()
        {
            if (string.IsNullOrEmpty(Content))
            {
                Sections = Enumerable.Empty<string>();
                return;
            }

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