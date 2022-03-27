using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer {


	/// <summary>
	/// projects the arrylayout class. makes custom field property for rowData and row that makes interface like the 2d array. 
	/// takes the row and column lenght from the level genertor class.
	/// </summary>
	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		EditorGUI.PrefixLabel(position,label);
		Rect newposition = position;
		newposition.y += 18f;
		SerializedProperty data = property.FindPropertyRelative("rows");
		SerializedProperty ysize = property.FindPropertyRelative("ysize");
		//data.rows[0][]
		for (int j=0;j< data.arraySize; j++){
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
			
			newposition.height = 18f;
			if(row.arraySize != ysize.intValue)
				row.arraySize = ysize.intValue;
			newposition.width = position.width/ ysize.intValue;
			for(int i=0;i< ysize.intValue; i++){
				EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(i),GUIContent.none);
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += 18f;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		return 18f * 15;
	}
}
