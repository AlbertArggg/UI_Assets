using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace PropollyGDS.Editor.CustomMenuItems.CreateFile
{

    public class DTO
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" - " + Name);

            foreach (var field in Fields)
            {
                sb.AppendLine("     - " + field.Name + ", " + field.Type);
            }

            return sb.ToString();
        }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public static class JsonToCsharpManager
    {
        private static List<DTO> dtos = new();
        private static readonly StringBuilder classDefinitions = new();

        public static List<DTO> GenerateClassFromJson(string json, string rootClassName = "JParsedClass")
        {
            dtos = new List<DTO>();
            var token = JToken.Parse(json);
            ProcessToken(token, rootClassName);
            return dtos;
        }
        
        private static void ProcessToken(JToken token, string className)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    ProcessJObject(token as JObject, className);
                    break;
                case JTokenType.Array:
                    ProcessJArray(token as JArray, className);
                    break;
            }
        }

        private static string GetClassName(string currentName)
        {
            string capitalizedCurrentName = char.ToUpper(currentName[0]) + currentName.Substring(1);
            if (currentName.StartsWith("DTO_")) return capitalizedCurrentName;
            return $"DTO_{capitalizedCurrentName}";
        }

        private static void ProcessJObject(JObject jObject, string className)
        {
            string objectClassName = GetClassName(className);
            var dto = new DTO { Name = objectClassName };

            foreach (var property in jObject.Properties())
            {
                string propType = GetPropertyType(property.Value, property.Name);
                dto.Fields.Add(new Field { Name = property.Name, Type = propType });
            }

            var existingDTO = FindExistingDTO(dto);
            if (existingDTO == null)
            {
                dtos.Add(dto);
            }
        }

        private static void ProcessJArray(JArray jArray, string className)
        {
            if (jArray.Count == 0) return;

            var elementType = jArray.First?.Type;
            if (elementType == JTokenType.Object)
            {
                ProcessJObject(jArray.First as JObject, className);
            }
        }

        private static string GetPropertyType(JToken token, string propName)
        {
            switch (token.Type)
            {
                case JTokenType.Integer:
                    return "int";
                case JTokenType.Float:
                    return "float";
                case JTokenType.String:
                    var success = DateTime.TryParse((string)token, out _);
                    return success ? "DateTime" : "string";
                case JTokenType.Boolean:
                    return "bool";
                case JTokenType.Date:
                    return "DateTime";
                case JTokenType.Array:
                    var arrayClassName = GetClassName(propName);
                    ProcessJArray(token as JArray, arrayClassName);
                    return $"List<{arrayClassName}>";
                case JTokenType.Object:
                    var objectClassName = GetClassName(propName);
                    var dto = CreateOrGetExistingDTO(token as JObject, objectClassName);
                    return dto.Name;
                default:
                    return "dynamic";
            }
        }

        private static DTO CreateOrGetExistingDTO(JObject jObject, string className)
        {
            var dto = new DTO();
            foreach (var property in jObject.Properties())
            {
                string propType = GetPropertyType(property.Value, property.Name);
                dto.Fields.Add(new Field { Name = property.Name, Type = propType });
            }

            var existingDTO = FindExistingDTO(dto);
            if (existingDTO != null)
            {
                return existingDTO;
            }

            dto.Name = className;
            dtos.Add(dto);
            return dto;
        }
        
        private static DTO FindExistingDTO(DTO newDto)
        {
            return dtos.FirstOrDefault(existingDto => IsEquivalentDTO(existingDto, newDto));
        }

        private static bool IsEquivalentDTO(DTO dto1, DTO dto2)
        {
            return dto1.Name == dto2.Name &&
                   dto1.Fields.Count == dto2.Fields.Count &&
                   dto1.Fields.All(f1 => dto2.Fields.Any(f2 => f1.Name == f2.Name && f1.Type == f2.Type));
        }

        public static string GenerateClassContent(DTO dto, string nameSpace = null)
        {
            StringBuilder sb = new StringBuilder();

            bool usingDateTime = dto.Fields.Any(field => field.Type.StartsWith("DateTime"));
            bool usingList = dto.Fields.Any(field => field.Type.StartsWith("List<"));
            
            if (usingDateTime)
            {
                sb.AppendLine("using System;");
            }
            
            if (usingList)
            {
                sb.AppendLine("using System.Collections.Generic;");
            }

            if (usingList || usingDateTime)
            {
                sb.AppendLine();
            }
            
            string indentLevel = string.IsNullOrEmpty(nameSpace) ? "" : "    ";
            
            if (!string.IsNullOrEmpty(nameSpace))
            {
                sb.AppendLine($"namespace {nameSpace}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"{indentLevel}public class {dto.Name}");
            sb.AppendLine($"{indentLevel}{{");

            foreach (var field in dto.Fields)
            {
                sb.AppendLine($"{indentLevel}    public {field.Type} {field.Name} {{ get; set; }}");
            }

            sb.AppendLine($"{indentLevel}}}");
            
            if (!string.IsNullOrEmpty(nameSpace))
            {
                sb.AppendLine("}");
            }
            
            return sb.ToString();
        }
    }
}