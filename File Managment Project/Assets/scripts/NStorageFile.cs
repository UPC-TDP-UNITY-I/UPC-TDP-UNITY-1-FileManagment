using System;
using System.IO;
using UnityEngine;
using System.Text;

namespace Assets.Scripts.com.pixattic.framewok.core.system.storage {

	public sealed class NStorageFile {

		// Header
		// TIPO: INT 4 BYTES
		// LENGTH: INT 4 BYTES
		// VALUE: XX XX BYTESS

		public const int DATATYPE_INT = 1;
		public const int DATATYPE_FLOAT = 2;
		public const int DATATYPE_BOOL = 3;
		public const int DATATYPE_STRING = 4;

		private static string PersistancePath = Application.persistentDataPath;
		private const string ExtensionFile = ".tdp";

		private static byte[] SIZE_ONE = BitConverter.GetBytes(1);
		private static byte[] EMPTY_ARRAY1 = new byte[1];
		private static byte[] EMPTY_ARRAY2 = new byte[2];
		private static byte[] EMPTY_ARRAY4 = new byte[4];

		private static byte[] BYTEARRAY_INT = BitConverter.GetBytes(NStorageFile.DATATYPE_INT);
		private static byte[] BYTEARRAY_FLOAT = BitConverter.GetBytes(NStorageFile.DATATYPE_FLOAT);
		private static byte[] BYTEARRAY_BOOL = BitConverter.GetBytes(NStorageFile.DATATYPE_BOOL);
		private static byte[] BYTEARRAY_STRING = BitConverter.GetBytes(NStorageFile.DATATYPE_STRING);

		private bool mFileOpened;
		private string mFilePath;
		private string mFileName;
		private Action mOnFinishSavingDataCallback;

		private MemoryStream mMS_Writing;
		private FileStream mFS_Reading;
		
		public string FileName 							{ get { return mFileName; } }
		public string FilePath 							{ get { return mFilePath; } }
		public bool AlreadyOpened 						{ get { return mFileOpened; } }

		public NStorageFile (string fileName) {
			mFileName = fileName;

			mFileOpened = false;
            mOnFinishSavingDataCallback = null;

			mMS_Writing = new MemoryStream ();

			mFilePath = NStorageFile.PersistancePath + "/" + mFileName + 
				NStorageFile.ExtensionFile;
        }

		public void OpenFile() {
			if (!mFileOpened) {
				mFileOpened = true;

				mFS_Reading = File.Open(mFilePath, FileMode.OpenOrCreate);
				mFS_Reading.Position = 0;
			}
		}

		public int ReadDataType() {
			int result = -1;
			// Si la posicion de la data en el stream es igual al tamaño del
			// archivo, entonces llego a su final.
			if (mFS_Reading.Position != mFS_Reading.Length) {
				mFS_Reading.Read (NStorageFile.EMPTY_ARRAY4, 0, 4);

				result = BitConverter.ToInt32 (NStorageFile.EMPTY_ARRAY4, 0);
			}

			return result;
		}

		public bool ReadBool() {
			mFS_Reading.Read (NStorageFile.EMPTY_ARRAY1, 0, 1);
			return BitConverter.ToBoolean (NStorageFile.EMPTY_ARRAY1, 0);
		}

		public int ReadInt() {
			mFS_Reading.Read (NStorageFile.EMPTY_ARRAY4, 0, 4);
			return BitConverter.ToInt32 (NStorageFile.EMPTY_ARRAY4, 0);
		}

		public float ReadFloat() {
			mFS_Reading.Read (NStorageFile.EMPTY_ARRAY4, 0, 4);
			return BitConverter.ToSingle (NStorageFile.EMPTY_ARRAY4, 0);
		}

		public string ReadString(int charCount) {
			StringBuilder sb = new StringBuilder ();

			for (int i = 0; i < charCount; i++) {
				mFS_Reading.Read (NStorageFile.EMPTY_ARRAY2, 0, 2);
				char charResult = BitConverter.ToChar(NStorageFile.EMPTY_ARRAY2, 0);
				sb.Append (charResult);
			}
				
			return sb.ToString();
		}

		public void WriteInt(int data) {
			byte[] rawData = BitConverter.GetBytes (data);

			mMS_Writing.Write (NStorageFile.BYTEARRAY_INT, 0, 4);
			mMS_Writing.Write (NStorageFile.SIZE_ONE, 0, 4);
			mMS_Writing.Write (rawData, 0, rawData.Length);
        }

		public void WriteFloat(float data) {
			byte[] rawData = BitConverter.GetBytes (data);

			mMS_Writing.Write (NStorageFile.BYTEARRAY_FLOAT, 0, 4);
			mMS_Writing.Write (NStorageFile.SIZE_ONE, 0, 4);
			mMS_Writing.Write (rawData, 0, rawData.Length);
		}

		public void WriteBool(bool data) {
			byte[] rawData = BitConverter.GetBytes (data);

			mMS_Writing.Write (NStorageFile.BYTEARRAY_BOOL, 0, 4);
			mMS_Writing.Write (NStorageFile.SIZE_ONE, 0, 4);
			mMS_Writing.Write (rawData, 0, rawData.Length);
		}

		public void WriteString(string data) {
			byte[] rawSize = BitConverter.GetBytes (data.Length);

			mMS_Writing.Write (NStorageFile.BYTEARRAY_STRING, 0, 4);
			mMS_Writing.Write (rawSize, 0, rawSize.Length);

			for (int i = 0; i < data.Length; i++) {
				byte[] rawData = BitConverter.GetBytes (data[i]);

				mMS_Writing.Write (rawData, 0, rawData.Length);
			}
		}

        public void SaveFile() {
			// Si el FileStream para leer esta abierto, hay que cerrarlo para poder sobreescribir
			if (mFS_Reading != null) {
				mFS_Reading.Dispose ();
				mFS_Reading.Close ();
			}
			mFS_Reading = null;

			// Ahora recien podemos empezar a guardar el archivo
			FileStream fs = File.Open(mFilePath, FileMode.OpenOrCreate);
			fs.Position = 0;

			byte[] allData = mMS_Writing.GetBuffer ();

			fs.Write(allData, 0, (int)mMS_Writing.Length);
            fs.Close();
			fs.Dispose();
            fs = null;
        }

		public void Free() {
			mFilePath = null;
			mFileName = null;

			if (mFS_Reading != null) {
				mFS_Reading.Dispose ();
			}

			mFS_Reading = null;

			if (mMS_Writing != null) {
				mMS_Writing.Dispose ();
			}

			mMS_Writing = null;
            mOnFinishSavingDataCallback = null;
        }
    }
}

