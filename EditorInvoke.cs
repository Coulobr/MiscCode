using UnityEngine;
using UnityEditor;
using System.Reflection;

/// <summary>
/// This script allows other scripts to have specific methods get invoked directly from the editor
/// SETUP:
/// - Add ` using EditorInvokable; ` to the top of the desired script
/// - Add [ExposeMethodInEditor] immediately before the funtion to call from the editor
/// - the function CANNOT require any arguments!
/// - Add the behavior on button press in the indicated space below (line 40 onwards)
/// This was taken and modified from https://c.atch.co/invoke-a-method-right-from-a-unity-inspector/
/// </summary>

// Place this file in any folder that is or is a descendant of a folder named "Editor"
namespace EditorInvokable
{
   [CanEditMultipleObjects] // Don't ruin everyone's day
   [CustomEditor(typeof(ScriptableObject), true)] // Target all ScriptableObjects and descendants
   public class ScriptableObjectCustomEditor : UnityEditor.Editor
   {
      public override void OnInspectorGUI()
      {
         DrawDefaultInspector(); // Draw the normal inspector
         
         // Currently this will only work in the Play mode. You'll see why
         // Get the type descriptor for the ScriptableObject we are drawing
         var scrType = target.GetType();
         
         // Iterate over each private or public instance method (no static methods atm)
         foreach (var method in scrType.GetMethods(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance))
         {
            // make sure it is decorated by our custom attribute
            var attributes = method.GetCustomAttributes(typeof(ExposeMethodInEditorAttribute), true);
            if (attributes.Length > 0)
            {
               
               if (GUILayout.Button("Run: " + method.Name))
               {
                  // If the user clicks the button, call the method immediately.
                  if (target is PlayerStats)
                  {
                     PlayerStats ps = (PlayerStats)target;
                     ps.ResetToDefault();
                  }
                  if (target is BulletStats)
                  {
                     BulletStats bs = (BulletStats)target;
                     bs.MarkAsChanged();
                  }

                  // ==============================
                  //     ADD MORE BEHAVIORS HERE
                  //  Use the same format as above
                  // ==============================
               }
            }
         }
      }
   }
}