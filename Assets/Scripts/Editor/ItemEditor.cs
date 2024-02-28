using AFSInterview.Items;
using UnityEditor;
using UnityEngine;

namespace AFSInterview.Editor
{
    [CustomPropertyDrawer(typeof(Item))]
    public class ItemEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var indent = EditorGUI.indentLevel;
            var currentSpace = 0;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.Separator();
            
            // Check if consumable is checked and draw consume button
            var isConsumable = property.FindPropertyRelative("consumable").boolValue;
            if (isConsumable)
            {
                if (GUILayout.Button("Consume (Play Mode only)"))
                {
                    property.FindPropertyRelative("consumed").boolValue = true;
                    InventoryController.Instance.RefreshItems();
                }
                
                currentSpace += 25;
                position.y += 40;
            }

            // Position rects
            currentSpace += 50;
            var nameField = new Rect(20, position.y + 25, 60, position.height);
            var name = new Rect(140, position.y + 25, 260, position.height);
            
            currentSpace += 50;
            var valueField = new Rect(20, position.y + 45, 60, position.height);
            var value = new Rect(140, position.y + 45, 40, position.height);

            currentSpace += 50;
            var consumableField = new Rect(20, position.y + 75, 80, position.height);
            var consumable = new Rect(140, position.y + 75, 40, position.height);
            
            // Base fields
            EditorGUI.LabelField(nameField, "Name:");
            EditorGUI.PropertyField(name, property.FindPropertyRelative("name"), GUIContent.none);
            
            EditorGUI.LabelField(valueField, "Value:");
            EditorGUI.PropertyField(value, property.FindPropertyRelative("value"), GUIContent.none);
            
            EditorGUI.LabelField(consumableField, "Consumable?");
            EditorGUI.PropertyField(consumable, property.FindPropertyRelative("consumable"), GUIContent.none);
            
            // Consumable fields
            if (isConsumable)
            {
                var additionalOffset = 0;
                
                currentSpace += 50;
                var addMoneyField = new Rect(40, position.y + 95, 80, position.height);
                var addMoney = new Rect(140, position.y + 95, 260, position.height);
                
                EditorGUI.LabelField(addMoneyField, "Add Money?");
                EditorGUI.PropertyField(addMoney, property.FindPropertyRelative("addMoney"), GUIContent.none);
                var willAddMoney = property.FindPropertyRelative("addMoney").boolValue;

                if (willAddMoney)
                {
                    currentSpace += 50;
                    var addMoneyValueField = new Rect(80, position.y + 120, 60, position.height);
                    var addMoneyValue = new Rect(140, position.y + 120, 40, position.height);
                    
                    EditorGUI.LabelField(addMoneyValueField, "Value:");
                    EditorGUI.PropertyField(addMoneyValue, property.FindPropertyRelative("addMoneyValue"), GUIContent.none);

                    additionalOffset += 25;
                }
                
                currentSpace += 50;
                var addItemField = new Rect(40, position.y + 120 + additionalOffset, 80, position.height);
                var addItem = new Rect(140, position.y + 120 + additionalOffset, 260, position.height);
                
                EditorGUI.LabelField(addItemField, "Add Item?");
                EditorGUI.PropertyField(addItem, property.FindPropertyRelative("addItem"), GUIContent.none);
                var willAddItem = property.FindPropertyRelative("addItem").boolValue;

                if (willAddItem)
                {
                    currentSpace += 50;
                    var addItemItemField = new Rect(80, position.y + 145 + additionalOffset, 80, position.height);
                    var addItemItem = new Rect(140, position.y + 145 + additionalOffset, 260, position.height);
                
                    EditorGUI.LabelField(addItemItemField, "Item:");
                    EditorGUI.PropertyField(addItemItem, property.FindPropertyRelative("addItemHolder"), GUIContent.none);
                    
                    currentSpace += 50;
                    var addItemNumField = new Rect(80, position.y + 165 + additionalOffset, 80, position.height);
                    var addItemNum = new Rect(140, position.y + 165 + additionalOffset, 40, position.height);
                
                    EditorGUI.LabelField(addItemNumField, "Amount:");
                    EditorGUI.PropertyField(addItemNum, property.FindPropertyRelative("addItemNum"), GUIContent.none);
                }
            }
            
            EditorGUI.indentLevel = indent;
            EditorGUILayout.Space(currentSpace / 2);
            EditorGUILayout.Separator();
            EditorGUI.EndProperty();
        }
    }
}
