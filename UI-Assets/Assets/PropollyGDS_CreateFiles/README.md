# CREATE FILES - Editor Window V-1.0.0

## Brief Description

Introducing an intuitive Unity Editor Window designed to streamline file creation within your projects. Featuring four user-friendly tabs, this tool simplifies the process of organizing and generating various file types, including text files (.txt, .json, .xml, etc.), C# files from templates (classes, interfaces, MonoBehaviour, etc.), and automatic C# file generation from JSON. With options for custom namespaces, constructors, and method inclusion, enhance your development workflow and maintain clean, efficient code across your Unity projects.

## Detailed Description

### Unlock Efficiency and Creativity in Unity File Management

Our Unity Editor Window tool is an asset designed to empower developers with a seamless and efficient way to create and manage a wide array of file types directly within Unity. With a focus on usability and customization, this tool is crafted to cater to both novice and experienced developers looking to enhance their productivity and code organization.

### Feature-Rich Tabs for Comprehensive File Management

- **Tab 1: Directory Selection**
  Begin by choosing the perfect directory for your new files, ensuring that your project remains well-organized from the start.

- **Tab 2: Text File Creation**
  Generate various types of text files (.txt, .json, .xml, .csv, .md, .yaml, .ini, .cfg, .log, .bat, .sh, .html, .css) with just a few clicks. This tab simplifies the process of adding essential text-based resources to your project.

- **Tab 3: C# Files from Templates**
  This tab is a treasure trove for developers looking to efficiently create C# files. Choose from templates for standard classes, abstract classes, interfaces, MonoBehaviour scripts, ScriptableObjects, Singletons, and UI documents, with customizable options for namespaces, constructors, and common Unity methods (Awake, OnEnable, Start, Update). The Singleton template uniquely facilitates automatic setup as a Singleton, ensuring best practices in design pattern implementation.

- **Tab 4: C# Files Generated from JSON**
  Elevate your data-driven projects with the ability to automatically generate C# files from JSON. This feature parses JSON files, suggests naming and data types, and allows for easy customization before generating the final C# objects in your chosen directory.

### Customization at Your Fingertips

Flexibility is a core component of our tool, enabling you to toggle features like namespaces, constructors, and Unity methods on or off based on your specific needs. This level of customization ensures that the generated code aligns perfectly with your project's architecture and coding standards.

### Automatic Namespacing Based on Directory Structure

To further streamline your workflow, our tool automatically generates namespaces based on the directory structure, promoting a clean and organized codebase.

### Enhance Your Unity Experience

Whether you're working on a small indie project or a large-scale game, our Unity Editor Window tool is designed to save you time, reduce repetitive tasks, and help maintain a high standard of code organization. Embrace a more efficient development process with our tool, an essential addition to any Unity developer's toolkit.

Available now on the Unity Asset Store, this tool is your key to unlocking a more streamlined, efficient, and organized development workflow. Enhance your Unity projects with our intuitive file management solution today!

## How to use

- **Step 1: Initiating Your Journey**
  Embark on your creative endeavor by navigating to the top menu. Here, select *Tools*, *Propollygds*, *Create files* to unveil the portal to productivity. This initial step places a world of file management and creation at your fingertips.

- **Step 2: Streamlining Your Environment**
  Optimize your workspace by seamlessly docking the editor window within easy reach. This strategic placement ensures swift access and enhances your workflow, making your project management more efficient and user-friendly.

- **Step 3: Navigating Your Project Landscape**
  Dive into your digital terrain by selecting the *Directory* tab. Locate the folder where you wish to bring your new files into existence and select it with a click. This step is your gateway to organized file creation.

- **Step 4: File Creation [Text Files]**
  Unleash the potential to generate various types of text files (.txt, .json, .xml, .csv, .md, .yaml, .ini, .cfg, .log, .bat, .sh, .html, .css) effortlessly. Simply enter your desired file name, select the file type from the dropdown menu, and hit create. Witness the instant birth of your new file, a testament to the simplified process of integrating essential text-based resources into your project.

- **Step 4: File Creation [C# Files]**
  Discover a goldmine for developers aiming to efficiently create C# files. Select from an array of templates, including standard classes, abstract classes, interfaces, MonoBehaviour scripts, ScriptableObjects, Singletons, and UI documents. Customize options for namespaces, constructors, and common Unity methods (Awake, OnEnable, Start, Update). The Singleton template notably ensures an automatic setup as a Singleton, embedding best practices in design pattern application. Enter the file name, select your template, and customize further to meet your project's needs. Toggle the 'Include namespace' option for automatic namespace generation based on the file's location.

- **Step 4: File Creation [C# Files from JSON]**
  Elevate your data-driven projects with the feature to automatically generate C# files from JSON. This tool parses JSON files, suggests naming and data types, and facilitates customization before generating the final C# objects in your chosen directory. Include your JSON file in the resources folder for automatic detection. Upon selecting the JSON, you can review its contents, adjust class and field names, and select appropriate data types. Once satisfied, proceed to create the classes directly in your desired directory.

- **Step 5: Enjoy!**
  Revel in the capabilities at your disposal and may this asset significantly enhance your productivity. We welcome your feedback and suggestions for future enhancements, many of which are already in development for the next release.


# CREATE FILES - Editor Window V-1.0.1
### Auto Namespace Generator Updates
In this update, we have significantly improved how namespaces are generated from directory paths to avoid conflicts with existing class names within the Unity environment and .NET Framework. These enhancements ensure that the automatic namespacing functionality is more robust and less likely to create issues during project compilation.

- **Dynamic Type Checking:** Introduced a method to dynamically check if a namespace part conflicts with any known class names across all currently loaded assemblies. This prevents the accidental creation of namespaces that could interfere with system and Unity class names.

- **Conflict Resolution Dictionary:** Implemented a `Dictionary<string, string>` named `KNOWN_NAMESPACE_CONFLICTS` that maps potential conflicting directory names to alternative namespace parts. This preemptive measure addresses common conflicts directly, providing a first layer of defense against naming issues.

- **Namespace Part Adjustment:** For directory names that still pose a conflict after the initial dictionary check, we've updated the namespace generation logic to append a suffix `Ns` to the conflicting part, ensuring uniqueness.

### Enhanced JSON to C# Class Generation
We're excited to announce the latest update to our JSON to C# class generator within the CREATE FILES Editor Window. Version 1.0.1 introduces the highly requested features for dynamically managing C# classes and fields directly from JSON data. This update significantly streamlines the process of converting JSON into structured C# classes, providing developers with more flexibility and control over their data models.

- **Dynamic Class and Field Management:** Developers can now effortlessly add new classes and remove existing ones from the JSON data structure. This dynamic interaction extends to the fields within each class, allowing for a fully customizable approach to defining your data models.

- **Improved User Interface:** The Editor Window has been refined to include intuitive options for adding and removing both classes and fields. With just a click, users can insert a new class or field, or remove unwanted ones, making the management of your data structure more accessible than ever.

- **Class Name Changes:** Changing the class name in any Data Object will be reflected in other Data Object's field types.

© 2023 PropollyGDS.com. All rights reserved.