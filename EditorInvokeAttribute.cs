using System;

/// <summary>
/// This script allows other scripts to have specific methods get invoked directly from the editor
/// This was taken from https://c.atch.co/invoke-a-method-right-from-a-unity-inspector/
/// </summary>

// Place this file in any folder that is or is a descendant of a folder named "Scripts"
namespace EditorInvokable
{
   // Restrict to methods only
   [AttributeUsage(AttributeTargets.Method)]
   public class ExposeMethodInEditorAttribute : Attribute
   {
   }
}