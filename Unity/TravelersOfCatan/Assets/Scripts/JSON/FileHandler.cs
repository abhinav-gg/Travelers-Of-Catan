using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Class to handle the saving and loading of files from Unity's persistent data path
/// <Br/>Credit to https://www.youtube.com/watch?v=KZft1p8t2lQ for the tutorial on implementing this concept 
/// </summary>
public class FileHandler 
{
    private string filepath = "";
    public bool IsMade;
    private bool useEncryption = false;
    // practically speaking, this is not easily crackable, but it's not perfect as the same key is repeated
    private readonly string encryptionCodeWord = "SomeRandomStringKeyToConvertIntoBinaryForXOR->aiuaeogmk3GJEK834FEJSAK->";

    public FileHandler(string filepath)
    {
        this.filepath = filepath;
        encryptionCodeWord += filepath; // add the filepath to the encryption key to make it more unique
        string suffix = FindSuffix();
        if (suffix == "")
        {
            IsMade = false;
            this.filepath += ".bin";
            useEncryption = true;
        }
        else
        {
            IsMade = true;
            useEncryption = suffix == ".bin";
            this.filepath += suffix;
        }
    }

    // method to find the suffix of the save file
    string FindSuffix()
    {
        if (File.Exists(filepath + ".json"))
        {
            return ".json";
        }
        else if (File.Exists(filepath + ".bin"))
        {
            return ".bin";
        }
        else
        {
            return "";
        }
    }

    // method to delete the file
    public void Delete()
    {
        if (IsMade)
        {
            File.Delete(filepath);
            IsMade = false;
        }
    }

    // function to return the data from the file
    public string Load()
    {

        string dataToLoad = "";
        if (File.Exists(filepath))
        {
            try
            {
                // load the serialized data from the file
                using (FileStream stream = new FileStream(filepath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // optionally decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when trying to load data {e}");
            }
        }
        return dataToLoad;
    }

    // method to save the data to the file
    public void Save(string data)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            if (useEncryption)
            {
                data = EncryptDecrypt(data);
            }

            // write the serialized data to the file
            using (FileStream stream = new FileStream(filepath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + filepath + "\n" + e);
        }
    }

    // Perform XOR encryption/decryption on the data
    // Credit to https://stackoverflow.com/questions/2532668/help-me-with-xor-encryption for the XOR encryption
    public string EncryptDecrypt(string data) 
    {
        string key = encryptionCodeWord;
        int dataLen = data.Length;
        int keyLen = key.Length;
        char[] output = new char[dataLen];

        for (int i = 0; i < dataLen; ++i)
        {
            output[i] = (char)(data[i] ^ key[i % keyLen]);
        }

        return new string(output);
    }

}