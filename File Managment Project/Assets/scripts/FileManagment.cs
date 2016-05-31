using UnityEngine;
using System.Collections;
using Assets.Scripts.com.pixattic.framewok.core.system.storage;

public class FileManagment : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		NStorageFile file = new NStorageFile ("data_juego2");
		file.OpenFile ();

		// Primero leemos el tipo, si es -1 es porque o no hay data o llego
		// al final del archivo.
		int nextDataType = file.ReadDataType ();
		// Recorremos el tipo de dato en iterador While y dependiendo del tipo
		// lo procesamos.
		while (nextDataType != -1) {
			int count = file.ReadInt ();

			switch (nextDataType) {
			case NStorageFile.DATATYPE_INT:
				int dataInt = file.ReadInt();
				Debug.Log ("dataInt: " + dataInt);
				break;
			case NStorageFile.DATATYPE_BOOL:
				bool dataBool = file.ReadBool();
				Debug.Log ("dataBool: " + dataBool);
				break;
			case NStorageFile.DATATYPE_FLOAT:
				float dataFloat = file.ReadFloat();
				Debug.Log ("dataFloat: " + dataFloat.ToString());
				break;
			case NStorageFile.DATATYPE_STRING:
				string dataString = file.ReadString(count);
				Debug.Log ("dataString: " + dataString);
				break;
			}
			// Seguimos leyendo el siguiente dato.
			nextDataType = file.ReadDataType ();
		}

		file.WriteInt (120);
		file.WriteBool (true);
		file.WriteInt (2000034);
		file.WriteFloat (10.45560011f);
		file.WriteString ("Clase TDP del 2016 ciclo 1.");
		file.WriteString ("Esto es otra prueba de texto");
		file.WriteFloat (12220.9091f);
		file.WriteBool (false);
		file.WriteString ("Se termino el archivo.");
		file.SaveFile ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
