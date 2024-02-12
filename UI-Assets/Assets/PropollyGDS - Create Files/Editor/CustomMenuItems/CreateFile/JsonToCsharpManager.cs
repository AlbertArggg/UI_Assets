using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace PropollyGDS.Editor.CustomMenuItems.CreateFile
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for generating C# class definitions from JSON.
    /// </summary>
    public class DTO
    {
        /// <summary>
        /// Gets or sets the name of the DTO.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the list of fields within the DTO.
        /// </summary>
        public List<Field> Fields { get; set; } = new();
        
        /// <summary>
        /// Returns a string representation of the DTO, including its name and fields.
        /// </summary>
        /// <returns>A string detailing the DTO's name and fields.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(" - " + Name);

            foreach (var field in Fields)
                sb.AppendLine("     - " + field.Name + ", " + field.Type);

            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a single field within a DTO, including its name and type.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Manages the conversion of JSON structures into C# class definitions.
    /// </summary>
    public static class JsonToCsharpManager
    {
        private static List<DTO> dtos = new();

        /// <summary>
        /// Parses JSON input and generates a list of DTOs representing the C# class structure.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <param name="rootClassName">The root class name for the generated classes.</param>
        /// <returns>A list of DTOs representing the C# classes derived from the JSON.</returns>
        public static List<DTO> GenerateClassFromJson(string json, string rootClassName = "JParsedClass")
        {
            dtos = new List<DTO>();
            var token = JToken.Parse(json);
            ProcessToken(token, rootClassName);
            return dtos;
        }
        
        /// <summary>
        /// Processes a JToken to generate DTOs based on its type.
        /// </summary>
        /// <param name="token">The JToken to process.</param>
        /// <param name="className">The class name to assign to generated DTOs.</param>
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

        /// <summary>
        /// Determines the appropriate C# class name for a given JSON property name.
        /// </summary>
        /// <param name="currentName">The JSON property name.</param>
        /// <returns>A formatted C# class name.</returns>
        private static string GetClassName(string currentName)
        {
            var capitalizedCurrentName = char.ToUpper(currentName[0]) + currentName[1..];
            return currentName.StartsWith("DTO_") ? capitalizedCurrentName : $"DTO_{capitalizedCurrentName}";
        }

        /// <summary>
        /// Processes a JObject to create or update a DTO representing a C# class.
        /// </summary>
        /// <param name="jObject">The JObject to process.</param>
        /// <param name="className">The class name for the generated DTO.</param>
        private static void ProcessJObject(JObject jObject, string className)
        {
            var objectClassName = GetClassName(className);
            var dto = new DTO { Name = objectClassName };

            foreach (var property in jObject.Properties())
            {
                var propType = GetPropertyType(property.Value, property.Name);
                dto.Fields.Add(new Field { Name = property.Name, Type = propType });
            }

            var existingDTO = FindExistingDTO(dto);
            if (existingDTO == null)
            {
                dtos.Add(dto);
            }
        }

        /// <summary>
        /// Processes a JArray to generate DTOs for its elements.
        /// </summary>
        /// <param name="jArray">The JArray to process.</param>
        /// <param name="className">The class name for DTOs generated from the array elements.</param>
        private static void ProcessJArray([NotNull] JArray jArray, string className)
        {
            if (jArray == null) throw new ArgumentNullException(nameof(jArray));
            if (jArray.Count == 0) return;

            var elementType = jArray.First?.Type;
            if (elementType == JTokenType.Object)
            {
                ProcessJObject(jArray.First as JObject, className);
            }
        }

        /// <summary>
        /// Attempts to determine the C# type for a given JSON property based on its value.
        /// </summary>
        /// <param name="token">The JToken representing the property value.</param>
        /// <param name="propName">The name of the property.</param>
        /// <returns>The C# type as a string.</returns>
        private static string GetPropertyType(JToken token, string propName)
        {
            // TODO: Better system to cover more crap and get better results
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

        /// <summary>
        /// Creates a new DTO or retrieves an existing one based on a JObject.
        /// </summary>
        /// <param name="jObject">The JObject to process.</param>
        /// <param name="className">The class name for the DTO.</param>
        /// <returns>A DTO representing the JObject.</returns>
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
        
        /// <summary>
        /// Searches for an existing DTO that matches the given new DTO.
        /// </summary>
        /// <param name="newDto">The DTO to find a match for.</param>
        /// <returns>The matching DTO if found; otherwise, null.</returns>
        private static DTO FindExistingDTO(DTO newDto)
        {
            return dtos.FirstOrDefault(existingDto => IsEquivalentDTO(existingDto, newDto));
        }

        /// <summary>
        /// Checks if two DTOs are equivalent based on their name and fields.
        /// </summary>
        /// <param name="dto1">The first DTO to compare.</param>
        /// <param name="dto2">The second DTO to compare.</param>
        /// <returns>True if the DTOs are equivalent; otherwise, false.</returns>
        private static bool IsEquivalentDTO(DTO dto1, DTO dto2)
        {
            return dto1.Name == dto2.Name &&
                   dto1.Fields.Count == dto2.Fields.Count &&
                   dto1.Fields.All(f1 => dto2.Fields.Any(f2 => f1.Name == f2.Name && f1.Type == f2.Type));
        }

        /// <summary>
        /// Generates the content for a C# class based on the specified DTO.
        /// </summary>
        /// <param name="dto">The DTO to generate class content for.</param>
        /// <param name="nameSpace">The namespace for the class. If null, the class will not be placed in a namespace.</param>
        /// <returns>A string containing the C# class definition.</returns>
        public static string GenerateClassContent(DTO dto, string nameSpace = null)
        {
            var sb = new StringBuilder();

            var usingDateTime = dto.Fields.Any(field => field.Type.StartsWith("DateTime"));
            var usingList = dto.Fields.Any(field => field.Type.StartsWith("List<"));
            
            if (usingDateTime)
                sb.AppendLine("using System;");
            
            if (usingList)
                sb.AppendLine("using System.Collections.Generic;");

            if (usingList || usingDateTime)
                sb.AppendLine();
            
            var indentLevel = string.IsNullOrEmpty(nameSpace) ? "" : "    ";
            
            if (!string.IsNullOrEmpty(nameSpace))
            {
                sb.AppendLine($"namespace {nameSpace}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"{indentLevel}public class {dto.Name}");
            sb.AppendLine($"{indentLevel}{{");

            foreach (var field in dto.Fields)
                sb.AppendLine($"{indentLevel}    public {field.Type} {field.Name} {{ get; set; }}");

            sb.AppendLine($"{indentLevel}}}");
            
            if (!string.IsNullOrEmpty(nameSpace))
                sb.AppendLine("}");
            
            return sb.ToString();
        }
    }
}